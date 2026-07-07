/*******************************************************************************************
*
*   raylib [shapes] example - lines bezier
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.7, last time updated with raylib 1.7
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2017-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class LinesBezier : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Lines Bezier";

    public string Title => "raylib [shapes] example - lines bezier";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private bool moveStartPoint;
    private bool moveEndPoint;

    public void Init()
    {
        startPoint = new(30, 30);
        endPoint = new(screenWidth - 30, screenHeight - 30);
        moveStartPoint = false;
        moveEndPoint = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var mouse = GetMousePosition();

        if (CheckCollisionPointCircle(mouse, startPoint, 10.0f) && IsMouseButtonDown(MouseButton.Left))
        {
            moveStartPoint = true;
        }
        else if (CheckCollisionPointCircle(mouse, endPoint, 10.0f) && IsMouseButtonDown(MouseButton.Left))
        {
            moveEndPoint = true;
        }

        if (moveStartPoint)
        {
            startPoint = mouse;
            if (IsMouseButtonReleased(MouseButton.Left))
            {
                moveStartPoint = false;
            }
        }

        if (moveEndPoint)
        {
            endPoint = mouse;
            if (IsMouseButtonReleased(MouseButton.Left))
            {
                moveEndPoint = false;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("MOVE START-END POINTS WITH MOUSE", 15, 20, 20, Color.Gray);

        // Draw line Cubic Bezier, in-out interpolation (easing), no control points
        DrawLineBezier(startPoint, endPoint, 4.0f, Color.Blue);

        // Draw start-end spline circles with some details
        DrawCircleV(startPoint, CheckCollisionPointCircle(mouse, startPoint, 10.0f) ? 14.0f : 8.0f, moveStartPoint ? Color.Red : Color.Blue);
        DrawCircleV(endPoint, CheckCollisionPointCircle(mouse, endPoint, 10.0f) ? 14.0f : 8.0f, moveEndPoint ? Color.Red : Color.Blue);

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
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - lines bezier");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LinesBezier();
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
