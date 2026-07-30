/*******************************************************************************************
*
*   raylib [core] example - input multitouch
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 2.1, last time updated with raylib 2.5
*
*   Example contributed by Berni (@Berni8k) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Berni (@Berni8k) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class InputMultitouch : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxTouchPoints = 10;

    public string Name => "Core / Input Multitouch";

    public string Title => "raylib [core] example - input multitouch";

    private Vector2[] touchPositions;

    public void Init()
    {
        touchPositions = new Vector2[MaxTouchPoints];
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Get the touch point count ( how many fingers are touching the screen )
        var tCount = GetTouchPointCount();

        // Clamp touch points available ( set the maximum touch points allowed )
        if (tCount > MaxTouchPoints)
        {
            tCount = MaxTouchPoints;
        }

        // Get touch points positions
        for (var i = 0; i < tCount; i++)
        {
            touchPositions[i] = GetTouchPosition(i);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        for (var i = 0; i < tCount; i++)
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

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - input multitouch");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //---------------------------------------------------------------------------------------

        var game = new InputMultitouch();
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
