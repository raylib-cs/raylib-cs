/*******************************************************************************************
*
*   raylib [shapes] example - basic shapes
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class BasicShapes : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Basic Shapes";

    public string Title => "raylib [shapes] example - basic shapes";

    private float rotation;

    public void Init()
    {
        rotation = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        rotation += 0.2f;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("some basic shapes available on raylib", 20, 20, 20, Color.DarkGray);

        // Circle shapes and lines
        DrawCircle(screenWidth / 5, 120, 35, Color.DarkBlue);
        DrawCircleGradient(new Vector2(screenWidth / 5.0f, 220.0f), 60, Color.Green, Color.SkyBlue);
        DrawCircleLines(screenWidth / 5, 340, 80, Color.DarkBlue);
        DrawEllipse(screenWidth / 5, 120, 25, 20, Color.Yellow);
        DrawEllipseLines(screenWidth / 5, 120, 30, 25, Color.Yellow);

        // Rectangle shapes and lines
        DrawRectangle(screenWidth / 4 * 2 - 60, 100, 120, 60, Color.Red);
        DrawRectangleGradientH(screenWidth / 4 * 2 - 90, 170, 180, 130, Color.Maroon, Color.Gold);
        DrawRectangleLines(screenWidth / 4 * 2 - 40, 320, 80, 60, Color.Orange);  // NOTE: Uses QUADS internally, not lines

        // Triangle shapes and lines
        DrawTriangle(
            new Vector2(screenWidth / 4.0f * 3.0f, 80.0f),
            new Vector2(screenWidth / 4.0f * 3.0f - 60.0f, 150.0f),
            new Vector2(screenWidth / 4.0f * 3.0f + 60.0f, 150.0f), Color.Violet
        );

        DrawTriangleLines(
            new Vector2(screenWidth / 4.0f * 3.0f, 160.0f),
            new Vector2(screenWidth / 4.0f * 3.0f - 20.0f, 230.0f),
            new Vector2(screenWidth / 4.0f * 3.0f + 20.0f, 230.0f), Color.DarkBlue
        );

        // Polygon shapes and lines
        DrawPoly(new Vector2(screenWidth / 4.0f * 3, 330), 6, 80, rotation, Color.Brown);
        DrawPolyLines(new Vector2(screenWidth / 4.0f * 3, 330), 6, 90, rotation, Color.Brown);
        DrawPolyLinesEx(new Vector2(screenWidth / 4.0f * 3, 330), 6, 85, rotation, 6, Color.Beige);

        // NOTE: We draw all LINES based shapes together to optimize internal drawing,
        // this way, all LINES are rendered in a single draw pass
        DrawLine(18, 42, screenWidth - 18, 42, Color.Black);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - basic shapes");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new BasicShapes();
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
