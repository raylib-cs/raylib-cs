using System.Runtime.InteropServices;
using System.Numerics;

namespace Raylib_cs;

/// <summary>
/// Pixel formats<br/>
/// NOTE: Support depends on OpenGL version and platform
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// 8 bit per pixel (no alpha)
    /// </summary>
    UncompressedGrayscale = 1,

    /// <summary>
    /// 8*2 bpp (2 channels)
    /// </summary>
    UncompressedGrayAlpha,

    /// <summary>
    /// 16 bpp
    /// </summary>
    UncompressedR5G6B5,

    /// <summary>
    /// 24 bpp
    /// </summary>
    UncompressedR8G8B8,

    /// <summary>
    /// 16 bpp (1 bit alpha)
    /// </summary>
    UncompressedR5G5B5A1,

    /// <summary>
    /// 16 bpp (4 bit alpha)
    /// </summary>
    UncompressedR4G4B4A4,

    /// <summary>
    /// 32 bpp
    /// </summary>
    UncompressedR8G8B8A8,

    /// <summary>
    /// 32 bpp (1 channel - float)
    /// </summary>
    UncompressedR32,

    /// <summary>
    /// 32*3 bpp (3 channels - float)
    /// </summary>
    UncompressedR32G32B32,

    /// <summary>
    /// 32*4 bpp (4 channels - float)
    /// </summary>
    UncompressedR32G32B32A32,

    /// <summary>
    /// 16 bpp (1 channel - half float)
    /// </summary>
    UncompressedR16,

    /// <summary>
    /// 16*3 bpp (3 channels - half float)
    /// </summary>
    UncompressedR16G16B16,

    /// <summary>
    /// 16*4 bpp (4 channels - half float)
    /// </summary>
    UncompressedR16G16B16A16,

    /// <summary>
    /// 4 bpp (no alpha)
    /// </summary>
    CompressedDxt1Rgb,

    /// <summary>
    /// 4 bpp (1 bit alpha)
    /// </summary>
    CompressedDxt1Rgba,

    /// <summary>
    /// 8 bpp
    /// </summary>
    CompressedDxt3Rgba,

    /// <summary>
    /// 8 bpp
    /// </summary>
    CompressedDxt5Rgba,

    /// <summary>
    /// 4 bpp
    /// </summary>
    CompressedEtc1Rgb,

    /// <summary>
    /// 4 bpp
    /// </summary>
    CompressedEtc2Rgb,

    /// <summary>
    /// 8 bpp
    /// </summary>
    CompressedEtc2EacRgba,

    /// <summary>
    /// 4 bpp
    /// </summary>
    CompressedPvrtRgb,

    /// <summary>
    /// 4 bpp
    /// </summary>
    CompressedPvrtRgba,

    /// <summary>
    /// 8 bpp
    /// </summary>
    CompressedAstc4X4Rgba,

    /// <summary>
    /// 2 bpp
    /// </summary>
    CompressedAstc8X8Rgba
}

/// <summary>
/// Image, pixel data stored in CPU memory (RAM)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Image
{
    /// <summary>
    /// Image raw data
    /// </summary>
    public void* Data;

    /// <summary>
    /// Image base width
    /// </summary>
    public int Width;

    /// <summary>
    /// Image base height
    /// </summary>
    public int Height;

    /// <summary>
    /// Mipmap levels, 1 by default
    /// </summary>
    public int Mipmaps;

    /// <summary>
    /// Data format (PixelFormat type)
    /// </summary>
    public PixelFormat Format;

    /// <summary>
    /// Get width and height packed in a Vector2
    /// </summary>
    public readonly Vector2 Dimensions => new Vector2(Width, Height);
    public readonly CBool IsValid => Raylib.IsImageValid(this);

    public static Image Load(string fileName)
    {
        return Raylib.LoadImage(fileName);
    }

    /// <summary>
    /// Create an image from text using the default font
    /// </summary>
    public static Image CreateFromText(string text, int fontSize, Color color)
    {
        return Raylib.ImageText(text, fontSize, color);
    }

    public static Image CreateFromText(Font font, string text, int fontSize, float spacing, Color color)
    {
        return Raylib.ImageTextEx(font, text, fontSize, spacing, color);
    }

    public static Image LoadFromMemory(string fileType, byte[] data)
    {
        return Raylib.LoadImageFromMemory(fileType, data);
    }

    public static Image LoadFromTexture(Texture2D texture)
    {
        return Raylib.LoadImageFromTexture(texture);
    }

    public static Image LoadRaw(string fileName, int width, int height, PixelFormat format, int headerSize)
    {
        return Raylib.LoadImageRaw(fileName, width, height, format, headerSize);
    }

    public readonly void Unload()
    {
        Raylib.UnloadImage(this);
    }

    public readonly unsafe Color[] GetPalette(int maxPaletteSize)
    {
        int colorCount = 0;
        Color* colors = Raylib.LoadImagePalette(this, maxPaletteSize, &colorCount);
        Color[] palette = new Color[colorCount];
        for (int i = 0; i < colorCount; i++)
        {
            palette[i] = colors[i];
        }
        Raylib.UnloadImagePalette(colors);
        return palette;
    }

    public static Image LoadFromScreen()
    {
        return Raylib.LoadImageFromScreen();
    }

    public void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        Raylib.ImageDrawLineV(ref this, start, end, color);
    }

    public void DrawLine(Vector2 start, Vector2 end, int thickness, Color color)
    {
        Raylib.ImageDrawLineEx(ref this, start, end, thickness, color);
    }

    public void DrawCircle(Vector2 position, float radius, Color color)
    {
        Raylib.ImageDrawCircle(ref this, (int)position.X, (int)position.Y, (int)radius, color);
    }

    public void DrawRectangle(Rectangle rectangle, Color color)
    {
        Raylib.ImageDrawRectangleRec(ref this, rectangle, color);
    }

    public void DrawText(string text, Vector2 position, int fontSize, Color color)
    {
        Raylib.ImageDrawText(ref this, text, (int)position.X, (int)position.Y, fontSize, color);
    }

    public void DrawText(Font font, string text, Vector2 position, int fontSize, float spacing, Color color)
    {
        Raylib.ImageDrawTextEx(ref this, font, text, position, fontSize, spacing, color);
    }

    public unsafe void DrawCircleLines(Vector2 position, float radius, Color color)
    {
        Raylib.ImageDrawCircleLinesV(ref this, position, (int)radius, color);
    }

    public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
    {
        Raylib.ImageDrawTriangle(ref this, p1, p2, p3, color);
    }

    public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color c1, Color c2, Color c3)
    {
        Raylib.ImageDrawTriangleEx(ref this, p1, p2, p3, c1, c2, c3);
    }

    public void DrawRectangleLines(Rectangle rectangle, Color color)
    {
        Raylib.ImageDrawRectangleLines(ref this, rectangle, 1, color);
    }

    public void DrawRectangleLines(Rectangle rectangle, int thickness, Color color)
    {
        Raylib.ImageDrawRectangleLines(ref this, rectangle, thickness, color);
    }
}
