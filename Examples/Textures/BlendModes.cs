/*******************************************************************************************
*
*   raylib [textures] example - blend modes
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example originally created with raylib 3.5, last time updated with raylib 3.5
*
*   Example contributed by Karlo Licudine (@accidentalrebel) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2020-2025 Karlo Licudine (@accidentalrebel)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class BlendModes : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int blendCountMax = 4;

    public string Name => "Textures / Blend Modes";

    public string Title => "raylib [textures] example - blend modes";

    private Texture2D bgTexture;
    private Texture2D fgTexture;

    private BlendMode blendMode;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
        var bgImage = LoadImage("resources/cyberpunk_street_background.png");
        bgTexture = LoadTextureFromImage(bgImage);

        var fgImage = LoadImage("resources/cyberpunk_street_foreground.png");
        fgTexture = LoadTextureFromImage(fgImage);

        // Once image has been converted to texture and uploaded to VRAM, it can be unloaded from RAM
        UnloadImage(bgImage);
        UnloadImage(fgImage);

        blendMode = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            if ((int)blendMode >= (blendCountMax - 1))
            {
                blendMode = 0;
            }
            else
            {
                blendMode++;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        var bgX = screenWidth / 2 - bgTexture.Width / 2;
        var bgY = screenHeight / 2 - bgTexture.Height / 2;
        DrawTexture(bgTexture, bgX, bgY, Color.White);

        // Apply the blend mode and then draw the foreground texture
        BeginBlendMode(blendMode);
        var fgX = screenWidth / 2 - fgTexture.Width / 2;
        var fgY = screenHeight / 2 - fgTexture.Height / 2;
        DrawTexture(fgTexture, fgX, fgY, Color.White);
        EndBlendMode();

        // Draw the texts
        DrawText("Press SPACE to change blend modes.", 310, 350, 10, Color.Gray);

        switch (blendMode)
        {
            case BlendMode.Alpha:
                DrawText("Current: BLEND_ALPHA", (screenWidth / 2) - 60, 370, 10, Color.Gray);
                break;
            case BlendMode.Additive:
                DrawText("Current: BLEND_ADDITIVE", (screenWidth / 2) - 60, 370, 10, Color.Gray);
                break;
            case BlendMode.Multiplied:
                DrawText("Current: BLEND_MULTIPLIED", (screenWidth / 2) - 60, 370, 10, Color.Gray);
                break;
            case BlendMode.AddColors:
                DrawText("Current: BLEND_ADD_COLORS", (screenWidth / 2) - 60, 370, 10, Color.Gray);
                break;
            default:
                break;
        }

        var text = "(c) Cyberpunk Street Environment by Luis Zuno (@ansimuz)";
        DrawText(text, screenWidth - 330, screenHeight - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(fgTexture); // Unload foreground texture
        UnloadTexture(bgTexture); // Unload background texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - blend modes");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //---------------------------------------------------------------------------------------

        var game = new BlendModes();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();            // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
