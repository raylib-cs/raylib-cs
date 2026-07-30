/*******************************************************************************************
*
*   raylib [core] example - compute hash
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

// NOTE: The upstream C example uses raygui (GuiTextBox/GuiButton/GuiLabel) for its UI.
// raygui is not bound in raylib-cs, so the widgets below are minimal re-implementations
// using plain raylib drawing/input. The hashing/Base64 logic is a faithful port.

using System.Text;

namespace Examples.Core;

public unsafe partial class ComputeHash : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Compute Hash";

    public string Title => "raylib [core] example - compute hash";

    // UI controls variables
    private StringBuilder textInput;
    private bool textBoxEditMode;
    private bool btnComputeHashes;

    // Data hash values
    private uint hashCRC32;
    private uint[] hashMD5;
    private uint[] hashSHA1;
    private uint[] hashSHA256;

    // Base64 encoded data
    private string base64Text;

    private int framesCounter;

    public void Init()
    {
        textInput = new StringBuilder("The quick brown fox jumps over the lazy dog.");
        textBoxEditMode = false;
        btnComputeHashes = false;

        hashCRC32 = 0;
        hashMD5 = null;
        hashSHA1 = null;
        hashSHA256 = null;

        base64Text = null;

        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        framesCounter++;

        if (btnComputeHashes)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(textInput.ToString());
            int textInputLen = bytes.Length;

            fixed (byte* textInputPtr = bytes)
            {
                int base64TextSize;

                // Encode data to Base64 string (includes NULL terminator), memory must be MemFree()
                sbyte* base64 = EncodeDataBase64(textInputPtr, textInputLen, &base64TextSize);
                base64Text = (base64 != null) ? new string(base64) : null;
                MemFree(base64); // Free Base64 text data (kept managed above)

                hashCRC32 = ComputeCRC32(textInputPtr, textInputLen);            // Compute CRC32 hash code (4 bytes)
                hashMD5 = CopyHash(ComputeMD5(textInputPtr, textInputLen), 4);   // Compute MD5 hash code, returns static int[4] (16 bytes)
                hashSHA1 = CopyHash(ComputeSHA1(textInputPtr, textInputLen), 5); // Compute SHA1 hash code, returns static int[5] (20 bytes)
                hashSHA256 = CopyHash(ComputeSHA256(textInputPtr, textInputLen), 8); // Compute SHA256 hash code, returns static int[8] (32 bytes)
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        GuiLabel(new Rectangle(40, 26, 720, 32), "INPUT DATA (TEXT):", 20);

        if (GuiTextBox(new Rectangle(40, 64, 720, 32), textInput, 95, textBoxEditMode, 10))
        {
            textBoxEditMode = !textBoxEditMode;
        }

        btnComputeHashes = GuiButton(new Rectangle(40, 64 + 40, 720, 32), "COMPUTE INPUT DATA HASHES", 10);

        GuiLabel(new Rectangle(40, 160, 720, 32), "INPUT DATA HASH VALUES:", 20);

        GuiLabel(new Rectangle(40, 200, 120, 32), "CRC32 [32 bit]:", 10);
        GuiTextBoxReadOnly(new Rectangle(40 + 120, 200, 720 - 120, 32), GetDataAsHexText(new uint[] { hashCRC32 }, 1), 10);
        GuiLabel(new Rectangle(40, 200 + 36, 120, 32), "MD5 [128 bit]:", 10);
        GuiTextBoxReadOnly(new Rectangle(40 + 120, 200 + 36, 720 - 120, 32), GetDataAsHexText(hashMD5, 4), 10);
        GuiLabel(new Rectangle(40, 200 + 36 * 2, 120, 32), "SHA1 [160 bit]:", 10);
        GuiTextBoxReadOnly(new Rectangle(40 + 120, 200 + 36 * 2, 720 - 120, 32), GetDataAsHexText(hashSHA1, 5), 10);
        GuiLabel(new Rectangle(40, 200 + 36 * 3, 120, 32), "SHA256 [256 bit]:", 10);
        GuiTextBoxReadOnly(new Rectangle(40 + 120, 200 + 36 * 3, 720 - 120, 32), GetDataAsHexText(hashSHA256, 8), 10);

        GuiLabel(new Rectangle(40, 200 + 36 * 5 - 30, 320, 32), "BONUS - BAS64 ENCODED STRING:", 10);
        GuiLabel(new Rectangle(40, 200 + 36 * 5, 120, 32), "BASE64 ENCODING:", 10);
        GuiTextBoxReadOnly(new Rectangle(40 + 120, 200 + 36 * 5, 720 - 120, 32), base64Text, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    private static uint[] CopyHash(uint* data, int count)
    {
        if (data == null)
        {
            return null;
        }

        uint[] result = new uint[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = data[i];
        }

        return result;
    }

    private static string GetDataAsHexText(uint[] data, int dataSize)
    {
        if ((data != null) && (dataSize > 0) && (dataSize < ((128 / 8) - 1)))
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < dataSize; i++)
            {
                text.Append(data[i].ToString("X8"));
            }

            return text.ToString();
        }

        return "00000000";
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiLabel(Rectangle bounds, string text, int fontSize)
    {
        DrawText(text, (int)bounds.X, (int)(bounds.Y + (bounds.Height - fontSize) / 2), fontSize, Color.DarkGray);
    }

    private static bool GuiButton(Rectangle bounds, string text, int fontSize)
    {
        bool pressed = false;
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);

        Color fill = hover ? (IsMouseButtonDown(MouseButton.Left) ? Color.SkyBlue : Color.LightGray) : Color.RayWhite;
        DrawRectangleRec(bounds, fill);
        DrawRectangleLinesEx(bounds, 1, hover ? Color.Blue : Color.Gray);

        int textWidth = MeasureText(text, fontSize);
        DrawText(text, (int)(bounds.X + (bounds.Width - textWidth) / 2), (int)(bounds.Y + (bounds.Height - fontSize) / 2), fontSize, Color.DarkGray);

        if (hover && IsMouseButtonReleased(MouseButton.Left))
        {
            pressed = true;
        }

        return pressed;
    }

    private bool GuiTextBox(Rectangle bounds, StringBuilder text, int maxChars, bool editMode, int fontSize)
    {
        bool pressed = false;
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);

        if (editMode)
        {
            int key = GetCharPressed();
            while (key > 0)
            {
                if ((key >= 32) && (key <= 125) && (text.Length < maxChars - 1))
                {
                    text.Append((char)key);
                }

                key = GetCharPressed();
            }

            if (IsKeyPressed(KeyboardKey.Backspace) && (text.Length > 0))
            {
                text.Remove(text.Length - 1, 1);
            }
        }

        DrawRectangleRec(bounds, Color.RayWhite);
        DrawRectangleLinesEx(bounds, editMode ? 2 : 1, editMode ? Color.Red : (hover ? Color.Blue : Color.Gray));

        string content = text.ToString();
        DrawText(content, (int)bounds.X + 4, (int)(bounds.Y + (bounds.Height - fontSize) / 2), fontSize, Color.DarkGray);

        if (editMode && ((framesCounter / 20) % 2 == 0))
        {
            DrawText("_", (int)bounds.X + 4 + MeasureText(content, fontSize), (int)(bounds.Y + (bounds.Height - fontSize) / 2), fontSize, Color.DarkGray);
        }

        if (hover && IsMouseButtonPressed(MouseButton.Left))
        {
            pressed = true;
        }

        return pressed;
    }

    private static void GuiTextBoxReadOnly(Rectangle bounds, string text, int fontSize)
    {
        DrawRectangleRec(bounds, Color.LightGray);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (text != null)
        {
            DrawText(text, (int)bounds.X + 4, (int)(bounds.Y + (bounds.Height - fontSize) / 2), fontSize, Color.Gray);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - compute hash");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ComputeHash();
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
