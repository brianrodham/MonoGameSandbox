using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox;

public class Player
{

    private const int SPRITE_WIDTH = 70;
    private const int SPRITE_HEIGHT = 100;
    private const int SPEED = 5;

    public Rectangle CharacterRectangle { get; set; }
    public Texture2D SpriteTexture { get; set; }

    public Player(ContentManager content)
    {
        SpriteTexture = content.Load<Texture2D>("Character");
        CharacterRectangle = new Rectangle(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT);
    }


    public void Move(KeyboardState state)
    {
        if (state.IsKeyDown(Keys.Right))
        {
            MoveRight();
        }
        if (state.IsKeyDown(Keys.Left))
        {
            MoveLeft();
        }
        if (state.IsKeyDown(Keys.Up))
        {
            MoveUp();
        }
        if (state.IsKeyDown(Keys.Down))
        {
            MoveDown();
        }
    }

    private void MoveRight()
    {
        if (CharacterRectangle.Right >= 820 )
        {
            return;
        }
        var rect = CharacterRectangle;
        rect.X += SPEED;
        CharacterRectangle = rect;
    }

    private void MoveLeft()
    {
        if (CharacterRectangle.Left <= -20)
        {
            return;
        }
        var rect = CharacterRectangle;
        rect.X -= SPEED;
        CharacterRectangle = rect;
    }

    private void MoveUp()
    {
        if (CharacterRectangle.Top <= -20)
        {
            return;
        }
        var rect = CharacterRectangle;
        rect.Y -= SPEED;
        CharacterRectangle = rect;
    }

    private void MoveDown()
    {
        if (CharacterRectangle.Bottom >= 600 - SPRITE_HEIGHT)
        {
            return;
        }
        var rect = CharacterRectangle;
        rect.Y += SPEED;
        CharacterRectangle = rect;
    }


}