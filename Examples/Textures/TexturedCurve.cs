/*******************************************************************************************
*
*   raylib [textures] example - textured curve
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by Jeffery Myers (@JeffM2501) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 Jeffery Myers (@JeffM2501) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public unsafe partial class TexturedCurve : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Textured Curve";

    public string Title => "raylib [textures] example - textured curve";

    public ConfigFlags ConfigFlags => ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint;

    public class CurvePoint
    {
        public Vector2 value;

        public float X => value.X;
        public float Y => value.Y;

        public static implicit operator CurvePoint(Vector2 v) => new CurvePoint { value = v };
        public static implicit operator Vector2(CurvePoint v) => v.value;
    }

    private Texture2D texRoad;
    private bool showCurve;
    private float curveWidth;
    private int curveSegments;
    private CurvePoint curveStartPosition;
    private CurvePoint curveStartPositionTangent;
    private CurvePoint curveEndPosition;
    private CurvePoint curveEndPositionTangent;
    private CurvePoint curveSelectedPoint;

    public void Init()
    {
        // Load the road texture
        texRoad = LoadTexture("resources/road.png");
        SetTextureFilter(texRoad, TextureFilter.Bilinear);

        // Setup the curve
        curveStartPosition = new Vector2(80, 100);
        curveStartPositionTangent = new Vector2(100, 300);

        curveEndPosition = new Vector2(700, 350);
        curveEndPositionTangent = new Vector2(600, 100);

        showCurve = false;
        curveWidth = 50;
        curveSegments = 24;
        curveSelectedPoint = null;
    }

    public void Update()
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

    public void Unload()
    {
        UnloadTexture(texRoad);
    }

    private void DrawCurve()
    {
        if (showCurve)
        {
            DrawSplineSegmentBezierCubic(
                curveStartPosition,
                curveEndPosition,
                curveStartPositionTangent,
                curveEndPositionTangent,
                2,
                Color.Blue
            );
        }

        // Draw the various control points and highlight where the mouse is
        DrawLineV(curveStartPosition, curveStartPositionTangent, Color.SkyBlue);
        DrawLineV(curveStartPositionTangent, curveEndPositionTangent, Fade(Color.LightGray, 0.4f));
        DrawLineV(curveEndPosition, curveEndPositionTangent, Color.Purple);
        var mouse = GetMousePosition();

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

    private void UpdateCurve()
    {
        // If the mouse is not down, we are not editing the curve so clear the selection
        if (!IsMouseButtonDown(MouseButton.Left))
        {
            curveSelectedPoint = null;
            return;
        }

        // If a point was selected, move it
        if (curveSelectedPoint != null)
        {
            curveSelectedPoint.value += GetMouseDelta();
        }

        // The mouse is down, and nothing was selected, so see if anything was picked
        var mouse = GetMousePosition();

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

    private void DrawTexturedCurve()
    {
        var step = 1.0f / curveSegments;

        Vector2 previous = curveStartPosition;
        var previousTangent = Vector2.Zero;
        float previousV = 0;

        // We can't compute a tangent for the first point, so we need to reuse the tangent from the first segment
        var tangentSet = false;

        var current = Vector2.Zero;
        var t = 0.0f;

        for (var i = 1; i <= curveSegments; i++)
        {
            // Segment the curve
            t = step * i;
            var a = MathF.Pow(1 - t, 3);
            var b = 3 * MathF.Pow(1 - t, 2) * t;
            var c = 3 * (1 - t) * MathF.Pow(t, 2);
            var d = MathF.Pow(t, 3);

            // Compute the endpoint for this segment
            current.Y = a * curveStartPosition.Y + b * curveStartPositionTangent.Y;
            current.Y += c * curveEndPositionTangent.Y + d * curveEndPosition.Y;
            current.X = a * curveStartPosition.X + b * curveStartPositionTangent.X;
            current.X += c * curveEndPositionTangent.X + d * curveEndPosition.X;

            // Vector from previous to current
            Vector2 delta = new(current.X - previous.X, current.Y - previous.Y);

            // The right hand normal to the delta vector
            var normal = Vector2.Normalize(new Vector2(-delta.Y, delta.X));

            // The v texture coordinate of the segment (add up the length of all the segments so far)
            var v = previousV + delta.Length() / (texRoad.Height * 2);

            // Make sure the start point has a normal
            if (!tangentSet)
            {
                previousTangent = normal;
                tangentSet = true;
            }

            // Extend out the normals from the previous and current points to get the quad for this segment
            var prevPosNormal = previous + (previousTangent * curveWidth);
            var prevNegNormal = previous + (previousTangent * -curveWidth);

            var currentPosNormal = current + (normal * curveWidth);
            var currentNegNormal = current + (normal * -curveWidth);

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

            // The current step is the start of the next step
            previous = current;
            previousTangent = normal;
            previousV = v;
        }
    }

    private void UpdateOptions()
    {
        if (IsKeyPressed(KeyboardKey.Space))
        {
            showCurve = !showCurve;
        }

        // Update width
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

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - textured curve");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new TexturedCurve();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
