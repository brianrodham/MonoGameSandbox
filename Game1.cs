using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;

namespace MonoGameSandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // The reference to the loaded sprite
    private Player player;
    private InteractableObject interactableObject;
    private SpriteFont font;
    public static InputManager Input { get; private set; }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Initialize the input manager
        Input = new InputManager();

        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here
        player = new Player(Content);
        interactableObject = new InteractableObject(new Vector2(300, 200), Content);
        font = Content.Load<SpriteFont>("GameFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        //Input.Update(gameTime);
        // TODO: Add your update logic here
        // TODO: Add your update logic here
        KeyboardState state = Keyboard.GetState();
        // Use the Player class to handle movement
        player.TakeAction(state);

        // 2. State Change Check: Was 'E' *just* pressed this frame?
        bool interactionKeyPressed = state.IsKeyDown(Keys.E);
        
        bool isNearChest = player.InteractionRange.Intersects(interactableObject.Bounds);

        if (isNearChest && interactionKeyPressed)
        {
            interactableObject.OnInteract();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.Draw(player.SpriteTexture, player.CharacterRectangle, Color.White);
        //_spriteBatch.Draw(spriteTexture, interactableObject.Bounds, Color.White);

        bool isNearChest = player.InteractionRange.Intersects(interactableObject.Bounds);
        interactableObject.Draw(_spriteBatch, font, isNearChest);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
