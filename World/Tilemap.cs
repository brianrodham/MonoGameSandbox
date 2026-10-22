using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

/// <summary>
/// A grid of tile IDs drawn from a tileset image. Tile ID n is the n-th tile
/// in the tileset, counting left to right, then top to bottom.
/// </summary>
public class Tilemap
{
    private readonly int[,] _tiles;
    private readonly Texture2D _tileset;
    private readonly int _tilesetColumns;

    public int TileSize { get; }
    public int Width => _tiles.GetLength(0);
    public int Height => _tiles.GetLength(1);

    /// <summary>The whole map in world pixels.</summary>
    public Rectangle Bounds => new Rectangle(0, 0, Width * TileSize, Height * TileSize);

    public Tilemap(Texture2D tileset, int tileSize, int[,] tiles)
    {
        _tileset = tileset;
        _tiles = tiles;
        TileSize = tileSize;
        _tilesetColumns = tileset.Width / tileSize;
    }

    /// <summary>
    /// Draws only the tiles that overlap <paramref name="visibleArea"/>, so the
    /// cost stays the same however large the map is.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Rectangle visibleArea)
    {
        int firstX = Math.Max(0, visibleArea.Left / TileSize);
        int firstY = Math.Max(0, visibleArea.Top / TileSize);
        int lastX = Math.Min(Width - 1, visibleArea.Right / TileSize);
        int lastY = Math.Min(Height - 1, visibleArea.Bottom / TileSize);

        for (int y = firstY; y <= lastY; y++)
        {
            for (int x = firstX; x <= lastX; x++)
            {
                int id = _tiles[x, y];
                var source = new Rectangle(
                    id % _tilesetColumns * TileSize, id / _tilesetColumns * TileSize, TileSize, TileSize);
                var destination = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                spriteBatch.Draw(_tileset, destination, source, Color.White);
            }
        }
    }
}
