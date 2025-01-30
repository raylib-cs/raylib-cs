using System.Runtime.InteropServices;

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
public unsafe partial struct Image
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
}
