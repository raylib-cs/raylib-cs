/*******************************************************************************************
*
*   raylib [shapes] example - dashed line
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Luís Almeida (@luis605)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Luís Almeida (@luis605)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class DashedLine : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Dashed Line";

    public string Title => "raylib [shapes] example - dashed line";

    // Line Properties
    private Vector2 lineStartPosition;
    private Vector2 lineEndPosition;
    private float dashLength;
    private float blankLength;

    // Color selection
    private Color[] lineColors;
    private int colorIndex;

    public void Init()
    {
        // Line Properties
        lineStartPosition = new Vector2(20.0f, 50.0f);
        lineEndPosition = new Vector2(780.0f, 400.0f);
        dashLength = 25.0f;
        blankLength = 15.0f;

        // Color selection
        lineColors = new[] { Color.Red, Color.Orange, Color.Gold, Color.Green, Color.Blue, Color.Violet, Color.Pink, Color.Black };
        colorIndex = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        lineEndPosition = GetMousePosition(); // Line endpoint follows the mouse

        // Change Dash Length (UP/DOWN arrows)
        if (IsKeyDown(KeyboardKey.Up)) dashLength += 1.0f;
        if (IsKeyDown(KeyboardKey.Down) && dashLength > 1.0f) dashLength -= 1.0f;

        // Change Space Length (LEFT/RIGHT arrows)
        if (IsKeyDown(KeyboardKey.Right)) blankLength += 1.0f;
        if (IsKeyDown(KeyboardKey.Left) && blankLength > 1.0f) blankLength -= 1.0f;

        // Cycle through colors ('C' key)
        if (IsKeyPressed(KeyboardKey.C)) colorIndex = (colorIndex + 1)%lineColors.Length;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

            ClearBackground(Color.RayWhite);

            // Draw the dashed line with the current properties
            DrawLineDashed(lineStartPosition, lineEndPosition, (int)dashLength, (int)blankLength, lineColors[colorIndex]);

            // Draw UI and Instructions
            DrawRectangle(5, 5, 265, 95, Fade(Color.SkyBlue, 0.5f));
            DrawRectangleLines(5, 5, 265, 95, Color.Blue);

            DrawText("CONTROLS:", 15, 15, 10, Color.Black);
            DrawText("UP/DOWN: Change Dash Length", 15, 35, 10, Color.Black);
            DrawText("LEFT/RIGHT: Change Space Length", 15, 55, 10, Color.Black);
            DrawText("C: Cycle Color", 15, 75, 10, Color.Black);

            DrawText($"Dash: {dashLength:F0} | Space: {blankLength:F0}", 15, 115, 10, Color.DarkGray);

            DrawFPS(screenWidth - 80, 10);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - dashed line");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DashedLine();
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
