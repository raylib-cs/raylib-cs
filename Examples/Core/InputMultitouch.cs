/*******************************************************************************************
*
*   raylib [core] example - Input multitouch
*
*   This example has been created using raylib 2.1 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Example contributed by Berni (@Berni8k) and reviewed by Ramon Santamaria (@raysan5)
*
*   Copyright (c) 2019 Berni (@Berni8k) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class InputMultitouch
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - input multitouch");

        const int MaxTouchPoints = 10;
        Vector2[] touchPositions = new Vector2[MaxTouchPoints];

        SetTargetFPS(60);
        //---------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            // Get the touch point count (how many fingers are touching the screen )
            int tCount = GetTouchPointCount();

            // Clamp touch points available (set the maximum touch points allowed )
            if (tCount > MaxTouchPoints)
            {
                tCount = MaxTouchPoints;
            }

            // Get touch points positions
            for (int i = 0; i < tCount; i++)
            {
                touchPositions[i] = GetTouchPosition(i);
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            for (int i = 0; i < tCount; i++)
            {
                // Make sure point is not (0, 0) as this means there is no touch for it
                if ((touchPositions[i].X > 0) && (touchPositions[i].Y > 0))
                {
                    // Draw circle and touch index number
                    DrawCircleV(touchPositions[i], 34, Color.Orange);
                    DrawText(i.ToString(),
                        (int)touchPositions[i].X - 10,
                        (int)touchPositions[i].Y - 70,
                        40,
                        Color.Black
                    );
                }
            }

            DrawText("touch the screen at multiple locations to get multiple balls", 10, 10, 20, Color.DarkGray);

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
