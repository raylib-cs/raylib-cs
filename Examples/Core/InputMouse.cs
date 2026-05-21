/*******************************************************************************************
*
*   raylib [core] example - input mouse
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 5.5
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

public class InputMouse
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - mouse input");

        Vector2 ballPosition = new(-100.0f, -100.0f);
        Color ballColor = Color.DarkBlue;

        SetTargetFPS(60);
        //---------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------

            if (IsKeyPressed(KeyboardKey.H))
            {
                if (IsCursorHidden())
                {
                    ShowCursor();
                }
                else
                {
                    HideCursor();
                }
            }

            ballPosition = GetMousePosition();

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                ballColor = Color.Maroon;
            }
            else if (IsMouseButtonPressed(MouseButton.Middle))
            {
                ballColor = Color.Lime;
            }
            else if (IsMouseButtonPressed(MouseButton.Right))
            {
                ballColor = Color.DarkBlue;
            }
            else if (IsMouseButtonPressed(MouseButton.Extra))
            {
                ballColor = Color.Yellow;
            }
            else if (IsMouseButtonPressed(MouseButton.Forward))
            {
                ballColor = Color.Orange;
            }
            else if (IsMouseButtonPressed(MouseButton.Back))
            {
                ballColor = Color.Beige;
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawCircleV(ballPosition, 40, ballColor);

            DrawText("move ball with mouse and click mouse button to change color", 10, 10, 20, Color.DarkGray);
            DrawText("Press 'H' to toggle cursor visibility", 10, 30, 20, Color.DarkGray);

            if (IsCursorHidden())
            {
                DrawText("CURSOR HIDDEN", 20, 60, 20, Color.Red);
            }
            else
            {
                DrawText("CURSOR VISIBLE", 20, 60, 20, Color.Lime);
            }

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
