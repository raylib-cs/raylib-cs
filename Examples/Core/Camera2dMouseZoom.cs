/*******************************************************************************************
*
*   raylib [core] example - 2d camera mouse zoom
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example contributed by Jeffery Myers (@JeffM2501) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 Jeffery Myers (@JeffM2501)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Core;

public partial class Camera2dMouseZoom : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Camera 2D Mouse Zoom";

    public string Title => "raylib [core] example - 2d camera mouse zoom";

    private Camera2D camera;
    private int zoomMode;       // 0-Mouse Wheel, 1-Mouse Move

    public void Init()
    {
        camera = new Camera2D();
        camera.Zoom = 1.0f;

        zoomMode = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.One))
        {
            zoomMode = 0;
        }
        else if (IsKeyPressed(KeyboardKey.Two))
        {
            zoomMode = 1;
        }

        // Translate based on mouse right click
        if (IsMouseButtonDown(MouseButton.Left))
        {
            Vector2 delta = GetMouseDelta();
            delta = Vector2Scale(delta, -1.0f / camera.Zoom);
            camera.Target = Vector2Add(camera.Target, delta);
        }

        if (zoomMode == 0)
        {
            // Zoom based on mouse wheel
            float wheel = GetMouseWheelMove();
            if (wheel != 0)
            {
                // Get the world point that is under the mouse
                Vector2 mouseWorldPos = GetScreenToWorld2D(GetMousePosition(), camera);

                // Set the offset to where the mouse is
                camera.Offset = GetMousePosition();

                // Set the target to match, so that the camera maps the world space point
                // under the cursor to the screen space point under the cursor at any zoom
                camera.Target = mouseWorldPos;

                // Zoom increment
                // Uses log scaling to provide consistent zoom speed
                float scale = 0.2f * wheel;
                camera.Zoom = Clamp(MathF.Exp(MathF.Log(camera.Zoom) + scale), 0.125f, 64.0f);
            }
        }
        else
        {
            // Zoom based on mouse right click
            if (IsMouseButtonPressed(MouseButton.Right))
            {
                // Get the world point that is under the mouse
                Vector2 mouseWorldPos = GetScreenToWorld2D(GetMousePosition(), camera);

                // Set the offset to where the mouse is
                camera.Offset = GetMousePosition();

                // Set the target to match, so that the camera maps the world space point
                // under the cursor to the screen space point under the cursor at any zoom
                camera.Target = mouseWorldPos;
            }

            if (IsMouseButtonDown(MouseButton.Right))
            {
                // Zoom increment
                // Uses log scaling to provide consistent zoom speed
                float deltaX = GetMouseDelta().X;
                float scale = 0.005f * deltaX;
                camera.Zoom = Clamp(MathF.Exp(MathF.Log(camera.Zoom) + scale), 0.125f, 64.0f);
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode2D(camera);
        // Draw the 3d grid, rotated 90 degrees and centered around 0,0
        // just so we have something in the XY plane
        Rlgl.PushMatrix();
        Rlgl.Translatef(0, 25 * 50, 0);
        Rlgl.Rotatef(90, 1, 0, 0);
        DrawGrid(100, 50);
        Rlgl.PopMatrix();

        // Draw a reference circle
        DrawCircle(GetScreenWidth() / 2, GetScreenHeight() / 2, 50, Color.Maroon);
        EndMode2D();

        // Draw mouse reference
        //Vector2 mousePos = GetWorldToScreen2D(GetMousePosition(), camera)
        DrawCircleV(GetMousePosition(), 4, Color.DarkGray);
        DrawTextEx(GetFontDefault(), $"[{GetMouseX()}, {GetMouseY()}]",
            Vector2Add(GetMousePosition(), new Vector2(-44, -24)), 20, 2, Color.Black);

        DrawText("[1][2] Select mouse zoom mode (Wheel or Move)", 20, 20, 20, Color.DarkGray);
        if (zoomMode == 0)
        {
            DrawText("Mouse left button drag to move, mouse wheel to zoom", 20, 50, 20, Color.DarkGray);
        }
        else
        {
            DrawText("Mouse left button drag to move, mouse press and move to zoom", 20, 50, 20, Color.DarkGray);
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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 2d camera mouse zoom");

        SetTargetFPS(60);       // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Camera2dMouseZoom();
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
