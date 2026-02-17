using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public unsafe class TexturedCurve
{
    public class CurvePoint
    {
        public Vector2 value;

        public float X => value.X;
        public float Y => value.Y;

        public static implicit operator CurvePoint(Vector2 v) => new CurvePoint { value = v };
        public static implicit operator Vector2(CurvePoint v) => v.value;
    }

    static Texture2D texRoad;
    static bool showCurve = false;
    static float curveWidth = 50;
    static int curveSegments = 24;
    static CurvePoint curveStartPosition;
    static CurvePoint curveStartPositionTangent;
    static CurvePoint curveEndPosition;
    static CurvePoint curveEndPositionTangent;
    static CurvePoint curveSelectedPoint;

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [textures] examples - textured curve");

        // Load the road texture
        texRoad = LoadTexture("resources/road.png");
        SetTextureFilter(texRoad, TextureFilter.Bilinear);

        // Setup the curve
        curveStartPosition = new Vector2(80, 100);
        curveStartPositionTangent = new Vector2(100, 300);

        curveEndPosition = new Vector2(700, 350);
        curveEndPositionTangent = new Vector2(600, 100);

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            UpdateCurve();
            UpdateOptions();
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawTexturedCurve();
            DrawCurve();

            DrawText("Drag points to move curve, press SPACE to show/hide base curve", 10, 10, 10, Color.DarkGray);
            DrawText($"Curve width: {curveWidth} (Use + and - to adjust)", 10, 30, 10, Color.DarkGray);
            DrawText($"Curve segments: {curveSegments} (Use LEFT and RIGHT to adjust)", 10, 50, 10, Color.DarkGray);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadTexture(texRoad);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }

    static void DrawCurve()
    {
        if (showCurve)
        {
            //TODO: DrawLineBezier was deprecated, I merely replaced to the fallback one.
            DrawLineBezier(
                curveStartPosition,
                curveEndPosition,
                2,
                Color.Blue
            );
        }

        // Draw the various control points and highlight where the mouse is
        DrawLineV(curveStartPosition, curveStartPositionTangent, Color.SkyBlue);
        DrawLineV(curveEndPosition, curveEndPositionTangent, Color.Purple);
        Vector2 mouse = GetMousePosition();

        if (CheckCollisionPointCircle(mouse, curveStartPosition, 6))
        {
            DrawCircleV(curveStartPosition, 7, Color.Yellow);
        }
        DrawCircleV(curveStartPosition, 5, Color.Red);

        if (CheckCollisionPointCircle(mouse, curveStartPositionTangent, 6))
        {
            DrawCircleV(curveStartPositionTangent, 7, Color.Yellow);
        }
        DrawCircleV(curveStartPositionTangent, 5, Color.Maroon);

        if (CheckCollisionPointCircle(mouse, curveEndPosition, 6))
        {
            DrawCircleV(curveEndPosition, 7, Color.Yellow);
        }
        DrawCircleV(curveEndPosition, 5, Color.Green);

        if (CheckCollisionPointCircle(mouse, curveEndPositionTangent, 6))
        {
            DrawCircleV(curveEndPositionTangent, 7, Color.Yellow);
        }
        DrawCircleV(curveEndPositionTangent, 5, Color.DarkGreen);
    }

    static void UpdateCurve()
    {
        // If the mouse is not down, we are not editing the curve so clear the selection
        if (!IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        // If a point was selected, move it
        if (curveSelectedPoint != null)
        {
            curveSelectedPoint.value += GetMouseDelta();
        }

        // The mouse is down, and nothing was selected, so see if anything was picked
        Vector2 mouse = GetMousePosition();

        if (CheckCollisionPointCircle(mouse, curveStartPosition, 6))
        {
            curveSelectedPoint = curveStartPosition;
        }
        else if (CheckCollisionPointCircle(mouse, curveStartPositionTangent, 6))
        {
            curveSelectedPoint = curveStartPositionTangent;
        }
        else if (CheckCollisionPointCircle(mouse, curveEndPosition, 6))
        {
            curveSelectedPoint = curveEndPosition;
        }
        else if (CheckCollisionPointCircle(mouse, curveEndPositionTangent, 6))
        {
            curveSelectedPoint = curveEndPositionTangent;
        }
    }

    static void DrawTexturedCurve()
    {
        float step = 1.0f / curveSegments;

        Vector2 previous = curveStartPosition;
        Vector2 previousTangent = Vector2.Zero;
        float previousV = 0;

        // We can't compute a tangent for the first point, so we need to reuse the tangent from the first segment
        bool tangentSet = false;

        Vector2 current = Vector2.Zero;
        float t = 0.0f;

        for (int i = 1; i <= curveSegments; i++)
        {
            // Segment the curve
            t = step * i;
            float a = MathF.Pow(1 - t, 3);
            float b = 3 * MathF.Pow(1 - t, 2) * t;
            float c = 3 * (1 - t) * MathF.Pow(t, 2);
            float d = MathF.Pow(t, 3);

            // Compute the endpoint for this segment
            current.Y = a * curveStartPosition.Y + b * curveStartPositionTangent.Y;
            current.Y += c * curveEndPositionTangent.Y + d * curveEndPosition.Y;
            current.X = a * curveStartPosition.X + b * curveStartPositionTangent.X;
            current.X += c * curveEndPositionTangent.X + d * curveEndPosition.X;

            // Vector from previous to current
            Vector2 delta = new(current.X - previous.X, current.Y - previous.Y);

            // The right hand normal to the delta vector
            Vector2 normal = Vector2.Normalize(new Vector2(-delta.Y, delta.X));

            // The v teXture coordinate of the segment (add up the length of all the segments so far)
            float v = previousV + delta.Length();

            // Make sure the start point has a normal
            if (!tangentSet)
            {
                previousTangent = normal;
                tangentSet = true;
            }

            // EXtend out the normals from the previous and current points to get the quad for this segment
            Vector2 prevPosNormal = previous + (previousTangent * curveWidth);
            Vector2 prevNegNormal = previous + (previousTangent * -curveWidth);

            Vector2 currentPosNormal = current + (normal * curveWidth);
            Vector2 currentNegNormal = current + (normal * -curveWidth);

            // Draw the segment as a quad
            Rlgl.SetTexture(texRoad.Id);
            Rlgl.Begin(DrawMode.Quads);

            Rlgl.Color4ub(255, 255, 255, 255);
            Rlgl.Normal3f(0.0f, 0.0f, 1.0f);

            Rlgl.TexCoord2f(0, previousV);
            Rlgl.Vertex2f(prevNegNormal.X, prevNegNormal.Y);

            Rlgl.TexCoord2f(1, previousV);
            Rlgl.Vertex2f(prevPosNormal.X, prevPosNormal.Y);

            Rlgl.TexCoord2f(1, v);
            Rlgl.Vertex2f(currentPosNormal.X, currentPosNormal.Y);

            Rlgl.TexCoord2f(0, v);
            Rlgl.Vertex2f(currentNegNormal.X, currentNegNormal.Y);

            Rlgl.End();

            // The current step is the start of the neXt step
            previous = current;
            previousTangent = normal;
            previousV = v;
        }
    }

    static void UpdateOptions()
    {
        if (IsKeyPressed(KeyboardKey.Space))
        {
            showCurve = !showCurve;
        }

        // Update with
        if (IsKeyPressed(KeyboardKey.Equal))
        {
            curveWidth += 2;
        }
        if (IsKeyPressed(KeyboardKey.Minus))
        {
            curveWidth -= 2;
        }

        if (curveWidth < 2)
        {
            curveWidth = 2;
        }

        // Update segments
        if (IsKeyPressed(KeyboardKey.Left))
        {
            curveSegments -= 2;
        }
        if (IsKeyPressed(KeyboardKey.Right))
        {
            curveSegments += 2;
        }
        if (curveSegments < 2)
        {
            curveSegments = 2;
        }
    }
}
