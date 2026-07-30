/*******************************************************************************************
*
*   raylib [shapes] example - math sine cosine
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Jopestpe (@jopestpe) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jopestpe (@jopestpe)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Shapes;

public partial class MathSineCosine : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Wave points for sine/cosine visualization
    private const int WAVE_POINTS = 36;

    public string Name => "Shapes / Math Sine Cosine";

    public string Title => "raylib [shapes] example - math sine cosine";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Vector2[] sinePoints;
    private Vector2[] cosPoints;
    private Vector2 center;
    private Rectangle start;
    private float radius;
    private float angle;
    private bool pause;

    public void Init()
    {
        sinePoints = new Vector2[WAVE_POINTS];
        cosPoints = new Vector2[WAVE_POINTS];
        center = new((screenWidth / 2.0f) - 30.0f, screenHeight / 2.0f);
        start = new(20.0f, screenHeight - 120.0f, 200.0f, 100.0f);
        radius = 130.0f;
        angle = 0.0f;
        pause = false;

        for (int i = 0; i < WAVE_POINTS; i++)
        {
            float t = i / (float)(WAVE_POINTS - 1);
            float currentAngle = t * 360.0f * DEG2RAD;
            sinePoints[i] = new(start.X + t * start.Width, start.Y + start.Height / 2.0f - MathF.Sin(currentAngle) * (start.Height / 2.0f));
            cosPoints[i] = new(start.X + t * start.Width, start.Y + start.Height / 2.0f - MathF.Cos(currentAngle) * (start.Height / 2.0f));
        }
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float angleRad = angle * DEG2RAD;
        float cosRad = MathF.Cos(angleRad);
        float sinRad = MathF.Sin(angleRad);

        Vector2 point = new(center.X + cosRad * radius, center.Y - sinRad * radius);
        Vector2 limitMin = new(center.X - radius, center.Y - radius);
        Vector2 limitMax = new(center.X + radius, center.Y + radius);

        float complementary = 90.0f - angle;
        float supplementary = 180.0f - angle;
        float explementary = 360.0f - angle;

        float tangent = Clamp(MathF.Tan(angleRad), -10.0f, 10.0f);
        float cotangent = (MathF.Abs(tangent) > 0.001f) ? Clamp(1.0f / tangent, -radius, radius) : 0.0f;
        Vector2 tangentPoint = new(center.X + radius, center.Y - tangent * radius);
        Vector2 cotangentPoint = new(center.X + cotangent * radius, center.Y - radius);

        angle = Wrap(angle + (!pause ? 1.0f : 0.0f), 0.0f, 360.0f);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Cotangent (orange)
        DrawLineEx(new Vector2(center.X, limitMin.Y), new Vector2(cotangentPoint.X, limitMin.Y), 2.0f, Color.Orange);
        DrawLineDashed(center, cotangentPoint, 10, 4, Color.Orange);

        // Side background
        DrawLine(580, 0, 580, GetScreenHeight(), new Color(218, 218, 218, 255));
        DrawRectangle(580, 0, GetScreenWidth(), GetScreenHeight(), new Color(232, 232, 232, 255));

        // Base circle and axes
        DrawCircleLinesV(center, radius, Color.Gray);
        DrawLineEx(new Vector2(center.X, limitMin.Y), new Vector2(center.X, limitMax.Y), 1.0f, Color.Gray);
        DrawLineEx(new Vector2(limitMin.X, center.Y), new Vector2(limitMax.X, center.Y), 1.0f, Color.Gray);

        // Wave graph axes
        DrawLineEx(new Vector2(start.X, start.Y), new Vector2(start.X, start.Y + start.Height), 2.0f, Color.Gray);
        DrawLineEx(new Vector2(start.X + start.Width, start.Y), new Vector2(start.X + start.Width, start.Y + start.Height), 2.0f, Color.Gray);
        DrawLineEx(new Vector2(start.X, start.Y + start.Height / 2), new Vector2(start.X + start.Width, start.Y + start.Height / 2), 2.0f, Color.Gray);

        // Wave graph axis labels
        DrawText("1", (int)start.X - 8, (int)start.Y, 6, Color.Gray);
        DrawText("0", (int)start.X - 8, (int)start.Y + (int)start.Height / 2 - 6, 6, Color.Gray);
        DrawText("-1", (int)start.X - 12, (int)start.Y + (int)start.Height - 8, 6, Color.Gray);
        DrawText("0", (int)start.X - 2, (int)start.Y + (int)start.Height + 4, 6, Color.Gray);
        DrawText("360", (int)start.X + (int)start.Width - 8, (int)start.Y + (int)start.Height + 4, 6, Color.Gray);

        // Sine (red - vertical)
        DrawLineEx(new Vector2(center.X, center.Y), new Vector2(center.X, point.Y), 2.0f, Color.Red);
        DrawLineDashed(new Vector2(point.X, center.Y), new Vector2(point.X, point.Y), 10, 4, Color.Red);
        DrawText($"Sine {sinRad:0.00}", 640, 190, 6, Color.Red);
        DrawCircleV(new Vector2(start.X + (angle / 360.0f) * start.Width, start.Y + ((-sinRad + 1) * start.Height / 2.0f)), 4.0f, Color.Red);
        fixed (Vector2* p = sinePoints)
        {
            DrawSplineLinear(p, WAVE_POINTS, 1.0f, Color.Red);
        }

        // Cosine (blue - horizontal)
        DrawLineEx(new Vector2(center.X, center.Y), new Vector2(point.X, center.Y), 2.0f, Color.Blue);
        DrawLineDashed(new Vector2(center.X, point.Y), new Vector2(point.X, point.Y), 10, 4, Color.Blue);
        DrawText($"Cosine {cosRad:0.00}", 640, 210, 6, Color.Blue);
        DrawCircleV(new Vector2(start.X + (angle / 360.0f) * start.Width, start.Y + ((-cosRad + 1) * start.Height / 2.0f)), 4.0f, Color.Blue);
        fixed (Vector2* p = cosPoints)
        {
            DrawSplineLinear(p, WAVE_POINTS, 1.0f, Color.Blue);
        }

        // Tangent (purple)
        DrawLineEx(new Vector2(limitMax.X, center.Y), new Vector2(limitMax.X, tangentPoint.Y), 2.0f, Color.Purple);
        DrawLineDashed(center, tangentPoint, 10, 4, Color.Purple);
        DrawText($"Tangent {tangent:0.00}", 640, 230, 6, Color.Purple);

        // Cotangent (orange)
        DrawText($"Cotangent {cotangent:0.00}", 640, 250, 6, Color.Orange);

        // Complementary angle (beige)
        DrawCircleSectorLines(center, radius * 0.6f, -angle, -90.0f, 36, Color.Beige);
        DrawText($"Complementary  {complementary:0}°", 640, 150, 6, Color.Beige);

        // Supplementary angle (darkblue)
        DrawCircleSectorLines(center, radius * 0.5f, -angle, -180.0f, 36, Color.DarkBlue);
        DrawText($"Supplementary  {supplementary:0}°", 640, 130, 6, Color.DarkBlue);

        // Explementary angle (pink)
        DrawCircleSectorLines(center, radius * 0.4f, -angle, -360.0f, 36, Color.Pink);
        DrawText($"Explementary  {explementary:0}°", 640, 170, 6, Color.Pink);

        // Current angle - arc (lime), radius (black), endpoint (black)
        DrawCircleSectorLines(center, radius * 0.7f, -angle, 0.0f, 36, Color.Lime);
        DrawLineEx(new Vector2(center.X, center.Y), point, 2.0f, Color.Black);
        DrawCircleV(point, 4.0f, Color.Black);

        // Draw GUI controls
        // NOTE: raygui is not bound in raylib-cs, so the Pause toggle and Angle slider
        // are unavailable; the angle animates continuously.
        //------------------------------------------------------------------------------
        /*
        GuiSetStyle(LABEL, TEXT_COLOR_NORMAL, ColorToInt(GRAY));
        GuiToggle((Rectangle){ 640, 70, 120, 20}, TextFormat("Pause"), &pause);
        GuiSetStyle(LABEL, TEXT_COLOR_NORMAL, ColorToInt(LIME));
        GuiSliderBar((Rectangle){ 640, 40, 120, 20}, "Angle", TextFormat("%.0f°", angle), &angle, 0.0f, 360.0f);

        // Angle values panel
        GuiGroupBox((Rectangle){ 620, 110, 140, 170}, "Angle Values");
        */
        //------------------------------------------------------------------------------

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
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - math sine cosine");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new MathSineCosine();
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
