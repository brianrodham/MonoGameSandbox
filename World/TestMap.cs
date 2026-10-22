using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

/// <summary>
/// Builds a placeholder map and tileset in code, so scrolling can be tested
/// before real tile art exists. Replace with a Tiled map later.
/// </summary>
public static class TestMap
{
    public const int TILE_SIZE = 16;

    private const int GRASS_LIGHT = 0;
    private const int GRASS_DARK = 1;
    private const int PATH = 2;
    private const int WATER = 3;

    private static readonly Color[] TileColors =
    {
        new Color(106, 190, 48),   // GRASS_LIGHT
        new Color(91, 168, 40),    // GRASS_DARK
        new Color(194, 160, 106),  // PATH
        new Color(48, 96, 180),    // WATER
    };

    /// <summary>
    /// The returned tileset texture was created in code rather than loaded through
    /// a ContentManager, so the caller must Dispose it.
    /// </summary>
    public static Tilemap Create(GraphicsDevice graphicsDevice, int width, int height, out Texture2D tileset)
    {
        tileset = CreateTileset(graphicsDevice);

        var tiles = new int[width, height];
        int pathX = width / 2;
        int pathY = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isEdge = x < 2 || y < 2 || x >= width - 2 || y >= height - 2;
                bool isPath = x == pathX || x == pathX + 1 || y == pathY || y == pathY + 1;

                if (isEdge) tiles[x, y] = WATER;
                else if (isPath) tiles[x, y] = PATH;
                else tiles[x, y] = (x + y) % 2 == 0 ? GRASS_LIGHT : GRASS_DARK;
            }
        }

        return new Tilemap(tileset, TILE_SIZE, tiles);
    }

    private static Texture2D CreateTileset(GraphicsDevice graphicsDevice)
    {
        int width = TILE_SIZE * TileColors.Length;
        var pixels = new Color[width * TILE_SIZE];

        for (int tile = 0; tile < TileColors.Length; tile++)
        {
            for (int y = 0; y < TILE_SIZE; y++)
            {
                for (int x = 0; x < TILE_SIZE; x++)
                {
                    // A few darker speckles so movement is easy to see
                    bool speckle = (x * 7 + y * 13 + tile * 5) % 23 == 0;
                    var color = speckle ? TileColors[tile] * 0.8f : TileColors[tile];
                    color.A = 255;
                    pixels[y * width + tile * TILE_SIZE + x] = color;
                }
            }
        }

        var texture = new Texture2D(graphicsDevice, width, TILE_SIZE);
        texture.SetData(pixels);
        return texture;
    }
}
