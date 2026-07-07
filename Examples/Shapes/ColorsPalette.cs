/*******************************************************************************************
*
*   raylib [shapes] example - colors palette
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 2.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class ColorsPalette : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int MaxColorsCount = 21;   // Number of colors available

    public string Name => "Shapes / Colors Palette";

    public string Title => "raylib [shapes] example - colors palette";

    private Color[] colors;
    private string[] colorNames;
    private Rectangle[] colorsRecs;
    private int[] colorState;
    private Vector2 mousePoint;

    public void Init()
    {
        colors = new[]
        {
            Color.DarkGray,
            Color.Maroon,
            Color.Orange,
            Color.DarkGreen,
            Color.DarkBlue,
            Color.DarkPurple,
            Color.DarkBrown,
            Color.Gray,
            Color.Red,
            Color.Gold,
            Color.Lime,
            Color.Blue,
            Color.Violet,
            Color.Brown,
            Color.LightGray,
            Color.Pink,
            Color.Yellow,
            Color.Green,
            Color.SkyBlue,
            Color.Purple,
            Color.Beige
        };

        colorNames = new[]
        {
            "DARKGRAY",
            "MAROON",
            "ORANGE",
            "DARKGREEN",
            "DARKBLUE",
            "DARKPURPLE",
            "DARKBROWN",
            "GRAY",
            "RED",
            "GOLD",
            "LIME",
            "BLUE",
            "VIOLET",
            "BROWN",
            "LIGHTGRAY",
            "PINK",
            "YELLOW",
            "GREEN",
            "SKYBLUE",
            "PURPLE",
            "BEIGE"
        };

        // Rectangles array
        colorsRecs = new Rectangle[colors.Length];

        // Fills colorsRecs data (for every rectangle)
        for (var i = 0; i < colorsRecs.Length; i++)
        {
            colorsRecs[i].X = 20 + 100 * (i % 7) + 10 * (i % 7);
            colorsRecs[i].Y = 80 + 100 * (i / 7) + 10 * (i / 7);
            colorsRecs[i].Width = 100;
            colorsRecs[i].Height = 100;
        }

        // Color state: 0-DEFAULT, 1-MOUSE_HOVER
        colorState = new int[colors.Length];

        mousePoint = new(0.0f, 0.0f);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        mousePoint = GetMousePosition();

        for (var i = 0; i < colors.Length; i++)
        {
            if (CheckCollisionPointRec(mousePoint, colorsRecs[i]))
            {
                colorState[i] = 1;
            }
            else
            {
                colorState[i] = 0;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("raylib colors palette", 28, 42, 20, Color.Black);
        DrawText(
            "press SPACE to see all colors",
            GetScreenWidth() - 180,
            GetScreenHeight() - 40,
            10,
            Color.Gray
        );

        for (var i = 0; i < colorsRecs.Length; i++)    // Draw all rectangles
        {
            DrawRectangleRec(colorsRecs[i], Fade(colors[i], colorState[i] != 0 ? 0.6f : 1.0f));

            if (IsKeyDown(KeyboardKey.Space) || colorState[i] != 0)
            {
                DrawRectangle(
                    (int)colorsRecs[i].X,
                    (int)(colorsRecs[i].Y + colorsRecs[i].Height - 26),
                    (int)colorsRecs[i].Width,
                    20,
                    Color.Black
                );
                DrawRectangleLinesEx(colorsRecs[i], 6, Fade(Color.Black, 0.3f));
                DrawText(
                    colorNames[i],
                    (int)(colorsRecs[i].X + colorsRecs[i].Width - MeasureText(colorNames[i], 10) - 12),
                    (int)(colorsRecs[i].Y + colorsRecs[i].Height - 20),
                    10,
                    colors[i]
                );
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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - colors palette");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ColorsPalette();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
