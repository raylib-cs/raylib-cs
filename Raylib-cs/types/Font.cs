using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Font type, defines generation method
/// </summary>
public enum FontType
{
    /// <summary>
    /// Default font generation, anti-aliased
    /// </summary>
    Default = 0,

    /// <summary>
    /// Bitmap font generation, no anti-aliasing
    /// </summary>
    Bitmap,

    /// <summary>
    /// SDF font generation, requires external shader
    /// </summary>
    Sdf
}

/// <summary>
/// GlyphInfo, font characters glyphs info
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct GlyphInfo
{
    /// <summary>
    /// Character value (Unicode)
    /// </summary>
    public int Value;

    /// <summary>
    /// Character offset X when drawing
    /// </summary>
    public int OffsetX;

    /// <summary>
    /// Character offset Y when drawing
    /// </summary>
    public int OffsetY;

    /// <summary>
    /// Character advance position X
    /// </summary>
    public int AdvanceX;

    /// <summary>
    /// Character image data
    /// </summary>
    public Image Image;
}

/// <summary>
/// Font, font texture and GlyphInfo array data
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Font
{
    /// <summary>
    /// Base size (default chars height)
    /// </summary>
    public int BaseSize;

    /// <summary>
    /// Number of characters
    /// </summary>
    public int GlyphCount;

    /// <summary>
    /// Padding around the glyph characters
    /// </summary>
    public int GlyphPadding;

    /// <summary>
    /// Texture atlas containing the glyphs
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// Rectangles in texture for the glyphs
    /// </summary>
    public Rectangle* Recs;

    /// <summary>
    /// Glyphs info data
    /// </summary>
    public GlyphInfo* Glyphs;
}
