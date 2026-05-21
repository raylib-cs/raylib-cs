/*******************************************************************************************
*
*   raylib [core] example - 2d camera
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.5, last time updated with raylib 3.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class Camera2dDemo
{
    public const int MaxBuildings = 100;

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - 2d camera");

        Rectangle player = new(400, 280, 40, 40);
        Rectangle[] buildings = new Rectangle[MaxBuildings];
        Color[] buildColors = new Color[MaxBuildings];

        int spacing = 0;

        for (int i = 0; i < MaxBuildings; i++)
        {
            buildings[i].Width = GetRandomValue(50, 200);
            buildings[i].Height = GetRandomValue(100, 800);
            buildings[i].Y = screenHeight - 130 - buildings[i].Height;
            buildings[i].X = -6000 + spacing;

            spacing += (int)buildings[i].Width;

            buildColors[i] = new Color(
                GetRandomValue(200, 240),
                GetRandomValue(200, 240),
                GetRandomValue(200, 250),
                255
            );
        }

        Camera2D camera = new();
        camera.Target = new Vector2(player.X + 20, player.Y + 20);
        camera.Offset = new Vector2(screenWidth / 2, screenHeight / 2);
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------

            // Player movement
            if (IsKeyDown(KeyboardKey.Right))
            {
                player.X += 2;
            }
            else if (IsKeyDown(KeyboardKey.Left))
            {
                player.X -= 2;
            }

            // Camera3D target follows player
            camera.Target = new Vector2(player.X + 20, player.Y + 20);

            // Camera3D rotation controls
            if (IsKeyDown(KeyboardKey.A))
            {
                camera.Rotation--;
            }
            else if (IsKeyDown(KeyboardKey.S))
            {
                camera.Rotation++;
            }

            // Limit camera rotation to 80 degrees (-40 to 40)
            if (camera.Rotation > 40)
            {
                camera.Rotation = 40;
            }
            else if (camera.Rotation < -40)
            {
                camera.Rotation = -40;
            }

            // Camera3D zoom controls
            camera.Zoom += ((float)GetMouseWheelMove() * 0.05f);

            if (camera.Zoom > 3.0f)
            {
                camera.Zoom = 3.0f;
            }
            else if (camera.Zoom < 0.1f)
            {
                camera.Zoom = 0.1f;
            }

            // Camera3D reset (zoom and rotation)
            if (IsKeyPressed(KeyboardKey.R))
            {
                camera.Zoom = 1.0f;
                camera.Rotation = 0.0f;
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            BeginMode2D(camera);

            DrawRectangle(-6000, 320, 13000, 8000, Color.DarkGray);

            for (int i = 0; i < MaxBuildings; i++)
            {
                DrawRectangleRec(buildings[i], buildColors[i]);
            }

            DrawRectangleRec(player, Color.Red);

            DrawRectangle((int)camera.Target.X, -500, 1, (int)(screenHeight * 4), Color.Green);
            DrawLine(
                (int)(-screenWidth * 10),
                (int)camera.Target.Y,
                (int)(screenWidth * 10),
                (int)camera.Target.Y,
                Color.Green
            );

            EndMode2D();

            DrawText("SCREEN AREA", 640, 10, 20, Color.Red);

            DrawRectangle(0, 0, (int)screenWidth, 5, Color.Red);
            DrawRectangle(0, 5, 5, (int)screenHeight - 10, Color.Red);
            DrawRectangle((int)screenWidth - 5, 5, 5, (int)screenHeight - 10, Color.Red);
            DrawRectangle(0, (int)screenHeight - 5, (int)screenWidth, 5, Color.Red);

            DrawRectangle(10, 10, 250, 113, ColorAlpha(Color.SkyBlue, 0.5f));
            DrawRectangleLines(10, 10, 250, 113, Color.Blue);

            DrawText("Free 2d camera controls:", 20, 20, 10, Color.Black);
            DrawText("- Right/Left to move Offset", 40, 40, 10, Color.DarkGray);
            DrawText("- Mouse Wheel to Zoom in-out", 40, 60, 10, Color.DarkGray);
            DrawText("- A / S to Rotate", 40, 80, 10, Color.DarkGray);
            DrawText("- R to reset Zoom and Rotation", 40, 100, 10, Color.DarkGray);

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
