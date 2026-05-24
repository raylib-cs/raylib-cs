/*******************************************************************************************
*
*   raylib [shaders] example - Apply a shader to some shape or texture
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version.
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3), to test this example
*         on OpenGL ES 2.0 platforms (Android, Raspberry Pi, HTML5), use #version 100 shaders
*         raylib comes with shaders ready for both versions, check raylib/shaders install folder
*
*   This example has been created using raylib 1.7 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2015 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class ShapesTextures
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - shapes and texture shaders");

        Texture2D fudesumi = LoadTexture("resources/fudesumi.png");

        // NOTE: Using GLSL 330 shader version, on OpenGL ES 2.0 use GLSL 100 shader version
        Shader shader = LoadShader(
            "resources/shaders/glsl330/base.vs",
            "resources/shaders/glsl330/grayscale.fs"
        );

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            // TODO: Update your variables here
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            // Start drawing with default shader
            DrawText("USING DEFAULT SHADER", 20, 40, 10, Color.Red);

            DrawCircle(80, 120, 35, Color.DarkBlue);
            DrawCircleGradient(new Vector2(80, 220), 60, Color.Green, Color.SkyBlue);
            DrawCircleLines(80, 340, 80, Color.DarkBlue);


            // Activate our custom shader to be applied on next shapes/textures drawings
            BeginShaderMode(shader);

            DrawText("USING CUSTOM SHADER", 190, 40, 10, Color.Red);

            DrawRectangle(250 - 60, 90, 120, 60, Color.Red);
            DrawRectangleGradientH(250 - 90, 170, 180, 130, Color.Maroon, Color.Gold);
            DrawRectangleLines(250 - 40, 320, 80, 60, Color.Orange);

            // Activate our default shader for next drawings
            EndShaderMode();

            DrawText("USING DEFAULT SHADER", 370, 40, 10, Color.Red);

            DrawTriangle(
                new Vector2(430, 80),
                new Vector2(430 - 60, 150),
                new Vector2(430 + 60, 150), Color.Violet
            );

            DrawTriangleLines(
                new Vector2(430, 160),
                new Vector2(430 - 20, 230),
                new Vector2(430 + 20, 230), Color.DarkBlue
            );

            DrawPoly(new Vector2(430, 320), 6, 80, 0, Color.Brown);

            // Activate our custom shader to be applied on next shapes/textures drawings
            BeginShaderMode(shader);

            // Using custom shader
            DrawTexture(fudesumi, 500, -30, Color.White);

            // Activate our default shader for next drawings
            EndShaderMode();

            DrawText("(c) Fudesumi sprite by Eiden Marsal", 380, screenHeight - 20, 10, Color.Gray);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadShader(shader);
        UnloadTexture(fudesumi);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }

}
