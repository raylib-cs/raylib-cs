/*******************************************************************************************
*
*   raylib [shapes] example - following eyes
*
*   This example has been created using raylib 2.5 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2013-2019 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public class FollowingEyes
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - following eyes");

        Vector2 scleraLeftPosition = new(GetScreenWidth() / 2 - 100, GetScreenHeight() / 2);
        Vector2 scleraRightPosition = new(GetScreenWidth() / 2 + 100, GetScreenHeight() / 2);
        float scleraRadius = 80;

        Vector2 irisLeftPosition = new(GetScreenWidth() / 2 - 100, GetScreenHeight() / 2);
        Vector2 irisRightPosition = new(GetScreenWidth() / 2 + 100, GetScreenHeight() / 2);
        float irisRadius = 24;

        float angle = 0.0f;
        float dx = 0.0f, dy = 0.0f, dxx = 0.0f, dyy = 0.0f;

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            irisLeftPosition = GetMousePosition();
            irisRightPosition = GetMousePosition();

            // Check not inside the left eye sclera
            if (!CheckCollisionPointCircle(irisLeftPosition, scleraLeftPosition, scleraRadius - 20))
            {
                dx = irisLeftPosition.X - scleraLeftPosition.X;
                dy = irisLeftPosition.Y - scleraLeftPosition.Y;

                angle = MathF.Atan2(dy, dx);

                dxx = (scleraRadius - irisRadius) * MathF.Cos(angle);
                dyy = (scleraRadius - irisRadius) * MathF.Sin(angle);

                irisLeftPosition.X = scleraLeftPosition.X + dxx;
                irisLeftPosition.Y = scleraLeftPosition.Y + dyy;
            }

            // Check not inside the right eye sclera
            if (!CheckCollisionPointCircle(irisRightPosition, scleraRightPosition, scleraRadius - 20))
            {
                dx = irisRightPosition.X - scleraRightPosition.X;
                dy = irisRightPosition.Y - scleraRightPosition.Y;

                angle = MathF.Atan2(dy, dx);

                dxx = (scleraRadius - irisRadius) * MathF.Cos(angle);
                dyy = (scleraRadius - irisRadius) * MathF.Sin(angle);

                irisRightPosition.X = scleraRightPosition.X + dxx;
                irisRightPosition.Y = scleraRightPosition.Y + dyy;
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawCircleV(scleraLeftPosition, scleraRadius, Color.LightGray);
            DrawCircleV(irisLeftPosition, irisRadius, Color.Brown);
            DrawCircleV(irisLeftPosition, 10, Color.Black);

            DrawCircleV(scleraRightPosition, scleraRadius, Color.LightGray);
            DrawCircleV(irisRightPosition, irisRadius, Color.DarkGreen);
            DrawCircleV(irisRightPosition, 10, Color.Black);

            DrawFPS(10, 10);

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
