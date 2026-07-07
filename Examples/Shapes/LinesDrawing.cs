/*******************************************************************************************
*
*   raylib [shapes] example - lines drawing
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 5.6
*
*   Example contributed by Robin (@RobinsAviary) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Robin (@RobinsAviary)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Shapes;

public partial class LinesDrawing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Lines Drawing";

    public string Title => "raylib [shapes] example - lines drawing";

    // Hint text that shows before you click the screen
    private bool startText;

    // The mouse's position on the previous frame
    private Vector2 mousePositionPrevious;

    // The canvas to draw lines on
    private RenderTexture2D canvas;

    // The line's thickness
    private float lineThickness;
    // The lines hue (in HSV, from 0-360)
    private float lineHue;

    public void Init()
    {
        // Hint text that shows before you click the screen
        startText = true;

        // The mouse's position on the previous frame
        mousePositionPrevious = GetMousePosition();

        // The canvas to draw lines on
        canvas = LoadRenderTexture(screenWidth, screenHeight);

        // The line's thickness
        lineThickness = 8.0f;
        // The lines hue (in HSV, from 0-360)
        lineHue = 0.0f;

        // Clear the canvas to the background color
        BeginTextureMode(canvas);
            ClearBackground(Color.RayWhite);
        EndTextureMode();
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Disable the hint text once the user clicks
        if (IsMouseButtonPressed(MouseButton.Left) && startText) startText = false;

        // Clear the canvas when the user middle-clicks
        if (IsMouseButtonPressed(MouseButton.Middle))
        {
            BeginTextureMode(canvas);
                ClearBackground(Color.RayWhite);
            EndTextureMode();
        }

        // Store whether the left and right buttons are down
        bool leftButtonDown = IsMouseButtonDown(MouseButton.Left);
        bool rightButtonDown = IsMouseButtonDown(MouseButton.Right);

        if (leftButtonDown || rightButtonDown)
        {
            // The color for the line
            Color drawColor = Color.White;

            if (leftButtonDown)
            {
                // Increase the hue value by the distance our cursor has moved since the last frame (divided by 3)
                lineHue += Vector2Distance(mousePositionPrevious, GetMousePosition())/3.0f;

                // While the hue is >=360, subtract it to bring it down into the range 0-360
                // This is more visually accurate than resetting to zero
                while (lineHue >= 360.0f) lineHue -= 360.0f;

                // Create the final color
                drawColor = ColorFromHSV(lineHue, 1.0f, 1.0f);
            }
            else if (rightButtonDown) drawColor = Color.RayWhite; // Use the background color as an "eraser"

            // Draw the line onto the canvas
            BeginTextureMode(canvas);
                // Circles act as "caps", smoothing corners
                DrawCircleV(mousePositionPrevious, lineThickness/2.0f, drawColor);
                DrawCircleV(GetMousePosition(), lineThickness/2.0f, drawColor);
                DrawLineEx(mousePositionPrevious, GetMousePosition(), lineThickness, drawColor);
            EndTextureMode();
        }

        // Update line thickness based on mousewheel
        lineThickness += GetMouseWheelMove();
        lineThickness = Clamp(lineThickness, 1.0f, 500.0f);

        // Update mouse's previous position
        mousePositionPrevious = GetMousePosition();
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

            // Draw the render texture to the screen, flipped vertically to make it appear top-side up
            DrawTextureRec(canvas.Texture, new Rectangle(0.0f, 0.0f, (float)canvas.Texture.Width, (float)-canvas.Texture.Height), Vector2Zero(), Color.White);

            // Draw the preview circle
            if (!leftButtonDown) DrawCircleLinesV(GetMousePosition(), lineThickness/2.0f, new Color(127, 127, 127, 127));

            // Draw the hint text
            if (startText) DrawText("try clicking and dragging!", 275, 215, 20, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(canvas); // Unload the canvas render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - lines drawing");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new LinesDrawing();
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
