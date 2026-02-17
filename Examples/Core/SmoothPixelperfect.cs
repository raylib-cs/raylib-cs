/*******************************************************************************************
*
*   raylib [core] example - smooth pixel-perfect camera
*
*   This example has been created using raylib 3.7 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Example contributed by Giancamillo Alessandroni (@NotManyIdeasDev) and
*   reviewed by Ramon Santamaria (@raysan5)
*
*   Copyright (c) 2021 Giancamillo Alessandroni (@NotManyIdeasDev) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public static class SmoothPixelPerfect
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
                                 const int screenWidth = 800;
                                 const int screenHeight = 450;

                                 const int virtualScreenWidth = 160;
                                 const int virtualScreenHeight = 90;

                                 const float virtualRatio = (float)screenWidth / (float)virtualScreenWidth;

                                 InitWindow(screenWidth, screenHeight, "raylib [core] example - smooth pixel-perfect camera");

                                 // Game world camera
                                 Camera2D worldSpaceCamera = new();
                                 worldSpaceCamera.Zoom = 1.0f;

                                 // Smoothing camera
                                 Camera2D screenSpaceCamera = new();
                                 screenSpaceCamera.Zoom = 1.0f;

                                 // This is where we'll draw all our objects.
                                 RenderTexture2D target = LoadRenderTexture(virtualScreenWidth, virtualScreenHeight);

                                 Rectangle rec01 = new(70.0f, 35.0f, 20.0f, 20.0f);
                                 Rectangle rec02 = new(90.0f, 55.0f, 30.0f, 10.0f);
                                 Rectangle rec03 = new(80.0f, 65.0f, 15.0f, 25.0f);

                                 // The target's height is flipped (in the source Rectangle), due to OpenGL reasons
                                 Rectangle sourceRec = new(
                                     0.0f,
                                     0.0f,
                                     (float)target.Texture.Width,
                                     -(float)target.Texture.Height
                                 );
                                 Rectangle destRec = new(
                                     -virtualRatio,
                                     -virtualRatio,
                                     screenWidth + (virtualRatio * 2),
                                     screenHeight + (virtualRatio * 2)
                                 );

                                 Vector2 origin = new(0.0f, 0.0f);

                                 float rotation = 0.0f;

                                 float cameraX = 0.0f;
                                 float cameraY = 0.0f;

                                 SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            rotation += 60.0f * GetFrameTime();   // Rotate the rectangles, 60 degrees per second

            // Make the camera move to demonstrate the effect
            cameraX = (MathF.Sin((float)GetTime()) * 50.0f) - 10.0f;
            cameraY = MathF.Cos((float)GetTime()) * 30.0f;

            // Set the camera's target to the values computed above
            screenSpaceCamera.Target = new Vector2(cameraX, cameraY);

            // Round worldSpace coordinates, keep decimals into screenSpace coordinates
            worldSpaceCamera.Target.X = (int)screenSpaceCamera.Target.X;
            screenSpaceCamera.Target.X -= worldSpaceCamera.Target.X;
            screenSpaceCamera.Target.X *= virtualRatio;

            worldSpaceCamera.Target.Y = (int)screenSpaceCamera.Target.Y;
            screenSpaceCamera.Target.Y -= worldSpaceCamera.Target.Y;
            screenSpaceCamera.Target.Y *= virtualRatio;
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginTextureMode(target);
            ClearBackground(Color.RayWhite);

            BeginMode2D(worldSpaceCamera);
            DrawRectanglePro(rec01, origin, rotation, Color.Black);
            DrawRectanglePro(rec02, origin, -rotation, Color.Red);
            DrawRectanglePro(rec03, origin, rotation + 45.0f, Color.Blue);
            EndMode2D();

            EndTextureMode();

            BeginDrawing();
            ClearBackground(Color.Red);

            BeginMode2D(screenSpaceCamera);
            DrawTexturePro(target.Texture, sourceRec, destRec, origin, 0.0f, Color.White);
            EndMode2D();

            DrawText($"Screen resolution: {screenWidth}x{screenHeight}", 10, 10, 20, Color.DarkBlue);
            DrawText($"World resolution: {virtualScreenWidth}x{virtualScreenHeight}", 10, 40, 20, Color.DarkGreen);
            DrawFPS(GetScreenWidth() - 95, 10);
            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadRenderTexture(target);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
