/*******************************************************************************************
*
*   raylib [text] example - words alignment
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by JP Mortiboys (@themushroompirates) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 JP Mortiboys (@themushroompirates)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;    // Required for: Lerp()

namespace Examples.Text;

public partial class WordsAlignment : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // TextAlignment values: Left/Top = 0, Centre/Middle = 1, Right/Bottom = 2

    public string Name => "Text / Words Alignment";

    public string Title => "raylib [text] example - words alignment";

    // Define the rectangle we will draw the text in
    private Rectangle textContainerRect;

    // Some text to display the current alignment
    private static readonly string[] textAlignNameH = { "Left", "Centre", "Right" };
    private static readonly string[] textAlignNameV = { "Top", "Middle", "Bottom" };

    // Define the text we're going to draw in the rectangle
    private int wordIndex;
    private int wordCount;
    private string[] words;

    // Initialize the font size we're going to use
    private int fontSize;

    // And of course the font...
    private Font font;

    // Initialize the alignment variables
    private int hAlign;
    private int vAlign;

    public void Init()
    {
        // Define the rectangle we will draw the text in
        textContainerRect = new Rectangle((float)screenWidth / 2 - (float)screenWidth / 4, (float)screenHeight / 2 - (float)screenHeight / 3, (float)screenWidth / 2, (float)screenHeight * 2 / 3);

        // Define the text we're going to draw in the rectangle
        wordIndex = 0;
        words = "raylib is a simple and easy-to-use library to enjoy videogames programming".Split(' ');
        wordCount = words.Length;

        // Initialize the font size we're going to use
        fontSize = 40;

        // And of course the font...
        font = GetFontDefault();

        // Initialize the alignment variables
        hAlign = 1; // TEXT_ALIGN_CENTRE
        vAlign = 1; // TEXT_ALIGN_MIDDLE
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Left))
        {
            if (hAlign > 0) hAlign = hAlign - 1;
        }

        if (IsKeyPressed(KeyboardKey.Right))
        {
            hAlign = hAlign + 1;
            if (hAlign > 2) hAlign = 2;
        }

        if (IsKeyPressed(KeyboardKey.Up))
        {
            if (vAlign > 0) vAlign = vAlign - 1;
        }

        if (IsKeyPressed(KeyboardKey.Down))
        {
            vAlign = vAlign + 1;
            if (vAlign > 2) vAlign = 2;
        }

        // One word per second
        if (wordCount > 0) wordIndex = (int)GetTime() % wordCount;
        else wordIndex = 0;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.DarkBlue);

        DrawText("Use Arrow Keys to change the text alignment", 20, 20, 20, Color.LightGray);
        DrawText($"Alignment: Horizontal = {textAlignNameH[hAlign]}, Vertical = {textAlignNameV[vAlign]}", 20, 40, 20, Color.LightGray);

        DrawRectangleRec(textContainerRect, Color.Blue);

        // Get the size of the text to draw
        Vector2 textSize = MeasureTextEx(font, words[wordIndex], fontSize, fontSize * .1f);

        // Calculate the top-left text position based on the rectangle and alignment
        Vector2 textPos = new Vector2(
            textContainerRect.X + Lerp(0.0f, textContainerRect.Width - textSize.X, hAlign * 0.5f),
            textContainerRect.Y + Lerp(0.0f, textContainerRect.Height - textSize.Y, vAlign * 0.5f)
        );

        // Draw the text
        DrawTextEx(font, words[wordIndex], textPos, fontSize, fontSize * .1f, Color.RayWhite);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - words alignment");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new WordsAlignment();
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
