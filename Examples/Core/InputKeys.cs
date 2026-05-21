/*******************************************************************************************
*
*   raylib [core] example - input keys
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 1.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class InputKeys
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - keyboard input");

        Vector2 ballPosition = new((float)screenWidth / 2, (float)screenHeight / 2);

        SetTargetFPS(60);       // Set target frames-per-second
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            if (IsKeyDown(KeyboardKey.Right))
            {
                ballPosition.X += 2.0f;
            }

            if (IsKeyDown(KeyboardKey.Left))
            {
                ballPosition.X -= 2.0f;
            }

            if (IsKeyDown(KeyboardKey.Up))
            {
                ballPosition.Y -= 2.0f;
            }

            if (IsKeyDown(KeyboardKey.Down))
            {
                ballPosition.Y += 2.0f;
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawText("move the ball with arrow keys", 10, 10, 20, Color.DarkGray);

            DrawCircleV(ballPosition, 50, Color.Maroon);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
