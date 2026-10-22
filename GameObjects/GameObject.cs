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

    protected GameObject(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;
    }

    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Bounds, Color.White);
    }
}
