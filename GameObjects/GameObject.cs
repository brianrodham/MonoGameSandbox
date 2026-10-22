using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

public abstract class GameObject
{
    public Vector2 Position { get; set; }
    public Texture2D Texture { get; protected set; }
    public virtual Rectangle Bounds =>
        new((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

    public bool IsActive { get; set; } = true;   // lets you remove/disable things cleanly

    /// <summary>Set this to animate the object. When null, <see cref="Texture"/> is drawn as a static image.</summary>
    protected AnimatedSprite Sprite { get; set; }

    /// <summary>Sprite sheets face right; this flips the sprite horizontally when drawn.</summary>
    protected bool FacingLeft { get; set; }

    protected GameObject(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;
    }

    /// <summary>Game logic: movement, AI, input. Paused while dialogue is open.</summary>
    public virtual void Update(GameTime gameTime) { }

    /// <summary>Advances animation frames. Keeps running while dialogue is open so characters stay alive.</summary>
    public virtual void UpdateAnimation(GameTime gameTime)
    {
        Sprite?.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Sprite != null)
        {
            var effects = FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Sprite.Draw(spriteBatch, Bounds, Color.White, effects);
        }
        else
        {
            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
