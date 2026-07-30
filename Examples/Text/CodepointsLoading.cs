/*******************************************************************************************
*
*   raylib [text] example - codepoints loading
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Globalization;

namespace Examples.Text;

public partial class CodepointsLoading : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Text to be displayed, must be UTF-8 (save this code file as UTF-8)
    // NOTE: It can contain all the required text for the game,
    // this text will be scanned to get all the required codepoints
    private const string text =
        "いろはにほへと　ちりぬるを\nわかよたれそ　つねならむ\nうゐのおくやま　けふこえて\nあさきゆめみし　ゑひもせす";

    public string Name => "Text / Codepoints Loading";

    public string Title => "raylib [text] example - codepoints loading";

    private List<int> codepoints;
    private int[] codepointsNoDuplicates;
    private Font font;
    private bool showFontAtlas;

    public void Init()
    {
        // Get codepoints from text
        codepoints = GetCodePoints(text);

        // Remove duplicate codepoints to generate smaller font atlas
        codepointsNoDuplicates = codepoints.Distinct().ToArray();

        // Load font containing all the provided codepoint glyphs
        // A texture font atlas is automatically generated
        font = LoadFontEx(
            "resources/fonts/DotGothic16-Regular.ttf",
            36,
            codepointsNoDuplicates,
            codepointsNoDuplicates.Length
        );

        // Set bilinear scale filter for better font scaling
        SetTextureFilter(font.Texture, TextureFilter.Bilinear);

        SetTextLineSpacing(20);         // Set line spacing for multiline text (when line breaks are included '\n')

        showFontAtlas = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            showFontAtlas = !showFontAtlas;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawRectangle(0, 0, GetScreenWidth(), 70, Color.Black);
        DrawText($"Total codepoints contained in provided text: {codepoints.Count}", 10, 10, 20, Color.Green);
        DrawText(
            $"Total codepoints required for font atlas (duplicates excluded): {codepointsNoDuplicates.Length}",
            10,
            40,
            20,
            Color.Green
        );

        if (showFontAtlas)
        {
            // Draw generated font texture atlas containing provided codepoints
            DrawTexture(font.Texture, 150, 100, Color.Black);
            DrawRectangleLines(150, 100, font.Texture.Width, font.Texture.Height, Color.Black);
        }
        else
        {
            // Draw provided text with loaded font, containing all required codepoint glyphs
            DrawTextEx(font, text, new Vector2(160, 110), 48, 5, Color.Black);
        }

        DrawText("Press SPACE to toggle font atlas view!", 10, GetScreenHeight() - 30, 20, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(font);     // Unload font
    }

    private static List<int> GetCodePoints(string text)
    {
        List<int> codePoints = new();

        StringInfo stringInfo = new(text);
        var enumerator = StringInfo.GetTextElementEnumerator(text);

        while (enumerator.MoveNext())
        {
            var codePoint = char.ConvertToUtf32(enumerator.Current.ToString(), 0);
            codePoints.Add(codePoint);
        }

        return codePoints;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - codepoints loading");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new CodepointsLoading();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
