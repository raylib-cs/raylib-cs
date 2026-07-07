/*******************************************************************************************
*
*   raylib [shapes] example - splines drawing
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class SplinesDrawing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_SPLINE_POINTS = 32;

    public string Name => "Shapes / Splines Drawing";

    public string Title => "raylib [shapes] example - splines drawing";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    // Cubic Bezier spline control points
    // NOTE: Every segment has two control points
    private struct ControlPoint
    {
        public Vector2 start;
        public Vector2 end;
    }

    // Spline types
    private const int SPLINE_LINEAR = 0;      // Linear
    private const int SPLINE_BASIS = 1;       // B-Spline
    private const int SPLINE_CATMULLROM = 2;  // Catmull-Rom
    private const int SPLINE_BEZIER = 3;      // Cubic Bezier

    private Vector2[] points;
    private Vector2[] pointsInterleaved;

    private int pointCount;
    private int selectedPoint;
    private int focusedPoint;
    private int selectedControlIndex;   // -1 when none
    private bool selectedControlStart;
    private int focusedControlIndex;    // -1 when none
    private bool focusedControlStart;

    private ControlPoint[] control;

    // Spline config variables
    private float splineThickness;
    private int splineTypeActive; // 0-Linear, 1-BSpline, 2-CatmullRom, 3-Bezier
    private bool splineTypeEditMode;
    private bool splineHelpersActive;

    // Minimal raygui-like global lock
    private static bool guiLocked;

    public void Init()
    {
        points = new Vector2[MAX_SPLINE_POINTS];
        points[0] = new Vector2(50.0f, 400.0f);
        points[1] = new Vector2(160.0f, 220.0f);
        points[2] = new Vector2(340.0f, 380.0f);
        points[3] = new Vector2(520.0f, 60.0f);
        points[4] = new Vector2(710.0f, 260.0f);

        // Array required for spline bezier-cubic,
        // including control points interleaved with start-end segment points
        pointsInterleaved = new Vector2[3 * (MAX_SPLINE_POINTS - 1) + 1];

        pointCount = 5;
        selectedPoint = -1;
        focusedPoint = -1;
        selectedControlIndex = -1;
        focusedControlIndex = -1;

        // Cubic Bezier control points initialization
        control = new ControlPoint[MAX_SPLINE_POINTS - 1];
        for (int i = 0; i < pointCount - 1; i++)
        {
            control[i].start = new Vector2(points[i].X + 50, points[i].Y);
            control[i].end = new Vector2(points[i + 1].X - 50, points[i + 1].Y);
        }

        splineThickness = 8.0f;
        splineTypeActive = SPLINE_LINEAR;
        splineTypeEditMode = false;
        splineHelpersActive = true;

        guiLocked = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Spline points creation logic (at the end of spline)
        if (IsMouseButtonPressed(MouseButton.Right) && (pointCount < MAX_SPLINE_POINTS))
        {
            points[pointCount] = GetMousePosition();
            int i = pointCount - 1;
            control[i].start = new Vector2(points[i].X + 50, points[i].Y);
            control[i].end = new Vector2(points[i + 1].X - 50, points[i + 1].Y);
            pointCount++;
        }

        // Spline point focus and selection logic
        if ((selectedPoint == -1) && ((splineTypeActive != SPLINE_BEZIER) || (selectedControlIndex == -1)))
        {
            focusedPoint = -1;
            for (int i = 0; i < pointCount; i++)
            {
                if (CheckCollisionPointCircle(GetMousePosition(), points[i], 8.0f))
                {
                    focusedPoint = i;
                    break;
                }
            }
            if (IsMouseButtonPressed(MouseButton.Left)) selectedPoint = focusedPoint;
        }

        // Spline point movement logic
        if (selectedPoint >= 0)
        {
            points[selectedPoint] = GetMousePosition();
            if (IsMouseButtonReleased(MouseButton.Left)) selectedPoint = -1;
        }

        // Cubic Bezier spline control points logic
        if ((splineTypeActive == SPLINE_BEZIER) && (focusedPoint == -1))
        {
            // Spline control point focus and selection logic
            if (selectedControlIndex == -1)
            {
                focusedControlIndex = -1;
                for (int i = 0; i < pointCount - 1; i++)
                {
                    if (CheckCollisionPointCircle(GetMousePosition(), control[i].start, 6.0f))
                    {
                        focusedControlIndex = i;
                        focusedControlStart = true;
                        break;
                    }
                    else if (CheckCollisionPointCircle(GetMousePosition(), control[i].end, 6.0f))
                    {
                        focusedControlIndex = i;
                        focusedControlStart = false;
                        break;
                    }
                }
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    selectedControlIndex = focusedControlIndex;
                    selectedControlStart = focusedControlStart;
                }
            }

            // Spline control point movement logic
            if (selectedControlIndex != -1)
            {
                if (selectedControlStart) control[selectedControlIndex].start = GetMousePosition();
                else control[selectedControlIndex].end = GetMousePosition();
                if (IsMouseButtonReleased(MouseButton.Left)) selectedControlIndex = -1;
            }
        }

        // Spline selection logic
        if (IsKeyPressed(KeyboardKey.One)) splineTypeActive = 0;
        else if (IsKeyPressed(KeyboardKey.Two)) splineTypeActive = 1;
        else if (IsKeyPressed(KeyboardKey.Three)) splineTypeActive = 2;
        else if (IsKeyPressed(KeyboardKey.Four)) splineTypeActive = 3;

        // Clear selection when changing to a spline without control points
        if (IsKeyPressed(KeyboardKey.One) || IsKeyPressed(KeyboardKey.Two) || IsKeyPressed(KeyboardKey.Three)) selectedControlIndex = -1;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (splineTypeActive == SPLINE_LINEAR)
        {
            // Draw spline: linear
            DrawSplineLinear(points, pointCount, splineThickness, Color.Red);
        }
        else if (splineTypeActive == SPLINE_BASIS)
        {
            // Draw spline: basis
            DrawSplineBasis(points, pointCount, splineThickness, Color.Red);  // Provide connected points array
        }
        else if (splineTypeActive == SPLINE_CATMULLROM)
        {
            // Draw spline: catmull-rom
            DrawSplineCatmullRom(points, pointCount, splineThickness, Color.Red); // Provide connected points array
        }
        else if (splineTypeActive == SPLINE_BEZIER)
        {
            // NOTE: Cubic-bezier spline requires the 2 control points of each segment to be
            // provided interleaved with the start and end point of every segment
            for (int i = 0; i < (pointCount - 1); i++)
            {
                pointsInterleaved[3 * i] = points[i];
                pointsInterleaved[3 * i + 1] = control[i].start;
                pointsInterleaved[3 * i + 2] = control[i].end;
            }

            pointsInterleaved[3 * (pointCount - 1)] = points[pointCount - 1];

            // Draw spline: cubic-bezier (with control points)
            DrawSplineBezierCubic(pointsInterleaved, 3 * (pointCount - 1) + 1, splineThickness, Color.Red);

            // Draw spline control points
            for (int i = 0; i < pointCount - 1; i++)
            {
                // Every cubic bezier point have two control points
                DrawCircleV(control[i].start, 6, Color.Gold);
                DrawCircleV(control[i].end, 6, Color.Gold);
                if (focusedControlIndex == i && focusedControlStart) DrawCircleV(control[i].start, 8, Color.Green);
                else if (focusedControlIndex == i && !focusedControlStart) DrawCircleV(control[i].end, 8, Color.Green);
                DrawLineEx(points[i], control[i].start, 1.0f, Color.LightGray);
                DrawLineEx(points[i + 1], control[i].end, 1.0f, Color.LightGray);

                // Draw spline control lines
                DrawLineV(points[i], control[i].start, Color.Gray);
                DrawLineV(control[i].end, points[i + 1], Color.Gray);
            }
        }

        if (splineHelpersActive)
        {
            // Draw spline point helpers
            for (int i = 0; i < pointCount; i++)
            {
                DrawCircleLinesV(points[i], (focusedPoint == i) ? 12.0f : 8.0f, (focusedPoint == i) ? Color.Blue : Color.DarkBlue);
                if ((splineTypeActive != SPLINE_LINEAR) &&
                    (splineTypeActive != SPLINE_BEZIER) &&
                    (i < pointCount - 1)) DrawLineV(points[i], points[i + 1], Color.Gray);

                DrawText($"[{points[i].X:F0}, {points[i].Y:F0}]", (int)points[i].X, (int)points[i].Y + 10, 10, Color.Black);
            }
        }

        // Check all possible UI states that require controls lock
        if (splineTypeEditMode || (selectedPoint != -1) || (selectedControlIndex != -1)) GuiLock();

        // Draw spline config
        GuiLabel(new Rectangle(12, 62, 140, 24), $"Spline thickness: {(int)splineThickness}");
        GuiSliderBar(new Rectangle(12, 60 + 24, 140, 16), null, null, ref splineThickness, 1.0f, 40.0f);

        GuiCheckBox(new Rectangle(12, 110, 20, 20), "Show point helpers", ref splineHelpersActive);

        if (splineTypeEditMode) GuiUnlock();

        GuiLabel(new Rectangle(12, 10, 140, 24), "Spline type:");
        if (GuiDropdownBox(new Rectangle(12, 8 + 24, 140, 28), "LINEAR;BSPLINE;CATMULLROM;BEZIER", ref splineTypeActive, splineTypeEditMode)) splineTypeEditMode = !splineTypeEditMode;

        GuiUnlock();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiLock() => guiLocked = true;
    private static void GuiUnlock() => guiLocked = false;

    private static void GuiLabel(Rectangle bounds, string text)
    {
        DrawText(text, (int)bounds.X, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    private static void GuiSliderBar(Rectangle bounds, string textLeft, string textRight, ref float value, float minValue, float maxValue)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (!guiLocked && hover && IsMouseButtonDown(MouseButton.Left))
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
        if (!guiLocked && hover && IsMouseButtonPressed(MouseButton.Left)) active = !active;

        DrawRectangleLinesEx(bounds, 1, hover ? Color.DarkBlue : Color.Gray);
        if (active) DrawRectangle((int)bounds.X + 4, (int)bounds.Y + 4, (int)bounds.Width - 8, (int)bounds.Height - 8, Color.DarkGray);
        if (text != null) DrawText(text, (int)(bounds.X + bounds.Width + 8), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    private static bool GuiDropdownBox(Rectangle bounds, string text, ref int active, bool editMode)
    {
        string[] items = text.Split(';');
        bool pressed = false;
        Vector2 mouse = GetMousePosition();
        bool clickable = !guiLocked;

        bool hoverMain = CheckCollisionPointRec(mouse, bounds);
        DrawRectangleRec(bounds, editMode ? Color.SkyBlue : (hoverMain ? Color.LightGray : Color.RayWhite));
        DrawRectangleLinesEx(bounds, 1, editMode ? Color.Blue : Color.Gray);
        DrawText(items[active], (int)bounds.X + 6, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);

        if (editMode)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Rectangle itemRec = new Rectangle(bounds.X, bounds.Y + bounds.Height * (i + 1), bounds.Width, bounds.Height);
                bool hoverItem = CheckCollisionPointRec(mouse, itemRec);
                DrawRectangleRec(itemRec, hoverItem ? Color.SkyBlue : Color.RayWhite);
                DrawRectangleLinesEx(itemRec, 1, Color.Gray);
                DrawText(items[i], (int)itemRec.X + 6, (int)(itemRec.Y + itemRec.Height / 2 - 5), 10, Color.DarkGray);
                if (clickable && hoverItem && IsMouseButtonPressed(MouseButton.Left))
                {
                    active = i;
                    pressed = true;
                }
            }
        }

        if (clickable && hoverMain && IsMouseButtonPressed(MouseButton.Left)) pressed = true;

        return pressed;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - splines drawing");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SplinesDrawing();
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
