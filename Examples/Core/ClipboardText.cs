/*******************************************************************************************
*
*   raylib [core] example - clipboard text
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Ananth S (@Ananth1839) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ananth S (@Ananth1839)
*
********************************************************************************************/

// NOTE: The upstream C example uses raygui (GuiTextBox/GuiButton/GuiLabel) for its UI.
// raygui is not bound in raylib-cs, so the widgets below are minimal re-implementations
// using plain raylib drawing/input. Clipboard behaviour (cut/copy/paste and CTRL+X/C/V
// shortcuts) matches the original.

using System;
using System.Numerics;
using System.Text;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class ClipboardText : IExample
{
    private const int MaxTextSamples = 5;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Clipboard Text";

    public string Title => "raylib [core] example - clipboard text";

    private string[] sampleTexts;
    private string clipboardText;
    private StringBuilder inputBuffer;

    // UI required variables
    private bool textBoxEditMode;

    private bool btnCutPressed;
    private bool btnCopyPressed;
    private bool btnPastePressed;
    private bool btnClearPressed;
    private bool btnRandomPressed;

    private int framesCounter;

    public void Init()
    {
        // Define some sample texts
        sampleTexts = new string[]
        {
            "Hello from raylib!",
            "The quick brown fox jumps over the lazy dog",
            "Clipboard operations are useful!",
            "raylib is a simple and easy-to-use library",
            "Copy and paste me!"
        };

        clipboardText = null;
        inputBuffer = new StringBuilder("Hello from raylib!"); // Random initial string

        // UI required variables
        textBoxEditMode = false;

        btnCutPressed = false;
        btnCopyPressed = false;
        btnPastePressed = false;
        btnClearPressed = false;
        btnRandomPressed = false;

        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        framesCounter++;

        // Handle button interactions
        if (btnCutPressed)
        {
            SetClipboardText(inputBuffer.ToString());
            clipboardText = GetClipboardText_();
            inputBuffer.Clear(); // Quick solution to clear text
        }

        if (btnCopyPressed)
        {
            SetClipboardText(inputBuffer.ToString()); // Copy text to clipboard
            clipboardText = GetClipboardText_(); // Get text from clipboard
        }

        if (btnPastePressed)
        {
            // Paste text from clipboard
            clipboardText = GetClipboardText_();
            if (clipboardText != null) SetInputBuffer(clipboardText);
        }

        if (btnClearPressed)
        {
            inputBuffer.Clear(); // Quick solution to clear text
        }

        if (btnRandomPressed)
        {
            // Get random text from sample list
            SetInputBuffer(sampleTexts[GetRandomValue(0, MaxTextSamples - 1)]);
        }

        // Quick cut/copy/paste with keyboard shortcuts
        if (IsKeyDown(KeyboardKey.LeftControl) || IsKeyDown(KeyboardKey.RightControl))
        {
            if (IsKeyPressed(KeyboardKey.X))
            {
                SetClipboardText(inputBuffer.ToString());
                inputBuffer.Clear(); // Quick solution to clear text
            }

            if (IsKeyPressed(KeyboardKey.C)) SetClipboardText(inputBuffer.ToString());

            if (IsKeyPressed(KeyboardKey.V))
            {
                clipboardText = GetClipboardText_();
                if (clipboardText != null) SetInputBuffer(clipboardText);
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw instructions
        GuiLabel(new Rectangle(50, 20, 700, 36), "Use the BUTTONS or KEY SHORTCUTS:");
        DrawText("[CTRL+X] - CUT | [CTRL+C] COPY | [CTRL+V] | PASTE", 50, 60, 20, Color.Maroon);

        // Draw text box
        if (GuiTextBox(new Rectangle(50, 120, 652, 40), inputBuffer, 256, textBoxEditMode)) textBoxEditMode = !textBoxEditMode;

        // Random text button
        btnRandomPressed = GuiButton(new Rectangle(50 + 652 + 8, 120, 40, 40), "RND");

        // Draw buttons
        btnCutPressed = GuiButton(new Rectangle(50, 180, 158, 40), "CUT");
        btnCopyPressed = GuiButton(new Rectangle(50 + 165, 180, 158, 40), "COPY");
        btnPastePressed = GuiButton(new Rectangle(50 + 165 * 2, 180, 158, 40), "PASTE");
        btnClearPressed = GuiButton(new Rectangle(50 + 165 * 3, 180, 158, 40), "CLEAR");

        // Draw clipboard status
        GuiLabel(new Rectangle(50, 260, 700, 40), "Clipboard current text data:");
        GuiTextBoxReadOnly(new Rectangle(50, 300, 700, 40), clipboardText);
        GuiLabel(new Rectangle(50, 360, 700, 40), "Try copying text from other applications and pasting here!");

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    // Replace input buffer contents (equivalent to raylib TextCopy into a fixed buffer)
    private void SetInputBuffer(string text)
    {
        inputBuffer.Clear();
        if (text != null)
        {
            if (text.Length > 255) text = text.Substring(0, 255);
            inputBuffer.Append(text);
        }
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiLabel(Rectangle bounds, string text)
    {
        DrawText(text, (int)bounds.X, (int)(bounds.Y + (bounds.Height - 20) / 2), 20, Color.DarkGray);
    }

    private static bool GuiButton(Rectangle bounds, string text)
    {
        bool pressed = false;
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);

        Color fill = hover ? (IsMouseButtonDown(MouseButton.Left) ? Color.SkyBlue : Color.LightGray) : Color.RayWhite;
        DrawRectangleRec(bounds, fill);
        DrawRectangleLinesEx(bounds, 1, hover ? Color.Blue : Color.Gray);

        int textWidth = MeasureText(text, 20);
        DrawText(text, (int)(bounds.X + (bounds.Width - textWidth) / 2), (int)(bounds.Y + (bounds.Height - 20) / 2), 20, Color.DarkGray);

        if (hover && IsMouseButtonReleased(MouseButton.Left)) pressed = true;

        return pressed;
    }

    private bool GuiTextBox(Rectangle bounds, StringBuilder text, int maxChars, bool editMode)
    {
        bool pressed = false;
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);

        if (editMode)
        {
            // Get char pressed (unicode character) on the queue
            int key = GetCharPressed();
            while (key > 0)
            {
                if ((key >= 32) && (key <= 125) && (text.Length < maxChars - 1))
                {
                    text.Append((char)key);
                }

                key = GetCharPressed();
            }

            if (IsKeyPressed(KeyboardKey.Backspace) && (text.Length > 0)) text.Remove(text.Length - 1, 1);
        }

        DrawRectangleRec(bounds, Color.RayWhite);
        DrawRectangleLinesEx(bounds, editMode ? 2 : 1, editMode ? Color.Red : (hover ? Color.Blue : Color.Gray));

        string content = text.ToString();
        DrawText(content, (int)bounds.X + 4, (int)(bounds.Y + (bounds.Height - 20) / 2), 20, Color.DarkGray);

        // Draw blinking cursor while editing
        if (editMode && ((framesCounter / 20) % 2 == 0))
        {
            DrawText("_", (int)bounds.X + 4 + MeasureText(content, 20), (int)(bounds.Y + (bounds.Height - 20) / 2), 20, Color.DarkGray);
        }

        if (hover && IsMouseButtonPressed(MouseButton.Left)) pressed = true;

        return pressed;
    }

    private static void GuiTextBoxReadOnly(Rectangle bounds, string text)
    {
        DrawRectangleRec(bounds, Color.LightGray);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (text != null) DrawText(text, (int)bounds.X + 4, (int)(bounds.Y + (bounds.Height - 20) / 2), 20, Color.Gray);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - clipboard text");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ClipboardText();
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
