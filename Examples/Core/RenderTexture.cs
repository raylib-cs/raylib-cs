/*******************************************************************************************
*
*   raylib [core] example - render texture
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class RenderTextureDemo : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Define a render texture to render
    private const int renderTextureWidth = 300;
    private const int renderTextureHeight = 300;

    public string Name => "Core / Render Texture";

    public string Title => "raylib [core] example - render texture";

    private RenderTexture2D target;
    private Vector2 ballPosition;
    private Vector2 ballSpeed;
    private int ballRadius;
    private float rotation;

    public void Init()
    {
        target = LoadRenderTexture(renderTextureWidth, renderTextureHeight);

        ballPosition = new Vector2(renderTextureWidth / 2.0f, renderTextureHeight / 2.0f);
        ballSpeed = new Vector2(5.0f, 4.0f);
        ballRadius = 20;

        rotation = 0.0f;
    }

    public void Update()
    {
        // Update
        //-----------------------------------------------------
        // Ball movement logic
        ballPosition.X += ballSpeed.X;
        ballPosition.Y += ballSpeed.Y;

        // Check walls collision for bouncing
        if ((ballPosition.X >= (renderTextureWidth - ballRadius)) || (ballPosition.X <= ballRadius))
        {
            ballSpeed.X *= -1.0f;
        }
        if ((ballPosition.Y >= (renderTextureHeight - ballRadius)) || (ballPosition.Y <= ballRadius))
        {
            ballSpeed.Y *= -1.0f;
        }

        // Render texture rotation
        rotation += 0.5f;
        //-----------------------------------------------------

        // Draw
        //-----------------------------------------------------
        // Draw our scene to the render texture
        BeginTextureMode(target);

        ClearBackground(Color.SkyBlue);

        DrawRectangle(0, 0, 20, 20, Color.Red);
        DrawCircleV(ballPosition, ballRadius, Color.Maroon);

        EndTextureMode();

        // Draw render texture to main framebuffer
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw our render texture with rotation applied
        // NOTE 1: We set the origin of the texture to the center of the render texture
        // NOTE 2: We flip vertically the texture setting negative source rectangle height
        DrawTexturePro(target.Texture,
            new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
            new Rectangle(screenWidth / 2.0f, screenHeight / 2.0f, target.Texture.Width, target.Texture.Height),
            new Vector2(target.Texture.Width / 2.0f, target.Texture.Height / 2.0f), rotation, Color.White);

        DrawText("DRAWING BOUNCING BALL INSIDE RENDER TEXTURE!", 10, screenHeight - 40, 20, Color.Black);

        DrawFPS(10, 10);

        EndDrawing();
        //-----------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(target);
    }

    public static int Main()
    {
        // Initialization
        //---------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - render texture");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //----------------------------------------------------------

        var game = new RenderTextureDemo();
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
