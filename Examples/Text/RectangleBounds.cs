/*******************************************************************************************
*
*   raylib [text] example - rectangle bounds
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 4.0
*
*   Example contributed by Vlad Adrian (@demizdor) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Vlad Adrian (@demizdor) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class RectangleBounds : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Minimum width and heigh for the container rectangle
    private const float minWidth = 60;
    private const float minHeight = 60;
    private const float maxWidth = screenWidth - 50.0f;
    private const float maxHeight = screenHeight - 160.0f;

    public string Name => "Text / Rectangle Bounds";

    public string Title => "raylib [text] example - rectangle bounds";

    private string text;
    private bool resizing;
    private bool wordWrap;

    private Rectangle container;
    private Rectangle resizer;

    private Vector2 lastMouse;
    private Color borderColor;
    private Font font;

    public void Init()
    {
        text = "Text cannot escape\tthis container\t...word wrap also works when active so here's " +
            "a long text for testing.\n\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod " +
            "tempor incididunt ut labore et dolore magna aliqua. Nec ullamcorper sit amet risus nullam eget felis eget.";

        resizing = false;
        wordWrap = true;

        container = new(25.0f, 25.0f, screenWidth - 50.0f, screenHeight - 250.0f);
        resizer = new(
            container.X + container.Width - 17,
            container.Y + container.Height - 17,
            14,
            14
        );

        lastMouse = new(0.0f, 0.0f); // Stores last mouse coordinates
        borderColor = Color.Maroon;  // Container border color
        font = GetFontDefault();     // Get default system font
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            wordWrap = !wordWrap;
        }

        var mouse = GetMousePosition();

        // Check if the mouse is inside the container and toggle border color
        if (CheckCollisionPointRec(mouse, container))
        {
            borderColor = Fade(Color.Maroon, 0.4f);
        }
        else if (!resizing)
        {
            borderColor = Color.Maroon;
        }

        // Container resizing logic
        if (resizing)
        {
            if (IsMouseButtonReleased(MouseButton.Left))
            {
                resizing = false;
            }

            var width = container.Width + (mouse.X - lastMouse.X);
            container.Width = (width > minWidth) ? ((width < maxWidth) ? width : maxWidth) : minWidth;

            var height = container.Height + (mouse.Y - lastMouse.Y);
            container.Height = (height > minHeight) ? ((height < maxHeight) ? height : maxHeight) : minHeight;
        }
        else
        {
            // Check if we're resizing
            if (IsMouseButtonDown(MouseButton.Left) && CheckCollisionPointRec(mouse, resizer))
            {
                resizing = true;
            }
        }

        // Move resizer rectangle properly
        resizer.X = container.X + container.Width - 17;
        resizer.Y = container.Y + container.Height - 17;

        lastMouse = mouse; // Update mouse
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawRectangleLinesEx(container, 3, borderColor);    // Draw container border

        // Draw text in container (add some padding)
        DrawTextBoxed(
            font,
            text,
            new Rectangle(container.X + 4, container.Y + 4, container.Width - 4, container.Height - 4),
            20.0f,
            2.0f,
            wordWrap,
            Color.Gray
        );

        DrawRectangleRec(resizer, borderColor);             // Draw the resize box

        // Draw bottom info
        DrawRectangle(0, screenHeight - 54, screenWidth, 54, Color.Gray);
        DrawRectangleRec(new Rectangle(382, screenHeight - 34, 12, 12), Color.Maroon);

        DrawText("Word Wrap: ", 313, screenHeight - 115, 20, Color.Black);

        if (wordWrap)
        {
            DrawText("ON", 447, screenHeight - 115, 20, Color.Red);
        }
        else
        {
            DrawText("OFF", 447, screenHeight - 115, 20, Color.Black);
        }

        DrawText("Press [SPACE] to toggle word wrap", 218, screenHeight - 86, 20, Color.Gray);
        DrawText("Click hold & drag the    to resize the container", 155, screenHeight - 38, 20, Color.RayWhite);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    // Draw text using font inside rectangle limits
    private static void DrawTextBoxed(
        Font font,
        string text,
        Rectangle rec,
        float fontSize,
        float spacing,
        bool wordWrap,
        Color tint
    )
    {
        DrawTextBoxedSelectable(font, text, rec, fontSize, spacing, wordWrap, tint, 0, 0, Color.White, Color.White);
    }

    // Draw text using font inside rectangle limits with support for text selection
    private static unsafe void DrawTextBoxedSelectable(
        Font font,
        string text,
        Rectangle rec,
        float fontSize,
        float spacing,
        bool wordWrap,
        Color tint,
        int selectStart,
        int selectLength,
        Color selectTint,
        Color selectBackTint
    )
    {
        var length = text.Length;

        // Offset between lines (on line break '\n')
        float textOffsetY = 0;

        // Offset X to next character to draw
        var textOffsetX = 0.0f;

        // Character rectangle scaling factor
        var scaleFactor = fontSize / (float)font.BaseSize;

        // Word/character wrapping mechanism variables
        var shouldMeasure = wordWrap;

        // Index where to begin drawing (where a line begins)
        var startLine = -1;

        // Index where to stop drawing (where a line ends)
        var endLine = -1;

        // Holds last value of the character position
        var lastk = -1;

        using var textNative = new Utf8Buffer(text);

        for (int i = 0, k = 0; i < length; i++, k++)
        {
            // Get next codepoint from byte string and glyph index in font
            var codepointByteCount = 0;
            var codepoint = GetCodepoint(&textNative.AsPointer()[i], &codepointByteCount);
            var index = GetGlyphIndex(font, codepoint);

            // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol moving one byte
            if (codepoint == 0x3f)
            {
                codepointByteCount = 1;
            }

            i += (codepointByteCount - 1);

            float glyphWidth = 0;
            if (codepoint != '\n')
            {
                glyphWidth = (font.Glyphs[index].AdvanceX == 0) ?
                    font.Recs[index].Width * scaleFactor :
                    font.Glyphs[index].AdvanceX * scaleFactor;

                if (i + 1 < length)
                {
                    glyphWidth = glyphWidth + spacing;
                }
            }

            // NOTE: When wordWrap is ON we first measure how much of the text we can draw before going outside of
            // the rec container. We store this info in startLine and endLine, then we change states, draw the text
            // between those two variables and change states again and again recursively until the end of the text
            // (or until we get outside of the container). When wordWrap is OFF we don't need the measure state so
            // we go to the drawing state immediately and begin drawing on the next line before we can get outside
            // the container.
            if (shouldMeasure)
            {
                // TODO: There are multiple types of spaces in UNICODE, maybe it's a good idea to add support for
                // more. Ref: http://jkorpela.fi/chars/spaces.html
                if ((codepoint == ' ') || (codepoint == '\t') || (codepoint == '\n'))
                {
                    endLine = i;
                }

                if ((textOffsetX + glyphWidth) > rec.Width)
                {
                    endLine = (endLine < 1) ? i : endLine;
                    if (i == endLine)
                    {
                        endLine -= codepointByteCount;
                    }
                    if ((startLine + codepointByteCount) == endLine)
                    {
                        endLine = (i - codepointByteCount);
                    }

                    shouldMeasure = !shouldMeasure;
                }
                else if ((i + 1) == length)
                {
                    endLine = i;
                    shouldMeasure = !shouldMeasure;
                }
                else if (codepoint == '\n')
                {
                    shouldMeasure = !shouldMeasure;
                }

                if (!shouldMeasure)
                {
                    textOffsetX = 0;
                    i = startLine;
                    glyphWidth = 0;

                    // Save character position when we switch states
                    var tmp = lastk;
                    lastk = k - 1;
                    k = tmp;
                }
            }
            else
            {
                if (codepoint == '\n')
                {
                    if (!wordWrap)
                    {
                        textOffsetY += (font.BaseSize + font.BaseSize / 2) * scaleFactor;
                        textOffsetX = 0;
                    }
                }
                else
                {
                    if (!wordWrap && ((textOffsetX + glyphWidth) > rec.Width))
                    {
                        textOffsetY += (font.BaseSize + font.BaseSize / 2) * scaleFactor;
                        textOffsetX = 0;
                    }

                    // When text overflows rectangle height limit, just stop drawing
                    if ((textOffsetY + font.BaseSize * scaleFactor) > rec.Height)
                    {
                        break;
                    }

                    // Draw selection background
                    var isGlyphSelected = false;
                    if ((selectStart >= 0) && (k >= selectStart) && (k < (selectStart + selectLength)))
                    {
                        DrawRectangleRec(
                            new Rectangle(
                                rec.X + textOffsetX - 1,
                                rec.Y + textOffsetY,
                                glyphWidth,
                                (float)font.BaseSize * scaleFactor
                            ),
                            selectBackTint
                        );
                        isGlyphSelected = true;
                    }

                    // Draw current character glyph
                    if ((codepoint != ' ') && (codepoint != '\t'))
                    {
                        DrawTextCodepoint(
                            font,
                            codepoint,
                            new Vector2(rec.X + textOffsetX, rec.Y + textOffsetY),
                            fontSize,
                            isGlyphSelected ? selectTint : tint
                        );
                    }
                }

                if (wordWrap && (i == endLine))
                {
                    textOffsetY += (font.BaseSize + font.BaseSize / 2) * scaleFactor;
                    textOffsetX = 0;
                    startLine = endLine;
                    endLine = -1;
                    glyphWidth = 0;
                    selectStart += lastk - k;
                    k = lastk;

                    shouldMeasure = !shouldMeasure;
                }
            }

            if ((textOffsetX != 0) || (codepoint != ' '))
            {
                // avoid leading spaces
                textOffsetX += glyphWidth;
            }
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - rectangle bounds");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new RectangleBounds();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
