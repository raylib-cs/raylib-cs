using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Camera system modes
/// </summary>
public enum CameraMode
{
    Custom = 0,
    Free,
    Orbital,
    FirstPerson,
    ThirdPerson
}

/// <summary>
/// Camera projection
/// </summary>
public enum CameraProjection
{
    Perspective = 0,
    Orthographic
}

/// <summary>
/// Camera3D, defines position/orientation in 3d space
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Camera3D
{
    /// <summary>
    /// Camera position
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Camera target it looks-at
    /// </summary>
    public Vector3 Target;

    /// <summary>
    /// Camera up vector (rotation over its axis)
    /// </summary>
    public Vector3 Up;

    /// <summary>
    /// Camera field-of-view apperture in Y (degrees) in perspective, used as near plane width in orthographic
    /// </summary>
    public float FovY;

    /// <summary>
    /// Camera type, defines projection type: CAMERA_PERSPECTIVE or CAMERA_ORTHOGRAPHIC
    /// </summary>
    public CameraProjection Projection;

    public Camera3D(Vector3 position, Vector3 target, Vector3 up, float fovY, CameraProjection projection)
    {
        this.Position = position;
        this.Target = target;
        this.Up = up;
        this.FovY = fovY;
        this.Projection = projection;
    }
}
