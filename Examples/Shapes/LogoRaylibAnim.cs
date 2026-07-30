/*******************************************************************************************
*
*   raylib [shapes] example - logo raylib anim
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class LogoRaylibAnim : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Logo Raylib Anim";

    public string Title => "raylib [shapes] example - logo raylib anim";

    private int logoPositionX;
    private int logoPositionY;

    private int framesCounter;
    private int lettersCount;

    private int topSideRecWidth;
    private int leftSideRecHeight;

    private int bottomSideRecWidth;
    private int rightSideRecHeight;

    private int state;                      // Tracking animation states (State Machine)
    private float alpha;                    // Useful for fading

    private Color outline;

    public void Init()
    {
        logoPositionX = screenWidth / 2 - 128;
        logoPositionY = screenHeight / 2 - 128;

        framesCounter = 0;
        lettersCount = 0;

        topSideRecWidth = 16;
        leftSideRecHeight = 16;

        bottomSideRecWidth = 16;
        rightSideRecHeight = 16;

        state = 0;
        alpha = 1.0f;

        outline = new(139, 71, 135, 255);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (state == 0)                 // State 0: Small box blinking
        {
            framesCounter++;

            if (framesCounter == 120)
            {
                state = 1;
                framesCounter = 0;      // Reset counter... will be used later...
            }
        }
        else if (state == 1)            // State 1: Top and left bars growing
        {
            topSideRecWidth += 4;
            leftSideRecHeight += 4;

            if (topSideRecWidth == 256)
            {
                state = 2;
            }
        }
        else if (state == 2)            // State 2: Bottom and right bars growing
        {
            bottomSideRecWidth += 4;
            rightSideRecHeight += 4;

            if (bottomSideRecWidth == 256)
            {
                state = 3;
            }
        }
        else if (state == 3)            // State 3: Letters appearing (one by one)
        {
            framesCounter++;

            // Every 12 frames, one more letter!
            if (framesCounter / 12 != 0)
            {
                lettersCount++;
                framesCounter = 0;
            }

            // When all letters have appeared, just fade out everything
            if (lettersCount >= 10)
            {
                alpha -= 0.02f;

                if (alpha <= 0.0f)
                {
                    alpha = 0.0f;
                    state = 4;
                }
            }
        }
        else if (state == 4)            // State 4: Reset and Replay
        {
            if (IsKeyPressed(KeyboardKey.R))
            {
                framesCounter = 0;
                lettersCount = 0;

                topSideRecWidth = 16;
                leftSideRecHeight = 16;

                bottomSideRecWidth = 16;
                rightSideRecHeight = 16;

                alpha = 1.0f;
                state = 0;          // Return to State 0
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        if (state == 0)
        {
            if ((framesCounter / 15) % 2 != 0)
            {
                DrawRectangle(logoPositionX, logoPositionY, 16, 16, outline);
            }
        }
        else if (state == 1)
        {
            DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, outline);
            DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, outline);
        }
        else if (state == 2)
        {
            DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, outline);
            DrawRectangle(logoPositionX, logoPositionY, 16, leftSideRecHeight, outline);

            DrawRectangle(logoPositionX + 240, logoPositionY, 16, rightSideRecHeight, outline);
            DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, outline);
        }
        else if (state == 3)
        {
            var outlineFade = Fade(outline, alpha);
            DrawRectangle(logoPositionX, logoPositionY, topSideRecWidth, 16, outlineFade);
            DrawRectangle(logoPositionX, logoPositionY + 16, 16, leftSideRecHeight - 32, outlineFade);

            DrawRectangle(logoPositionX + 240, logoPositionY + 16, 16, rightSideRecHeight - 32, outlineFade);
            DrawRectangle(logoPositionX, logoPositionY + 240, bottomSideRecWidth, 16, outlineFade);

            var whiteFade = Fade(Color.RayWhite, alpha);
            DrawRectangle(screenWidth / 2 - 112, screenHeight / 2 - 112, 224, 224, whiteFade);

            var label = Fade(new Color(155, 79, 151, 255), alpha);
            var text = "raylib".SubText(0, lettersCount);
            DrawText(text, screenWidth / 2 - 44, screenHeight / 2 + 28, 50, label);

            DrawText("cs".SubText(0, lettersCount), screenWidth / 2 - 44, screenHeight / 2 + 58, 50, label);
        }
        else if (state == 4)
        {
            DrawText("[R] REPLAY", 340, 200, 20, Color.Gray);
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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - logo raylib anim");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LogoRaylibAnim();
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
