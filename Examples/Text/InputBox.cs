/*******************************************************************************************
*
*   raylib [text] example - input box
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.7, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2017-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Text;

public partial class InputBox : IExample
{
    public const int MaxInputChars = 9;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Text / Input Box";

    public string Title => "raylib [text] example - input box";

    private char[] name;
    private int letterCount;

    private Rectangle textBox;
    private bool mouseOnText;

    private int framesCounter;

    public void Init()
    {
        // NOTE: One extra space required for null terminator char '\0'
        name = new char[MaxInputChars + 1];
        letterCount = 0;

        textBox = new(screenWidth / 2 - 100, 180, 225, 50);
        mouseOnText = false;

        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (CheckCollisionPointRec(GetMousePosition(), textBox))
        {
            mouseOnText = true;
        }
        else
        {
            mouseOnText = false;
        }

        if (mouseOnText)
        {
            // Set the window's cursor to the I-Beam
            SetMouseCursor(MouseCursor.IBeam);

            // Get char pressed (unicode character) on the queue
            var key = GetCharPressed();

            // Check if more characters have been pressed on the same frame
            while (key > 0)
            {
                // NOTE: Only allow keys in range [32..125]
                if ((key >= 32) && (key <= 125) && (letterCount < MaxInputChars))
                {
                    name[letterCount] = (char)key;
                    name[letterCount + 1] = '\0'; // Add null terminator at the end of the string
                    letterCount++;
                }

                key = GetCharPressed();  // Check next character in the queue
            }

            if (IsKeyPressed(KeyboardKey.Backspace))
            {
                letterCount -= 1;
                if (letterCount < 0)
                {
                    letterCount = 0;
                }
                name[letterCount] = '\0';
            }
        }
        else
        {
            SetMouseCursor(MouseCursor.Default);
        }

        if (mouseOnText)
        {
            framesCounter += 1;
        }
        else
        {
            framesCounter = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("PLACE MOUSE OVER INPUT BOX!", 240, 140, 20, Color.Gray);
        DrawRectangleRec(textBox, Color.LightGray);

        if (mouseOnText)
        {
            DrawRectangleLines(
                (int)textBox.X,
                (int)textBox.Y,
                (int)textBox.Width,
                (int)textBox.Height,
                Color.Red
            );
        }
        else
        {
            DrawRectangleLines(
                (int)textBox.X,
                (int)textBox.Y,
                (int)textBox.Width,
                (int)textBox.Height,
                Color.DarkGray
            );
        }

        DrawText(new string(name), (int)textBox.X + 5, (int)textBox.Y + 8, 40, Color.Maroon);
        DrawText($"INPUT CHARS: {letterCount}/{MaxInputChars}", 315, 250, 20, Color.DarkGray);

        if (mouseOnText)
        {
            if (letterCount < MaxInputChars)
            {
                // Draw blinking underscore char
                if ((framesCounter / 20 % 2) == 0)
                {
                    DrawText(
                        "_",
                        (int)textBox.X + 8 + MeasureText(new string(name), 40),
                        (int)textBox.Y + 12,
                        40,
                        Color.Maroon
                    );
                }
            }
            else
            {
                DrawText("Press BACKSPACE to delete chars...", 230, 300, 20, Color.Gray);
            }
        }

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
        InitWindow(screenWidth, screenHeight, "raylib [text] example - input box");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new InputBox();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
