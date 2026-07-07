/*******************************************************************************************
*
*   raylib [core] example - keyboard testbed
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: raylib defined keys refer to ENG-US Keyboard layout,
*   mapping to other layouts is up to the user
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class KeyboardTestbed : IExample
{
    private const int KeyRecSpacing = 4;    // Space in pixels between key rectangles

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Keyboard Testbed";

    public string Title => "raylib [core] example - keyboard testbed";

    private int[] line01KeyWidths;
    private int[] line01Keys;
    private int[] line02KeyWidths;
    private int[] line02Keys;
    private int[] line03KeyWidths;
    private int[] line03Keys;
    private int[] line04KeyWidths;
    private int[] line04Keys;
    private int[] line05KeyWidths;
    private int[] line05Keys;
    private int[] line06KeyWidths;
    private int[] line06Keys;

    private Vector2 keyboardOffset;

    public void Init()
    {
        SetExitKey(KeyboardKey.Null); // Avoid exit on KEY_ESCAPE

        // Keyboard line 01
        line01KeyWidths = new int[15];
        for (int i = 0; i < 15; i++) line01KeyWidths[i] = 45;
        line01KeyWidths[13] = 62;   // PRINTSCREEN
        line01Keys = new int[]
        {
            (int)KeyboardKey.Escape, (int)KeyboardKey.F1, (int)KeyboardKey.F2, (int)KeyboardKey.F3, (int)KeyboardKey.F4, (int)KeyboardKey.F5,
            (int)KeyboardKey.F6, (int)KeyboardKey.F7, (int)KeyboardKey.F8, (int)KeyboardKey.F9, (int)KeyboardKey.F10, (int)KeyboardKey.F11,
            (int)KeyboardKey.F12, (int)KeyboardKey.PrintScreen, (int)KeyboardKey.Pause
        };

        // Keyboard line 02
        line02KeyWidths = new int[15];
        for (int i = 0; i < 15; i++) line02KeyWidths[i] = 45;
        line02KeyWidths[0] = 25;    // GRAVE
        line02KeyWidths[13] = 82;   // BACKSPACE
        line02Keys = new int[]
        {
            (int)KeyboardKey.Grave, (int)KeyboardKey.One, (int)KeyboardKey.Two, (int)KeyboardKey.Three, (int)KeyboardKey.Four,
            (int)KeyboardKey.Five, (int)KeyboardKey.Six, (int)KeyboardKey.Seven, (int)KeyboardKey.Eight, (int)KeyboardKey.Nine,
            (int)KeyboardKey.Zero, (int)KeyboardKey.Minus, (int)KeyboardKey.Equal, (int)KeyboardKey.Backspace, (int)KeyboardKey.Delete
        };

        // Keyboard line 03
        line03KeyWidths = new int[15];
        for (int i = 0; i < 15; i++) line03KeyWidths[i] = 45;
        line03KeyWidths[0] = 50;    // TAB
        line03KeyWidths[13] = 57;   // BACKSLASH
        line03Keys = new int[]
        {
            (int)KeyboardKey.Tab, (int)KeyboardKey.Q, (int)KeyboardKey.W, (int)KeyboardKey.E, (int)KeyboardKey.R, (int)KeyboardKey.T, (int)KeyboardKey.Y,
            (int)KeyboardKey.U, (int)KeyboardKey.I, (int)KeyboardKey.O, (int)KeyboardKey.P, (int)KeyboardKey.LeftBracket,
            (int)KeyboardKey.RightBracket, (int)KeyboardKey.Backslash, (int)KeyboardKey.Insert
        };

        // Keyboard line 04
        line04KeyWidths = new int[14];
        for (int i = 0; i < 14; i++) line04KeyWidths[i] = 45;
        line04KeyWidths[0] = 68;    // CAPS
        line04KeyWidths[12] = 88;   // ENTER
        line04Keys = new int[]
        {
            (int)KeyboardKey.CapsLock, (int)KeyboardKey.A, (int)KeyboardKey.S, (int)KeyboardKey.D, (int)KeyboardKey.F, (int)KeyboardKey.G,
            (int)KeyboardKey.H, (int)KeyboardKey.J, (int)KeyboardKey.K, (int)KeyboardKey.L, (int)KeyboardKey.Semicolon,
            (int)KeyboardKey.Apostrophe, (int)KeyboardKey.Enter, (int)KeyboardKey.PageUp
        };

        // Keyboard line 05
        line05KeyWidths = new int[14];
        for (int i = 0; i < 14; i++) line05KeyWidths[i] = 45;
        line05KeyWidths[0] = 80;    // LSHIFT
        line05KeyWidths[11] = 76;   // RSHIFT
        line05Keys = new int[]
        {
            (int)KeyboardKey.LeftShift, (int)KeyboardKey.Z, (int)KeyboardKey.X, (int)KeyboardKey.C, (int)KeyboardKey.V, (int)KeyboardKey.B,
            (int)KeyboardKey.N, (int)KeyboardKey.M, (int)KeyboardKey.Comma, (int)KeyboardKey.Period, /*KEY_MINUS*/
            (int)KeyboardKey.Slash, (int)KeyboardKey.RightShift, (int)KeyboardKey.Up, (int)KeyboardKey.PageDown
        };

        // Keyboard line 06
        line06KeyWidths = new int[11];
        for (int i = 0; i < 11; i++) line06KeyWidths[i] = 45;
        line06KeyWidths[0] = 80;    // LCTRL
        line06KeyWidths[3] = 208;   // SPACE
        line06KeyWidths[7] = 60;    // RCTRL
        line06Keys = new int[]
        {
            (int)KeyboardKey.LeftControl, (int)KeyboardKey.LeftSuper, (int)KeyboardKey.LeftAlt,
            (int)KeyboardKey.Space, (int)KeyboardKey.RightAlt, 162, (int)KeyboardKey.Null,
            (int)KeyboardKey.RightControl, (int)KeyboardKey.Left, (int)KeyboardKey.Down, (int)KeyboardKey.Right
        };

        keyboardOffset = new Vector2(26, 80);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        int key = GetKeyPressed(); // Get pressed keycode
        if (key > 0) TraceLog(TraceLogLevel.Info, $"KEYBOARD TESTBED: KEY PRESSED:    {key}");

        int ch = GetCharPressed(); // Get pressed char for text input, using OS mapping
        if (ch > 0) TraceLog(TraceLogLevel.Info, $"KEYBOARD TESTBED: CHAR PRESSED:   {(char)ch} ({ch})");
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText("KEYBOARD LAYOUT: ENG-US", 26, 38, 20, Color.LightGray);

        // Keyboard line 01 - 15 keys
        // ESC, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, IMP, CLOSE
        for (int i = 0, recOffsetX = 0; i < 15; i++)
        {
            GuiKeyboardKey(new Rectangle(keyboardOffset.X + recOffsetX, keyboardOffset.Y, (float)line01KeyWidths[i], 30.0f), line01Keys[i]);
            recOffsetX += line01KeyWidths[i] + KeyRecSpacing;
        }

        // Keyboard line 02 - 15 keys
        // `, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, -, =, BACKSPACE, DEL
        for (int i = 0, recOffsetX = 0; i < 15; i++)
        {
            GuiKeyboardKey(new Rectangle(keyboardOffset.X + recOffsetX, keyboardOffset.Y + 30 + KeyRecSpacing, (float)line02KeyWidths[i], 38.0f), line02Keys[i]);
            recOffsetX += line02KeyWidths[i] + KeyRecSpacing;
        }

        // Keyboard line 03 - 15 keys
        // TAB, Q, W, E, R, T, Y, U, I, O, P, [, ], \, INS
        for (int i = 0, recOffsetX = 0; i < 15; i++)
        {
            GuiKeyboardKey(new Rectangle(keyboardOffset.X + recOffsetX, keyboardOffset.Y + 30 + 38 + KeyRecSpacing * 2, (float)line03KeyWidths[i], 38.0f), line03Keys[i]);
            recOffsetX += line03KeyWidths[i] + KeyRecSpacing;
        }

        // Keyboard line 04 - 14 keys
        // MAYUS, A, S, D, F, G, H, J, K, L, ;, ', ENTER, REPAG
        for (int i = 0, recOffsetX = 0; i < 14; i++)
        {
            GuiKeyboardKey(new Rectangle(keyboardOffset.X + recOffsetX, keyboardOffset.Y + 30 + 38 * 2 + KeyRecSpacing * 3, (float)line04KeyWidths[i], 38.0f), line04Keys[i]);
            recOffsetX += line04KeyWidths[i] + KeyRecSpacing;
        }

        // Keyboard line 05 - 14 keys
        // LSHIFT, Z, X, C, V, B, N, M, ,, ., /, RSHIFT, UP, AVPAG
        for (int i = 0, recOffsetX = 0; i < 14; i++)
        {
            GuiKeyboardKey(new Rectangle(keyboardOffset.X + recOffsetX, keyboardOffset.Y + 30 + 38 * 3 + KeyRecSpacing * 4, (float)line05KeyWidths[i], 38.0f), line05Keys[i]);
            recOffsetX += line05KeyWidths[i] + KeyRecSpacing;
        }

        // Keyboard line 06 - 11 keys
        // LCTRL, WIN, LALT, SPACE, ALTGR, \, FN, RCTRL, LEFT, DOWN, RIGHT
        for (int i = 0, recOffsetX = 0; i < 11; i++)
        {
            GuiKeyboardKey(new Rectangle(keyboardOffset.X + recOffsetX, keyboardOffset.Y + 30 + 38 * 4 + KeyRecSpacing * 5, (float)line06KeyWidths[i], 38.0f), line06Keys[i]);
            recOffsetX += line06KeyWidths[i] + KeyRecSpacing;
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //------------------------------------------------------------------------------------
    // Module Functions Definition
    //------------------------------------------------------------------------------------
    // Get keyboard keycode as text (US keyboard)
    // NOTE: Mapping for other keyboard layouts can be done here
    private static string GetKeyText(int key)
    {
        switch ((KeyboardKey)key)
        {
            case KeyboardKey.Apostrophe: return "'";          // Key: '
            case KeyboardKey.Comma: return ",";          // Key: ,
            case KeyboardKey.Minus: return "-";          // Key: -
            case KeyboardKey.Period: return ".";          // Key: .
            case KeyboardKey.Slash: return "/";          // Key: /
            case KeyboardKey.Zero: return "0";          // Key: 0
            case KeyboardKey.One: return "1";          // Key: 1
            case KeyboardKey.Two: return "2";          // Key: 2
            case KeyboardKey.Three: return "3";          // Key: 3
            case KeyboardKey.Four: return "4";          // Key: 4
            case KeyboardKey.Five: return "5";          // Key: 5
            case KeyboardKey.Six: return "6";          // Key: 6
            case KeyboardKey.Seven: return "7";          // Key: 7
            case KeyboardKey.Eight: return "8";          // Key: 8
            case KeyboardKey.Nine: return "9";          // Key: 9
            case KeyboardKey.Semicolon: return ";";          // Key: ;
            case KeyboardKey.Equal: return "=";          // Key: =
            case KeyboardKey.A: return "A";          // Key: A | a
            case KeyboardKey.B: return "B";          // Key: B | b
            case KeyboardKey.C: return "C";          // Key: C | c
            case KeyboardKey.D: return "D";          // Key: D | d
            case KeyboardKey.E: return "E";          // Key: E | e
            case KeyboardKey.F: return "F";          // Key: F | f
            case KeyboardKey.G: return "G";          // Key: G | g
            case KeyboardKey.H: return "H";          // Key: H | h
            case KeyboardKey.I: return "I";          // Key: I | i
            case KeyboardKey.J: return "J";          // Key: J | j
            case KeyboardKey.K: return "K";          // Key: K | k
            case KeyboardKey.L: return "L";          // Key: L | l
            case KeyboardKey.M: return "M";          // Key: M | m
            case KeyboardKey.N: return "N";          // Key: N | n
            case KeyboardKey.O: return "O";          // Key: O | o
            case KeyboardKey.P: return "P";          // Key: P | p
            case KeyboardKey.Q: return "Q";          // Key: Q | q
            case KeyboardKey.R: return "R";          // Key: R | r
            case KeyboardKey.S: return "S";          // Key: S | s
            case KeyboardKey.T: return "T";          // Key: T | t
            case KeyboardKey.U: return "U";          // Key: U | u
            case KeyboardKey.V: return "V";          // Key: V | v
            case KeyboardKey.W: return "W";          // Key: W | w
            case KeyboardKey.X: return "X";          // Key: X | x
            case KeyboardKey.Y: return "Y";          // Key: Y | y
            case KeyboardKey.Z: return "Z";          // Key: Z | z
            case KeyboardKey.LeftBracket: return "[";          // Key: [
            case KeyboardKey.Backslash: return "\\";         // Key: '\'
            case KeyboardKey.RightBracket: return "]";          // Key: ]
            case KeyboardKey.Grave: return "`";          // Key: `
            case KeyboardKey.Space: return "SPACE";      // Key: Space
            case KeyboardKey.Escape: return "ESC";        // Key: Esc
            case KeyboardKey.Enter: return "ENTER";      // Key: Enter
            case KeyboardKey.Tab: return "TAB";        // Key: Tab
            case KeyboardKey.Backspace: return "BACK";       // Key: Backspace
            case KeyboardKey.Insert: return "INS";        // Key: Ins
            case KeyboardKey.Delete: return "DEL";        // Key: Del
            case KeyboardKey.Right: return "RIGHT";      // Key: Cursor right
            case KeyboardKey.Left: return "LEFT";       // Key: Cursor left
            case KeyboardKey.Down: return "DOWN";       // Key: Cursor down
            case KeyboardKey.Up: return "UP";         // Key: Cursor up
            case KeyboardKey.PageUp: return "PGUP";       // Key: Page up
            case KeyboardKey.PageDown: return "PGDOWN";     // Key: Page down
            case KeyboardKey.Home: return "HOME";       // Key: Home
            case KeyboardKey.End: return "END";        // Key: End
            case KeyboardKey.CapsLock: return "CAPS";       // Key: Caps lock
            case KeyboardKey.ScrollLock: return "LOCK";       // Key: Scroll down
            case KeyboardKey.NumLock: return "NUMLOCK";    // Key: Num lock
            case KeyboardKey.PrintScreen: return "PRINTSCR";   // Key: Print screen
            case KeyboardKey.Pause: return "PAUSE";      // Key: Pause
            case KeyboardKey.F1: return "F1";         // Key: F1
            case KeyboardKey.F2: return "F2";         // Key: F2
            case KeyboardKey.F3: return "F3";         // Key: F3
            case KeyboardKey.F4: return "F4";         // Key: F4
            case KeyboardKey.F5: return "F5";         // Key: F5
            case KeyboardKey.F6: return "F6";         // Key: F6
            case KeyboardKey.F7: return "F7";         // Key: F7
            case KeyboardKey.F8: return "F8";         // Key: F8
            case KeyboardKey.F9: return "F9";         // Key: F9
            case KeyboardKey.F10: return "F10";        // Key: F10
            case KeyboardKey.F11: return "F11";        // Key: F11
            case KeyboardKey.F12: return "F12";        // Key: F12
            case KeyboardKey.LeftShift: return "LSHIFT";     // Key: Shift left
            case KeyboardKey.LeftControl: return "LCTRL";      // Key: Control left
            case KeyboardKey.LeftAlt: return "LALT";       // Key: Alt left
            case KeyboardKey.LeftSuper: return "WIN";        // Key: Super left
            case KeyboardKey.RightShift: return "RSHIFT";     // Key: Shift right
            case KeyboardKey.RightControl: return "RCTRL";      // Key: Control right
            case KeyboardKey.RightAlt: return "ALTGR";      // Key: Alt right
            case KeyboardKey.RightSuper: return "RSUPER";     // Key: Super right
            case KeyboardKey.KeyboardMenu: return "KBMENU";     // Key: KB menu
            case KeyboardKey.Kp0: return "KP0";        // Key: Keypad 0
            case KeyboardKey.Kp1: return "KP1";        // Key: Keypad 1
            case KeyboardKey.Kp2: return "KP2";        // Key: Keypad 2
            case KeyboardKey.Kp3: return "KP3";        // Key: Keypad 3
            case KeyboardKey.Kp4: return "KP4";        // Key: Keypad 4
            case KeyboardKey.Kp5: return "KP5";        // Key: Keypad 5
            case KeyboardKey.Kp6: return "KP6";        // Key: Keypad 6
            case KeyboardKey.Kp7: return "KP7";        // Key: Keypad 7
            case KeyboardKey.Kp8: return "KP8";        // Key: Keypad 8
            case KeyboardKey.Kp9: return "KP9";        // Key: Keypad 9
            case KeyboardKey.KpDecimal: return "KPDEC";      // Key: Keypad .
            case KeyboardKey.KpDivide: return "KPDIV";      // Key: Keypad /
            case KeyboardKey.KpMultiply: return "KPMUL";      // Key: Keypad *
            case KeyboardKey.KpSubtract: return "KPSUB";      // Key: Keypad -
            case KeyboardKey.KpAdd: return "KPADD";      // Key: Keypad +
            case KeyboardKey.KpEnter: return "KPENTER";    // Key: Keypad Enter
            case KeyboardKey.KpEqual: return "KPEQU";      // Key: Keypad =
            default: return "";
        }
    }

    // Draw keyboard key
    private static void GuiKeyboardKey(Rectangle bounds, int key)
    {
        if (key == (int)KeyboardKey.Null) DrawRectangleLinesEx(bounds, 2.0f, Color.LightGray);
        else
        {
            if (IsKeyDown((KeyboardKey)key))
            {
                DrawRectangleLinesEx(bounds, 2.0f, Color.Maroon);
                DrawText(GetKeyText(key), (int)(bounds.X + 4), (int)(bounds.Y + 4), 10, Color.Maroon);
            }
            else
            {
                DrawRectangleLinesEx(bounds, 2.0f, Color.DarkGray);
                DrawText(GetKeyText(key), (int)(bounds.X + 4), (int)(bounds.Y + 4), 10, Color.DarkGray);
            }
        }

        if (CheckCollisionPointRec(GetMousePosition(), bounds))
        {
            DrawRectangleRec(bounds, Fade(Color.Red, 0.2f));
            DrawRectangleLinesEx(bounds, 3.0f, Color.Red);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - keyboard testbed");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new KeyboardTestbed();
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
