/*******************************************************************************************
*
*   raylib [core] example - 2d camera
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.5, last time updated with raylib 3.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class Camera2dDemo : IExample
{
    public const int MaxBuildings = 100;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Camera 2D Demo";

    public string Title => "raylib [core] example - 2d camera";

    private Rectangle player;
    private Rectangle[] buildings;
    private Color[] buildColors;
    private Camera2D camera;

    public void Init()
    {
        player = new(400, 280, 40, 40);
        buildings = new Rectangle[MaxBuildings];
        buildColors = new Color[MaxBuildings];

        var spacing = 0;

        for (var i = 0; i < MaxBuildings; i++)
        {
            buildings[i].Width = GetRandomValue(50, 200);
            buildings[i].Height = GetRandomValue(100, 800);
            buildings[i].Y = screenHeight - 130 - buildings[i].Height;
            buildings[i].X = -6000 + spacing;

            spacing += (int)buildings[i].Width;

            buildColors[i] = new Color(
                GetRandomValue(200, 240),
                GetRandomValue(200, 240),
                GetRandomValue(200, 250),
                255
            );
        }

        camera = new();
        camera.Target = new Vector2(player.X + 20, player.Y + 20);
        camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Player movement
        if (IsKeyDown(KeyboardKey.Right))
        {
            player.X += 2;
        }
        else if (IsKeyDown(KeyboardKey.Left))
        {
            player.X -= 2;
        }

        // Camera target follows player
        camera.Target = new Vector2(player.X + 20, player.Y + 20);

        // Camera rotation controls
        if (IsKeyDown(KeyboardKey.A))
        {
            camera.Rotation--;
        }
        else if (IsKeyDown(KeyboardKey.S))
        {
            camera.Rotation++;
        }

        // Limit camera rotation to 80 degrees (-40 to 40)
        if (camera.Rotation > 40)
        {
            camera.Rotation = 40;
        }
        else if (camera.Rotation < -40)
        {
            camera.Rotation = -40;
        }

        // Camera zoom controls
        // Uses log scaling to provide consistent zoom speed
        camera.Zoom = MathF.Exp(MathF.Log(camera.Zoom) + ((float)GetMouseWheelMove() * 0.1f));

        if (camera.Zoom > 3.0f)
        {
            camera.Zoom = 3.0f;
        }
        else if (camera.Zoom < 0.1f)
        {
            camera.Zoom = 0.1f;
        }

        // Camera reset (zoom and rotation)
        if (IsKeyPressed(KeyboardKey.R))
        {
            camera.Zoom = 1.0f;
            camera.Rotation = 0.0f;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode2D(camera);

        DrawRectangle(-6000, 320, 13000, 8000, Color.DarkGray);

        for (var i = 0; i < MaxBuildings; i++)
        {
            DrawRectangleRec(buildings[i], buildColors[i]);
        }

        DrawRectangleRec(player, Color.Red);

        DrawLine((int)camera.Target.X, -screenHeight * 10, (int)camera.Target.X, screenHeight * 10, Color.Green);
        DrawLine(-screenWidth * 10, (int)camera.Target.Y, screenWidth * 10, (int)camera.Target.Y, Color.Green);

        EndMode2D();

        DrawText("SCREEN AREA", 640, 10, 20, Color.Red);

        DrawRectangle(0, 0, screenWidth, 5, Color.Red);
        DrawRectangle(0, 5, 5, screenHeight - 10, Color.Red);
        DrawRectangle(screenWidth - 5, 5, 5, screenHeight - 10, Color.Red);
        DrawRectangle(0, screenHeight - 5, screenWidth, 5, Color.Red);

        DrawRectangle(10, 10, 250, 113, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(10, 10, 250, 113, Color.Blue);

        DrawText("Free 2D camera controls:", 20, 20, 10, Color.Black);
        DrawText("- Right/Left to move player", 40, 40, 10, Color.DarkGray);
        DrawText("- Mouse Wheel to Zoom in-out", 40, 60, 10, Color.DarkGray);
        DrawText("- A / S to Rotate", 40, 80, 10, Color.DarkGray);
        DrawText("- R to reset Zoom and Rotation", 40, 100, 10, Color.DarkGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 2d camera");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Camera2dDemo();
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
