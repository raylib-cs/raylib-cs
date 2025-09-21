using System.Runtime.InteropServices;
using System;

namespace Raylib_cs;

/// <summary>
/// Color type, RGBA (32bit)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    // Example - Color.RED instead of RED
    // Custom raylib color palette for amazing visuals
    public static readonly Color LightGray = new Color(200, 200, 200, 255);
    public static readonly Color Gray = new Color(130, 130, 130, 255);
    public static readonly Color DarkGray = new Color(80, 80, 80, 255);
    public static readonly Color Yellow = new Color(253, 249, 0, 255);
    public static readonly Color Gold = new Color(255, 203, 0, 255);
    public static readonly Color Orange = new Color(255, 161, 0, 255);
    public static readonly Color Pink = new Color(255, 109, 194, 255);
    public static readonly Color Red = new Color(230, 41, 55, 255);
    public static readonly Color Maroon = new Color(190, 33, 55, 255);
    public static readonly Color Green = new Color(0, 228, 48, 255);
    public static readonly Color Lime = new Color(0, 158, 47, 255);
    public static readonly Color DarkGreen = new Color(0, 117, 44, 255);
    public static readonly Color SkyBlue = new Color(102, 191, 255, 255);
    public static readonly Color Blue = new Color(0, 121, 241, 255);
    public static readonly Color DarkBlue = new Color(0, 82, 172, 255);
    public static readonly Color Purple = new Color(200, 122, 255, 255);
    public static readonly Color Violet = new Color(135, 60, 190, 255);
    public static readonly Color DarkPurple = new Color(112, 31, 126, 255);
    public static readonly Color Beige = new Color(211, 176, 131, 255);
    public static readonly Color Brown = new Color(127, 106, 79, 255);
    public static readonly Color DarkBrown = new Color(76, 63, 47, 255);
    public static readonly Color White = new Color(255, 255, 255, 255);
    public static readonly Color Black = new Color(0, 0, 0, 255);
    public static readonly Color Blank = new Color(0, 0, 0, 0);
    public static readonly Color Magenta = new Color(255, 0, 255, 255);
    public static readonly Color RayWhite = new Color(245, 245, 245, 255);

    /// <summary>
    /// Constructor with transparency
    /// </summary>
    public Color(byte r, byte g, byte b, byte a)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    /// <summary>
    /// Constructor without transparency, the color is made opaque by setting <see cref="A"/> to 255
    /// </summary>
    public Color(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = 255;
    }

    /// <summary>
    /// <inheritdoc cref="Color(byte, byte, byte, byte)"/>.
    /// Accepts <see cref="int"/>'s and converts them into <see cref="byte"/>'s by <see cref="Convert.ToByte(int)"/>
    /// </summary>
    public Color(int r, int g, int b, int a)
    {
        this.R = Convert.ToByte(r);
        this.G = Convert.ToByte(g);
        this.B = Convert.ToByte(b);
        this.A = Convert.ToByte(a);
    }

    /// <summary>
    /// <inheritdoc cref="Color(byte, byte, byte)"/>.
    /// Accepts <see cref="int"/>'s and converts them into <see cref="byte"/>'s by <see cref="Convert.ToByte(int)"/>
    /// </summary>
    public Color(int r, int g, int b)
    {
        this.R = Convert.ToByte(r);
        this.G = Convert.ToByte(g);
        this.B = Convert.ToByte(b);
        this.A = 255;
    }

    /// <summary>
    /// <inheritdoc cref="Color(byte, byte, byte, byte)"/>.
    /// Accepts <see cref="float"/>'s, upscales and clamps them to the range 0..255.
    /// Then they are converted to <see cref="byte"/>'s by rounding.
    /// </summary>
    public Color(float r, float g, float b, float a)
    {
        this.R = (byte)(Math.Clamp(r, 0.0f, 1.0f) * 255);
        this.G = (byte)(Math.Clamp(g, 0.0f, 1.0f) * 255);
        this.B = (byte)(Math.Clamp(b, 0.0f, 1.0f) * 255);
        this.A = (byte)(Math.Clamp(a, 0.0f, 1.0f) * 255);
    }

    /// <summary>
    /// <inheritdoc cref="Color(byte, byte, byte)"/>.
    /// Accepts <see cref="float"/>'s, upscales and clamps them to the range 0...255.
    /// Then they are converted to <see cref="byte"/>'s by rounding.
    /// </summary>
    public Color(float r, float g, float b)
    {
        this.R = (byte)(Math.Clamp(r, 0.0f, 1.0f) * 255);
        this.G = (byte)(Math.Clamp(g, 0.0f, 1.0f) * 255);
        this.B = (byte)(Math.Clamp(b, 0.0f, 1.0f) * 255);
        this.A = 255;
    }

    public readonly void GetHSV(out float h, out float s, out float v)
    {
        float rf = this.R / 255f;
        float gf = this.G / 255f;
        float bf = this.B / 255f;

        float max = MathF.Max(rf, MathF.Max(gf, bf));
        float min = MathF.Min(rf, MathF.Min(gf, bf));
        float delta = max - min;

        // Value
        v = max;

        // Saturation
        s = (max == 0f) ? 0f : delta / max;

        // Hue
        if (delta == 0f)
        {
            h = 0f;
        }
        else if (max == rf)
        {
            h = 60f * (((gf - bf) / delta) % 6f);
        }
        else if (max == gf)
        {
            h = 60f * (((bf - rf) / delta) + 2f);
        }
        else // max == bf
        {
            h = 60f * (((rf - gf) / delta) + 4f);
        }

        // Keep h inside 0 and 360
        if (h < 0f)
        {
            h += 360f;
        }
    }

    public static Color FromHSV(float h, float s, float v)
    {
        float c = v * s;
        float x = c * (1f - MathF.Abs((h / 60f) % 2f - 1f));
        float m = v - c;

        float rf, gf, bf;

        if (h < 60f)
        {
            rf = c;
            gf = x;
            bf = 0;
        }
        else if (h < 120f)
        {
            rf = x;
            gf = c;
            bf = 0;
        }
        else if (h < 180f)
        {
            rf = 0;
            gf = c;
            bf = x;
        }
        else if (h < 240f)
        {
            rf = 0;
            gf = x;
            bf = c;
        }
        else if (h < 300f)
        {
            rf = x;
            gf = 0;
            bf = c;
        }
        else
        {
            rf = c;
            gf = 0;
            bf = x;
        }

        byte r = (byte)((rf + m) * 255f + 0.5f);
        byte g = (byte)((gf + m) * 255f + 0.5f);
        byte b = (byte)((bf + m) * 255f + 0.5f);
        return new Color(r, g, b, byte.MaxValue);
    }

    public static Color Lerp(Color origin, Color target, float t)
    {
        byte r = LerpB(origin.R, target.R, t);
        byte g = LerpB(origin.R, target.G, t);
        byte b = LerpB(origin.R, target.B, t);
        byte a = LerpB(origin.R, target.A, t);
        return new Color(r, g, b, a);
    }

    // Lerp byte
    private static byte LerpB(byte a, byte b, float t)
    {
        return (byte)(a + (b - a) * t);
    }

    public readonly override string ToString()
    {
        return $"{{R:{R} G:{G} B:{B} A:{A}}}";
    }
}
