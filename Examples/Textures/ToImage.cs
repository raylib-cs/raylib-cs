/*******************************************************************************************
*
*   raylib [textures] example - to image
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class ToImage : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Texture to Image";

    public string Title => "raylib [textures] example - to image";

    private Texture2D texture;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)

        var image = LoadImage("resources/raylib-cs_logo.png");  // Load image data into CPU memory (RAM)
        texture = LoadTextureFromImage(image);                 // Image converted to texture, GPU memory (RAM -> VRAM)
        UnloadImage(image);                                    // Unload image data from CPU memory (RAM)

        image = LoadImageFromTexture(texture);                 // Load image from GPU texture (VRAM -> RAM)
        UnloadTexture(texture);                                // Unload texture from GPU memory (VRAM)

        texture = LoadTextureFromImage(image);                 // Recreate texture from retrieved image data (RAM -> VRAM)
        UnloadImage(image);                                    // Unload retrieved image data from CPU memory (RAM)
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        var x = screenWidth / 2 - texture.Width / 2;
        var y = screenHeight / 2 - texture.Height / 2;
        DrawTexture(texture, x, y, Color.White);

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
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - to image");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ToImage();
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
