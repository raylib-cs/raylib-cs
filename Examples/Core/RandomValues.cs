/*******************************************************************************************
*
*   raylib [core] example - random values
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.1, last time updated with raylib 1.1
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class RandomValues : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Random Values";

    public string Title => "raylib [core] example - random values";

    private int randValue;   // Get a random integer number between -8 and 5 (both included)
    private int framesCounter; // Variable used to count frames

    public void Init()
    {
        // SetRandomSeed(0xaabbccff);   // Set a custom random seed if desired, by default: "time(NULL)"

        randValue = GetRandomValue(-8, 5);   // Get a random integer number between -8 and 5 (both included)

        framesCounter = 0; // Variable used to count frames
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        framesCounter++;

        // Every two seconds (120 frames) a new random value is generated
        if (((framesCounter / 120) % 2) == 1)
        {
            randValue = GetRandomValue(-8, 5);
            framesCounter = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("Every 2 seconds a new random value is generated:", 130, 100, 20, Color.Maroon);

        DrawText($"{randValue}", 360, 180, 80, Color.LightGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - random values");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new RandomValues();
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
