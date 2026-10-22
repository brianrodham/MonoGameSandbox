using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox;

public class OverworldScene : Scene
{
    private const int CAMERA_ZOOM = 3;
    private const int MAP_WIDTH_TILES = 60;
    private const int MAP_HEIGHT_TILES = 40;

    private readonly List<GameObject> gameObjects = new List<GameObject>();

    private Camera2D camera;
    private Tilemap map;
    private Texture2D tileset;
    private Player player;
    private SpriteFont font;
    private DialogueBox dialogue;

    // The interactable the player is currently close enough to use (null if none)
    private IInteractable focusedInteractable;

    public OverworldScene(Game1 game) : base(game)
    {
    }

    public override void LoadContent()
    {
        map = TestMap.Create(GraphicsDevice, MAP_WIDTH_TILES, MAP_HEIGHT_TILES, out tileset);
        camera = new Camera2D(GraphicsDevice.Viewport.Bounds, CAMERA_ZOOM);

        // Load each asset once here, then hand textures to the objects that use them
        var npcSheet = Content.Load<Texture2D>("npc_spritesheet");
        var chestTexture = Content.Load<Texture2D>("InteractableObject");
        var playerSheet = Content.Load<Texture2D>("character_spritesheet");
        font = Content.Load<SpriteFont>("GameFont");
        dialogue = new DialogueBox(GraphicsDevice, font);

        // Keep the player off the water border until there's real tile collision
        var walkableArea = map.Bounds;
        walkableArea.Inflate(-map.TileSize * 2, -map.TileSize * 2);
        player = new Player(playerSheet, new Vector2(440, 280), walkableArea);

        gameObjects.Add(player);
        gameObjects.Add(new InteractableObject(chestTexture, new Vector2(300, 200),
            ["You open the chest.", "Inside you find 10 gold coins!"],
            ["The chest is empty."]));
        gameObjects.Add(new InteractableObject(chestTexture, new Vector2(700, 450),
            ["The lid is stuck... you pry it open with a loud creak. Dust pours out from inside, along with a faded note that reads: \"Whoever finds this, the treasure is hidden somewhere else entirely. Better luck next time.\""],
            ["Just dust and a rude note."]));
        gameObjects.Add(new InteractableObject(chestTexture, new Vector2(150, 500),
            ["You found a health potion!"],
            ["Nothing else in here."]));
        gameObjects.Add(new NPC(npcSheet, new Vector2(520, 260),
            ["Hello there, traveler!"],
            ["Good to see you again."]));

        camera.Follow(player.Bounds.Center.ToVector2(), map.Bounds);
    }

    public override void UnloadContent()
    {
        // These were created in code, not loaded by Content, so they're freed by hand
        tileset.Dispose();
        dialogue.Dispose();
        base.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // Animations always run, so characters keep moving (e.g. talking) while dialogue is open
        foreach (var obj in gameObjects)
        {
            obj.UpdateAnimation(gameTime);
        }

        // While dialogue is open it takes all input and the rest of the world is paused
        if (dialogue.IsOpen)
        {
            dialogue.Update(gameTime);
            return;
        }

        foreach (var obj in gameObjects)
        {
            obj.Update(gameTime);
        }
        gameObjects.RemoveAll(x => !x.IsActive);

        foreach (var npc in gameObjects.OfType<NPC>())
        {
            npc.NoticePlayer(player);
        }

        camera.Follow(player.Bounds.Center.ToVector2(), map.Bounds);

        focusedInteractable = gameObjects
            .OfType<IInteractable>()
            .FirstOrDefault(i => player.InteractionRange.Intersects(i.Bounds));

        if (focusedInteractable != null && Game1.Input.Keyboard.WasKeyJustPressed(Keys.E))
        {
            player.Halt();
            focusedInteractable.OnInteract(dialogue);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // World pass: everything is positioned and scaled through the camera
        var visibleArea = camera.VisibleArea;
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);
        map.Draw(spriteBatch, visibleArea);
        foreach (var obj in gameObjects.Where(o => o.Bounds.Intersects(visibleArea)).OrderBy(o => o.Bounds.Bottom))
        {
            obj.Draw(spriteBatch);
        }
        spriteBatch.End();

        // UI pass: no camera, so this stays fixed on screen
        spriteBatch.Begin();
        if (focusedInteractable != null && !dialogue.IsOpen)
        {
            DrawPrompt(spriteBatch, focusedInteractable);
        }
        dialogue.Draw(spriteBatch);
        spriteBatch.End();
    }

    /// <summary>Draws the prompt centred above the interactable, converting its world position to the screen.</summary>
    private void DrawPrompt(SpriteBatch spriteBatch, IInteractable interactable)
    {
        var bounds = interactable.Bounds;
        var anchor = camera.WorldToScreen(new Vector2(bounds.Center.X, bounds.Top));
        var textSize = font.MeasureString(interactable.InteractionPrompt);
        var position = new Vector2(anchor.X - textSize.X / 2f, anchor.Y - textSize.Y - 4);
        spriteBatch.DrawString(font, interactable.InteractionPrompt, Vector2.Round(position), Color.White);
    }
}
