/*******************************************************************************************
*
*   raylib [textures] example - image rotate
*
*   Example complexity rating: [★★☆☆] 2/4
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

public partial class ImageRotate : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int NumTextures = 3;

    public string Name => "Textures / Image Rotate";

    public string Title => "raylib [textures] example - image rotate";

    private Texture2D[] textures;
    private int currentTexture;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
        var image45 = LoadImage("resources/raylib_logo.png");
        var image90 = LoadImage("resources/raylib_logo.png");
        var imageNeg90 = LoadImage("resources/raylib_logo.png");

        ImageRotate(ref image45, 45);
        ImageRotate(ref image90, 90);
        ImageRotate(ref imageNeg90, -90);

        textures = new Texture2D[NumTextures];

        textures[0] = LoadTextureFromImage(image45);
        textures[1] = LoadTextureFromImage(image90);
        textures[2] = LoadTextureFromImage(imageNeg90);

        UnloadImage(image45);
        UnloadImage(image90);
        UnloadImage(imageNeg90);

        currentTexture = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsMouseButtonPressed(MouseButton.Left) || IsKeyPressed(KeyboardKey.Right))
        {
            currentTexture = (currentTexture + 1) % NumTextures; // Cycle between the textures
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawTexture(
            textures[currentTexture],
            screenWidth / 2 - textures[currentTexture].Width / 2,
            screenHeight / 2 - textures[currentTexture].Height / 2,
            Color.White);

        DrawText("Press LEFT MOUSE BUTTON to rotate the image clockwise", 250, 420, 10, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        for (var i = 0; i < NumTextures; i++)
        {
            UnloadTexture(textures[i]);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image rotate");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ImageRotate();
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
