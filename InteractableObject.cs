using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public  class InteractableObject
{
    public Vector2 Position { get; private set; }
    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
    public bool IsInteractedWith { get; private set; }
    
    private Texture2D spriteTexture;
    private ContentManager Content { get; set; }

    public InteractableObject(Vector2 position, ContentManager content)
    {
        Position = position;
        IsInteractedWith = false;
        Content = content;
        spriteTexture = Content.Load<Texture2D>("InteractableObject");

    }

    public void OnInteract()
    {
        // Toggle state or trigger an event/dialogue
        IsInteractedWith = !IsInteractedWith;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, bool playerIsNear)
    {
        // Draw the object
        Color objectColor = IsInteractedWith ? Color.Green : Color.Red;
        spriteBatch.Draw(spriteTexture, Bounds, objectColor);

        // UI Prompt: If the player is near, display an interaction hint
        if (playerIsNear)
        {
            spriteBatch.DrawString(font, "Press [E] to Interact", new Vector2(Position.X - 20, Position.Y - 20), Color.White);
        }
    }
}
