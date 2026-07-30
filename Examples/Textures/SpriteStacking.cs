/*******************************************************************************************
*
*   raylib [textures] example - sprite stacking
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Robin (@RobinsAviary) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Redbooth model (c) 2017-2025 @kluchek under https://creativecommons.org/licenses/by/4.0/ https://github.com/kluchek/vox-models/
*   Copyright (c) 2025 Robin (@RobinsAviary)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class SpriteStacking : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const float speedChange = 0.25f; // Amount speed will change by when the user presses A/D

    public string Name => "Textures / Sprite Stacking";

    public string Title => "raylib [textures] example - sprite stacking";

    private Texture2D booth;
    private float stackScale;    // Overall scale of the stacked sprite
    private float stackSpacing;  // Vertical spacing between each layer
    private uint stackCount;     // Number of layers, used for calculating the size of a single slice
    private float rotationSpeed; // Stacked sprites rotation speed
    private float rotation;      // Current rotation of the stacked sprite

    public void Init()
    {
        booth = LoadTexture("resources/booth.png");

        stackScale = 3.0f;
        stackSpacing = 2.0f;
        stackCount = 122;
        rotationSpeed = 30.0f;
        rotation = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Use mouse wheel to affect stack separation
        stackSpacing += GetMouseWheelMove() * 0.1f;
        stackSpacing = Math.Clamp(stackSpacing, 0.0f, 5.0f);

        // Add a positive/negative offset to spin right/left at different speeds
        if (IsKeyDown(KeyboardKey.Left) || IsKeyDown(KeyboardKey.A))
        {
            rotationSpeed -= speedChange;
        }
        if (IsKeyDown(KeyboardKey.Right) || IsKeyDown(KeyboardKey.D))
        {
            rotationSpeed += speedChange;
        }

        rotation += rotationSpeed * GetFrameTime();
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Get the size of a single slice
        var frameWidth = (float)booth.Width;
        var frameHeight = (float)booth.Height / (float)stackCount;

        // Get the scaled resolution to draw at
        var scaledWidth = frameWidth * stackScale;
        var scaledHeight = frameHeight * stackScale;

        // Draw the stacked sprite, rotated to the correct angle, with an vertical offset applied based on its y location
        for (var i = (int)stackCount - 1; i >= 0; i--)
        {
            // Center vertically
            Rectangle source = new(0.0f, (float)i * frameHeight, frameWidth, frameHeight);
            Rectangle dest = new(screenWidth / 2.0f, (screenHeight / 2.0f) + (i * stackSpacing) - (stackSpacing * stackCount / 2.0f), scaledWidth, scaledHeight);
            Vector2 origin = new(scaledWidth / 2.0f, scaledHeight / 2.0f);

            DrawTexturePro(booth, source, dest, origin, rotation, Color.White);
        }

        DrawText("A/D to spin\nmouse wheel to change separation (aka 'angle')", 10, 10, 20, Color.DarkGray);
        DrawText($"current spacing: {stackSpacing:F1}", 10, 50, 20, Color.DarkGray);
        DrawText($"current speed: {rotationSpeed:F2}", 10, 70, 20, Color.DarkGray);
        DrawText("redbooth model (c) kluchek under cc 4.0", 10, 420, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(booth);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - sprite stacking");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new SpriteStacking();
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
