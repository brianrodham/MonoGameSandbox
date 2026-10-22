using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameSandbox;

public class NPC : InteractableObject
{
    public NPC(Texture2D texture, Vector2 position, string[] firstLines, string[] repeatLines)
        : base(texture, position, firstLines, repeatLines)
    {
    }
}