/*******************************************************************************************
*
*   raylib [text] example - font sdf
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Text;

public partial class FontSdf : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Text / Font SDF";

    public string Title => "raylib [text] example - font sdf";

    private string msg;

    private Font fontDefault;
    private Font fontSDF;
    private Shader shader;

    private Vector2 fontPosition;
    private Vector2 textSize;
    private float fontSize;
    private int currentFont;

    public unsafe void Init()
    {
        // NOTE: Textures/Fonts MUST be loaded after Window initialization (OpenGL context is required)

        msg = "Signed Distance Fields";

        // Loading file to memory
        var fileSize = 0;
        var fileData = LoadFileData("resources/fonts/anonymous_pro_bold.ttf", ref fileSize);

        // Build the fonts in locals first: taking the address of a struct's field (&font.GlyphCount,
        // &font.Recs) is only allowed for a stack local, not a heap field. Assign to the fields after.

        // Default font generation from TTF font
        Font fontDefault = new();
        fontDefault.BaseSize = 16;
        fontDefault.GlyphCount = 95;

        // Loading font data from memory data
        // Parameters > font size: 16, no glyphs array provided (0), glyphs count: 95 (autogenerate chars array)
        fontDefault.Glyphs = LoadFontData(fileData, (int)fileSize, 16, null, 95, FontType.Default, &fontDefault.GlyphCount);
        // Parameters > glyphs count: 95, font size: 16, glyphs padding in image: 4 px, pack method: 0 (default)
        var atlas = GenImageFontAtlas(fontDefault.Glyphs, &fontDefault.Recs, 95, 16, 4, 0);
        fontDefault.Texture = LoadTextureFromImage(atlas);
        UnloadImage(atlas);
        this.fontDefault = fontDefault;

        // SDF font generation from TTF font
        Font fontSDF = new();
        fontSDF.BaseSize = 16;
        fontSDF.GlyphCount = 95;
        // Parameters > font size: 16, no glyphs array provided (0), glyphs count: 0 (defaults to 95)
        fontSDF.Glyphs = LoadFontData(fileData, (int)fileSize, 16, null, 0, FontType.Sdf, &fontSDF.GlyphCount);
        // Parameters > glyphs count: 95, font size: 16, glyphs padding in image: 0 px, pack method: 1 (Skyline algorythm)
        atlas = GenImageFontAtlas(fontSDF.Glyphs, &fontSDF.Recs, 95, 16, 0, 1);
        fontSDF.Texture = LoadTextureFromImage(atlas);
        UnloadImage(atlas);
        this.fontSDF = fontSDF;

        UnloadFileData(fileData);      // Free memory from loaded file

        // Load SDF required shader (we use default vertex shader)
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/sdf.fs");
        SetTextureFilter(fontSDF.Texture, TextureFilter.Bilinear);    // Required for SDF font

        fontPosition = new(40, screenHeight / 2.0f - 50);
        textSize = new(0.0f);
        fontSize = 16.0f;
        currentFont = 0;            // 0 - fontDefault, 1 - fontSDF
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        fontSize += GetMouseWheelMove() * 8.0f;

        if (fontSize < 6)
        {
            fontSize = 6;
        }

        if (IsKeyDown(KeyboardKey.Space))
        {
            currentFont = 1;
        }
        else
        {
            currentFont = 0;
        }

        if (currentFont == 0)
        {
            textSize = MeasureTextEx(fontDefault, msg, fontSize, 0);
        }
        else
        {
            textSize = MeasureTextEx(fontSDF, msg, fontSize, 0);
        }

        fontPosition.X = GetScreenWidth() / 2 - textSize.X / 2;
        fontPosition.Y = GetScreenHeight() / 2 - textSize.Y / 2 + 80;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        if (currentFont == 1)
        {
            // NOTE: SDF fonts require a custom SDf shader to compute fragment color
            BeginShaderMode(shader);    // Activate SDF font shader
            DrawTextEx(fontSDF, msg, fontPosition, fontSize, 0, Color.Black);
            EndShaderMode();            // Activate our default shader for next drawings

            DrawTexture(fontSDF.Texture, 10, 10, Color.Black);
        }
        else
        {
            DrawTextEx(fontDefault, msg, fontPosition, fontSize, 0, Color.Black);
            DrawTexture(fontDefault.Texture, 10, 10, Color.Black);
        }

        if (currentFont == 1)
        {
            DrawText("SDF!", 320, 20, 80, Color.Red);
        }
        else
        {
            DrawText("default font", 315, 40, 30, Color.Gray);
        }

        DrawText("FONT SIZE: 16.0", GetScreenWidth() - 240, 20, 20, Color.DarkGray);
        DrawText($"RENDER SIZE: {fontSize:00.00}", GetScreenWidth() - 240, 50, 20, Color.DarkGray);
        DrawText("Use MOUSE WHEEL to SCALE TEXT!", GetScreenWidth() - 240, 90, 10, Color.DarkGray);

        DrawText("HOLD SPACE to USE SDF FONT VERSION!", 340, GetScreenHeight() - 30, 20, Color.Maroon);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(fontDefault);    // Default font unloading
        UnloadFont(fontSDF);        // SDF font unloading

        UnloadShader(shader);       // Unload SDF shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - font sdf");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new FontSdf();
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
