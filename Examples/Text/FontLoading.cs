/*******************************************************************************************
*
*   raylib [text] example - font loading
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   NOTE: raylib can load fonts from multiple input file formats:
*
*     - TTF/OTF > Sprite font atlas is generated on loading, user can configure
*                 some of the generation parameters (size, characters to include)
*     - BMFonts > Angel code font fileformat, sprite font image must be provided
*                 together with the .fnt file, font generation can not be configured
*     - XNA Spritefont > Sprite font image, following XNA Spritefont conventions,
*                 Characters in image must follow some spacing and order rules
*
*   Example originally created with raylib 1.4, last time updated with raylib 3.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class FontLoading : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Text / Font Loading";

    public string Title => "raylib [text] example - font loading";

    private string msg;
    private Font fontBm;
    private Font fontTtf;
    private bool useTtf;

    public void Init()
    {
        // Define characters to draw
        // NOTE: raylib supports UTF-8 encoding, following list is actually codified as UTF8 internally
        msg = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHI\nJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmn\nopqrstuvwxyz{|}~¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓ\nÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷\nøùúûüýþÿ";

        // NOTE: Textures/Fonts MUST be loaded after Window initialization (OpenGL context is required)

        // BMFont (AngelCode) : Font data and image atlas have been generated using external program
        fontBm = LoadFont("resources/fonts/pixantiqua.fnt"); // Requires "resources/fonts/pixantiqua.png"

        // TTF font : Font data and atlas are generated directly from TTF
        // NOTE: We define a font base size of 32 pixels tall and up-to 250 characters
        fontTtf = LoadFontEx("resources/fonts/pixantiqua.ttf", 32, null, 250);

        SetTextLineSpacing(16);         // Set line spacing for multiline text (when line breaks are included '\n')

        useTtf = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyDown(KeyboardKey.Space))
        {
            useTtf = true;
        }
        else
        {
            useTtf = false;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("Hold SPACE to use TTF generated font", 20, 20, 20, Color.LightGray);

        if (!useTtf)
        {
            DrawTextEx(fontBm, msg, new Vector2(20.0f, 100.0f), fontBm.BaseSize, 2, Color.Maroon);
            DrawText("Using BMFont (Angelcode) imported", 20, GetScreenHeight() - 30, 20, Color.Gray);
        }
        else
        {
            DrawTextEx(fontTtf, msg, new Vector2(20.0f, 100.0f), fontTtf.BaseSize, 2, Color.Lime);
            DrawText("Using TTF font generated", 20, GetScreenHeight() - 30, 20, Color.Gray);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(fontBm);     // AngelCode Font unloading
        UnloadFont(fontTtf);    // TTF Font unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - font loading");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new FontLoading();
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
