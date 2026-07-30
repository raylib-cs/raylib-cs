/*******************************************************************************************
*
*   raylib [shapes] example - ellipse collision
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Ziya (@Monjaris)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ziya (@Monjaris)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class EllipseCollision : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Ellipse Collision";

    public string Title => "raylib [shapes] example - collision ellipses";

    private Vector2 ellipseACenter;
    private float ellipseARx;
    private float ellipseARy;

    private Vector2 ellipseBCenter;
    private float ellipseBRx;
    private float ellipseBRy;

    // 0 = controlling A, 1 = controlling B
    private int controlled;

    // Check if point is inside ellipse
    private static bool CheckCollisionPointEllipse(Vector2 point, Vector2 center, float rx, float ry)
    {
        float dx = (point.X - center.X) / rx;
        float dy = (point.Y - center.Y) / ry;
        return (dx * dx + dy * dy) <= 1.0f;
    }

    // Check if two ellipses collide
    // Uses radial boundary distance in the direction between centers — scales correctly with radii
    private static bool CheckCollisionEllipses(Vector2 c1, float rx1, float ry1, Vector2 c2, float rx2, float ry2)
    {
        float dx = c2.X - c1.X;
        float dy = c2.Y - c1.Y;
        float dist = MathF.Sqrt(dx * dx + dy * dy);

        // Ellipses are on top of each other
        if (dist == 0.0f)
        {
            return true;
        }

        float theta = MathF.Atan2(dy, dx);
        float cosT = MathF.Cos(theta);
        float sinT = MathF.Sin(theta);

        // Radial distance from center to ellipse boundary in direction theta
        // r(theta) = (rx * ry) / sqrt((ry*cos)^2 + (rx*sin)^2)
        float r1 = (rx1 * ry1) / MathF.Sqrt((ry1 * cosT) * (ry1 * cosT) + (rx1 * sinT) * (rx1 * sinT));
        float r2 = (rx2 * ry2) / MathF.Sqrt((ry2 * cosT) * (ry2 * cosT) + (rx2 * sinT) * (rx2 * sinT));

        return dist <= (r1 + r2);
    }

    public void Init()
    {
        ellipseACenter = new((float)screenWidth / 4, (float)screenHeight / 2);
        ellipseARx = 120.0f;
        ellipseARy = 70.0f;

        ellipseBCenter = new((float)screenWidth * 3 / 4, (float)screenHeight / 2);
        ellipseBRx = 90.0f;
        ellipseBRy = 140.0f;

        // 0 = controlling A, 1 = controlling B
        controlled = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.A))
        {
            controlled = 0;
        }
        if (IsKeyPressed(KeyboardKey.B))
        {
            controlled = 1;
        }

        if (controlled == 0)
        {
            ellipseACenter = GetMousePosition();
        }
        else
        {
            ellipseBCenter = GetMousePosition();
        }

        bool ellipsesCollide = CheckCollisionEllipses(
            ellipseACenter, ellipseARx, ellipseARy,
            ellipseBCenter, ellipseBRx, ellipseBRy
        );

        bool mouseInA = CheckCollisionPointEllipse(GetMousePosition(), ellipseACenter, ellipseARx, ellipseARy);
        bool mouseInB = CheckCollisionPointEllipse(GetMousePosition(), ellipseBCenter, ellipseBRx, ellipseBRy);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawEllipse((int)ellipseACenter.X, (int)ellipseACenter.Y, ellipseARx, ellipseARy, ellipsesCollide ? Color.Red : Color.Blue);

        DrawEllipse((int)ellipseBCenter.X, (int)ellipseBCenter.Y, ellipseBRx, ellipseBRy, ellipsesCollide ? Color.Red : Color.Green);

        DrawEllipseLines((int)ellipseACenter.X, (int)ellipseACenter.Y, ellipseARx, ellipseARy, Color.White);

        DrawEllipseLines((int)ellipseBCenter.X, (int)ellipseBCenter.Y, ellipseBRx, ellipseBRy, Color.White);

        DrawCircleV(ellipseACenter, 4, Color.White);
        DrawCircleV(ellipseBCenter, 4, Color.White);

        if (ellipsesCollide)
        {
            DrawText("ELLIPSES COLLIDE", screenWidth / 2 - 120, 40, 28, Color.Red);
        }
        else
        {
            DrawText("NO COLLISION", screenWidth / 2 - 80, 40, 28, Color.DarkGray);
        }

        DrawText(controlled == 0 ? "Controlling: A" : "Controlling: B", 20, screenHeight - 40, 20, Color.Yellow);

        if (mouseInA && controlled != 0)
        {
            DrawText("Mouse inside ellipse A", 20, screenHeight - 70, 20, Color.Blue);
        }
        if (mouseInB && controlled != 1)
        {
            DrawText("Mouse inside ellipse B", 20, screenHeight - 70, 20, Color.Green);
        }

        DrawText("Press [A] or [B] to switch control", 20, 20, 20, Color.Gray);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - collision ellipses");
        SetTargetFPS(60);

        var game = new EllipseCollision();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
