using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

public class InteractableObject : GameObject, IInteractable
{
    private readonly string[] _firstLines;
    private readonly string[] _repeatLines;

    /// <summary>Width and height in world pixels. Override for larger objects.</summary>
    protected virtual int Size => 16;

    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Size, Size);
    public bool IsInteractedWith { get; private set; }
    public virtual string InteractionPrompt => "Press [E] to Interact";

    /// <param name="firstLines">Shown the first time the player interacts.</param>
    /// <param name="repeatLines">Shown on every later interaction.</param>
    public InteractableObject(Texture2D texture, Vector2 position, string[] firstLines, string[] repeatLines)
        : base(texture, position)
    {
        _firstLines = firstLines;
        _repeatLines = repeatLines;
    }

    public virtual void OnInteract(DialogueBox dialogue)
    {
        dialogue.Show(IsInteractedWith ? _repeatLines : _firstLines, OnDialogueClosed);
        IsInteractedWith = true;
    }

    /// <summary>Called when the dialogue started by <see cref="OnInteract"/> is dismissed.</summary>
    protected virtual void OnDialogueClosed() { }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Sprite != null)
        {
            base.Draw(spriteBatch);
            return;
        }

        // Static objects (chests) are tinted to show whether they've been used
        Color objectColor = IsInteractedWith ? Color.Green : Color.Red;
        spriteBatch.Draw(Texture, Bounds, objectColor);
    }
}
