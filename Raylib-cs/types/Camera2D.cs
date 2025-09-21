using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Camera2D, defines position/orientation in 2d space
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Camera2D
{
    /// <summary>
    /// Camera offset (displacement from target)
    /// </summary>
    public Vector2 Offset;

    /// <summary>
    /// Camera target (rotation and zoom origin)
    /// </summary>
    public Vector2 Target;

    /// <summary>
    ///  Camera rotation in degrees
    /// </summary>
    public float Rotation;

    /// <summary>
    /// Camera zoom (scaling), should be 1.0f by default
    /// </summary>
    public float Zoom;

    public Camera2D(Vector2 offset, Vector2 target, float rotation, float zoom)
    {
        this.Offset = offset;
        this.Target = target;
        this.Rotation = rotation;
        this.Zoom = zoom;
    }
}
