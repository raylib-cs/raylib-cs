/*******************************************************************************************
*
*   raylib [text] example - unicode ranges
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Vadim Gunko (@GuvaCode) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Vadim Gunko (@GuvaCode) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class UnicodeRanges : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // path differs from upstream: font lives under resources/fonts/
    private const string FontPath = "resources/fonts/NotoSansTC-Regular.ttf";

    public string Name => "Text / Unicode Ranges";

    public string Title => "raylib [text] example - unicode ranges";

    private Font font;
    private int unicodeRange;       // Track the ranges of codepoints added to font
    private int prevUnicodeRange;   // Previous Unicode range to avoid reloading every frame

    public void Init()
    {
        // Load font with default Unicode range: Basic ASCII [32-127]
        font = LoadFont(FontPath);
        SetTextureFilter(font.Texture, TextureFilter.Bilinear);

        unicodeRange = 0;
        prevUnicodeRange = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (unicodeRange != prevUnicodeRange)
        {
            UnloadFont(font);

            // Load font with default Unicode range: Basic ASCII [32-127]
            font = LoadFont(FontPath);

            // Add required ranges to loaded font
            // NOTE: The upstream switch uses fall-through so range N also loads all lower
            // ranges, in the same order (4 -> 3 -> 2 -> 1); the descending if-chain preserves that.

            /*
            if (unicodeRange >= 5)
            {
                // Unicode range: Devanari, Arabic, Hebrew
                // WARNING: Glyphs not available on provided font!
                AddCodepointRange(ref font, FontPath, 0x900, 0x97f);  // Devanagari
                AddCodepointRange(ref font, FontPath, 0x600, 0x6ff);  // Arabic
                AddCodepointRange(ref font, FontPath, 0x5d0, 0x5ea);  // Hebrew
            }
            */
            if (unicodeRange >= 4)
            {
                // Unicode range: CJK (Japanese and Chinese)
                // WARNING: Loading thousands of codepoints requires lot of time!
                // A better strategy is prefilter the required codepoints for the text
                // in the game and just load the required ones
                AddCodepointRange(ref font, FontPath, 0x4e00, 0x9fff);
                AddCodepointRange(ref font, FontPath, 0x3400, 0x4dbf);
                AddCodepointRange(ref font, FontPath, 0x3000, 0x303f);
                AddCodepointRange(ref font, FontPath, 0x3040, 0x309f);
                AddCodepointRange(ref font, FontPath, 0x30A0, 0x30ff);
                AddCodepointRange(ref font, FontPath, 0x31f0, 0x31ff);
                AddCodepointRange(ref font, FontPath, 0xff00, 0xffef);
                AddCodepointRange(ref font, FontPath, 0xac00, 0xd7af);
                AddCodepointRange(ref font, FontPath, 0x1100, 0x11ff);
            }
            if (unicodeRange >= 3)
            {
                // Unicode range: Cyrillic
                AddCodepointRange(ref font, FontPath, 0x400, 0x4ff);
                AddCodepointRange(ref font, FontPath, 0x500, 0x52f);
                AddCodepointRange(ref font, FontPath, 0x2de0, 0x2Dff);
                AddCodepointRange(ref font, FontPath, 0xa640, 0xA69f);
            }
            if (unicodeRange >= 2)
            {
                // Unicode range: Greek
                AddCodepointRange(ref font, FontPath, 0x370, 0x3ff);
                AddCodepointRange(ref font, FontPath, 0x1f00, 0x1fff);
            }
            if (unicodeRange >= 1)
            {
                // Unicode range: European Languages
                AddCodepointRange(ref font, FontPath, 0xc0, 0x17f);
                AddCodepointRange(ref font, FontPath, 0x180, 0x24f);
                //AddCodepointRange(ref font, FontPath, 0x1e00, 0x1eff);
                //AddCodepointRange(ref font, FontPath, 0x2c60, 0x2c7f);
            }

            prevUnicodeRange = unicodeRange;
            SetTextureFilter(font.Texture, TextureFilter.Bilinear); // Set font atlas scale filter
        }

        if (IsKeyPressed(KeyboardKey.Zero)) unicodeRange = 0;
        else if (IsKeyPressed(KeyboardKey.One)) unicodeRange = 1;
        else if (IsKeyPressed(KeyboardKey.Two)) unicodeRange = 2;
        else if (IsKeyPressed(KeyboardKey.Three)) unicodeRange = 3;
        else if (IsKeyPressed(KeyboardKey.Four)) unicodeRange = 4;
        //else if (IsKeyPressed(KeyboardKey.Five)) unicodeRange = 5;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText("ADD CODEPOINTS: [1][2][3][4]", 20, 20, 20, Color.Maroon);

        // Render test strings in different languages
        DrawTextEx(font, "> English: Hello World!", new Vector2(50, 70), 32, 1, Color.DarkGray); // English
        DrawTextEx(font, "> Español: Hola mundo!", new Vector2(50, 120), 32, 1, Color.DarkGray); // Spanish
        DrawTextEx(font, "> Ελληνικά: Γειά σου κόσμε!", new Vector2(50, 170), 32, 1, Color.DarkGray); // Greek
        DrawTextEx(font, "> Русский: Привет мир!", new Vector2(50, 220), 32, 0, Color.DarkGray); // Russian
        DrawTextEx(font, "> 中文: 你好世界!", new Vector2(50, 270), 32, 1, Color.DarkGray);        // Chinese
        DrawTextEx(font, "> 日本語: こんにちは世界!", new Vector2(50, 320), 32, 1, Color.DarkGray); // Japanese
        //DrawTextEx(font, "देवनागरी: होला मुंडो!", new Vector2(50, 350), 32, 1, Color.DarkGray);     // Devanagari (glyphs not available in font)

        // Draw font texture scaled to screen
        float atlasScale = 380.0f / font.Texture.Width;
        DrawRectangleRec(new Rectangle(400.0f, 16.0f, font.Texture.Width * atlasScale, font.Texture.Height * atlasScale), Color.Black);
        DrawTexturePro(font.Texture, new Rectangle(0, 0, font.Texture.Width, font.Texture.Height),
            new Rectangle(400.0f, 16.0f, font.Texture.Width * atlasScale, font.Texture.Height * atlasScale), new Vector2(0, 0), 0.0f, Color.White);
        DrawRectangleLines(400, 16, 380, 380, Color.Red);

        DrawText($"ATLAS SIZE: {font.Texture.Width}x{font.Texture.Height} px (x{atlasScale:00.00})", 20, 380, 20, Color.Blue);
        DrawText($"CODEPOINTS GLYPHS LOADED: {font.GlyphCount}", 20, 410, 20, Color.Lime);

        // Display font attribution
        DrawText("Font: Noto Sans TC. License: SIL Open Font License 1.1", screenWidth - 300, screenHeight - 20, 10, Color.Gray);

        if (prevUnicodeRange != unicodeRange)
        {
            DrawRectangle(0, 0, screenWidth, screenHeight, Fade(Color.White, 0.8f));
            DrawRectangle(0, 125, screenWidth, 200, Color.Gray);
            DrawText("GENERATING FONT ATLAS...", 120, 210, 40, Color.Black);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(font);        // Unload font resource
    }

    //--------------------------------------------------------------------------------------
    // Module Functions Definition
    //--------------------------------------------------------------------------------------
    // Add codepoint range to existing font
    private static unsafe void AddCodepointRange(ref Font font, string fontPath, int start, int stop)
    {
        int rangeSize = stop - start + 1;
        int currentRangeSize = font.GlyphCount;

        // TODO: Load glyphs from provided vector font (if available),
        // add them to existing font, regenerating font image and texture

        int updatedCodepointCount = currentRangeSize + rangeSize;
        int[] updatedCodepoints = new int[updatedCodepointCount];

        // Get current codepoint list
        for (int i = 0; i < currentRangeSize; i++) updatedCodepoints[i] = font.Glyphs[i].Value;

        // Add new codepoints to list (provided range)
        for (int i = currentRangeSize; i < updatedCodepointCount; i++)
            updatedCodepoints[i] = start + (i - currentRangeSize);

        UnloadFont(font);
        font = LoadFontEx(fontPath, 32, updatedCodepoints, updatedCodepointCount);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - unicode ranges");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new UnicodeRanges();
        game.Init();

        // Main loop
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
