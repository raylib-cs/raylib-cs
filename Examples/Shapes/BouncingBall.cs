/*******************************************************************************************
*
*   raylib [shapes] example - bouncing ball
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example contributed by Ramon Santamaria (@raysan5), reviewed by Jopestpe (@jopestpe)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2013-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class BouncingBall : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Bouncing Ball";

    public string Title => "raylib [shapes] example - bouncing ball";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Vector2 ballPosition;
    private Vector2 ballSpeed;
    private int ballRadius;
    private float gravity;

    private bool useGravity;
    private bool pause;
    private int framesCounter;

    public void Init()
    {
        ballPosition = new(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
        ballSpeed = new(5.0f, 4.0f);
        ballRadius = 20;
        gravity = 0.2f;

        useGravity = true;
        pause = false;
        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //-----------------------------------------------------
        if (IsKeyPressed(KeyboardKey.G))
        {
            useGravity = !useGravity;
        }
        if (IsKeyPressed(KeyboardKey.Space))
        {
            pause = !pause;
        }

        if (!pause)
        {
            ballPosition.X += ballSpeed.X;
            ballPosition.Y += ballSpeed.Y;

            if (useGravity)
            {
                ballSpeed.Y += gravity;
            }

            // Check walls collision for bouncing
            if ((ballPosition.X >= (GetScreenWidth() - ballRadius)) || (ballPosition.X <= ballRadius))
            {
                ballSpeed.X *= -1.0f;
            }
            if ((ballPosition.Y >= (GetScreenHeight() - ballRadius)) || (ballPosition.Y <= ballRadius))
            {
                ballSpeed.Y *= -0.95f;
            }
        }
        else
        {
            framesCounter++;
        }
        //-----------------------------------------------------

        // Draw
        //-----------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawCircleV(ballPosition, (float)ballRadius, Color.Maroon);
        DrawText("PRESS SPACE to PAUSE BALL MOVEMENT", 10, GetScreenHeight() - 25, 20, Color.LightGray);

        if (useGravity)
        {
            DrawText("GRAVITY: ON (Press G to disable)", 10, GetScreenHeight() - 50, 20, Color.DarkGreen);
        }
        else
        {
            DrawText("GRAVITY: OFF (Press G to enable)", 10, GetScreenHeight() - 50, 20, Color.Red);
        }

        // On pause, we draw a blinking message
        if (pause && ((framesCounter / 30) % 2) != 0)
        {
            DrawText("PAUSED", 350, 200, 30, Color.Gray);
        }

        DrawFPS(10, 10);

        EndDrawing();
        //-----------------------------------------------------
    }

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //---------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - bouncing ball");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //----------------------------------------------------------

        var game = new BouncingBall();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //---------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //----------------------------------------------------------

        return 0;
    }
}
