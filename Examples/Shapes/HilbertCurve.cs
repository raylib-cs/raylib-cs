/*******************************************************************************************
*
*   raylib [shapes] example - hilbert curve
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example contributed by Hamza RAHAL (@hmz-rhl) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Hamza RAHAL (@hmz-rhl)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class HilbertCurve : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Hilbert Curve";

    public string Title => "raylib [shapes] example - hilbert curve";

    private int order;
    private float size;
    private int strokeCount;
    private Vector2[] hilbertPath;

    private int prevOrder;
    private int prevSize;       // NOTE: Size from slider is float but for comparison we use int
    private int counter;
    private float thick;
    private bool animate;

    public void Init()
    {
        order = 2;
        size = (float)GetScreenHeight();
        strokeCount = 0;
        hilbertPath = LoadHilbertPath(order, size, out strokeCount);

        prevOrder = order;
        prevSize = (int)size;
        counter = 0;
        thick = 2.0f;
        animate = true;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Check if order or size have changed to regenerate
        // NOTE: Size from slider is float but for comparison we use int
        if ((prevOrder != order) || (prevSize != (int)size))
        {
            hilbertPath = LoadHilbertPath(order, size, out strokeCount);

            if (animate) counter = 0;
            else counter = strokeCount;

            prevOrder = order;
            prevSize = (int)size;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //--------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (counter < strokeCount)
        {
            // Draw Hilbert path animation, one stroke every frame
            for (int i = 1; i <= counter; i++)
            {
                DrawLineEx(hilbertPath[i], hilbertPath[i - 1], thick, ColorFromHSV(((float)i / strokeCount) * 360.0f, 1.0f, 1.0f));
            }

            counter += 1;
        }
        else
        {
            // Draw full Hilbert path
            for (int i = 1; i < strokeCount; i++)
            {
                DrawLineEx(hilbertPath[i], hilbertPath[i - 1], thick, ColorFromHSV(((float)i / strokeCount) * 360.0f, 1.0f, 1.0f));
            }
        }

        // Draw UI (minimal raygui-like controls, raygui is not bound in raylib-cs)
        GuiCheckBox(new Rectangle(450, 50, 20, 20), "ANIMATE GENERATION ON CHANGE", ref animate);
        GuiSpinner(new Rectangle(585, 100, 180, 30), "HILBERT CURVE ORDER:  ", ref order, 2, 8);
        GuiSlider(new Rectangle(524, 150, 240, 24), "THICKNESS:  ", null, ref thick, 1.0f, 10.0f);
        GuiSlider(new Rectangle(524, 190, 240, 24), "TOTAL SIZE: ", null, ref size, 10.0f, GetScreenHeight() * 1.5f);

        EndDrawing();
        //--------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //------------------------------------------------------------------------------------
    // Module Functions Definition
    //------------------------------------------------------------------------------------
    // Load the whole Hilbert Path (including each U and their link)
    private static Vector2[] LoadHilbertPath(int order, float size, out int strokeCount)
    {
        int N = 1 << order;
        float len = size / N;
        strokeCount = N * N;

        Vector2[] hilbertPath = new Vector2[strokeCount];

        for (int i = 0; i < strokeCount; i++)
        {
            hilbertPath[i] = ComputeHilbertStep(order, i);
            hilbertPath[i].X = hilbertPath[i].X * len + len / 2.0f;
            hilbertPath[i].Y = hilbertPath[i].Y * len + len / 2.0f;
        }

        return hilbertPath;
    }

    // Compute Hilbert path U positions
    private static Vector2 ComputeHilbertStep(int order, int index)
    {
        // Hilbert points base pattern
        Vector2[] hilbertPoints = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
        };

        int hilbertIndex = index & 3;
        Vector2 vect = hilbertPoints[hilbertIndex];
        float temp = 0.0f;
        int len = 0;

        for (int j = 1; j < order; j++)
        {
            index = index >> 2;
            hilbertIndex = index & 3;
            len = 1 << j;

            switch (hilbertIndex)
            {
                case 0:
                {
                    temp = vect.X;
                    vect.X = vect.Y;
                    vect.Y = temp;
                } break;
                case 2:
                {
                    vect.X += len;
                    vect.Y += len;
                } break;
                case 1: vect.Y += len; break;
                case 3:
                {
                    temp = len - 1 - vect.X;
                    vect.X = 2 * len - 1 - vect.Y;
                    vect.Y = temp;
                } break;
                default: break;
            }
        }

        return vect;
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiCheckBox(Rectangle bounds, string text, ref bool active)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (hover && IsMouseButtonPressed(MouseButton.Left)) active = !active;

        DrawRectangleLinesEx(bounds, 1, hover ? Color.DarkBlue : Color.Gray);
        if (active) DrawRectangle((int)bounds.X + 4, (int)bounds.Y + 4, (int)bounds.Width - 8, (int)bounds.Height - 8, Color.DarkGray);
        if (text != null) DrawText(text, (int)(bounds.X + bounds.Width + 8), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    private static void GuiSpinner(Rectangle bounds, string text, ref int value, int minValue, int maxValue)
    {
        Vector2 mouse = GetMousePosition();
        Rectangle left = new Rectangle(bounds.X, bounds.Y, bounds.Height, bounds.Height);
        Rectangle right = new Rectangle(bounds.X + bounds.Width - bounds.Height, bounds.Y, bounds.Height, bounds.Height);
        Rectangle mid = new Rectangle(bounds.X + bounds.Height, bounds.Y, bounds.Width - 2 * bounds.Height, bounds.Height);

        if (CheckCollisionPointRec(mouse, left) && IsMouseButtonPressed(MouseButton.Left) && value > minValue) value--;
        if (CheckCollisionPointRec(mouse, right) && IsMouseButtonPressed(MouseButton.Left) && value < maxValue) value++;

        DrawRectangleRec(mid, Color.RayWhite);
        DrawRectangleLinesEx(mid, 1, Color.Gray);
        DrawRectangleRec(left, Color.LightGray);
        DrawRectangleLinesEx(left, 1, Color.Gray);
        DrawRectangleRec(right, Color.LightGray);
        DrawRectangleLinesEx(right, 1, Color.Gray);
        DrawText("-", (int)(left.X + left.Width / 2 - 2), (int)(left.Y + left.Height / 2 - 5), 10, Color.DarkGray);
        DrawText("+", (int)(right.X + right.Width / 2 - 3), (int)(right.Y + right.Height / 2 - 5), 10, Color.DarkGray);
        string vs = value.ToString();
        DrawText(vs, (int)(mid.X + mid.Width / 2 - MeasureText(vs, 10) / 2), (int)(mid.Y + mid.Height / 2 - 5), 10, Color.DarkGray);
        if (text != null) DrawText(text, (int)bounds.X - MeasureText(text, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    private static void GuiSlider(Rectangle bounds, string textLeft, string textRight, ref float value, float minValue, float maxValue)
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
        float handleX = bounds.X + pct * bounds.Width;
        DrawRectangle((int)(handleX - 5), (int)bounds.Y, 10, (int)bounds.Height, Color.DarkGray);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (textLeft != null) DrawText(textLeft, (int)bounds.X - MeasureText(textLeft, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        if (textRight != null) DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - hilbert curve");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new HilbertCurve();
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
