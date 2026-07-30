/*******************************************************************************************
*
*   raylib [text] example - inline styling
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Wagner Barongello (@SultansOfCode) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Wagner Barongello (@SultansOfCode) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Text;

namespace Examples.Text;

public partial class InlineStyling : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Text / Inline Styling";

    public string Title => "raylib [text] example - inline styling";

    private Vector2 textSize;    // Measure text box for provided font and text
    private Color colRandom;     // Random color used on text
    private int frameCounter;    // Used to generate a new random color every certain frames

    public void Init()
    {
        textSize = new(0, 0);
        colRandom = Color.Red;
        frameCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        frameCounter++;

        if ((frameCounter % 20) == 0)
        {
            colRandom.R = (byte)GetRandomValue(0, 255);
            colRandom.G = (byte)GetRandomValue(0, 255);
            colRandom.B = (byte)GetRandomValue(0, 255);
            colRandom.A = 255;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Text inline styling strategy used: [ ] delimiters for format
        // - Define foreground color:      [cRRGGBBAA]
        // - Define background color:      [bRRGGBBAA]
        // - Reset formating:              [r]
        // Colors defined with [cRRGGBBAA] or [bRRGGBBAA] are multiplied by the base color alpha
        // This allows global transparency control while keeping per-section styling (ex. text fade effects)
        // Example: [bAA00AAFF][cFF0000FF]red text on gray background[r] normal text

        DrawTextStyled(GetFontDefault(), "This changes the [cFF0000FF]foreground color[r] of provided text!!!",
            new Vector2(100, 80), 20.0f, 2.0f, Color.Black);

        DrawTextStyled(GetFontDefault(), "This changes the [bFF00FFFF]background color[r] of provided text!!!",
            new Vector2(100, 120), 20.0f, 2.0f, Color.Black);

        DrawTextStyled(GetFontDefault(), "This changes the [c00ff00ff][bff0000ff]foreground and background colors[r]!!!",
            new Vector2(100, 160), 20.0f, 2.0f, Color.Black);

        DrawTextStyled(GetFontDefault(), "This changes the [c00ff00ff]alpha[r] relative [cffffffff][b000000ff]from source[r] [cff000088]color[r]!!!",
            new Vector2(100, 200), 20.0f, 2.0f, new Color(0, 0, 0, 100));

        // Get pointer to formated text
        string text = $"Let's be [c{colRandom.R:x2}{colRandom.G:x2}{colRandom.B:x2}FF]CREATIVE[r] !!!";
        DrawTextStyled(GetFontDefault(), text, new Vector2(100, 240), 40.0f, 2.0f, Color.Black);

        textSize = MeasureTextStyled(GetFontDefault(), text, 40.0f, 2.0f);
        DrawRectangleLines(100, 240, (int)textSize.X, (int)textSize.Y, Color.Green);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    // Draw text using inline styling
    // PARAM: color is the default text color, background color is BLANK by default
    // NOTE: Using input color as the base alpha multiplied to inline styles
    private static unsafe void DrawTextStyled(Font font, string text, Vector2 position, float fontSize, float spacing, Color color)
    {
        // Text inline styling strategy used: [ ] delimiters for format
        // - Define foreground color:      [cRRGGBBAA]
        // - Define background color:      [bRRGGBBAA]
        // - Reset formating:              [r]
        // Example: [bAA00AAFF][cFF0000FF]red text on gray background[r] normal text

        if (font.Texture.Id == 0)
        {
            font = GetFontDefault();
        }

        using var textNative = new Utf8Buffer(text);
        sbyte* t = textNative.AsPointer();
        int textLen = Encoding.UTF8.GetByteCount(text);

        Color colFront = color;
        Color colBack = Color.Blank;
        int backRecPadding = 4; // Background rectangle padding

        float textOffsetY = 0.0f;
        float textOffsetX = 0.0f;
        float textLineSpacing = 0.0f;
        float scaleFactor = fontSize / font.BaseSize;

        for (int i = 0; i < textLen;)
        {
            int codepointByteCount = 0;
            int codepoint = GetCodepointNext(&t[i], &codepointByteCount);

            if (codepoint == '\n')
            {
                textOffsetY += (fontSize + textLineSpacing);
                textOffsetX = 0.0f;
            }
            else
            {
                if (codepoint == '[') // Process pipe styling
                {
                    if (((i + 2) < textLen) && ((char)t[i + 1] == 'r') && ((char)t[i + 2] == ']')) // Reset styling
                    {
                        colFront = color;
                        colBack = Color.Blank;

                        i += 3;     // Skip "[r]"
                        continue;   // Do not draw characters
                    }
                    else if (((i + 1) < textLen) && (((char)t[i + 1] == 'c') || ((char)t[i + 1] == 'b')))
                    {
                        i += 2;     // Skip "[c" or "[b" to start parsing color

                        // Parse following color
                        var colHexText = new StringBuilder();
                        int colHexCount = 0;
                        while ((i + colHexCount < textLen) && (t[i + colHexCount] != 0) && ((char)t[i + colHexCount] != ']'))
                        {
                            char ch = (char)t[i + colHexCount];
                            if (((ch >= '0') && (ch <= '9')) ||
                                ((ch >= 'A') && (ch <= 'F')) ||
                                ((ch >= 'a') && (ch <= 'f')))
                            {
                                colHexText.Append(ch);
                                colHexCount++;
                            }
                            else
                            {
                                break; // Only affects while loop
                            }
                        }

                        // Convert hex color text into actual Color
                        uint colHexValue = colHexText.Length > 0 ? Convert.ToUInt32(colHexText.ToString(), 16) : 0;
                        if ((char)t[i - 1] == 'c')
                        {
                            colFront = GetColor(colHexValue);
                        }
                        else if ((char)t[i - 1] == 'b')
                        {
                            colBack = GetColor(colHexValue);
                        }

                        i += (colHexCount + 1); // Skip color value retrieved and ']'
                        continue;   // Do not draw characters
                    }
                }

                int index = GetGlyphIndex(font, codepoint);
                float increaseX = 0.0f;

                if (font.Glyphs[index].AdvanceX == 0)
                {
                    increaseX = (font.Recs[index].Width * scaleFactor + spacing);
                }
                else
                {
                    increaseX += (font.Glyphs[index].AdvanceX * scaleFactor + spacing);
                }

                // Draw background rectangle color (if required)
                if (colBack.A > 0)
                {
                    DrawRectangleRec(new Rectangle(position.X + textOffsetX, position.Y + textOffsetY - backRecPadding, increaseX, fontSize + 2 * backRecPadding), colBack);
                }

                if ((codepoint != ' ') && (codepoint != '\t'))
                {
                    DrawTextCodepoint(font, codepoint, new Vector2(position.X + textOffsetX, position.Y + textOffsetY), fontSize, colFront);
                }

                textOffsetX += increaseX;
            }

            i += codepointByteCount;
        }
    }

    // Measure inline styled text
    // NOTE: Measuring styled text requires skipping styling data
    // WARNING: Not considering line breaks
    private static unsafe Vector2 MeasureTextStyled(Font font, string text, float fontSize, float spacing)
    {
        Vector2 textSize = new(0, 0);

        if ((font.Texture.Id == 0) || (text == null) || (text.Length == 0))
        {
            return textSize; // Security check
        }

        using var textNative = new Utf8Buffer(text);
        sbyte* t = textNative.AsPointer();
        int textLen = Encoding.UTF8.GetByteCount(text); // Get size in bytes of text

        float textWidth = 0.0f;
        float textHeight = fontSize;
        float scaleFactor = fontSize / (float)font.BaseSize;

        int codepoint = 0;              // Current character
        int index = 0;                  // Index position in sprite font
        int validCodepointCounter = 0;

        for (int i = 0; i < textLen;)
        {
            int codepointByteCount = 0;
            codepoint = GetCodepointNext(&t[i], &codepointByteCount);

            if (codepoint == '[') // Ignore pipe inline styling
            {
                if (((i + 2) < textLen) && ((char)t[i + 1] == 'r') && ((char)t[i + 2] == ']')) // Reset styling
                {
                    i += 3;     // Skip "[r]"
                    continue;   // Do not measure characters
                }
                else if (((i + 1) < textLen) && (((char)t[i + 1] == 'c') || ((char)t[i + 1] == 'b')))
                {
                    i += 2;     // Skip "[c" or "[b" to start parsing color

                    int colHexCount = 0;
                    while ((i + colHexCount < textLen) && (t[i + colHexCount] != 0) && ((char)t[i + colHexCount] != ']'))
                    {
                        char ch = (char)t[i + colHexCount];
                        if (((ch >= '0') && (ch <= '9')) ||
                            ((ch >= 'A') && (ch <= 'F')) ||
                            ((ch >= 'a') && (ch <= 'f')))
                        {
                            colHexCount++;
                        }
                        else
                        {
                            break; // Only affects while loop
                        }
                    }

                    i += (colHexCount + 1); // Skip color value retrieved and ']'
                    continue;   // Do not measure characters
                }
            }
            else if (codepoint != '\n')
            {
                index = GetGlyphIndex(font, codepoint);

                if (font.Glyphs[index].AdvanceX > 0)
                {
                    textWidth += font.Glyphs[index].AdvanceX;
                }
                else
                {
                    textWidth += (font.Recs[index].Width + font.Glyphs[index].OffsetX);
                }

                validCodepointCounter++;
                i += codepointByteCount;
            }
        }

        textSize.X = textWidth * scaleFactor + (validCodepointCounter - 1) * spacing;
        textSize.Y = textHeight;

        return textSize;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - inline styling");

        SetTargetFPS(60);           // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new InlineStyling();
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
