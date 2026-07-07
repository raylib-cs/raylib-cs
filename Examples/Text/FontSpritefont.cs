/*******************************************************************************************
*
*   raylib [text] example - font spritefont
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   NOTE: Sprite fonts should be generated following this conventions:
*
*     - Characters must be ordered starting with character 32 (Space)
*     - Every character must be contained within the same Rectangle height
*     - Every character and every line must be separated by the same distance (margin/padding)
*     - Rectangles must be defined by a MAGENTA color background
*
*   Following those constraints, a font can be provided just by an image,
*   this is quite handy to avoid additional font descriptor files (like BMFonts use)
*
*   Example originally created with raylib 1.0, last time updated with raylib 1.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class FontSpritefont : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Text / Font Spritefont";

    public string Title => "raylib [text] example - font spritefont";

    private string msg1;
    private string msg2;
    private string msg3;

    private Font font1;
    private Font font2;
    private Font font3;

    private Vector2 fontPosition1;
    private Vector2 fontPosition2;
    private Vector2 fontPosition3;

    public void Init()
    {
        msg1 = "THIS IS A custom SPRITE FONT...";
        msg2 = "...and this is ANOTHER CUSTOM font...";
        msg3 = "...and a THIRD one! GREAT! :D";

        // NOTE: Textures/Fonts MUST be loaded after Window initialization (OpenGL context is required)
        font1 = LoadFont("resources/custom_mecha.png");          // Font loading
        font2 = LoadFont("resources/custom_alagard.png");        // Font loading
        font3 = LoadFont("resources/custom_jupiter_crash.png");  // Font loading

        fontPosition1 = new(
            screenWidth / 2 - MeasureTextEx(font1, msg1, font1.BaseSize, -3).X / 2,
            screenHeight / 2 - font1.BaseSize / 2 - 80
        );

        fontPosition2 = new(
            screenWidth / 2 - MeasureTextEx(font2, msg2, font2.BaseSize, -2).X / 2,
            screenHeight / 2 - font2.BaseSize / 2 - 10
        );

        fontPosition3 = new(
            screenWidth / 2 - MeasureTextEx(font3, msg3, font3.BaseSize, 2).X / 2,
            screenHeight / 2 - font3.BaseSize / 2 + 50
        );
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // TODO: Update variables here...
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawTextEx(font1, msg1, fontPosition1, font1.BaseSize, -3, Color.White);
        DrawTextEx(font2, msg2, fontPosition2, font2.BaseSize, -2, Color.White);
        DrawTextEx(font3, msg3, fontPosition3, font3.BaseSize, 2, Color.White);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(font1);      // Font unloading
        UnloadFont(font2);      // Font unloading
        UnloadFont(font3);      // Font unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - font spritefont");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new FontSpritefont();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();          // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
