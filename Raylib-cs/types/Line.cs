using System.Numerics;


namespace Raylib_cs;

public struct Line
{
    public Vector2 pointA;
    public Vector2 pointB;

    public Line(Vector2 p1, Vector2 p2)
    {
        pointA = p1;
        pointB = p2;
    }

    public Line(Vector2 startPos, float endPosX, float endPosY)
    {
        pointA = startPos;
        pointB = new Vector2(endPosX, endPosY);
    }

    public Line(float startPosX, float startPosY, Vector2 endPos)
    {
        pointA = new Vector2(startPosX, startPosY);
        pointB = endPos;
    }

    public Line(float startPosX, float startPosY, float endPosX, float endPosY)
    {
        pointA = new Vector2(startPosX, startPosY);
        pointB = new Vector2(endPosX, endPosY);
    }

    public readonly override string ToString()
    {
        return $"P1: {pointA}, P2: {pointB}";
    }
}
