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

namespace Examples.Core;

public partial class InputKeys : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Input Keys";

    public string Title => "raylib [core] example - input keys";

    private Vector2 ballPosition;

    public void Init()
    {
        ballPosition = new((float)screenWidth / 2, (float)screenHeight / 2);
    }

    public void Update()
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

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - input keys");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new InputKeys();
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
