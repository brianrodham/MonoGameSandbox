using System;
using Microsoft.Xna.Framework;

namespace MonoGameSandbox;

/// <summary>
/// Converts world coordinates (positions on the map) to screen coordinates.
/// Pass <see cref="Transform"/> to SpriteBatch.Begin to draw the world through it.
/// </summary>
public class Camera2D
{
    private readonly Rectangle _viewport;

    /// <summary>The world position shown at the centre of the screen.</summary>
    public Vector2 Position { get; set; }

    /// <summary>Screen pixels per world pixel. Keep it a whole number for crisp pixel art.</summary>
    public int Zoom { get; }

    public Camera2D(Rectangle viewport, int zoom)
    {
        _viewport = viewport;
        Zoom = zoom;
    }

    public Matrix Transform
    {
        get
        {
            // Rounding to whole screen pixels stops sprites and tiles from shimmering
            var offset = new Vector2(
                MathF.Round(_viewport.Width / 2f - Position.X * Zoom),
                MathF.Round(_viewport.Height / 2f - Position.Y * Zoom));
            return Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(offset.X, offset.Y, 0);
        }
    }

    /// <summary>The part of the world currently on screen, used to skip drawing anything outside it.</summary>
    public Rectangle VisibleArea
    {
        get
        {
            var topLeft = ScreenToWorld(Vector2.Zero);
            var bottomRight = ScreenToWorld(new Vector2(_viewport.Width, _viewport.Height));
            return new Rectangle(
                (int)MathF.Floor(topLeft.X), (int)MathF.Floor(topLeft.Y),
                (int)MathF.Ceiling(bottomRight.X - topLeft.X) + 1, (int)MathF.Ceiling(bottomRight.Y - topLeft.Y) + 1);
        }
    }

    /// <summary>
    /// Centres the camera on a target without showing anything past the edges
    /// of <paramref name="worldBounds"/>. If the world is smaller than the screen
    /// along an axis, it is centred on that axis instead.
    /// </summary>
    public void Follow(Vector2 target, Rectangle worldBounds)
    {
        float halfWidth = _viewport.Width / 2f / Zoom;
        float halfHeight = _viewport.Height / 2f / Zoom;

        Position = new Vector2(
            ClampAxis(target.X, worldBounds.Left, worldBounds.Right, halfWidth),
            ClampAxis(target.Y, worldBounds.Top, worldBounds.Bottom, halfHeight));
    }

    public Vector2 WorldToScreen(Vector2 world) => Vector2.Transform(world, Transform);
    public Vector2 ScreenToWorld(Vector2 screen) => Vector2.Transform(screen, Matrix.Invert(Transform));

    private static float ClampAxis(float target, float min, float max, float halfView)
    {
        if (max - min <= halfView * 2) return (min + max) / 2f;
        return MathHelper.Clamp(target, min + halfView, max - halfView);
    }
}
