using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox;

public class Player : GameObject
{
    private enum PlayerState { Idle, Walk, Jump, Attack }

    // Sprite sheet layout: 4x4 grid of 32x32 frames, one animation per row
    private const int FRAME_SIZE = 32;
    private const int FRAMES_PER_ROW = 4;
    private const int SIZE = FRAME_SIZE;   // drawn at native size; the camera zoom scales it up

    private const float SPEED = 100f;      // world pixels per second
    private const int INTERACTION_PADDING = 6;
    private const float JUMP_HEIGHT = 14f;

    private readonly Rectangle _worldBounds;
    private readonly Animation _idle;
    private readonly Animation _walk;
    private readonly Animation _jump;
    private readonly Animation _attack;

    private PlayerState _state = PlayerState.Idle;
    private TimeSpan _jumpElapsed;

    private TimeSpan JumpDuration => _jump.FrameDuration * _jump.Frames.Length;

    // Bounds stay on the ground during a jump, so sorting and interaction aren't affected
    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, SIZE, SIZE);
    public Rectangle InteractionRange
    {
        get
        {
            var range = Bounds;
            range.Inflate(INTERACTION_PADDING, INTERACTION_PADDING);
            return range;
        }
    }

    public Player(Texture2D spriteSheet, Vector2 position, Rectangle worldBounds) : base(spriteSheet, position)
    {
        _worldBounds = worldBounds;

        _idle = Animation.FromRow(spriteSheet, 0, FRAMES_PER_ROW, FRAME_SIZE, FRAME_SIZE, TimeSpan.FromMilliseconds(250));
        _walk = Animation.FromRow(spriteSheet, 1, FRAMES_PER_ROW, FRAME_SIZE, FRAME_SIZE, TimeSpan.FromMilliseconds(120));
        _jump = Animation.FromRow(spriteSheet, 2, FRAMES_PER_ROW, FRAME_SIZE, FRAME_SIZE, TimeSpan.FromMilliseconds(100), isLooping: false);
        _attack = Animation.FromRow(spriteSheet, 3, FRAMES_PER_ROW, FRAME_SIZE, FRAME_SIZE, TimeSpan.FromMilliseconds(80), isLooping: false);

        Sprite = new AnimatedSprite(_idle);
    }

    /// <summary>
    /// Stops whatever the player is doing and returns to idle, e.g. when a
    /// conversation starts and the world pauses.
    /// </summary>
    public void Halt()
    {
        _state = PlayerState.Idle;
        Sprite.Play(_idle);
    }

    public override void Update(GameTime gameTime)
    {
        // Jump and attack end when their time is up or their animation has finished
        bool jumpDone = _state == PlayerState.Jump && _jumpElapsed >= JumpDuration;
        bool attackDone = _state == PlayerState.Attack && Sprite.IsFinished;
        if (jumpDone || attackDone)
        {
            _state = PlayerState.Idle;
        }

        var keyboard = Game1.Input.Keyboard;
        var direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.Right)) direction.X += 1;
        if (keyboard.IsKeyDown(Keys.Left)) direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.Up)) direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.Down)) direction.Y += 1;

        // Jumping and attacking play to completion; idle and walk can be interrupted
        bool isBusy = _state == PlayerState.Jump || _state == PlayerState.Attack;
        if (!isBusy)
        {
            if (keyboard.WasKeyJustPressed(Keys.Space))
            {
                _state = PlayerState.Jump;
                _jumpElapsed = TimeSpan.Zero;
            }
            else if (keyboard.WasKeyJustPressed(Keys.F))
            {
                _state = PlayerState.Attack;
            }
            else
            {
                _state = direction == Vector2.Zero ? PlayerState.Idle : PlayerState.Walk;
            }
        }

        // You can steer mid-jump, but you stand still while attacking
        if (_state != PlayerState.Attack)
        {
            if (direction.X != 0) FacingLeft = direction.X < 0;
            Move(direction, gameTime);
        }

        if (_state == PlayerState.Jump)
        {
            _jumpElapsed += gameTime.ElapsedGameTime;
        }

        Sprite.Play(_state switch
        {
            PlayerState.Walk => _walk,
            PlayerState.Jump => _jump,
            PlayerState.Attack => _attack,
            _ => _idle,
        });
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var destination = Bounds;
        destination.Y -= (int)GetJumpOffset();

        var effects = FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Sprite.Draw(spriteBatch, destination, Color.White, effects);
    }

    private void Move(Vector2 direction, GameTime gameTime)
    {
        if (direction == Vector2.Zero) return;

        // Normalising stops diagonal movement from being faster than straight movement
        direction.Normalize();

        // Keep the player fully inside the world
        var newPosition = Position + direction * SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        newPosition.X = MathHelper.Clamp(newPosition.X, _worldBounds.Left, _worldBounds.Right - SIZE);
        newPosition.Y = MathHelper.Clamp(newPosition.Y, _worldBounds.Top, _worldBounds.Bottom - SIZE);
        Position = newPosition;
    }

    /// <summary>
    /// Top-down games have no real vertical axis, so a jump is just the sprite
    /// drawn higher along a smooth arc while the player's ground position stays put.
    /// </summary>
    private float GetJumpOffset()
    {
        if (_state != PlayerState.Jump) return 0f;

        float progress = (float)(_jumpElapsed / JumpDuration);
        return MathF.Sin(MathHelper.Clamp(progress, 0f, 1f) * MathHelper.Pi) * JUMP_HEIGHT;
    }
}
