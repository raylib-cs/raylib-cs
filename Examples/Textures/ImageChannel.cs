/*******************************************************************************************
*
*   raylib [textures] example - image channel
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Bruno Cabral (@brccabral) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2025 Bruno Cabral (@brccabral) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class ImageChannel : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Image Channel";

    public string Title => "raylib [textures] example - image channel";

    private Texture2D fudesumiTexture;
    private Texture2D textureAlpha;
    private Texture2D textureRed;
    private Texture2D textureGreen;
    private Texture2D textureBlue;
    private Texture2D backgroundTexture;

    private Rectangle fudesumiRec;
    private Rectangle fudesumiPos;
    private Rectangle redPos;
    private Rectangle greenPos;
    private Rectangle bluePos;
    private Rectangle alphaPos;

    public void Init()
    {
        var fudesumiImage = LoadImage("resources/fudesumi.png");

        var imageAlpha = ImageFromChannel(fudesumiImage, 3);
        ImageAlphaMask(ref imageAlpha, imageAlpha);

        var imageRed = ImageFromChannel(fudesumiImage, 0);
        ImageAlphaMask(ref imageRed, imageAlpha);

        var imageGreen = ImageFromChannel(fudesumiImage, 1);
        ImageAlphaMask(ref imageGreen, imageAlpha);

        var imageBlue = ImageFromChannel(fudesumiImage, 2);
        ImageAlphaMask(ref imageBlue, imageAlpha);

        var backgroundImage = GenImageChecked(screenWidth, screenHeight, screenWidth / 20, screenHeight / 20, Color.Orange, Color.Yellow);

        fudesumiTexture = LoadTextureFromImage(fudesumiImage);
        textureAlpha = LoadTextureFromImage(imageAlpha);
        textureRed = LoadTextureFromImage(imageRed);
        textureGreen = LoadTextureFromImage(imageGreen);
        textureBlue = LoadTextureFromImage(imageBlue);
        backgroundTexture = LoadTextureFromImage(backgroundImage);

        fudesumiRec = new Rectangle(0, 0, fudesumiImage.Width, fudesumiImage.Height);

        fudesumiPos = new Rectangle(50, 10, fudesumiImage.Width * 0.8f, fudesumiImage.Height * 0.8f);
        redPos = new Rectangle(410, 10, fudesumiPos.Width / 2.0f, fudesumiPos.Height / 2.0f);
        greenPos = new Rectangle(600, 10, fudesumiPos.Width / 2.0f, fudesumiPos.Height / 2.0f);
        bluePos = new Rectangle(410, 230, fudesumiPos.Width / 2.0f, fudesumiPos.Height / 2.0f);
        alphaPos = new Rectangle(600, 230, fudesumiPos.Width / 2.0f, fudesumiPos.Height / 2.0f);

        UnloadImage(fudesumiImage);
        UnloadImage(imageAlpha);
        UnloadImage(imageRed);
        UnloadImage(imageGreen);
        UnloadImage(imageBlue);
        UnloadImage(backgroundImage);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Nothing to update...
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        DrawTexture(backgroundTexture, 0, 0, Color.White);
        DrawTexturePro(fudesumiTexture, fudesumiRec, fudesumiPos, new Vector2(0, 0), 0, Color.White);

        DrawTexturePro(textureRed, fudesumiRec, redPos, new Vector2(0, 0), 0, Color.Red);
        DrawTexturePro(textureGreen, fudesumiRec, greenPos, new Vector2(0, 0), 0, Color.Green);
        DrawTexturePro(textureBlue, fudesumiRec, bluePos, new Vector2(0, 0), 0, Color.Blue);
        DrawTexturePro(textureAlpha, fudesumiRec, alphaPos, new Vector2(0, 0), 0, Color.White);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(backgroundTexture);
        UnloadTexture(fudesumiTexture);
        UnloadTexture(textureRed);
        UnloadTexture(textureGreen);
        UnloadTexture(textureBlue);
        UnloadTexture(textureAlpha);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image channel");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ImageChannel();
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
