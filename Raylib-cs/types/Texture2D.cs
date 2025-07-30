using System.Runtime.InteropServices;
using System.Numerics;

namespace Raylib_cs;

/// <summary>
/// Texture parameters: filter mode<br/>
/// NOTE 1: Filtering considers mipmaps if available in the texture<br/>
/// NOTE 2: Filter is accordingly set for minification and magnification
/// </summary>
public enum TextureFilter
{
    /// <summary>
    /// No filter, just pixel aproximation
    /// </summary>
    Point = 0,

    /// <summary>
    /// Linear filtering
    /// </summary>
    Bilinear,

    /// <summary>
    /// Trilinear filtering (linear with mipmaps)
    /// </summary>
    Trilinear,

    /// <summary>
    /// Anisotropic filtering 4x
    /// </summary>
    Anisotropic4X,

    /// <summary>
    /// Anisotropic filtering 8x
    /// </summary>
    Anisotropic8X,

    /// <summary>
    /// Anisotropic filtering 16x
    /// </summary>
    Anisotropic16X,
}

/// <summary>
/// Texture parameters: wrap mode
/// </summary>
public enum TextureWrap
{
    /// <summary>
    /// Repeats texture in tiled mode
    /// </summary>
    Repeat = 0,

    /// <summary>
    /// Clamps texture to edge pixel in tiled mode
    /// </summary>
    Clamp,

    /// <summary>
    /// Mirrors and repeats the texture in tiled mode
    /// </summary>
    MirrorRepeat,

    /// <summary>
    /// Mirrors and clamps to border the texture in tiled mode
    /// </summary>
    MirrorClamp
}

/// <summary>
/// Cubemap layouts
/// </summary>
public enum CubemapLayout
{
    /// <summary>
    /// Automatically detect layout type
    /// </summary>
    AutoDetect = 0,

    /// <summary>
    /// Layout is defined by a vertical line with faces
    /// </summary>
    LineVertical,

    /// <summary>
    /// Layout is defined by a horizontal line with faces
    /// </summary>
    LineHorizontal,

    /// <summary>
    /// Layout is defined by a 3x4 cross with cubemap faces
    /// </summary>
    CrossThreeByFour,

    /// <summary>
    /// Layout is defined by a 4x3 cross with cubemap faces
    /// </summary>
    CrossFourByThree
}

/// <summary>
/// Texture2D type<br/>
/// NOTE: Data stored in GPU memory
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Texture2D
{
    /// <summary>
    /// OpenGL texture id
    /// </summary>
    public uint Id;

    /// <summary>
    /// Texture base width
    /// </summary>
    public int Width;

    /// <summary>
    /// Texture base height
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
    public readonly CBool IsValid => Raylib.IsTextureValid(this);

    public static Texture2D Load(string fileName)
    {
        return Raylib.LoadTexture(fileName);
    }

    public static Texture2D LoadFromImage(Image image)
    {
        return Raylib.LoadTextureFromImage(image);
    }

    public static Texture2D LoadFromMemory(string fileType, byte[] fileData)
    {
        Image img = Raylib.LoadImageFromMemory(fileType, fileData);
        Texture2D texture = Raylib.LoadTextureFromImage(img);
        Raylib.UnloadImage(img);
        return texture;
    }

    public void Unload()
    {
        Raylib.UnloadTexture(this);
    }

    public void SetFilter(TextureFilter filter)
    {
        Raylib.SetTextureFilter(this, filter);
    }

    public void SetWrap(TextureWrap wrap)
    {
        Raylib.SetTextureWrap(this, wrap);
    }

    public void Update<T>(T[] pixels) where T : unmanaged
    {
        Raylib.UpdateTexture(this, pixels);
    }

    public readonly void Draw(int x, int y)
    {
        Raylib.DrawTexture(this, x, y, Color.White);
    }

    public readonly void Draw(int x, int y, Color color)
    {
        Raylib.DrawTexture(this, x, y, color);
    }

    public readonly void Draw(Vector2 position)
    {
        Raylib.DrawTextureV(this, position, Color.White);
    }

    public readonly void Draw(Vector2 position, Color color)
    {
        Raylib.DrawTextureV(this, position, color);
    }

    public readonly void Draw(Vector2 position, Vector2 origin)
    {
        Vector2 size = this.Dimensions;
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        Raylib.DrawTexturePro(this, source, target, origin, 0, Color.White);
    }

    public readonly void Draw(Vector2 position, Vector2 origin, Color color)
    {
        Vector2 size = this.Dimensions;
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        Raylib.DrawTexturePro(this, source, target, origin, 0, color);
    }

    public readonly void Draw(Vector2 position, float rotation, float scale)
    {
        Raylib.DrawTextureEx(this, position, rotation, scale, Color.White);
    }

    public readonly void Draw(Vector2 position, float rotation, float scale, Color color)
    {
        Raylib.DrawTextureEx(this, position, rotation, scale, color);
    }

    public readonly void Draw(Vector2 position, float rotation, Vector2 scale)
    {
        Draw(position, rotation, scale, Color.White);
    }

    public readonly void Draw(Vector2 position, float rotation, Vector2 scale, Color color)
    {
        Vector2 size = this.Dimensions;
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        target.Size *= scale;
        Raylib.DrawTexturePro(this, source, target, Vector2.Zero, rotation, color);
    }

    public readonly void Draw(Rectangle source, Rectangle dest)
    {
        Raylib.DrawTexturePro(this, source, dest, Vector2.Zero, 0, Color.White);
    }

    public readonly void Draw(Rectangle source, Rectangle dest, Vector2 origin)
    {
        Raylib.DrawTexturePro(this, source, dest, origin, 0, Color.White);
    }

    public readonly void Draw(Rectangle source, Rectangle dest, Vector2 origin, float rotation)
    {
        Raylib.DrawTexturePro(this, source, dest, origin, rotation, Color.White);
    }

    public readonly void Draw(Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color color)
    {
        Raylib.DrawTexturePro(this, source, dest, origin, rotation, color);
    }
}
