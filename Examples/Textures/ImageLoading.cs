/*******************************************************************************************
*
*   raylib [textures] example - image loading
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example originally created with raylib 1.3, last time updated with raylib 1.3
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class ImageLoading : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Image Loading";

    public string Title => "raylib [textures] example - image loading";

    private Texture2D texture;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)

        var image = LoadImage("resources/raylib-cs_logo.png");  // Loaded in CPU memory (RAM)
        texture = LoadTextureFromImage(image);          // Image converted to texture, GPU memory (VRAM)
        UnloadImage(image);   // Once image has been converted to texture and uploaded to VRAM, it can be unloaded from RAM
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

        DrawText("this IS a texture loaded from an image!", 300, 370, 10, Color.Gray);

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
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image loading");

        SetTargetFPS(60);     // Set our game to run at 60 frames-per-second
        //---------------------------------------------------------------------------------------

        var game = new ImageLoading();
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
