/*******************************************************************************************
*
*   raylib [core] example - input mouse wheel
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.1, last time updated with raylib 1.3
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class InputMouseWheel : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Mouse Wheel";

    public string Title => "raylib [core] example - input mouse wheel";

    private int boxPositionY;
    private int scrollSpeed;                // Scrolling speed in pixels

    public void Init()
    {
        boxPositionY = screenHeight / 2 - 40;
        scrollSpeed = 4;            // Scrolling speed in pixels
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        boxPositionY -= (int)(GetMouseWheelMove() * scrollSpeed);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawRectangle(screenWidth / 2 - 40, boxPositionY, 80, 80, Color.Maroon);

        DrawText("Use mouse wheel to move the cube up and down!", 10, 10, 20, Color.Gray);
        DrawText($"Box position Y: {boxPositionY:000}", 10, 40, 20, Color.LightGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - input mouse wheel");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new InputMouseWheel();
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
