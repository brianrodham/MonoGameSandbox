using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

/// <summary>
/// Plays an <see cref="Animation"/>, advancing frames over time.
/// </summary>
public class AnimatedSprite
{
    private int _frameIndex;
    private TimeSpan _elapsed;

    public Animation Current { get; private set; }

    /// <summary>True once a non-looping animation has shown its last frame for its full duration.</summary>
    public bool IsFinished { get; private set; }

    public Rectangle SourceRectangle => Current.Frames[_frameIndex];

    public AnimatedSprite(Animation initial)
    {
        Play(initial);
    }

    /// <summary>
    /// Switches to the given animation. Calling this every frame with the same
    /// animation is safe: it only restarts when the animation actually changes.
    /// </summary>
    public void Play(Animation animation)
    {
        if (animation == Current) return;

        Current = animation;
        _frameIndex = 0;
        _elapsed = TimeSpan.Zero;
        IsFinished = false;
    }

    public void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        _elapsed += gameTime.ElapsedGameTime;
        while (_elapsed >= Current.FrameDuration)
        {
            _elapsed -= Current.FrameDuration;

            if (_frameIndex < Current.Frames.Length - 1)
            {
                _frameIndex++;
            }
            else if (Current.IsLooping)
            {
                _frameIndex = 0;
            }
            else
            {
                IsFinished = true;
                break;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color, SpriteEffects effects = SpriteEffects.None)
    {
        spriteBatch.Draw(Current.Texture, destination, SourceRectangle, color, 0f, Vector2.Zero, effects, 0f);
    }
}
