using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;

namespace MonoGameSandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private readonly List<GameObject> gameObjects = new List<GameObject>();

    private Player player;
    private SpriteFont font;
    private DialogueBox dialogue;

    // The interactable the player is currently close enough to use (null if none)
    private IInteractable focusedInteractable;

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

        // Load each asset once here, then hand textures to the objects that use them
        var characterTexture = Content.Load<Texture2D>("Character");
        var chestTexture = Content.Load<Texture2D>("InteractableObject");
        font = Content.Load<SpriteFont>("GameFont");
        dialogue = new DialogueBox(GraphicsDevice, font);

        player = new Player(characterTexture, Vector2.Zero, GraphicsDevice.Viewport.Bounds);

        gameObjects.Add(player);
        gameObjects.Add(new InteractableObject(chestTexture, new Vector2(300, 200),
            ["You open the chest.", "Inside you find 10 gold coins!"],
            ["The chest is empty."]));
        gameObjects.Add(new InteractableObject(chestTexture, new Vector2(400, 400),
            ["The lid is stuck... you pry it open with a loud creak. Dust pours out from inside, along with a faded note that reads: \"Whoever finds this, the treasure is hidden somewhere else entirely. Better luck next time.\""],
            ["Just dust and a rude note."]));
        gameObjects.Add(new InteractableObject(chestTexture, new Vector2(500, 100),
            ["You found a health potion!"],
            ["Nothing else in here."]));
        gameObjects.Add(new NPC(characterTexture, new Vector2(200, 300),
            ["Hello there, traveler!"],
            ["Good to see you again."]));
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);

        if (Input.GamePads[0].IsButtonDown(Buttons.Back) || Input.Keyboard.IsKeyDown(Keys.Escape))
            Exit();

        // While dialogue is open it takes all input and the world is paused
        if (dialogue.IsOpen)
        {
            dialogue.Update(gameTime);
            base.Update(gameTime);
            return;
        }

        foreach (var obj in gameObjects)
        {
            obj.Update(gameTime);
        }
        gameObjects.RemoveAll(x => !x.IsActive);

        focusedInteractable = gameObjects
            .OfType<IInteractable>()
            .FirstOrDefault(i => player.InteractionRange.Intersects(i.Bounds));

        if (focusedInteractable != null && Input.Keyboard.WasKeyJustPressed(Keys.E))
        {
            focusedInteractable.OnInteract(dialogue);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // World pass: sort by the bottom edge so objects lower on screen draw in front
        _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp);
        foreach (var obj in gameObjects.OrderBy(o => o.Bounds.Bottom))
        {
            obj.Draw(_spriteBatch);
        }
        _spriteBatch.End();

        // UI pass: kept separate so a future camera transform won't move UI
        _spriteBatch.Begin();
        if (focusedInteractable != null && !dialogue.IsOpen)
        {
            var bounds = focusedInteractable.Bounds;
            _spriteBatch.DrawString(font, focusedInteractable.InteractionPrompt, new Vector2(bounds.X - 20, bounds.Y - 20), Color.White);
        }
        dialogue.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
