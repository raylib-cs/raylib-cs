/*******************************************************************************************
*
*   raylib [shapes] example - easings ball
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using Examples.Shared;

namespace Examples.Shapes;

public partial class EasingsBallAnim : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Easings Ball Anim";

    public string Title => "raylib [shapes] example - easings ball";

    // Ball variable value to be animated with easings
    private int ballPositionX;
    private int ballRadius;
    private float ballAlpha;

    private int state;
    private int framesCounter;

    public void Init()
    {
        ballPositionX = -100;
        ballRadius = 20;
        ballAlpha = 0.0f;

        state = 0;
        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (state == 0)             // Move ball position X with easing
        {
            framesCounter++;
            ballPositionX = (int)Easings.EaseElasticOut(framesCounter, -100, screenWidth / 2.0f + 100, 120);

            if (framesCounter >= 120)
            {
                framesCounter = 0;
                state = 1;
            }
        }
        else if (state == 1)        // Increase ball radius with easing
        {
            framesCounter++;
            ballRadius = (int)Easings.EaseElasticIn(framesCounter, 20, 500, 200);

            if (framesCounter >= 200)
            {
                framesCounter = 0;
                state = 2;
            }
        }
        else if (state == 2)        // Change ball alpha with easing (background color blending)
        {
            framesCounter++;
            ballAlpha = Easings.EaseCubicOut(framesCounter, 0.0f, 1.0f, 200);

            if (framesCounter >= 200)
            {
                framesCounter = 0;
                state = 3;
            }
        }
        else if (state == 3)        // Reset state to play again
        {
            if (IsKeyPressed(KeyboardKey.Enter))
            {
                // Reset required variables to play again
                ballPositionX = -100;
                ballRadius = 20;
                ballAlpha = 0.0f;
                state = 0;
            }
        }

        if (IsKeyPressed(KeyboardKey.R))
        {
            framesCounter = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        if (state >= 2)
        {
            DrawRectangle(0, 0, screenWidth, screenHeight, Color.Green);
        }

        DrawCircle(ballPositionX, 200, ballRadius, Fade(Color.Red, 1.0f - ballAlpha));

        if (state == 3)
        {
            DrawText("PRESS [ENTER] TO PLAY AGAIN!", 240, 200, 20, Color.Black);
        }

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - easings ball");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new EasingsBallAnim();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
