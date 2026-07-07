/*******************************************************************************************
*
*   raylib [models] example - directional billboard
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Robin (@RobinsAviary) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Robin (@RobinsAviary)
*   Killbot art by patvanmackelberg https://opengameart.org/content/killbot-8-directional under CC0
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Models;

public partial class DirectionalBillboard : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Directional Billboard";

    public string Title => "raylib [models] example - directional billboard";

    private Camera3D camera;
    private Texture2D skillbot;
    private float animTimer;
    private uint anim;

    public void Init()
    {
        // Set up the camera
        camera = new();
        camera.Position = new Vector3(2.0f, 1.0f, 2.0f); // Starting position
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);  // Target position
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f); // Up vector
        camera.FovY = 45.0f; // FOV
        camera.Projection = CameraProjection.Perspective; // Projection type (Standard 3D perspective)

        // Load billboard texture
        skillbot = LoadTexture("resources/skillbot.png");

        // Timer to update animation
        animTimer = 0.0f;
        // Animation frame
        anim = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Update timer with delta time
        animTimer += GetFrameTime();

        // Update frame index after a certain amount of time (half a second)
        if (animTimer > 0.5f)
        {
            animTimer = 0.0f;
            anim += 1;
        }

        // Reset frame index to zero on overflow
        if (anim >= 4)
        {
            anim = 0;
        }

        // Find the current direction frame based on the camera position to the billboard object
        var dir = (float)Math.Floor(((Vector2Angle(new Vector2(2.0f, 0.0f), new Vector2(camera.Position.X, camera.Position.Z)) / MathF.PI) * 4.0f) + 0.25f);

        // Correct frame index if angle is negative
        if (dir < 0.0f)
        {
            dir = 8.0f - Math.Abs((int)dir);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawGrid(10, 1.0f);

        // Draw billboard pointing straight up to the sky, rotated relative to the camera and offset from the bottom
        DrawBillboardPro(camera, skillbot, new Rectangle(0.0f + (anim * 24.0f), 0.0f + (dir * 24.0f), 24.0f, 24.0f),
            Vector3.Zero, new Vector3(0.0f, 1.0f, 0.0f), Vector2.One, new Vector2(0.5f, 0.0f), 0, Color.White);

        EndMode3D();

        // Render various variables for reference
        DrawText($"animation: {anim}", 10, 10, 20, Color.DarkGray);
        DrawText($"direction frame: {dir:0}", 10, 40, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // Unload billboard texture
        UnloadTexture(skillbot);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - directional billboard");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new DirectionalBillboard();
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
