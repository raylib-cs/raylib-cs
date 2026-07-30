/*******************************************************************************************
*
*   raylib [textures] example - logo raylib
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 1.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class LogoRaylibTexture : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Logo Raylib Texture";

    public string Title => "raylib [textures] example - logo raylib";

    private Texture2D texture;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
        texture = LoadTexture("resources/raylib-cs_logo.png");        // Texture loading
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawTexture(
            texture,
            screenWidth / 2 - texture.Width / 2,
            screenHeight / 2 - texture.Height / 2,
            Color.White
        );

        DrawText("this IS a texture!", 360, 370, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);       // Texture unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - logo raylib");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //---------------------------------------------------------------------------------------

        var game = new LogoRaylibTexture();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
