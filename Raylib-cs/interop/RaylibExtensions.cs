using System.Collections.Generic;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Raylib_cs;

public static class RaylibExtensions
{
    public static unsafe void SaveFileText(string fileName, string text)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        using AnsiBuffer textBuffer = text.ToAnsiBuffer();
        Raylib.SaveFileText(fileBuffer.AsPointer(), textBuffer.AsPointer());
    }

    public static unsafe string LoadFileText(string fileName)
    {
        using AnsiBuffer nameBuffer = fileName.ToAnsiBuffer();
        sbyte* data = Raylib.LoadFileText(nameBuffer.AsPointer());
        return new string(data);
    }

    #region Rectangle

    public static void Draw(this Rectangle rectangle, Color color)
    {
        DrawRectangleRec(rectangle, color);
    }

    public static void Draw(this Rectangle rec, Vector2 origin, Color color)
    {
        rec.Draw(origin, 0.0f, color);
    }

    public static void Draw(this Rectangle rec, Vector2 origin, float rotation, Color color)
    {
        DrawRectanglePro(rec, origin, rotation, color);
    }

    public static void GetIntegerPosition(this Rectangle rec, out int x, out int y)
    {
        x = (int)rec.X;
        y = (int)rec.Y;
    }

    public static void GetIntegerDimentions(this Rectangle rec, out int width, out int height)
    {
        width = (int)rec.Width;
        height = (int)rec.Height;
    }

    public static void GetIntegerValues(this Rectangle rec, out int x, out int y, out int width, out int height)
    {
        x = (int)rec.X;
        y = (int)rec.Y;
        width = (int)rec.Width;
        height = (int)rec.Height;
    }

    public static void DrawLines(this Rectangle rectangle, Color color)
    {
        rectangle.GetIntegerValues(out int x, out int y, out int w, out int h);
        DrawRectangleLines(x, y, w, h, color);
    }

    public static void DrawLines(this Rectangle rectangle, float thickness, Color color)
    {
        DrawRectangleLinesEx(rectangle, thickness, color);
    }

    public static void DrawGradient(this Rectangle rec, Color topLeft, Color bottomLeft, Color topRight, Color bottomRight)
    {
        DrawRectangleGradientEx(rec, topLeft, bottomLeft, topRight, bottomRight);
    }

    public static void DrawGradientV(this Rectangle rectangle, Color top, Color bottom)
    {
        rectangle.GetIntegerValues(out int x, out int y, out int w, out int h);
        DrawRectangleGradientV(x, y, w, h, top, bottom);
    }

    public static void DrawGradientH(this Rectangle rec, Color left, Color right)
    {
        rec.GetIntegerValues(out int x, out int y, out int w, out int h);
        DrawRectangleGradientH(x, y, w, h, left, right);
    }

    public static void DrawRounded(this Rectangle rec, float roundness, Color color)
    {
        DrawRectangleRounded(rec, roundness, 10, color);
    }

    public static void DrawRounded(this Rectangle rec, float roundness, int segments, Color color)
    {
        DrawRectangleRounded(rec, roundness, segments, color);
    }

    public static void DrawRoundedLines(this Rectangle rec, float roundness, Color color)
    {
        DrawRectangleRoundedLines(rec, roundness, 10, color);
    }

    public static void DrawRoundedLines(this Rectangle rec, float roundness, int segments, Color color)
    {
        DrawRectangleRoundedLines(rec, roundness, segments, color);
    }

    public static void DrawRoundedLines(this Rectangle rec, float roundness, float thickness, Color color)
    {
        DrawRectangleRoundedLinesEx(rec, roundness, 10, thickness, color);
    }

    public static void DrawRoundedLines(this Rectangle rec, float roundness, int segments, float thickness, Color color)
    {
        DrawRectangleRoundedLinesEx(rec, roundness, segments, thickness, color);
    }

    public static CBool CheckCollision(this Rectangle rec, Rectangle rectangle)
    {
        return CheckCollisionRecs(rec, rectangle);
    }

    public static CBool CheckCollision(this Rectangle rec, Vector2 point)
    {
        return CheckCollisionPointRec(point, rec);
    }

    public static CBool CheckCircleCollision(this Rectangle rec, Vector2 center, float radius)
    {
        return CheckCollisionCircleRec(center, radius, rec);
    }

    public static CBool CheckColllision(this Rectangle rec, Circle circle)
    {
        return CheckCollisionCircleRec(circle.position, circle.radius, rec);
    }

    public static Rectangle GetCollisionRectangle(this Rectangle rectangle, Rectangle rectangle2)
    {
        return GetCollisionRec(rectangle, rectangle2);
    }

    #endregion

    #region Circle

    public static void Draw(this Circle circle)
    {
        DrawCircleV(circle.position, circle.radius, circle.color);
    }

    public static void Draw(this Circle circle, Color colorOverride)
    {
        DrawCircleV(circle.position, circle.radius, colorOverride);
    }

    public static void DrawLines(this Circle circle)
    {
        DrawCircleLinesV(circle.position, circle.radius, circle.color);
    }

    public static void DrawLines(this Circle circle, Color colorOverride)
    {
        DrawCircleLinesV(circle.position, circle.radius, colorOverride);
    }

    public static void DrawGradient(this Circle circle, Color innerColor, Color outerColor)
    {
        int x = (int)circle.position.X;
        int y = (int)circle.position.Y;
        DrawCircleGradient(x, y, circle.radius, innerColor, outerColor);
    }

    public static void DrawSector(this Circle circle, float startAngle, float endAngle)
    {
        circle.DrawSector(startAngle, endAngle, 36);
    }

    public static void DrawSector(this Circle circle, float startAngle, float endAngle, int segments)
    {
        circle.DrawSector(startAngle, endAngle, segments, circle.color);
    }

    public static void DrawSector(this Circle circle, float startAngle, float endAngle, int segments, Color colorOverride)
    {
        DrawCircleSector(circle.position, circle.radius, startAngle, endAngle, segments, colorOverride);
    }

    public static void DrawSectorLines(this Circle circle, float startAngle, float endAngle)
    {
        circle.DrawSectorLines(startAngle, endAngle, 36);
    }

    public static void DrawSectorLines(this Circle circle, float startAngle, float endAngle, int segments)
    {
        circle.DrawSectorLines(startAngle, endAngle, segments, circle.color);
    }

    public static void DrawSectorLines(this Circle circle, float startAngle, float endAngle, int segments, Color colorOverride)
    {
        DrawCircleSectorLines(circle.position, circle.radius, startAngle, endAngle, segments, colorOverride);
    }

    public static CBool CheckCollision(this Circle circle, Circle circle2)
    {
        return CheckCollisionCircles(circle.position, circle.radius, circle2.position, circle2.radius);
    }

    public static CBool CheckCollision(this Circle circle, Rectangle rec)
    {
        return CheckCollisionCircleRec(circle.position, circle.radius, rec);
    }

    public static CBool CheckCollision(this Circle circle, Vector2 point)
    {
        return CheckCollisionPointCircle(point, circle.position, circle.radius);
    }

    public static CBool CheckCollision(this Circle circle, Line line)
    {
        return CheckCollisionCircleLine(circle.position, circle.radius, line.pointA, line.pointB);
    }

    #endregion

    #region Line

    public static CBool CheckCollision(this Line line1, Line line2, ref Vector2 collisionPoint)
    {
        return CheckCollisionLines(line1.pointA, line1.pointB, line2.pointA, line2.pointB, ref collisionPoint);
    }

    public static CBool CheckCollision(this Line line, Circle circle)
    {
        return CheckCollisionCircleLine(circle.position, circle.radius, line.pointA, line.pointB);
    }

    public static CBool CheckCollision(this Line line, Vector2 point, int threshold)
    {
        return CheckCollisionPointLine(point, line.pointA, line.pointB, threshold);
    }

    public static void Draw(this Line line, Color color)
    {
        DrawLineV(line.pointA, line.pointB, color);
    }

    public static void DrawBezier(this Line line, Color color)
    {
        line.DrawBezier(1, color);
    }

    public static void DrawBezier(this Line line, float thick, Color color)
    {
        DrawLineBezier(line.pointA, line.pointB, thick, color);
    }

    public static void DrawBezierCubic(this Line line, Line control, Color color)
    {
        line.DrawBezierCubic(control, 1, color);
    }

    public static void DrawBezierCubic(this Line line, Line control, float thick, Color color)
    {
        DrawLineBezierCubic(line.pointA, line.pointB, control.pointA, control.pointB, thick, color);
    }

    public static void DrawBezierQuad(this Line line, Vector2 control, Color color)
    {
        line.DrawBezierQuad(control, 1, color);
    }

    public static void DrawBezierQuad(this Line line, Vector2 control, float thick, Color color)
    {
        DrawLineBezierQuad(line.pointA, line.pointB, control, thick, color);
    }

    public static float GetDistance(this Line line)
    {
        return Vector2.Distance(line.pointA, line.pointB);
    }

    public static void SwapPoints(this Line line)
    {
        Vector2 temp = line.pointA;
        line.pointA = line.pointB;
        line.pointB = temp;
    }

    public static void DrawLines(this IEnumerable<Line> lines, Color color)
    {
        Line[] linesArray = System.Linq.Enumerable.ToArray<Line>(lines);
        Vector2[] points = new Vector2[linesArray.Length * 2];
        for (int i = 0; i < linesArray.Length; i++)
        {
            Line l = linesArray[i];
            points[i * 2] = l.pointA;
            points[i * 2 + 1] = l.pointB;
        }
        DrawLineStrip(points, points.Length, color);
    }

    public static void DrawLines(this IEnumerable<Vector2> points, Color color)
    {
        Vector2[] pointArray = System.Linq.Enumerable.ToArray<Vector2>(points);

        DrawLineStrip(pointArray, pointArray.Length, color);
    }

    #endregion

    #region Image

    public static unsafe Image* GetPointer(this ref Image image)
    {
        fixed (Image* ptr = &image)
        {
            return ptr;
        }
    }

    public static void Load(this ref Image image, string fileName)
    {
        image = LoadImage(fileName);
    }

    public static void Load(this ref Image image, string fileType, byte[] data)
    {
        image = LoadImageFromMemory(fileType, data);
    }

    public static void Load(this ref Image image, Texture2D texture)
    {
        image = LoadImageFromTexture(texture);
    }

    public static void Load(this ref Image image, string fileName, int width, int height, PixelFormat format, int headerSize)
    {
        image = LoadImageRaw(fileName, width, height, format, headerSize);
    }

    public static void Unload(this Image image)
    {
        UnloadImage(image);
    }

    public static unsafe Color[] GetPalette(this Image image, int maxPaletteSize)
    {
        int colorCount = 0;
        Color* colors = LoadImagePalette(image, maxPaletteSize, &colorCount);
        Color[] palette = new Color[colorCount];
        for (int i = 0; i < colorCount; i++)
        {
            palette[i] = colors[i];
        }
        UnloadImagePalette(colors);
        return palette;
    }

    public static void LoadFromScreen(this ref Image image)
    {
        image = LoadImageFromScreen();
    }

    public static void Draw(this ref Image image, Line line, Color color)
    {
        ImageDrawLineV(ref image, line.pointA, line.pointB, color);
    }

    public static void Draw(this ref Image image, Line line, int thickness, Color color)
    {
        ImageDrawLineEx(ref image, line.pointA, line.pointB, thickness, color);
    }

    public static void Draw(this ref Image image, Circle circle)
    {
        ImageDrawCircle(ref image, (int)circle.X, (int)circle.Y, (int)circle.radius, circle.color);
    }

    public static void Draw(this ref Image image, Rectangle rectangle, Color color)
    {
        ImageDrawRectangleRec(ref image, rectangle, color);
    }

    public static void Draw(this ref Image image, string text, Vector2 position, int fontSize, Color color)
    {
        ImageDrawText(ref image, text, (int)position.X, (int)position.Y, fontSize, color);
    }

    public static void Draw(this ref Image image, Font font, string text, Vector2 position, int fontSize, float spacing, Color color)
    {
        ImageDrawTextEx(ref image, font, text, position, fontSize, spacing, color);
    }

    public static unsafe void DrawLines(this ref Image image, Circle circle)
    {
        ImageDrawCircleLinesV(ref image, circle.position, (int)circle.radius, circle.color);
    }

    public static void DrawTriangle(this ref Image image, Vector2 p1, Vector2 p2, Vector2 p3, Color color)
    {
        ImageDrawTriangle(ref image, p1, p2, p3, color);
    }

    public static void DrawTriangle(this ref Image image, Vector2 p1, Vector2 p2, Vector2 p3, Color c1, Color c2, Color c3)
    {
        ImageDrawTriangleEx(ref image, p1, p2, p3, c1, c2, c3);
    }

    public static void DrawLines(this ref Image image, Rectangle rectangle, Color color)
    {
        ImageDrawRectangleLines(ref image, rectangle, 1, color);
    }

    public static void DrawLines(this ref Image image, Rectangle rectangle, int thickness, Color color)
    {
        ImageDrawRectangleLines(ref image, rectangle, thickness, color);
    }

    #endregion

    #region Texture2D

    public static void Load(this ref Texture2D texture, string fileName)
    {
        texture = LoadTexture(fileName);
    }

    public static void Load(this ref Texture2D texture, Image image)
    {
        texture = LoadTextureFromImage(image);
    }

    public static void Unload(this Texture2D texture)
    {
        UnloadTexture(texture);
    }

    public static void SetFilter(this Texture2D texture, TextureFilter filter)
    {
        SetTextureFilter(texture, filter);
    }

    public static void SetWrap(this Texture2D texture, TextureWrap wrap)
    {
        SetTextureWrap(texture, wrap);
    }

    public static void Update<T>(this Texture2D texture, T[] pixels) where T : unmanaged
    {
        UpdateTexture(texture, pixels);
    }

    public static Rectangle GetSourceRectangle(this Texture2D texture)
    {
        return new Rectangle(Vector2.Zero, texture.Width, texture.Height);
    }

    public static Vector2 GetDimensions(this Texture2D texture)
    {
        return new Vector2(texture.Width, texture.Height);
    }

    public static void Draw(this Texture2D texture, int x, int y)
    {
        DrawTexture(texture, x, y, Color.White);
    }

    public static void Draw(this Texture2D texture, int x, int y, Color color)
    {
        DrawTexture(texture, x, y, color);
    }

    public static void Draw(this Texture2D texture, Vector2 position)
    {
        DrawTexture(texture, (int)position.X, (int)position.Y, Color.White);
    }

    public static void Draw(this Texture2D texture, Vector2 position, Vector2 origin)
    {
        Vector2 size = texture.GetDimensions();
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        DrawTexturePro(texture, source, target, origin, 0, Color.White);
    }

    public static void Draw(this Texture2D texture, Vector2 position, Vector2 origin, Color color)
    {
        Vector2 size = texture.GetDimensions();
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        DrawTexturePro(texture, source, target, origin, 0, color);
    }

    public static void Draw(this Texture2D texture, Vector2 position, Color color)
    {
        DrawTexture(texture, (int)position.X, (int)position.Y, color);
    }

    public static void Draw(this Texture2D texture, Vector2 position, float rotation, float scale)
    {
        DrawTextureEx(texture, position, rotation, scale, Color.White);
    }

    public static void Draw(this Texture2D texture, Vector2 position, float rotation, float scale, Color color)
    {
        DrawTextureEx(texture, position, rotation, scale, color);
    }

    public static void Draw(this Texture2D texture, Vector2 position, float rotation, Vector2 scale)
    {
        Draw(texture, position, rotation, scale, Color.White);
    }

    public static void Draw(this Texture2D texture, Vector2 position, float rotation, Vector2 scale, Color color)
    {
        Vector2 size = new Vector2(texture.Width, texture.Height);
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        target.Size *= scale;
        DrawTexturePro(texture, source, target, Vector2.Zero, rotation, color);
    }

    public static void Draw(this Texture2D texture, Rectangle source, Rectangle dest)
    {
        DrawTexturePro(texture, source, dest, Vector2.Zero, 0, Color.White);
    }

    public static void Draw(this Texture2D texture, Rectangle source, Rectangle dest, Vector2 origin)
    {
        DrawTexturePro(texture, source, dest, origin, 0, Color.White);
    }

    public static void Draw(this Texture2D texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation)
    {
        DrawTexturePro(texture, source, dest, origin, rotation, Color.White);
    }

    public static void Draw(this Texture2D texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color color)
    {
        DrawTexturePro(texture, source, dest, origin, rotation, color);
    }

    #endregion
}
