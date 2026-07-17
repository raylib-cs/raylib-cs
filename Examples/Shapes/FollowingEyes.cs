/*******************************************************************************************
*
*   raylib [shapes] example - following eyes
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2013-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class FollowingEyes : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Following Eyes";

    public string Title => "raylib [shapes] example - following eyes";

    private Vector2 scleraLeftPosition;
    private Vector2 scleraRightPosition;
    private float scleraRadius;

    private Vector2 irisLeftPosition;
    private Vector2 irisRightPosition;
    private float irisRadius;

    private float angle;
    private float dx, dy, dxx, dyy;

    public void Init()
    {
        scleraLeftPosition = new(GetScreenWidth() / 2.0f - 100.0f, GetScreenHeight() / 2.0f);
        scleraRightPosition = new(GetScreenWidth() / 2.0f + 100.0f, GetScreenHeight() / 2.0f);
        scleraRadius = 80;

        irisLeftPosition = new(GetScreenWidth() / 2.0f - 100.0f, GetScreenHeight() / 2.0f);
        irisRightPosition = new(GetScreenWidth() / 2.0f + 100.0f, GetScreenHeight() / 2.0f);
        irisRadius = 24;

        angle = 0.0f;
        dx = 0.0f;
        dy = 0.0f;
        dxx = 0.0f;
        dyy = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        irisLeftPosition = GetMousePosition();
        irisRightPosition = GetMousePosition();

        // Check not inside the left eye sclera
        if (!CheckCollisionPointCircle(irisLeftPosition, scleraLeftPosition, scleraRadius - irisRadius))
        {
            dx = irisLeftPosition.X - scleraLeftPosition.X;
            dy = irisLeftPosition.Y - scleraLeftPosition.Y;

            angle = MathF.Atan2(dy, dx);

            dxx = (scleraRadius - irisRadius) * MathF.Cos(angle);
            dyy = (scleraRadius - irisRadius) * MathF.Sin(angle);

            irisLeftPosition.X = scleraLeftPosition.X + dxx;
            irisLeftPosition.Y = scleraLeftPosition.Y + dyy;
        }

        // Check not inside the right eye sclera
        if (!CheckCollisionPointCircle(irisRightPosition, scleraRightPosition, scleraRadius - irisRadius))
        {
            dx = irisRightPosition.X - scleraRightPosition.X;
            dy = irisRightPosition.Y - scleraRightPosition.Y;

            angle = MathF.Atan2(dy, dx);

            dxx = (scleraRadius - irisRadius) * MathF.Cos(angle);
            dyy = (scleraRadius - irisRadius) * MathF.Sin(angle);

            irisRightPosition.X = scleraRightPosition.X + dxx;
            irisRightPosition.Y = scleraRightPosition.Y + dyy;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawCircleV(scleraLeftPosition, scleraRadius, Color.LightGray);
        DrawCircleV(irisLeftPosition, irisRadius, Color.Brown);
        DrawCircleV(irisLeftPosition, 10, Color.Black);

        DrawCircleV(scleraRightPosition, scleraRadius, Color.LightGray);
        DrawCircleV(irisRightPosition, irisRadius, Color.DarkGreen);
        DrawCircleV(irisRightPosition, 10, Color.Black);

        DrawFPS(10, 10);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - following eyes");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new FollowingEyes();
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
