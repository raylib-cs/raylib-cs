/*******************************************************************************************
*
*   raylib [core] example - random sequence
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.0
*
*   Example contributed by Dalton Overmyer (@REDl3east) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Dalton Overmyer (@REDl3east)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Core;

public partial class RandomSequence : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Random Sequence";

    public string Title => "raylib [core] example - random sequence";

    private struct ColorRect
    {
        public Color Color;
        public Rectangle Rect;
    }

    private int rectCount;
    private float rectSize;
    private ColorRect[] rectangles;

    public void Init()
    {
        rectCount = 20;
        rectSize = (float)screenWidth / rectCount;
        rectangles = GenerateRandomColorRectSequence(rectCount, rectSize, screenWidth, 0.75f * screenHeight);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            ShuffleColorRectSequence(rectangles, rectCount);
        }

        if (IsKeyPressed(KeyboardKey.Up))
        {
            rectCount++;
            rectSize = (float)screenWidth / rectCount;

            // Re-generate random sequence with new count
            rectangles = GenerateRandomColorRectSequence(rectCount, rectSize, screenWidth, 0.75f * screenHeight);
        }

        if (IsKeyPressed(KeyboardKey.Down))
        {
            if (rectCount >= 4)
            {
                rectCount--;
                rectSize = (float)screenWidth / rectCount;

                // Re-generate random sequence with new count
                rectangles = GenerateRandomColorRectSequence(rectCount, rectSize, screenWidth, 0.75f * screenHeight);
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (int i = 0; i < rectCount; i++)
        {
            DrawRectangleRec(rectangles[i].Rect, rectangles[i].Color);

            DrawText("Press SPACE to shuffle the current sequence", 10, screenHeight - 96, 20, Color.Black);
            DrawText("Press UP to add a rectangle and generate a new sequence", 10, screenHeight - 64, 20, Color.Black);
            DrawText("Press DOWN to remove a rectangle and generate a new sequence", 10, screenHeight - 32, 20, Color.Black);
        }

        DrawText($"Count: {rectCount} rectangles", 10, 10, 20, Color.Maroon);

        DrawFPS(screenWidth - 80, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    private static Color GenerateRandomColor()
    {
        return new Color(
            GetRandomValue(0, 255),
            GetRandomValue(0, 255),
            GetRandomValue(0, 255),
            255
        );
    }

    private static ColorRect[] GenerateRandomColorRectSequence(float rectCount, float rectWidth, float screenWidth, float screenHeight)
    {
        ColorRect[] rectangles = new ColorRect[(int)rectCount];

        int[] seq = GetRandomSequence((uint)rectCount, 0, (int)rectCount - 1);
        float rectSeqWidth = rectCount * rectWidth;
        float startX = (screenWidth - rectSeqWidth) * 0.5f;

        for (int i = 0; i < rectCount; i++)
        {
            int rectHeight = (int)Remap(seq[i], 0, rectCount - 1, 0, screenHeight);

            rectangles[i].Color = GenerateRandomColor();
            rectangles[i].Rect = new Rectangle(startX + i * rectWidth, screenHeight - rectHeight, rectWidth, rectHeight);
        }

        return rectangles;
    }

    private static void ShuffleColorRectSequence(ColorRect[] rectangles, int rectCount)
    {
        int[] seq = GetRandomSequence((uint)rectCount, 0, rectCount - 1);

        for (int i1 = 0; i1 < rectCount; i1++)
        {
            int i2 = seq[i1];

            // Swap only the color and height
            ColorRect tmp = rectangles[i1];
            rectangles[i1].Color = rectangles[i2].Color;
            rectangles[i1].Rect.Height = rectangles[i2].Rect.Height;
            rectangles[i1].Rect.Y = rectangles[i2].Rect.Y;
            rectangles[i2].Color = tmp.Color;
            rectangles[i2].Rect.Height = tmp.Rect.Height;
            rectangles[i2].Rect.Y = tmp.Rect.Y;
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - random sequence");

        SetTargetFPS(60);

        var game = new RandomSequence();
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
