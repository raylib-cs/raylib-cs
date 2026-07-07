/*******************************************************************************************
*
*   raylib [shapes] example - ring drawing
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

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class DrawRing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Draw Ring";

    public string Title => "raylib [shapes] example - ring drawing";

    private Vector2 center;

    private float innerRadius;
    private float outerRadius;

    private float startAngle;
    private float endAngle;
    private float segments;

    private bool drawRing;
    private bool drawRingLines;
    private bool drawCircleLines;

    public void Init()
    {
        center = new((GetScreenWidth() - 300) / 2.0f, GetScreenHeight() / 2.0f);

        innerRadius = 80.0f;
        outerRadius = 190.0f;

        startAngle = 0.0f;
        endAngle = 360.0f;
        segments = 0.0f;

        drawRing = true;
        drawRingLines = false;
        drawCircleLines = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // NOTE: All variables update happens inside GUI control functions
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawLine(500, 0, 500, GetScreenHeight(), Fade(Color.LightGray, 0.6f));
        DrawRectangle(500, 0, GetScreenWidth() - 500, GetScreenHeight(), Fade(Color.LightGray, 0.3f));

        if (drawRing)
        {
            DrawRing(
                center,
                innerRadius,
                outerRadius,
                startAngle,
                endAngle,
                (int)segments,
                Fade(Color.Maroon, 0.3f)
            );
        }
        if (drawRingLines)
        {
            DrawRingLines(
                center,
                innerRadius,
                outerRadius,
                startAngle,
                endAngle,
                (int)segments,
                Fade(Color.Black, 0.4f)
            );
        }
        if (drawCircleLines)
        {
            DrawCircleSectorLines(
                center,
                outerRadius,
                startAngle,
                endAngle,
                (int)segments,
                Fade(Color.Black, 0.4f)
            );
        }

        // Draw GUI controls
        //------------------------------------------------------------------------------
        GuiSliderBar(new Rectangle(600, 40, 120, 20), "StartAngle", $"{startAngle:F2}", ref startAngle, -450, 450);
        GuiSliderBar(new Rectangle(600, 70, 120, 20), "EndAngle", $"{endAngle:F2}", ref endAngle, -450, 450);

        GuiSliderBar(new Rectangle(600, 140, 120, 20), "InnerRadius", $"{innerRadius:F2}", ref innerRadius, 0, 100);
        GuiSliderBar(new Rectangle(600, 170, 120, 20), "OuterRadius", $"{outerRadius:F2}", ref outerRadius, 0, 200);

        GuiSliderBar(new Rectangle(600, 240, 120, 20), "Segments", $"{segments:F2}", ref segments, 0, 100);

        GuiCheckBox(new Rectangle(600, 320, 20, 20), "Draw Ring", ref drawRing);
        GuiCheckBox(new Rectangle(600, 350, 20, 20), "Draw RingLines", ref drawRingLines);
        GuiCheckBox(new Rectangle(600, 380, 20, 20), "Draw CircleLines", ref drawCircleLines);
        //------------------------------------------------------------------------------

        var minSegments = (int)MathF.Ceiling((endAngle - startAngle) / 90);
        var color = (segments >= minSegments) ? Color.Maroon : Color.DarkGray;
        DrawText($"MODE: {((segments >= minSegments) ? "MANUAL" : "AUTO")}", 600, 270, 10, color);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiSliderBar(Rectangle bounds, string textLeft, string textRight, ref float value, float minValue, float maxValue)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (hover && IsMouseButtonDown(MouseButton.Left))
        {
            value = minValue + ((mouse.X - bounds.X) / bounds.Width) * (maxValue - minValue);
            if (value < minValue) value = minValue;
            if (value > maxValue) value = maxValue;
        }

        DrawRectangleRec(bounds, Color.LightGray);
        float pct = (value - minValue) / (maxValue - minValue);
        DrawRectangleRec(new Rectangle(bounds.X, bounds.Y, bounds.Width * pct, bounds.Height), Color.SkyBlue);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (!string.IsNullOrEmpty(textLeft)) DrawText(textLeft, (int)bounds.X - MeasureText(textLeft, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        if (!string.IsNullOrEmpty(textRight)) DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    private static void GuiCheckBox(Rectangle bounds, string text, ref bool active)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (hover && IsMouseButtonPressed(MouseButton.Left)) active = !active;

        DrawRectangleLinesEx(bounds, 1, hover ? Color.DarkBlue : Color.Gray);
        if (active) DrawRectangle((int)bounds.X + 4, (int)bounds.Y + 4, (int)bounds.Width - 8, (int)bounds.Height - 8, Color.DarkGray);
        if (text != null) DrawText(text, (int)(bounds.X + bounds.Width + 8), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - ring drawing");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DrawRing();
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
