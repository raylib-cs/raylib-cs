using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Transform, vertex transformation data
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Transform
{
    /// <summary>
    /// Translation
    /// </summary>
    public Vector3 Translation;

    /// <summary>
    /// Rotation
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// Scale
    /// </summary>
    public Vector3 Scale;

    public void TranslateLocal(Vector3 translation)
    {
        Translation += Vector3.Transform(translation, Rotation);
    }

    public void TranslateGlobal(Vector3 translation)
    {
        Translation += translation;
    }
}
