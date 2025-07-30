using System;

namespace Raylib_cs;

public static partial class Raymath
{
    public static float Clamp01(float value) => Clamp(value, 0.0f, 1.0f);

    /// <summary>
    /// Loops the value, so that it is never larger than length and never smaller than 0
    /// </summary>
    public static float Repeat(float value, float length)
    {
        return Clamp(value - MathF.Floor(value / length) * length, 0f, length);
    }

    /// <summary>
    /// Same as Lerp but makes sure the values interpolate correctly when they wrap around
    /// 360 degrees.
    /// </summary>
    /// <param name="a">The start angle. A float expressed in degrees.</param>
    /// <param name="b">The end angle. A float expressed in degrees.</param>
    /// <param name="t">The interpolation value between the start and end angles.
    /// This value is clamped to the range [0, 1].</param>
    /// <returns>Returns the interpolated float result between angle a and angle b,
    /// based on the interpolation value t.</returns>
    public static float LerpAngle(float a, float b, float t)
    {
        float num = Repeat(b - a, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return a + num * Clamp01(t);
    }

    public static int Sign(float value) => MathF.Sign(value);

    /// <summary>
    /// Returns a value that increments and decrements between zero and the
    /// length. It follows the triangle wave formula where the bottom is set to zero
    /// and the peak is set to length.
    /// </summary>
    public static float PingPong(float t, float length)
    {
        t = Repeat(t, length * 2f);
        return length - MathF.Abs(t - length);
    }

    /// <summary>
    /// Moves a value current towards target.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="target">The value to move towards.</param>
    /// <param name="maxDelta">The maximum change applied to the current value.</param>
    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
        {
            return target;
        }

        return current + Sign(target - current) * maxDelta;
    }

    /// <summary>
    /// Calculates the shortest difference between two angles.
    /// </summary>
    /// <param name="current">The current angle in degrees.</param>
    /// <param name="target">The target angle in degrees.</param>
    /// <returns>A value between -179 and 180, in degrees.</returns>
    public static float DeltaAngle(float current, float target)
    {
        float num = Repeat(target - current, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return num;
    }

    /// <summary>
    /// Same as MoveTowards but makes sure the values interpolate correctly when they wrap around 360 degrees.
    /// </summary>
    public static float MoveTowardsAngle(float current, float target, float maxDelta)
    {
        float num = DeltaAngle(current, target);
        if (0f - maxDelta < num && num < maxDelta)
        {
            return target;
        }

        target = current + num;
        return MoveTowards(current, target, maxDelta);
    }
}
