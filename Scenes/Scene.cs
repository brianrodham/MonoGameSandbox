using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

/// <summary>
/// One self-contained area or screen of the game (overworld, house interior,
/// title screen...). Each scene has its own ContentManager, so everything it
/// loaded is freed when the scene is left.
/// </summary>
public abstract class Scene
{
    protected Game1 Game { get; }
    protected ContentManager Content { get; }
    protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

    protected Scene(Game1 game)
    {
        Game = game;
        Content = new ContentManager(game.Services, game.Content.RootDirectory);
    }

    public virtual void LoadContent() { }

    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}
