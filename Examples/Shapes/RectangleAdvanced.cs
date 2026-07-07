/*******************************************************************************************
*
*   raylib [shapes] example - rectangle advanced
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Everton Jr. (@evertonse) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2025 Everton Jr. (@evertonse) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Rlgl;

namespace Examples.Shapes;

public partial class RectangleAdvanced : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Rectangle Advanced";

    public string Title => "raylib [shapes] example - rectangle advanced";

    public void Init()
    {
    }

    public void Update()
    {
        // Update rectangle bounds
        //----------------------------------------------------------------------------------
        float width = GetScreenWidth() / 2.0f, height = GetScreenHeight() / 6.0f;
        Rectangle rec = new Rectangle(
            GetScreenWidth() / 2.0f - width / 2,
            GetScreenHeight() / 2.0f - 5 * (height / 2),
            width, height
        );
        //--------------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw All Rectangles with different roundess for each side and different gradients
        DrawRectangleRoundedGradientH(rec, 0.8f, 0.8f, 36, Color.Blue, Color.Red);

        rec.Y += rec.Height + 1;
        DrawRectangleRoundedGradientH(rec, 0.5f, 1.0f, 36, Color.Red, Color.Pink);

        rec.Y += rec.Height + 1;
        DrawRectangleRoundedGradientH(rec, 1.0f, 0.5f, 36, Color.Red, Color.Blue);

        rec.Y += rec.Height + 1;
        DrawRectangleRoundedGradientH(rec, 0.0f, 1.0f, 36, Color.Blue, Color.Black);

        rec.Y += rec.Height + 1;
        DrawRectangleRoundedGradientH(rec, 1.0f, 0.0f, 36, Color.Blue, Color.Pink);
        EndDrawing();
        //--------------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //--------------------------------------------------------------------------------------
    // Module Functions Definition
    //--------------------------------------------------------------------------------------
    // Draw rectangle with rounded edges and horizontal gradient, with options to choose side of roundness
    // NOTE: Adapted from both 'DrawRectangleRounded()' and 'DrawRectangleGradientH()' raylib [rshapes] implementations
    private static void DrawRectangleRoundedGradientH(Rectangle rec, float roundnessLeft, float roundnessRight, int segments, Color left, Color right)
    {
        // Neither side is rounded
        if ((roundnessLeft <= 0.0f && roundnessRight <= 0.0f) || (rec.Width < 1) || (rec.Height < 1))
        {
            DrawRectangleGradientEx(rec, left, left, right, right);
            return;
        }

        if (roundnessLeft >= 1.0f) roundnessLeft = 1.0f;
        if (roundnessRight >= 1.0f) roundnessRight = 1.0f;

        // Calculate corner radius both from right and left
        float recSize = rec.Width > rec.Height ? rec.Height : rec.Width;
        float radiusLeft = (recSize * roundnessLeft) / 2;
        float radiusRight = (recSize * roundnessRight) / 2;

        if (radiusLeft <= 0.0f) radiusLeft = 0.0f;
        if (radiusRight <= 0.0f) radiusRight = 0.0f;

        if (radiusRight <= 0.0f && radiusLeft <= 0.0f) return;

        float stepLength = 90.0f / (float)segments;

        /*
        Diagram Copied here for reference, original at 'DrawRectangleRounded()' source code

              P0____________________P1
              /|                    |\
             /1|          2         |3\
         P7 /__|____________________|__\ P2
           |   |P8                P9|   |
           | 8 |          9         | 4 |
           | __|____________________|__ |
         P6 \  |P11              P10|  / P3
             \7|          6         |5/
              \|____________________|/
              P5                    P4
        */

        // Coordinates of the 12 points also adapted from `DrawRectangleRounded`
        Vector2[] point = new Vector2[12]
        {
            // PO, P1, P2
            new Vector2(rec.X + radiusLeft, rec.Y), new Vector2((rec.X + rec.Width) - radiusRight, rec.Y), new Vector2(rec.X + rec.Width, rec.Y + radiusRight),
            // P3, P4
            new Vector2(rec.X + rec.Width, (rec.Y + rec.Height) - radiusRight), new Vector2((rec.X + rec.Width) - radiusRight, rec.Y + rec.Height),
            // P5, P6, P7
            new Vector2(rec.X + radiusLeft, rec.Y + rec.Height), new Vector2(rec.X, (rec.Y + rec.Height) - radiusLeft), new Vector2(rec.X, rec.Y + radiusLeft),
            // P8, P9
            new Vector2(rec.X + radiusLeft, rec.Y + radiusLeft), new Vector2((rec.X + rec.Width) - radiusRight, rec.Y + radiusRight),
            // P10, P11
            new Vector2((rec.X + rec.Width) - radiusRight, (rec.Y + rec.Height) - radiusRight), new Vector2(rec.X + radiusLeft, (rec.Y + rec.Height) - radiusLeft)
        };

        Vector2[] centers = new Vector2[4] { point[8], point[9], point[10], point[11] };
        float[] angles = new float[4] { 180.0f, 270.0f, 0.0f, 90.0f };

        // Here we use the 'Diagram' to guide ourselves to which point receives what color
        // By choosing the color correctly associated with a point the gradient effect
        // will naturally come from OpenGL interpolation
        // But this time instead of Quad, we think in triangles

        Begin(DrawMode.Triangles);
        // Draw all of the 4 corners: [1] Upper Left Corner, [3] Upper Right Corner, [5] Lower Right Corner, [7] Lower Left Corner
        for (int k = 0; k < 4; ++k)
        {
            Color color = new Color(0, 0, 0, 0);
            float radius = 0.0f;
            if (k == 0) { color = left; radius = radiusLeft; }     // [1] Upper Left Corner
            if (k == 1) { color = right; radius = radiusRight; }   // [3] Upper Right Corner
            if (k == 2) { color = right; radius = radiusRight; }   // [5] Lower Right Corner
            if (k == 3) { color = left; radius = radiusLeft; }     // [7] Lower Left Corner

            float angle = angles[k];
            Vector2 center = centers[k];

            for (int i = 0; i < segments; i++)
            {
                Color4ub(color.R, color.G, color.B, color.A);
                Vertex2f(center.X, center.Y);
                Vertex2f(center.X + MathF.Cos(DEG2RAD * (angle + stepLength)) * radius, center.Y + MathF.Sin(DEG2RAD * (angle + stepLength)) * radius);
                Vertex2f(center.X + MathF.Cos(DEG2RAD * angle) * radius, center.Y + MathF.Sin(DEG2RAD * angle) * radius);
                angle += stepLength;
            }
        }

        // [2] Upper Rectangle
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[0].X, point[0].Y);
        Vertex2f(point[8].X, point[8].Y);
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[9].X, point[9].Y);
        Vertex2f(point[1].X, point[1].Y);
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[0].X, point[0].Y);
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[9].X, point[9].Y);

        // [4] Right Rectangle
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[9].X, point[9].Y);
        Vertex2f(point[10].X, point[10].Y);
        Vertex2f(point[3].X, point[3].Y);
        Vertex2f(point[2].X, point[2].Y);
        Vertex2f(point[9].X, point[9].Y);
        Vertex2f(point[3].X, point[3].Y);

        // [6] Bottom Rectangle
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[11].X, point[11].Y);
        Vertex2f(point[5].X, point[5].Y);
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[4].X, point[4].Y);
        Vertex2f(point[10].X, point[10].Y);
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[11].X, point[11].Y);
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[4].X, point[4].Y);

        // [8] Left Rectangle
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[7].X, point[7].Y);
        Vertex2f(point[6].X, point[6].Y);
        Vertex2f(point[11].X, point[11].Y);
        Vertex2f(point[8].X, point[8].Y);
        Vertex2f(point[7].X, point[7].Y);
        Vertex2f(point[11].X, point[11].Y);

        // [9] Middle Rectangle
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[8].X, point[8].Y);
        Vertex2f(point[11].X, point[11].Y);
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[10].X, point[10].Y);
        Vertex2f(point[9].X, point[9].Y);
        Color4ub(left.R, left.G, left.B, left.A);
        Vertex2f(point[8].X, point[8].Y);
        Color4ub(right.R, right.G, right.B, right.A);
        Vertex2f(point[10].X, point[10].Y);
        End();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - rectangle advanced");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new RectangleAdvanced();
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
