/*******************************************************************************************
*
*   raylib [textures] example - background scrolling
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 2.0, last time updated with raylib 2.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class BackgroundScrolling : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Background Scrolling";

    public string Title => "raylib [textures] example - background scrolling";

    private Texture2D background;
    private Texture2D midground;
    private Texture2D foreground;

    private float scrollingBack;
    private float scrollingMid;
    private float scrollingFore;

    public void Init()
    {
        // NOTE: Be careful, background width must be equal or bigger than screen width
        // if not, texture should be draw more than two times for scrolling effect
        background = LoadTexture("resources/cyberpunk_street_background.png");
        midground = LoadTexture("resources/cyberpunk_street_midground.png");
        foreground = LoadTexture("resources/cyberpunk_street_foreground.png");

        scrollingBack = 0.0f;
        scrollingMid = 0.0f;
        scrollingFore = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        scrollingBack -= 0.1f;
        scrollingMid -= 0.5f;
        scrollingFore -= 1.0f;

        // NOTE: Texture is scaled twice its size, so it sould be considered on scrolling
        if (scrollingBack <= -background.Width * 2)
        {
            scrollingBack = 0;
        }
        if (scrollingMid <= -midground.Width * 2)
        {
            scrollingMid = 0;
        }
        if (scrollingFore <= -foreground.Width * 2)
        {
            scrollingFore = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(GetColor(0x052c46ff));

        // Draw background image twice
        // NOTE: Texture is scaled twice its size
        DrawTextureEx(background, new Vector2(scrollingBack, 20), 0.0f, 2.0f, Color.White);
        DrawTextureEx(
            background,
            new Vector2(background.Width * 2 + scrollingBack, 20),
            0.0f,
            2.0f,
            Color.White
        );

        // Draw midground image twice
        DrawTextureEx(midground, new Vector2(scrollingMid, 20), 0.0f, 2.0f, Color.White);
        DrawTextureEx(midground, new Vector2(midground.Width * 2 + scrollingMid, 20), 0.0f, 2.0f, Color.White);

        // Draw foreground image twice
        DrawTextureEx(foreground, new Vector2(scrollingFore, 70), 0.0f, 2.0f, Color.White);
        DrawTextureEx(
            foreground,
            new Vector2(foreground.Width * 2 + scrollingFore, 70),
            0.0f,
            2.0f,
            Color.White
        );

        DrawText("BACKGROUND SCROLLING & PARALLAX", 10, 10, 20, Color.Red);
        DrawText("(c) Cyberpunk Street Environment by Luis Zuno (@ansimuz)", screenWidth - 330, screenHeight - 20, 10, Color.RayWhite);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(background);  // Unload background texture
        UnloadTexture(midground);   // Unload midground texture
        UnloadTexture(foreground);  // Unload foreground texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - background scrolling");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new BackgroundScrolling();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();              // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
