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
        //string text = new string(data);
        string text = System.Runtime.InteropServices.Marshal.PtrToStringUTF8((System.IntPtr)data);
        Raylib.UnloadFileText(data);
        return text;
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

    public static CBool CheckColllisionCircle(this Rectangle rec, Vector2 position, float radius)
    {
        return CheckCollisionCircleRec(position, radius, rec);
    }

    public static Rectangle GetCollisionRectangle(this Rectangle rectangle, Rectangle rectangle2)
    {
        return GetCollisionRec(rectangle, rectangle2);
    }

    #endregion

    public static void DrawLines(this IEnumerable<Vector2> points, Color color)
    {
        Vector2[] pointArray = System.Linq.Enumerable.ToArray<Vector2>(points);
        DrawLineStrip(pointArray, pointArray.Length, color);
    }

    #region Image

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

    public static void DrawLine(this ref Image image, Vector2 start, Vector2 end, Color color)
    {
        ImageDrawLineV(ref image, start, end, color);
    }

    public static void DrawLine(this ref Image image, Vector2 start, Vector2 end, int thickness, Color color)
    {
        ImageDrawLineEx(ref image, start, end, thickness, color);
    }

    public static void DrawCircle(this ref Image image, Vector2 position, float radius, Color color)
    {
        ImageDrawCircle(ref image, (int)position.X, (int)position.Y, (int)radius, color);
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

    public static unsafe void DrawCircleLines(this ref Image image, Vector2 position, float radius, Color color)
    {
        ImageDrawCircleLinesV(ref image, position, (int)radius, color);
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

    public static void Load(this ref Texture2D texture, string fileType, byte[] fileData)
    {
        Image img = LoadImageFromMemory(fileType, fileData);
        texture.Load(img);
        UnloadImage(img);
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
        Vector2 size = texture.Dimensions;
        Rectangle source = new Rectangle(Vector2.Zero, size);
        Rectangle target = new Rectangle(position, size);
        DrawTexturePro(texture, source, target, origin, 0, Color.White);
    }

    public static void Draw(this Texture2D texture, Vector2 position, Vector2 origin, Color color)
    {
        Vector2 size = texture.Dimensions;
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

    #endregion
}
