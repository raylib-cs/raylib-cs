/*******************************************************************************************
*
*   raylib [textures] example - image drawing
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example originally created with raylib 1.4, last time updated with raylib 1.4
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class ImageDrawing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Image Drawing";

    public string Title => "raylib [textures] example - image drawing";

    private Texture2D texture;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)

        var cat = LoadImage("resources/cat.png");            // Load image in CPU memory (RAM)
        ImageCrop(ref cat, new Rectangle(100, 10, 280, 380));  // Crop an image piece
        ImageFlipHorizontal(ref cat);                          // Flip cropped image horizontally
        ImageResize(ref cat, 150, 200);                        // Resize flipped-cropped image

        var parrots = LoadImage("resources/parrots.png");    // Load image in CPU memory (RAM)

        // Draw one image over the other with a scaling of 1.5f
        Rectangle src = new(0, 0, cat.Width, cat.Height);
        ImageDraw(ref parrots, cat, src, new Rectangle(30, 40, cat.Width * 1.5f, cat.Height * 1.5f), Color.White);
        ImageCrop(ref parrots, new Rectangle(0, 50, parrots.Width, parrots.Height - 100)); // Crop resulting image

        // Draw on the image with a few image draw methods
        ImageDrawPixel(ref parrots, 10, 10, Color.RayWhite);
        ImageDrawCircleLines(ref parrots, 10, 10, 5, Color.RayWhite);
        ImageDrawRectangle(ref parrots, 5, 20, 10, 10, Color.RayWhite);

        UnloadImage(cat);       // Unload image from RAM

        // Load custom font for drawing on image
        var font = LoadFont("resources/custom_jupiter_crash.png");

        // Draw over image using custom font
        ImageDrawTextEx(ref parrots, font, "PARROTS & CAT", new Vector2(300, 230), font.BaseSize, -2, Color.White);

        UnloadFont(font);       // Unload custom font (already drawn used on image)

        texture = LoadTextureFromImage(parrots);      // Image converted to texture, uploaded to GPU memory (VRAM)
        UnloadImage(parrots);   // Once image has been converted to texture and uploaded to VRAM, it can be unloaded from RAM
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        var x = screenWidth / 2 - texture.Width / 2;
        var y = screenHeight / 2 - texture.Height / 2;
        DrawTexture(texture, x, y - 40, Color.White);
        DrawRectangleLines(x, y - 40, texture.Width, texture.Height, Color.DarkGray);

        DrawText("We are drawing only one texture from various images composed!", 240, 350, 10, Color.DarkGray);
        DrawText("Source images have been cropped, scaled, flipped and copied one over the other.", 190, 370, 10, Color.DarkGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image drawing");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ImageDrawing();
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
