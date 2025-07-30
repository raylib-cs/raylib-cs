using System.Collections.Generic;
using System.Numerics;

namespace Raylib_cs;

public static class RaylibExtensions
{
    public static void DrawLines(this IEnumerable<Vector2> points, Color color)
    {
        Vector2[] pointArray = System.Linq.Enumerable.ToArray<Vector2>(points);
        Raylib.DrawLineStrip(pointArray, pointArray.Length, color);
    }

    public static void Log(this TraceLogLevel logLevel, string message)
    {
        Raylib.TraceLog(logLevel, message);
    }

    public static byte Lerp(this byte a, byte b, float t)
    {
        return (byte)(a + (b - a) * t);
    }

    #region Vector

    public static void Normalize(this ref Vector2 vector)
    {
        vector = Raymath.Vector2Normalize(vector);
    }

    public static void Normalize(this ref Vector3 vector)
    {
        vector = Raymath.Vector3Normalize(vector);
    }

    public static void Add(this ref Vector2 vector, float value)
    {
        vector = Raymath.Vector2AddValue(vector, value);
    }

    public static void Add(this ref Vector3 vector, float value)
    {
        vector = Raymath.Vector3AddValue(vector, value);
    }

    public static void Add(this ref Vector2 vector, Vector2 value)
    {
        vector = Raymath.Vector2Add(vector, value);
    }

    public static void Add(this ref Vector3 vector, Vector3 value)
    {
        vector = Raymath.Vector3Add(vector, value);
    }

    public static float GetDistance(this Vector2 v1, Vector2 v2)
    {
        return Raymath.Vector2Distance(v1, v2);
    }

    public static float GetDistance(this Vector3 v1, Vector3 v2)
    {
        return Raymath.Vector3Distance(v1, v2);
    }

    public static Vector2 Lerp(this Vector2 v1, Vector2 v2, float amount)
    {
        return Raymath.Vector2Lerp(v1, v2, amount);
    }

    public static Vector3 Lerp(this Vector3 v1, Vector3 v2, float amount)
    {
        return Raymath.Vector3Lerp(v1, v2, amount);
    }

    public static Vector2 MoveTowards(this Vector2 v1, Vector2 v2, float t)
    {
        return Raymath.Vector2MoveTowards(v1, v2, t);
    }

    public static Vector3 MoveTowards(this Vector3 v1, Vector3 v2, float t)
    {
        return Raymath.Vector3MoveTowards(v1, v2, t);
    }

    #endregion
}
