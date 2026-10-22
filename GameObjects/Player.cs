using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox;

public class Player : GameObject
{
    private const int SPRITE_WIDTH = 70;
    private const int SPRITE_HEIGHT = 100;
    private const int SPEED = 5;
    private const int INTERACTION_PADDING = 16;

    private readonly Rectangle _worldBounds;

    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT);
    public Rectangle InteractionRange
    {
        get
        {
            var range = Bounds;
            range.Inflate(INTERACTION_PADDING, INTERACTION_PADDING);
            return range;
        }
    }

    public Player(Texture2D texture, Vector2 position, Rectangle worldBounds) : base(texture, position)
    {
        _worldBounds = worldBounds;
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Game1.Input.Keyboard;
        var direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.Right)) direction.X += 1;
        if (keyboard.IsKeyDown(Keys.Left)) direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.Up)) direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.Down)) direction.Y += 1;

        if (keyboard.WasKeyJustPressed(Keys.Space))
        {
            // Implement action for space key here
        }

        // Keep the player fully inside the world
        var newPosition = Position + direction * SPEED;
        newPosition.X = MathHelper.Clamp(newPosition.X, _worldBounds.Left, _worldBounds.Right - SPRITE_WIDTH);
        newPosition.Y = MathHelper.Clamp(newPosition.Y, _worldBounds.Top, _worldBounds.Bottom - SPRITE_HEIGHT);
        Position = newPosition;
    }
}
