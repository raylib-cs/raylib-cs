/*******************************************************************************************
*
*   raylib [shapes] example - kaleidoscope
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Hugo ARNAL (@hugoarnal) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Hugo ARNAL (@hugoarnal) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Shapes;

public partial class Kaleidoscope : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_DRAW_LINES = 8192;

    // Line data type
    private struct Line
    {
        public Vector2 Start;
        public Vector2 End;
    }

    public string Name => "Shapes / Kaleidoscope";

    public string Title => "raylib [shapes] example - kaleidoscope";

    public int TargetFps => 20;

    // Lines array as a global static variable to be stored
    // in heap and avoid potential stack overflow (on Web platform)
    private Line[] lines;

    // Line drawing properties
    private int symmetry;
    private float angle;
    private float thickness;
    private Rectangle resetButtonRec;
    private Rectangle backButtonRec;
    private Rectangle nextButtonRec;
    private Vector2 mousePos;
    private Vector2 prevMousePos;
    private Vector2 scaleVector;
    private Vector2 offset;

    private Camera2D camera;

    private int currentLineCounter;
    private int totalLineCounter;
    private bool resetButtonClicked;
    private bool backButtonClicked;
    private bool nextButtonClicked;

    public void Init()
    {
        lines = new Line[MAX_DRAW_LINES];

        // Line drawing properties
        symmetry = 6;
        angle = 360.0f / (float)symmetry;
        thickness = 3.0f;
        resetButtonRec = new(screenWidth - 55.0f, 5.0f, 50, 25);
        backButtonRec = new(screenWidth - 55.0f, screenHeight - 30.0f, 25, 25);
        nextButtonRec = new(screenWidth - 30.0f, screenHeight - 30.0f, 25, 25);
        mousePos = new(0, 0);
        prevMousePos = new(0, 0);
        scaleVector = new(1.0f, -1.0f);
        offset = new((float)screenWidth / 2.0f, (float)screenHeight / 2.0f);

        camera = new();
        camera.Target = new(0, 0);
        camera.Offset = offset;
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;

        currentLineCounter = 0;
        totalLineCounter = 0;
        resetButtonClicked = false;
        backButtonClicked = false;
        nextButtonClicked = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        prevMousePos = mousePos;
        mousePos = GetMousePosition();

        Vector2 lineStart = Vector2Subtract(mousePos, offset);
        Vector2 lineEnd = Vector2Subtract(prevMousePos, offset);

        if (
            IsMouseButtonDown(MouseButton.Left)
            && !CheckCollisionPointRec(mousePos, resetButtonRec)
            && !CheckCollisionPointRec(mousePos, backButtonRec)
            && !CheckCollisionPointRec(mousePos, nextButtonRec)
        )
        {
            for (int s = 0; (s < symmetry) && (totalLineCounter < (MAX_DRAW_LINES - 1)); s++)
            {
                lineStart = Vector2Rotate(lineStart, angle * DEG2RAD);
                lineEnd = Vector2Rotate(lineEnd, angle * DEG2RAD);

                // Store mouse line
                lines[totalLineCounter].Start = lineStart;
                lines[totalLineCounter].End = lineEnd;

                // Store reflective line
                lines[totalLineCounter + 1].Start = Vector2Multiply(lineStart, scaleVector);
                lines[totalLineCounter + 1].End = Vector2Multiply(lineEnd, scaleVector);

                totalLineCounter += 2;
                currentLineCounter = totalLineCounter;
            }
        }

        if (resetButtonClicked)
        {
            Array.Clear(lines, 0, MAX_DRAW_LINES);
            currentLineCounter = 0;
            totalLineCounter = 0;
        }

        if (backButtonClicked && (currentLineCounter > 0))
        {
            currentLineCounter -= 1;
        }

        if (nextButtonClicked && (currentLineCounter < MAX_DRAW_LINES) && ((currentLineCounter + 1) <= totalLineCounter))
        {
            currentLineCounter += 1;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);
        BeginMode2D(camera);

        for (int s = 0; s < symmetry; s++)
        {
            for (int i = 0; i < currentLineCounter; i += 2)
            {
                DrawLineEx(lines[i].Start, lines[i].End, thickness, Color.Black);
                DrawLineEx(lines[i + 1].Start, lines[i + 1].End, thickness, Color.Black);
            }
        }

        EndMode2D();

        // NOTE: raygui is not bound in raylib-cs, so the on-screen back/next/reset
        // controls are unavailable; drawing with the mouse still works.
        //------------------------------------------------------------------------------
        /*
        if ((currentLineCounter - 1) < 0) GuiDisable();

        backButtonClicked = GuiButton(backButtonRec, "<");
        GuiEnable();

        if ((currentLineCounter + 1) > totalLineCounter) GuiDisable();

        nextButtonClicked = GuiButton(nextButtonRec, ">");
        GuiEnable();
        resetButtonClicked = GuiButton(resetButtonRec, "Reset");
        */
        //------------------------------------------------------------------------------

        DrawText($"LINES: {currentLineCounter}/{MAX_DRAW_LINES}", 10, screenHeight - 30, 20, Color.Maroon);
        DrawFPS(10, 10);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - kaleidoscope");

        SetTargetFPS(20);
        //--------------------------------------------------------------------------------------

        var game = new Kaleidoscope();
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
