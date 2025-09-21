using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// RenderTexture2D type, for texture rendering
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct RenderTexture2D
{
    /// <summary>
    /// OpenGL Framebuffer Object (FBO) id
    /// </summary>
    public uint Id;

    /// <summary>
    ///  Color buffer attachment texture
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// Depth buffer attachment texture
    /// </summary>
    public Texture2D Depth;
}
