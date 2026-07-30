/*******************************************************************************************
*
*   raylib [shapes] example - recursive tree
*
*   Example complexity rating: [★★★☆] 3/4
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

public partial class RecursiveTree : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Recursive Tree";

    public string Title => "raylib [shapes] example - recursive tree";

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    private struct Branch
    {
        public Vector2 start;
        public Vector2 end;
        public float angle;
        public float length;
    }

    private Vector2 start;
    private float angle;
    private float thick;
    private float treeDepth;
    private float branchDecay;
    private float length;
    private bool bezier;

    private Branch[] branches;

    public void Init()
    {
        start = new Vector2((screenWidth / 2.0f) - 125.0f, (float)screenHeight);
        angle = 40.0f;
        thick = 1.0f;
        treeDepth = 10.0f;
        branchDecay = 0.66f;
        length = 120.0f;
        bezier = false;

        branches = new Branch[1030];
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float theta = angle * DEG2RAD;
        int maxBranches = (int)(MathF.Pow(2, MathF.Floor(treeDepth)));
        int count = 0;

        Vector2 initialEnd = new Vector2(start.X + length * MathF.Sin(0.0f), start.Y - length * MathF.Cos(0.0f));
        branches[count++] = new Branch { start = start, end = initialEnd, angle = 0.0f, length = length };

        for (int i = 0; i < count; i++)
        {
            Branch branch = branches[i];
            if (branch.length < 2)
            {
                continue;
            }

            float nextLength = branch.length * branchDecay;

            if (count < maxBranches && nextLength >= 2)
            {
                Vector2 branchStart = branch.end;

                float angle1 = branch.angle + theta;
                Vector2 branchEnd1 = new Vector2(branchStart.X + nextLength * MathF.Sin(angle1), branchStart.Y - nextLength * MathF.Cos(angle1));
                branches[count++] = new Branch { start = branchStart, end = branchEnd1, angle = angle1, length = nextLength };

                float angle2 = branch.angle - theta;
                Vector2 branchEnd2 = new Vector2(branchStart.X + nextLength * MathF.Sin(angle2), branchStart.Y - nextLength * MathF.Cos(angle2));
                branches[count++] = new Branch { start = branchStart, end = branchEnd2, angle = angle2, length = nextLength };
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (int i = 0; i < count; i++)
        {
            Branch branch = branches[i];
            if (branch.length >= 2)
            {
                if (bezier)
                {
                    DrawLineBezier(branch.start, branch.end, thick, Color.Red);
                }
                else
                {
                    DrawLineEx(branch.start, branch.end, thick, Color.Red);
                }
            }
        }

        DrawLine(580, 0, 580, GetScreenHeight(), new Color(218, 218, 218, 255));
        DrawRectangle(580, 0, GetScreenWidth(), GetScreenHeight(), new Color(232, 232, 232, 255));

        // Draw GUI controls (minimal raygui-like controls, raygui is not bound in raylib-cs)
        //------------------------------------------------------------------------------
        GuiSliderBar(new Rectangle(640, 40, 120, 20), "Angle", $"{angle:F0}", ref angle, 0, 180);
        GuiSliderBar(new Rectangle(640, 70, 120, 20), "Length", $"{length:F0}", ref length, 12.0f, 240.0f);
        GuiSliderBar(new Rectangle(640, 100, 120, 20), "Decay", $"{branchDecay:F2}", ref branchDecay, 0.1f, 0.78f);
        GuiSliderBar(new Rectangle(640, 130, 120, 20), "Depth", $"{treeDepth:F0}", ref treeDepth, 1.0f, 10.0f);
        GuiSliderBar(new Rectangle(640, 160, 120, 20), "Thick", $"{thick:F0}", ref thick, 1, 8);
        GuiCheckBox(new Rectangle(640, 190, 20, 20), "Bezier", ref bezier);
        //------------------------------------------------------------------------------

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
            if (value < minValue)
            {
                value = minValue;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }
        }

        DrawRectangleRec(bounds, Color.LightGray);
        float pct = (value - minValue) / (maxValue - minValue);
        DrawRectangleRec(new Rectangle(bounds.X, bounds.Y, bounds.Width * pct, bounds.Height), Color.SkyBlue);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (textLeft != null)
        {
            DrawText(textLeft, (int)bounds.X - MeasureText(textLeft, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }

        if (textRight != null)
        {
            DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
    }

    private static void GuiCheckBox(Rectangle bounds, string text, ref bool active)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (hover && IsMouseButtonPressed(MouseButton.Left))
        {
            active = !active;
        }

        DrawRectangleLinesEx(bounds, 1, hover ? Color.DarkBlue : Color.Gray);
        if (active)
        {
            DrawRectangle((int)bounds.X + 4, (int)bounds.Y + 4, (int)bounds.Width - 8, (int)bounds.Height - 8, Color.DarkGray);
        }

        if (text != null)
        {
            DrawText(text, (int)(bounds.X + bounds.Width + 8), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - recursive tree");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new RecursiveTree();
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
