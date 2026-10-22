using Microsoft.Xna.Framework;

namespace MonoGameSandbox;

public interface IInteractable
{
    Rectangle Bounds { get; }
    string InteractionPrompt { get; }
    void OnInteract(DialogueBox dialogue);
}
