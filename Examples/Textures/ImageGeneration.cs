/*******************************************************************************************
*
*   raylib [textures] example - image generation
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.8, last time updated with raylib 1.8
*
*   Example contributed by Wilhem Barbier (@nounoursheureux) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2017-2025 Wilhem Barbier (@nounoursheureux) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class ImageGeneration : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Currently we have 8 generation algorithms but some have multiple purposes (Linear and Square Gradients)
    public const int NumTextures = 9;

    public string Name => "Textures / Image Generation";

    public string Title => "raylib [textures] example - image generation";

    private Texture2D[] textures;
    private int currentTexture;

    public void Init()
    {
        var verticalGradient = GenImageGradientLinear(screenWidth, screenHeight, 0, Color.Red, Color.Blue);
        var horizontalGradient = GenImageGradientLinear(screenWidth, screenHeight, 90, Color.Red, Color.Blue);
        var diagonalGradient = GenImageGradientLinear(screenWidth, screenHeight, 45, Color.Red, Color.Blue);
        var radialGradient = GenImageGradientRadial(screenWidth, screenHeight, 0.0f, Color.White, Color.Black);
        var squareGradient = GenImageGradientSquare(screenWidth, screenHeight, 0.0f, Color.White, Color.Black);
        var isChecked = GenImageChecked(screenWidth, screenHeight, 32, 32, Color.Red, Color.Blue);
        var whiteNoise = GenImageWhiteNoise(screenWidth, screenHeight, 0.5f);
        var perlinNoise = GenImagePerlinNoise(screenWidth, screenHeight, 50, 50, 4.0f);
        var cellular = GenImageCellular(screenWidth, screenHeight, 32);

        textures = new Texture2D[NumTextures];
        textures[0] = LoadTextureFromImage(verticalGradient);
        textures[1] = LoadTextureFromImage(horizontalGradient);
        textures[2] = LoadTextureFromImage(diagonalGradient);
        textures[3] = LoadTextureFromImage(radialGradient);
        textures[4] = LoadTextureFromImage(squareGradient);
        textures[5] = LoadTextureFromImage(isChecked);
        textures[6] = LoadTextureFromImage(whiteNoise);
        textures[7] = LoadTextureFromImage(perlinNoise);
        textures[8] = LoadTextureFromImage(cellular);

        // Unload image data (CPU RAM)
        UnloadImage(verticalGradient);
        UnloadImage(horizontalGradient);
        UnloadImage(diagonalGradient);
        UnloadImage(radialGradient);
        UnloadImage(squareGradient);
        UnloadImage(isChecked);
        UnloadImage(whiteNoise);
        UnloadImage(perlinNoise);
        UnloadImage(cellular);

        currentTexture = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsMouseButtonPressed(MouseButton.Left) || IsKeyPressed(KeyboardKey.Right))
        {
            // Cycle between the textures
            currentTexture = (currentTexture + 1) % NumTextures;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawTexture(textures[currentTexture], 0, 0, Color.White);

        DrawRectangle(30, 400, 325, 30, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(30, 400, 325, 30, Fade(Color.White, 0.5f));
        DrawText("MOUSE LEFT BUTTON to CYCLE PROCEDURAL TEXTURES", 40, 410, 10, Color.White);

        switch (currentTexture)
        {
            case 0:
                DrawText("VERTICAL GRADIENT", 560, 10, 20, Color.RayWhite);
                break;
            case 1:
                DrawText("HORIZONTAL GRADIENT", 540, 10, 20, Color.RayWhite);
                break;
            case 2:
                DrawText("DIAGONAL GRADIENT", 540, 10, 20, Color.RayWhite);
                break;
            case 3:
                DrawText("RADIAL GRADIENT", 580, 10, 20, Color.LightGray);
                break;
            case 4:
                DrawText("SQUARE GRADIENT", 580, 10, 20, Color.LightGray);
                break;
            case 5:
                DrawText("CHECKED", 680, 10, 20, Color.RayWhite);
                break;
            case 6:
                DrawText("WHITE NOISE", 640, 10, 20, Color.Red);
                break;
            case 7:
                DrawText("PERLIN NOISE", 640, 10, 20, Color.Red);
                break;
            case 8:
                DrawText("CELLULAR", 670, 10, 20, Color.RayWhite);
                break;
            default:
                break;
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        for (var i = 0; i < textures.Length; i++)
        {
            UnloadTexture(textures[i]);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image generation");

        SetTargetFPS(60);
        //---------------------------------------------------------------------------------------

        var game = new ImageGeneration();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
