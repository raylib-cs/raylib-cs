using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// N-patch layout
/// </summary>
public enum NPatchLayout
{
    /// <summary>
    /// Npatch defined by 3x3 tiles
    /// </summary>
    NinePatch = 0,

    /// <summary>
    /// Npatch defined by 1x3 tiles
    /// </summary>
    ThreePatchVertical,

    /// <summary>
    /// Npatch defined by 3x1 tiles
    /// </summary>
    ThreePatchHorizontal
}

/// <summary>
/// N-Patch layout info
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NPatchInfo
{
    /// <summary>
    /// Texture source rectangle
    /// </summary>
    public Rectangle Source;

    /// <summary>
    /// Left border offset
    /// </summary>
    public int Left;

    /// <summary>
    /// Top border offset
    /// </summary>
    public int Top;

    /// <summary>
    /// Right border offset
    /// </summary>
    public int Right;

    /// <summary>
    /// Bottom border offset
    /// </summary>
    public int Bottom;

    /// <summary>
    /// Layout of the n-patch: 3x3, 1x3 or 3x1
    /// </summary>
    public NPatchLayout Layout;
}
