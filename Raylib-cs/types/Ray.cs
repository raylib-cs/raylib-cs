using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Ray, ray for raycasting
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Ray
{
    /// <summary>
    /// Ray position (origin)
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Ray direction (normalized)
    /// </summary>
    public Vector3 Direction;

    public Ray(Vector3 position, Vector3 direction)
    {
        this.Position = position;
        this.Direction = direction;
    }
}

/// <summary>
/// Raycast hit information
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct RayCollision
{
    /// <summary>
    /// Did the ray hit something?
    /// </summary>
    public CBool Hit;

    /// <summary>
    /// Distance to the nearest hit
    /// </summary>
    public float Distance;

    /// <summary>
    /// Point of the nearest hit
    /// </summary>
    public Vector3 Point;

    /// <summary>
    /// Surface normal of hit
    /// </summary>
    public Vector3 Normal;
}
