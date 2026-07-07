/*******************************************************************************************
*
*   raylib [text] example - font filters
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: After font loading, font texture atlas filter could be configured for a softer
*   display of the font when scaling it to different sizes, that way, it's not required
*   to generate multiple fonts at multiple sizes (as long as the scaling is not very different)
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class FontFilters : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Text / Font Filters";

    public string Title => "raylib [text] example - font filters";

    private string msg;
    private Font font;
    private float fontSize;
    private Vector2 fontPosition;
    private Vector2 textSize;
    private TextureFilter currentFontFilter;

    public void Init()
    {
        msg = "Loaded Font";

        // NOTE: Textures/Fonts MUST be loaded after Window initialization (OpenGL context is required)

        // TTF Font loading with custom generation parameters
        font = LoadFontEx("resources/fonts/KAISG.ttf", 96, null, 0);

        // Generate mipmap levels to use trilinear filtering
        // NOTE: On 2D drawing it won't be noticeable, it looks like FILTER_BILINEAR
        GenTextureMipmaps(ref font.Texture);

        fontSize = font.BaseSize;
        fontPosition = new(40, screenHeight / 2 - 80);
        textSize = new(0.0f, 0.0f);

        // Setup texture scaling filter
        SetTextureFilter(font.Texture, TextureFilter.Point);
        currentFontFilter = TextureFilter.Point;      // TEXTURE_FILTER_POINT
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        fontSize += GetMouseWheelMove() * 4.0f;

        // Choose font texture filter method
        if (IsKeyPressed(KeyboardKey.One))
        {
            SetTextureFilter(font.Texture, TextureFilter.Point);
            currentFontFilter = TextureFilter.Point;
        }
        else if (IsKeyPressed(KeyboardKey.Two))
        {
            SetTextureFilter(font.Texture, TextureFilter.Bilinear);
            currentFontFilter = TextureFilter.Bilinear;
        }
        else if (IsKeyPressed(KeyboardKey.Three))
        {
            // NOTE: Trilinear filter won't be noticed on 2D drawing
            SetTextureFilter(font.Texture, TextureFilter.Trilinear);
            currentFontFilter = TextureFilter.Trilinear;
        }

        textSize = MeasureTextEx(font, msg, fontSize, 0);

        if (IsKeyDown(KeyboardKey.Left))
        {
            fontPosition.X -= 10;
        }
        else if (IsKeyDown(KeyboardKey.Right))
        {
            fontPosition.X += 10;
        }

#if BROWSER
        // NOTE: drag-and-drop font loading is not supported in the browser host; default loaded font is kept.
#else
        // Load a dropped TTF file dynamically (at current fontSize)
        if (IsFileDropped())
        {
            var files = Raylib.GetDroppedFiles();

            // NOTE: We only support first ttf file dropped
            if (IsFileExtension(files[0], ".ttf"))
            {
                UnloadFont(font);
                font = LoadFontEx(files[0], (int)fontSize, null, 0);
            }
        }
#endif
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("Use mouse wheel to change font size", 20, 20, 10, Color.Gray);
        DrawText("Use KEY_RIGHT and KEY_LEFT to move text", 20, 40, 10, Color.Gray);
        DrawText("Use 1, 2, 3 to change texture filter", 20, 60, 10, Color.Gray);
        DrawText("Drop a new TTF font for dynamic loading", 20, 80, 10, Color.DarkGray);

        DrawTextEx(font, msg, fontPosition, fontSize, 0, Color.Black);

        // TODO: It seems texSize measurement is not accurate due to chars offsets...
        //DrawRectangleLines((int)fontPosition.X, (int)fontPosition.Y, (int)textSize.X, (int)textSize.Y, Color.Red);

        DrawRectangle(0, screenHeight - 80, screenWidth, 80, Color.LightGray);
        DrawText($"Font size: {fontSize:00.00}", 20, screenHeight - 50, 10, Color.DarkGray);
        DrawText($"Text size: [{textSize.X:00.00}, {textSize.Y:00.00}]", 20, screenHeight - 30, 10, Color.DarkGray);
        DrawText("CURRENT TEXTURE FILTER:", 250, 400, 20, Color.Gray);

        if (currentFontFilter == TextureFilter.Point)
        {
            DrawText("POINT", 570, 400, 20, Color.Black);
        }
        else if (currentFontFilter == TextureFilter.Bilinear)
        {
            DrawText("BILINEAR", 570, 400, 20, Color.Black);
        }
        else if (currentFontFilter == TextureFilter.Trilinear)
        {
            DrawText("TRILINEAR", 570, 400, 20, Color.Black);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(font);           // Font unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - font filters");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new FontFilters();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();              // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
