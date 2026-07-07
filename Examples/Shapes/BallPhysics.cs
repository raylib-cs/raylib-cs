/*******************************************************************************************
*
*   raylib [shapes] example - ball physics
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by David Buzatto (@davidbuzatto) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 David Buzatto (@davidbuzatto)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Shapes;

public partial class BallPhysics : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_BALLS = 5000;  // Maximum quantity of balls

    public string Name => "Shapes / Ball Physics";

    public string Title => "raylib [shapes] example - ball physics";

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    // Ball data type
    private struct Ball
    {
        public Vector2 position;
        public Vector2 speed;
        public Vector2 prevPosition;
        public float radius;
        public float friction;
        public float elasticity;
        public Color color;
        public bool grabbed;
    }

    private Ball[] balls;
    private int ballCount;
    private int grabbedBallIndex;   // Index of the current ball that is grabbed (-1 if none)
    private Vector2 pressOffset;    // Mouse press offset relative to the ball that grabbedd

    private float gravity;          // World gravity

    private Vector2 windowPosition;

    public void Init()
    {
        balls = new Ball[MAX_BALLS];

        // Init first ball in the array
        balls[0] = new Ball
        {
            position = new Vector2(GetScreenWidth()/2.0f, GetScreenHeight()/2.0f),
            speed = new Vector2(200, 200),
            prevPosition = new Vector2(0, 0),
            radius = 40,
            friction = 0.99f,
            elasticity = 0.9f,
            color = Color.Blue,
            grabbed = false
        };

        ballCount = 1;
        grabbedBallIndex = -1;          // A reference to the current ball that is grabbed
        pressOffset = new Vector2(0, 0); // Mouse press offset relative to the ball that grabbedd

        gravity = 100;                  // World gravity

        windowPosition = GetWindowPosition();
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float delta = GetFrameTime();
        Vector2 mousePos = GetMousePosition();

        // Checks if a ball was grabbed
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            for (int i = ballCount - 1; i >= 0; i--)
            {
                pressOffset.X = mousePos.X - balls[i].position.X;
                pressOffset.Y = mousePos.Y - balls[i].position.Y;

                // If the distance between the ball position and the mouse press position
                // is less than or equal to the ball radius, the event occurred inside the ball
                if (MathF.Sqrt(pressOffset.X*pressOffset.X + pressOffset.Y*pressOffset.Y) <= balls[i].radius)
                {
                    balls[i].grabbed = true;
                    grabbedBallIndex = i;
                    break;
                }
            }
        }

        // Releases any ball the was grabbed
        if (IsMouseButtonReleased(MouseButton.Left))
        {
            if (grabbedBallIndex != -1)
            {
                balls[grabbedBallIndex].grabbed = false;
                grabbedBallIndex = -1;
            }
        }

        // Creates a new ball
        if (IsMouseButtonPressed(MouseButton.Right) || (IsKeyDown(KeyboardKey.LeftControl) && IsMouseButtonDown(MouseButton.Right)))
        {
            if (ballCount < MAX_BALLS)
            {
                balls[ballCount++] = new Ball
                {
                    position = mousePos,
                    speed = new Vector2(GetRandomValue(-300, 300), GetRandomValue(-300, 300)),
                    prevPosition = new Vector2(0, 0),
                    radius = 20.0f + GetRandomValue(0, 30),
                    friction = 0.99f,
                    elasticity = 0.9f,
                    color = new Color(GetRandomValue(0, 255), GetRandomValue(0, 255), GetRandomValue(0, 255), 255),
                    grabbed = false
                };
            }
        }

        // Get window position change for shaking
        Vector2 windowPositionDelta = Vector2Subtract(windowPosition, GetWindowPosition());

        if (Vector2Length(windowPositionDelta) > 5.0f)
        {
            for (int i = 0; i < ballCount; i++)
            {
                if (!balls[i].grabbed) balls[i].speed = Vector2Add(balls[i].speed, Vector2Scale(windowPositionDelta, 10.0f));
            }
        }

        // Shake balls
        if (IsMouseButtonPressed(MouseButton.Middle))
        {
            for (int i = 0; i < ballCount; i++)
            {
                if (!balls[i].grabbed) balls[i].speed = new Vector2(GetRandomValue(-2000, 2000), GetRandomValue(-2000, 2000));
            }
        }

        // Changes gravity
        gravity += GetMouseWheelMove()*5;

        // Updates each ball state
        for (int i = 0; i < ballCount; i++)
        {
            // The ball is not grabbed
            if (!balls[i].grabbed)
            {
                // Ball repositioning using the velocity
                balls[i].position.X += balls[i].speed.X * delta;
                balls[i].position.Y += balls[i].speed.Y * delta;

                // Does the ball hit the screen right boundary?
                if ((balls[i].position.X + balls[i].radius) >= screenWidth)
                {
                    balls[i].position.X = screenWidth - balls[i].radius; // Ball repositioning
                    balls[i].speed.X = -balls[i].speed.X*balls[i].elasticity;  // Elasticity makes the ball lose 10% of its velocity on hit
                }
                // Does the ball hit the screen left boundary?
                else if ((balls[i].position.X - balls[i].radius) <= 0)
                {
                    balls[i].position.X = balls[i].radius;
                    balls[i].speed.X = -balls[i].speed.X*balls[i].elasticity;
                }

                // The same for y axis
                if ((balls[i].position.Y + balls[i].radius) >= screenHeight)
                {
                    balls[i].position.Y = screenHeight - balls[i].radius;
                    balls[i].speed.Y = -balls[i].speed.Y*balls[i].elasticity;
                }
                else if ((balls[i].position.Y - balls[i].radius) <= 0)
                {
                    balls[i].position.Y = balls[i].radius;
                    balls[i].speed.Y = -balls[i].speed.Y*balls[i].elasticity;
                }

                // Friction makes the ball lose 1% of its velocity each frame
                balls[i].speed.X = balls[i].speed.X*balls[i].friction;
                // Gravity affects only the y axis
                balls[i].speed.Y = balls[i].speed.Y*balls[i].friction + gravity;
            }
            else
            {
                // Ball repositioning using the mouse position
                balls[i].position.X = mousePos.X - pressOffset.X;
                balls[i].position.Y = mousePos.Y - pressOffset.Y;

                // While the ball is grabbed, recalculates its velocity
                balls[i].speed.X = (balls[i].position.X - balls[i].prevPosition.X)/delta;
                balls[i].speed.Y = (balls[i].position.Y - balls[i].prevPosition.Y)/delta;
                balls[i].prevPosition = balls[i].position;
            }
        }

        windowPosition = GetWindowPosition();
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

            ClearBackground(Color.RayWhite);

            for (int i = 0; i < ballCount; i++)
            {
                DrawCircleV(balls[i].position, balls[i].radius, balls[i].color);
                DrawCircleLinesV(balls[i].position, balls[i].radius, Color.Black);
            }

            DrawText("grab a ball by pressing with the mouse and throw it by releasing", 10, 10, 10, Color.DarkGray);
            DrawText("right click to create new balls (keep left control pressed to create a lot)", 10, 30, 10, Color.DarkGray);
            DrawText("use mouse wheel to change gravity", 10, 50, 10, Color.DarkGray);
            DrawText("middle click to shake", 10, 70, 10, Color.DarkGray);
            DrawText($"BALL COUNT: {ballCount}", 10, GetScreenHeight() - 70, 20, Color.Black);
            DrawText($"GRAVITY: {gravity:F2}", 10, GetScreenHeight() - 40, 20, Color.Black);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - ball physics");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new BallPhysics();
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
