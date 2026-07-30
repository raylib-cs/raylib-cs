/*******************************************************************************************
*
*   raylib [core] example - text file loading
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Aanjishnu Bhattacharyya (@NimComPoo-04) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 0 Aanjishnu Bhattacharyya (@NimComPoo-04)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Core;

// NOTE: The upstream C code mutates the raw char buffers returned by LoadTextLines() in place,
// temporarily null-terminating to reuse MeasureText for word wrapping. This C# port keeps the
// same algorithm but works on managed char arrays: LoadFileText + split on '\n' reproduces
// raylib's LoadTextLines('\n') behaviour, and '\n' characters are inserted where lines wrap.
public partial class TextFileLoading : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Text File Loading";

    public string Title => "raylib [core] example - text file loading";

    private Camera2D cam;
    private string fileName;
    private string[] lines;
    private int lineCount;
    private int fontSize;
    private int textTop;
    private int wrapWidth;
    private int textHeight;
    private Rectangle scrollBar;

    public void Init()
    {
        // Setting up the camera
        cam = new Camera2D();
        cam.Offset = new Vector2(0, 0);
        cam.Target = new Vector2(0, 0);
        cam.Rotation = 0;
        cam.Zoom = 1;

        // Loading text file from resources/text_file.txt
        fileName = "resources/text_file.txt";
        string text = LoadFileText(fileName);

        // Loading all the text lines (raylib's LoadTextLines splits on '\n')
        lines = text.Split('\n');
        lineCount = lines.Length;

        // Stylistic choises
        fontSize = 20;
        textTop = 25 + fontSize; // Top of the screen from where the text is rendered
        wrapWidth = screenWidth - 20;

        // Wrap the lines as needed
        for (int i = 0; i < lineCount; i++)
        {
            char[] chars = lines[i].ToCharArray();
            int len = chars.Length;
            int j = 0;
            int lastSpace = 0;          // Keeping track of last valid space to insert '\n'
            int lastWrapStart = 0;      // Keeping track of the start of this wrapped line.

            while (j <= len)
            {
                char cur = (j < len) ? chars[j] : '\0';
                if (cur == ' ' || cur == '\0')
                {
                    // Making a C style string by "cutting" at the required location so that we can use MeasureText
                    string sub = new string(chars, lastWrapStart, j - lastWrapStart);

                    // Checking if the text has crossed the wrapWidth, then going back and inserting a newline
                    if (MeasureText(sub, fontSize) > wrapWidth)
                    {
                        chars[lastSpace] = '\n';

                        // Since we added a newline the place of wrap changed so we update our lastWrapStart
                        lastWrapStart = lastSpace + 1;
                    }

                    lastSpace = j; // Since we encountered a new space we update our last encountered space location
                }

                j++;
            }

            lines[i] = new string(chars);
        }

        // Calculating the total height so that we can show a scrollbar
        textHeight = 0;

        for (int i = 0; i < lineCount; i++)
        {
            Vector2 size = MeasureTextEx(GetFontDefault(), lines[i], (float)fontSize, 2);
            textHeight += (int)size.Y + 10;
        }

        // A simple scrollbar on the side to show how far we have read into the file
        scrollBar = new Rectangle(
            (float)screenWidth - 5,
            0,
            5,
            screenHeight * 100.0f / (textHeight - screenHeight)); // Scrollbar height is just a percentage
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float scroll = GetMouseWheelMove();
        cam.Target.Y -= scroll * fontSize * 1.5f;   // Choosing an arbitrary speed for scroll

        if (cam.Target.Y < 0)
        {
            cam.Target.Y = 0;  // Snapping to 0 if we go too far back
        }

        // Ensuring that the camera does not scroll past all text
        if (cam.Target.Y > textHeight - screenHeight + textTop)
        {
            cam.Target.Y = (float)textHeight - screenHeight + textTop;
        }

        // Computing the position of the scrollBar depending on the percentage of text covered
        scrollBar.Y = Lerp((float)textTop, (float)screenHeight - scrollBar.Height, (float)(cam.Target.Y - textTop) / (textHeight - screenHeight));
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode2D(cam);
        // Going through all the read lines
        for (int i = 0, t = textTop; i < lineCount; i++)
        {
            // Each time we go through and calculate the height of the text to move the cursor appropriately
            Vector2 size;
            if (lines[i] != "")
            {
                size = MeasureTextEx(GetFontDefault(), lines[i], (float)fontSize, 2);
            }
            else
            {
                // Fix for empty line in the text file
                size = MeasureTextEx(GetFontDefault(), " ", (float)fontSize, 2);
            }

            DrawText(lines[i], 10, t, fontSize, Color.Red);

            // Inserting extra space for real newlines,
            // wrapped lines are rendered closer together
            t += (int)size.Y + 10;
        }
        EndMode2D();

        // Header displaying which file is being read currently
        DrawRectangle(0, 0, screenWidth, textTop - 10, Color.Beige);
        DrawText($"File: {fileName}", 10, 10, fontSize, Color.Maroon);

        DrawRectangleRec(scrollBar, Color.Maroon);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - text file loading");

        var game = new TextFileLoading();
        game.Init();

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

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
