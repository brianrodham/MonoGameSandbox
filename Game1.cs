using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;

namespace MonoGameSandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Scene _currentScene;
    private Scene _nextScene;

    public static InputManager Input { get; private set; }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Input = new InputManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ChangeScene(new OverworldScene(this));
    }

    /// <summary>
    /// Switches to a new scene at the start of the next frame, so a scene can
    /// safely call this in the middle of its own Update.
    /// </summary>
    public void ChangeScene(Scene scene)
    {
        _nextScene = scene;
    }

    protected override void Update(GameTime gameTime)
    {
        if (_nextScene != null)
        {
            _currentScene?.UnloadContent();
            _currentScene = _nextScene;
            _nextScene = null;
            _currentScene.LoadContent();
        }

        Input.Update(gameTime);

        if (Input.GamePads[0].IsButtonDown(Buttons.Back) || Input.Keyboard.IsKeyDown(Keys.Escape))
            Exit();

        _currentScene.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _currentScene?.Draw(_spriteBatch);

        base.Draw(gameTime);
    }
}
