using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Rectangle type
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rectangle
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public Rectangle(float x, float y, float width, float height)
    {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
    }

    public Rectangle(Vector2 position, float width, float height)
    {
        this.X = position.X;
        this.Y = position.Y;
        this.Width = width;
        this.Height = height;
    }

    public Rectangle(float x, float y, Vector2 size)
    {
        this.X = x;
        this.Y = y;
        this.Width = size.X;
        this.Height = size.Y;
    }

    public Rectangle(Vector2 position, Vector2 size)
    {
        this.X = position.X;
        this.Y = position.Y;
        this.Width = size.X;
        this.Height = size.Y;
    }

    public Vector2 Position
    {
        readonly get
        {
            return new Vector2(X, Y);
        }
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Vector2 Size
    {
        readonly get
        {
            return new Vector2(Width, Height);
        }
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    public readonly void Draw(Color color)
    {
        Raylib.DrawRectangleRec(this, color);
    }

    public readonly void Draw(Vector2 origin, Color color)
    {
        Draw(origin, 0.0f, color);
    }

    public readonly void Draw(Vector2 origin, float rotation, Color color)
    {
        Raylib.DrawRectanglePro(this, origin, rotation, color);
    }

    public readonly void GetIntegerValues(out int x, out int y, out int width, out int height)
    {
        x = (int)this.X;
        y = (int)this.Y;
        width = (int)this.Width;
        height = (int)this.Height;
    }

    public readonly void DrawLines(Color color)
    {
        GetIntegerValues(out int x, out int y, out int w, out int h);
        Raylib.DrawRectangleLines(x, y, w, h, color);
    }

    public readonly void DrawLines(float thickness, Color color)
    {
        Raylib.DrawRectangleLinesEx(this, thickness, color);
    }

    public readonly void DrawGradient(Color topLeft, Color bottomLeft, Color topRight, Color bottomRight)
    {
        Raylib.DrawRectangleGradientEx(this, topLeft, bottomLeft, topRight, bottomRight);
    }

    public readonly void DrawGradientV(Color top, Color bottom)
    {
        GetIntegerValues(out int x, out int y, out int w, out int h);
        Raylib.DrawRectangleGradientV(x, y, w, h, top, bottom);
    }

    public readonly void DrawGradientH(Color left, Color right)
    {
        GetIntegerValues(out int x, out int y, out int w, out int h);
        Raylib.DrawRectangleGradientH(x, y, w, h, left, right);
    }

    public readonly void DrawRounded(float roundness, Color color)
    {
        Raylib.DrawRectangleRounded(this, roundness, 10, color);
    }

    public readonly void DrawRounded(float roundness, int segments, Color color)
    {
        Raylib.DrawRectangleRounded(this, roundness, segments, color);
    }

    public readonly void DrawRoundedLines(float roundness, Color color)
    {
        Raylib.DrawRectangleRoundedLines(this, roundness, 10, color);
    }

    public readonly void DrawRoundedLines(float roundness, int segments, Color color)
    {
        Raylib.DrawRectangleRoundedLines(this, roundness, segments, color);
    }

    public readonly void DrawRoundedLines(float roundness, float thickness, Color color)
    {
        Raylib.DrawRectangleRoundedLinesEx(this, roundness, 10, thickness, color);
    }

    public readonly void DrawRoundedLines(float roundness, int segments, float thickness, Color color)
    {
        Raylib.DrawRectangleRoundedLinesEx(this, roundness, segments, thickness, color);
    }

    public readonly CBool CheckCollisionRec(Rectangle rectangle)
    {
        return Raylib.CheckCollisionRecs(this, rectangle);
    }

    public readonly CBool CheckCollisionPoint(Vector2 point)
    {
        return Raylib.CheckCollisionPointRec(point, this);
    }

    public readonly CBool CheckCollisionCircle(Vector2 center, float radius)
    {
        return Raylib.CheckCollisionCircleRec(center, radius, this);
    }

    public readonly Rectangle GetCollisionRectangle(Rectangle rectangle2)
    {
        return Raylib.GetCollisionRec(this, rectangle2);
    }

    public readonly override string ToString()
    {
        return $"{{X:{X} Y:{Y} Width:{Width} Height:{Height}}}";
    }
}
