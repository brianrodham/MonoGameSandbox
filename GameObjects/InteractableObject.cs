using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

public class InteractableObject : GameObject, IInteractable
{
    private const int SIZE = 32;

    private readonly string[] _firstLines;
    private readonly string[] _repeatLines;

    public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, SIZE, SIZE);
    public bool IsInteractedWith { get; private set; }
    public string InteractionPrompt => "Press [E] to Interact";

    /// <param name="firstLines">Shown the first time the player interacts.</param>
    /// <param name="repeatLines">Shown on every later interaction.</param>
    public InteractableObject(Texture2D texture, Vector2 position, string[] firstLines, string[] repeatLines)
        : base(texture, position)
    {
        _firstLines = firstLines;
        _repeatLines = repeatLines;
    }

    public void OnInteract(DialogueBox dialogue)
    {
        dialogue.Show(IsInteractedWith ? _repeatLines : _firstLines);
        IsInteractedWith = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color objectColor = IsInteractedWith ? Color.Green : Color.Red;
        spriteBatch.Draw(Texture, Bounds, objectColor);
    }
}
