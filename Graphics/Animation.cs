using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

/// <summary>
/// A sequence of frames cut from a sprite sheet. This only describes the
/// animation; <see cref="AnimatedSprite"/> tracks playback.
/// </summary>
public class Animation
{
    public Texture2D Texture { get; }
    public Rectangle[] Frames { get; }
    public TimeSpan FrameDuration { get; }
    public bool IsLooping { get; }

    public Animation(Texture2D texture, Rectangle[] frames, TimeSpan frameDuration, bool isLooping = true)
    {
        Texture = texture;
        Frames = frames;
        FrameDuration = frameDuration;
        IsLooping = isLooping;
    }

    /// <summary>
    /// Builds an animation from one row of a sprite sheet laid out as a grid
    /// of equally sized frames.
    /// </summary>
    public static Animation FromRow(Texture2D sheet, int row, int frameCount, int frameWidth, int frameHeight,
        TimeSpan frameDuration, bool isLooping = true)
    {
        var frames = new Rectangle[frameCount];
        for (int i = 0; i < frameCount; i++)
        {
            frames[i] = new Rectangle(i * frameWidth, row * frameHeight, frameWidth, frameHeight);
        }
        return new Animation(sheet, frames, frameDuration, isLooping);
    }

    /// <summary>
    /// Builds one animation per row of a grid sprite sheet, keyed by name.
    /// Rows are read top to bottom in the order given.
    /// </summary>
    public static Dictionary<string, Animation> FromGrid(Texture2D sheet, int frameSize, int framesPerRow,
        params (string Name, int FrameMilliseconds, bool IsLooping)[] rows)
    {
        var animations = new Dictionary<string, Animation>();
        for (int row = 0; row < rows.Length; row++)
        {
            var (name, frameMilliseconds, isLooping) = rows[row];
            animations[name] = FromRow(sheet, row, framesPerRow, frameSize, frameSize,
                TimeSpan.FromMilliseconds(frameMilliseconds), isLooping);
        }
        return animations;
    }
}
