/*******************************************************************************************
*
*   raylib [shapes] example - vector angle
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 5.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;        // Required for: Vector2LineAngle()

namespace Examples.Shapes;

public partial class VectorAngle : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Vector Angle";

    public string Title => "raylib [shapes] example - vector angle";

    private Vector2 v0;
    private Vector2 v1;
    private Vector2 v2;             // Updated with mouse position

    private float angle;           // Angle in degrees
    private int angleMode;         // 0-Vector2Angle(), 1-Vector2LineAngle()

    public void Init()
    {
        v0 = new(screenWidth / 2.0f, screenHeight / 2.0f);
        v1 = Vector2Add(v0, new Vector2(100.0f, 80.0f));
        v2 = new(0, 0);            // Updated with mouse position

        angle = 0.0f;             // Angle in degrees
        angleMode = 0;            // 0-Vector2Angle(), 1-Vector2LineAngle()
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float startangle = 0.0f;

        if (angleMode == 0) startangle = -Vector2LineAngle(v0, v1) * RAD2DEG;
        if (angleMode == 1) startangle = 0.0f;

        v2 = GetMousePosition();

        if (IsKeyPressed(KeyboardKey.Space)) angleMode = (angleMode == 0) ? 1 : 0;

        if ((angleMode == 0) && IsMouseButtonDown(MouseButton.Right)) v1 = GetMousePosition();

        if (angleMode == 0)
        {
            // Calculate angle between two vectors, considering a common origin (v0)
            Vector2 v1Normal = Vector2Normalize(Vector2Subtract(v1, v0));
            Vector2 v2Normal = Vector2Normalize(Vector2Subtract(v2, v0));

            angle = Vector2Angle(v1Normal, v2Normal) * RAD2DEG;
        }
        else if (angleMode == 1)
        {
            // Calculate angle defined by a two vectors line, in reference to horizontal line
            angle = Vector2LineAngle(v0, v2) * RAD2DEG;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (angleMode == 0)
        {
            DrawText("MODE 0: Angle between V1 and V2", 10, 10, 20, Color.Black);
            DrawText("Right Click to Move V2", 10, 30, 20, Color.DarkGray);

            DrawLineEx(v0, v1, 2.0f, Color.Black);
            DrawLineEx(v0, v2, 2.0f, Color.Red);

            DrawCircleSector(v0, 40.0f, startangle, startangle + angle, 32, Fade(Color.Green, 0.6f));
        }
        else if (angleMode == 1)
        {
            DrawText("MODE 1: Angle formed by line V1 to V2", 10, 10, 20, Color.Black);

            DrawLine(0, screenHeight / 2, screenWidth, screenHeight / 2, Color.LightGray);
            DrawLineEx(v0, v2, 2.0f, Color.Red);

            DrawCircleSector(v0, 40.0f, startangle, startangle - angle, 32, Fade(Color.Green, 0.6f));
        }

        DrawText("v0", (int)v0.X, (int)v0.Y, 10, Color.DarkGray);

        // If the line from v0 to v1 would overlap the text, move it's position up 10
        if (angleMode == 0 && Vector2Subtract(v0, v1).Y > 0.0f) DrawText("v1", (int)v1.X, (int)v1.Y - 10, 10, Color.DarkGray);
        if (angleMode == 0 && Vector2Subtract(v0, v1).Y < 0.0f) DrawText("v1", (int)v1.X, (int)v1.Y, 10, Color.DarkGray);

        // If angle mode 1, use v1 to emphasize the horizontal line
        if (angleMode == 1) DrawText("v1", (int)v0.X + 40, (int)v0.Y, 10, Color.DarkGray);

        // position adjusted by -10 so it isn't hidden by cursor
        DrawText("v2", (int)v2.X - 10, (int)v2.Y - 10, 10, Color.DarkGray);

        DrawText("Press SPACE to change MODE", 460, 10, 20, Color.DarkGray);
        DrawText($"ANGLE: {angle:F2}", 10, 70, 20, Color.Lime);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - vector angle");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new VectorAngle();
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
