/*******************************************************************************************
*
*   raylib [shapes] example - rounded rectangle drawing
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example contributed by Vlad Adrian (@demizdor) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Vlad Adrian (@demizdor) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class DrawRectangleRounded : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Draw Rectangle Rounded";

    public string Title => "raylib [shapes] example - rounded rectangle drawing";

    private float roundness;
    private float width;
    private float height;
    private float segments;
    private float lineThick;

    private bool drawRect;
    private bool drawRoundedRect;
    private bool drawRoundedLines;

    public void Init()
    {
        roundness = 0.2f;
        width = 200.0f;
        height = 100.0f;
        segments = 0.0f;
        lineThick = 1.0f;

        drawRect = false;
        drawRoundedRect = true;
        drawRoundedLines = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        Rectangle rec = new(
            ((float)GetScreenWidth() - width - 250) / 2,
            (GetScreenHeight() - height) / 2.0f,
            (float)width,
            (float)height
        );
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawLine(560, 0, 560, GetScreenHeight(), Fade(Color.LightGray, 0.6f));
        DrawRectangle(560, 0, GetScreenWidth() - 500, GetScreenHeight(), Fade(Color.LightGray, 0.3f));

        if (drawRect)
        {
            DrawRectangleRec(rec, Fade(Color.Gold, 0.6f));
        }
        if (drawRoundedRect)
        {
            DrawRectangleRounded(rec, roundness, (int)segments, Fade(Color.Maroon, 0.2f));
        }
        if (drawRoundedLines)
        {
            DrawRectangleRoundedLinesEx(rec, roundness, (int)segments, lineThick, Fade(Color.Maroon, 0.4f));
        }

        // Draw GUI controls
        //------------------------------------------------------------------------------
        /*GuiSliderBar(new Rectangle( 640, 40, 105, 20 ), "Width", TextFormat("%.2f", width), ref width, 0, (float)GetScreenWidth() - 300);
        GuiSliderBar(new Rectangle( 640, 70, 105, 20 ), "Height", TextFormat("%.2f", height), ref height, 0, (float)GetScreenHeight() - 50);
        GuiSliderBar(new Rectangle( 640, 140, 105, 20 ), "Roundness", TextFormat("%.2f", roundness), ref roundness, 0.0f, 1.0f);
        GuiSliderBar(new Rectangle( 640, 170, 105, 20 ), "Thickness", TextFormat("%.2f", lineThick), ref lineThick, 0, 20);
        GuiSliderBar(new Rectangle( 640, 240, 105, 20), "Segments", TextFormat("%.2f", segments), ref segments, 0, 60);

        GuiCheckBox(new Rectangle( 640, 320, 20, 20 ), "DrawRoundedRect", ref drawRoundedRect);
        GuiCheckBox(new Rectangle( 640, 350, 20, 20 ), "DrawRoundedLines", ref drawRoundedLines);
        GuiCheckBox(new Rectangle( 640, 380, 20, 20), "DrawRect", ref drawRect);*/
        //------------------------------------------------------------------------------

        var text = $"MODE: {((segments >= 4) ? "MANUAL" : "AUTO")}";
        DrawText(text, 640, 280, 10, (segments >= 4) ? Color.Maroon : Color.DarkGray);
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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - rounded rectangle drawing");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DrawRectangleRounded();
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
