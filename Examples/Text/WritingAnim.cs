/*******************************************************************************************
*
*   raylib [text] example - writing anim
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.4, last time updated with raylib 1.4
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class WritingAnim : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Text / Writing Animation";

    public string Title => "raylib [text] example - writing anim";

    private string message;
    private int framesCounter;

    public void Init()
    {
        message = "This sample illustrates a text writing\nanimation effect! Check it out! ;)";

        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyDown(KeyboardKey.Space))
        {
            framesCounter += 8;
        }
        else
        {
            framesCounter += 1;
        }

        if (IsKeyPressed(KeyboardKey.Enter))
        {
            framesCounter = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText(message.SubText(0, framesCounter / 10), 210, 160, 20, Color.Maroon);

        DrawText("PRESS [ENTER] to RESTART!", 240, 260, 20, Color.LightGray);
        DrawText("HOLD [SPACE] to SPEED UP!", 239, 300, 20, Color.LightGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [text] example - writing anim");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new WritingAnim();
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
