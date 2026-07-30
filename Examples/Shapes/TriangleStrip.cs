/*******************************************************************************************
*
*   raylib [shapes] example - triangle strip
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Jopestpe (@jopestpe)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jopestpe (@jopestpe)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class TriangleStrip : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Triangle Strip";

    public string Title => "raylib [shapes] example - triangle strip";

    private Vector2[] points;
    private Vector2 center;
    private float segments;
    private float insideRadius;
    private float outsideRadius;
    private bool outline;

    public void Init()
    {
        points = new Vector2[122];
        center = new((screenWidth / 2.0f) - 125.0f, screenHeight / 2.0f);
        segments = 6.0f;
        insideRadius = 100.0f;
        outsideRadius = 150.0f;
        outline = true;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        int pointCount = (int)(segments);
        float angleStep = (360.0f / pointCount) * DEG2RAD;

        for (int i = 0, i2 = 0; i < pointCount; i++, i2 += 2)
        {
            float angle1 = i * angleStep;
            points[i2] = new Vector2(center.X + MathF.Cos(angle1) * insideRadius, center.Y + MathF.Sin(angle1) * insideRadius);
            float angle2 = angle1 + angleStep / 2.0f;
            points[i2 + 1] = new Vector2(center.X + MathF.Cos(angle2) * outsideRadius, center.Y + MathF.Sin(angle2) * outsideRadius);
        }

        points[pointCount * 2] = points[0];
        points[pointCount * 2 + 1] = points[1];
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (int i = 0; i < pointCount; i++)
        {
            Vector2 a = points[i * 2];
            Vector2 b = points[i * 2 + 1];
            Vector2 c = points[i * 2 + 2];
            Vector2 d = points[i * 2 + 3];

            float angle1 = i * angleStep;
            DrawTriangle(c, b, a, ColorFromHSV(angle1 * RAD2DEG, 1.0f, 1.0f));
            DrawTriangle(d, b, c, ColorFromHSV((angle1 + angleStep / 2) * RAD2DEG, 1.0f, 1.0f));

            if (outline)
            {
                DrawTriangleLines(a, b, c, Color.Black);
                DrawTriangleLines(c, b, d, Color.Black);
            }
        }

        DrawLine(580, 0, 580, GetScreenHeight(), new Color(218, 218, 218, 255));
        DrawRectangle(580, 0, GetScreenWidth(), GetScreenHeight(), new Color(232, 232, 232, 255));

        // Draw GUI controls
        //------------------------------------------------------------------------------
        // NOTE: raygui is not bound in raylib-cs, so the interactive controls are omitted
        // and 'segments'/'outline' keep their initial values.
        //GuiSliderBar(new Rectangle(640, 40, 120, 20), "Segments", TextFormat("%.0f", segments), ref segments, 6.0f, 60.0f);
        //GuiCheckBox(new Rectangle(640, 70, 20, 20), "Outline", ref outline);
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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - triangle strip");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new TriangleStrip();
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
