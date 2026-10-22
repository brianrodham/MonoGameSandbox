using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox;

/// <summary>
/// A modal text box drawn at the bottom of the screen. Text is word-wrapped,
/// split into pages that fit the box, and revealed with a typewriter effect.
/// </summary>
public class DialogueBox
{
    private const float CHARS_PER_SECOND = 40f;
    private const int PADDING = 16;
    private const int HEIGHT = 120;
    private const int MARGIN = 20;
    private const string CONTINUE_HINT = "[E]";

    private readonly SpriteFont _font;
    private readonly Texture2D _pixel;
    private readonly Rectangle _box;
    private readonly Queue<string> _pages = new Queue<string>();

    private string _currentPage;
    private float _charsVisible;

    public bool IsOpen => _currentPage != null;

    public DialogueBox(GraphicsDevice graphicsDevice, SpriteFont font)
    {
        _font = font;

        // A 1x1 white texture can be stretched and tinted to draw any solid rectangle
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        var viewport = graphicsDevice.Viewport.Bounds;
        _box = new Rectangle(MARGIN, viewport.Height - HEIGHT - MARGIN, viewport.Width - MARGIN * 2, HEIGHT);
    }

    /// <summary>
    /// Queues text to display. Each string starts on a new page; long strings
    /// are automatically split across multiple pages.
    /// </summary>
    public void Show(params string[] texts)
    {
        foreach (var text in texts)
        {
            foreach (var page in Paginate(text))
            {
                _pages.Enqueue(page);
            }
        }

        if (!IsOpen)
        {
            NextPage();
        }
    }

    public void Update(GameTime gameTime)
    {
        if (!IsOpen) return;

        _charsVisible += CHARS_PER_SECOND * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Game1.Input.Keyboard.WasKeyJustPressed(Keys.E))
        {
            if (_charsVisible < _currentPage.Length)
            {
                _charsVisible = _currentPage.Length;   // first press skips the typing animation
            }
            else
            {
                NextPage();
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsOpen) return;

        spriteBatch.Draw(_pixel, _box, Color.Black * 0.8f);

        int visible = (int)MathHelper.Min(_charsVisible, _currentPage.Length);
        var textPosition = new Vector2(_box.X + PADDING, _box.Y + PADDING);
        spriteBatch.DrawString(_font, _currentPage[..visible], textPosition, Color.White);

        if (visible == _currentPage.Length)
        {
            var hintSize = _font.MeasureString(CONTINUE_HINT);
            var hintPosition = new Vector2(_box.Right - PADDING - hintSize.X, _box.Bottom - PADDING - hintSize.Y);
            spriteBatch.DrawString(_font, CONTINUE_HINT, hintPosition, Color.Gray);
        }
    }

    private void NextPage()
    {
        _currentPage = _pages.Count > 0 ? _pages.Dequeue() : null;
        _charsVisible = 0;
    }

    /// <summary>
    /// Word-wraps text to the box width, then groups the lines into pages
    /// that fit the box height.
    /// </summary>
    private IEnumerable<string> Paginate(string text)
    {
        float maxWidth = _box.Width - PADDING * 2;
        int linesPerPage = System.Math.Max(1, (_box.Height - PADDING * 2) / _font.LineSpacing);

        var lines = new List<string>();
        foreach (var paragraph in text.Split('\n'))
        {
            var line = new StringBuilder();
            foreach (var word in paragraph.Split(' '))
            {
                string candidate = line.Length == 0 ? word : line + " " + word;
                if (line.Length > 0 && _font.MeasureString(candidate).X > maxWidth)
                {
                    lines.Add(line.ToString());
                    line.Clear().Append(word);
                }
                else
                {
                    line.Clear().Append(candidate);
                }
            }
            lines.Add(line.ToString());
        }

        for (int i = 0; i < lines.Count; i += linesPerPage)
        {
            yield return string.Join("\n", lines.GetRange(i, System.Math.Min(linesPerPage, lines.Count - i)));
        }
    }
}
