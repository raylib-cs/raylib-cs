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

    public readonly Vector2 Center
    {
        get
        {
            Vector2 center = new Vector2();
            center.X = X + (Width / 2.0f);
            center.Y = Y + (Height / 2.0f);
            return center;
        }
    }

    public readonly void GetIntegerValues(out int x, out int y, out int width, out int height)
    {
        x = (int)this.X;
        y = (int)this.Y;
        width = (int)this.Width;
        height = (int)this.Height;
    }

    public void Grow(float growth)
    {
        X -= growth;
        Y -= growth;
        Width += growth * 2.0f;
        Height += growth * 2.0f;
    }

    public void Shrink(float shrink)
    {
        X += shrink;
        Y += shrink;
        Width -= shrink * 2.0f;
        Height -= shrink * 2.0f;
    }

    public readonly override string ToString()
    {
        return $"{{X:{X} Y:{Y} Width:{Width} Height:{Height}}}";
    }
}
