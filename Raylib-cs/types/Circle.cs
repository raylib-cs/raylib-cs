using System.Numerics;

namespace Raylib_cs;

public struct Circle
{
    public Vector2 position;
    public float radius;
    public Color color;

    public float X
    {
        get
        {
            return position.X;
        }
        set
        {
            position.Y = value;
        }
    }

    public float Y
    {
        get
        {
            return position.Y;
        }
        set
        {
            position.Y = value;
        }
    }

    public Circle(Vector2 position)
    {
        this.position = position;
    }

    public Circle(float x, float y)
    {
        position = new Vector2(x, y);
    }

    public Circle(float x, float y, Color color)
    {
        position = new Vector2(x, y);
        this.color = color;
    }

    public Circle(Vector2 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }

    public Circle(float x, float y, float radius)
    {
        position = new Vector2(x, y);
        this.radius = radius;
    }

    public Circle(Vector2 position, float radius, Color color)
    {
        this.position = position;
        this.radius = radius;
        this.color = color;
    }
}
