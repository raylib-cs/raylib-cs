/*******************************************************************************************
*
*   raylib [shapes] example - penrose tile
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 6.0
*   Based on: https://processing.org/examples/penrosetile.html
*
*   Example contributed by David Buzatto (@davidbuzatto) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 David Buzatto (@davidbuzatto)
*
********************************************************************************************/

using System.Text;

namespace Examples.Shapes;

public partial class PenroseTile : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Penrose Tile";

    public string Title => "raylib [shapes] example - penrose tile";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    public int TargetFps => 120;

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    private struct TurtleState
    {
        public Vector2 origin;
        public float angle;
    }

    private class PenroseLSystem
    {
        public int steps;
        public StringBuilder production;
        public string ruleW;
        public string ruleX;
        public string ruleY;
        public string ruleZ;
        public float drawLength;
        public float theta;
    }

    //----------------------------------------------------------------------------------
    // Global Variables Definition
    //----------------------------------------------------------------------------------
    private Stack<TurtleState> turtleStack;

    private const float drawLength = 460.0f;
    private int minGenerations;
    private int maxGenerations;
    private int generations;
    private PenroseLSystem ls;

    public void Init()
    {
        turtleStack = new Stack<TurtleState>();

        minGenerations = 0;
        maxGenerations = 4;
        generations = 0;

        // Initialize new penrose tile
        ls = CreatePenroseLSystem(drawLength * (generations / (float)maxGenerations));
        for (int i = 0; i < generations; i++)
        {
            BuildProductionStep(ls);
        }
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        bool rebuild = false;
        if (IsKeyPressed(KeyboardKey.Up))
        {
            if (generations < maxGenerations)
            {
                generations++;
                rebuild = true;
            }
        }
        else if (IsKeyPressed(KeyboardKey.Down))
        {
            if (generations > minGenerations)
            {
                generations--;
                if (generations > 0)
                {
                    rebuild = true;
                }
            }
        }

        if (rebuild)
        {
            ls = CreatePenroseLSystem(drawLength * (generations / (float)maxGenerations));
            for (int i = 0; i < generations; i++)
            {
                BuildProductionStep(ls);
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (generations > 0)
        {
            DrawPenroseLSystem(ls);
        }

        DrawText("penrose l-system", 10, 10, 20, Color.DarkGray);
        DrawText("press up or down to change generations", 10, 30, 20, Color.DarkGray);
        DrawText($"generations: {generations}", 10, 50, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    // Push turtle state for next step
    private void PushTurtleState(TurtleState state)
    {
        turtleStack.Push(state);
    }

    // Pop turtle state step
    private TurtleState PopTurtleState()
    {
        if (turtleStack.Count > 0)
        {
            return turtleStack.Pop();
        }
        else
        {
            TraceLog(TraceLogLevel.Warning, "TURTLE STACK UNDERFLOW!");
        }

        return new TurtleState();
    }

    // Create a new penrose tile structure
    private static PenroseLSystem CreatePenroseLSystem(float drawLength)
    {
        PenroseLSystem ls = new PenroseLSystem
        {
            steps = 0,
            ruleW = "YF++ZF4-XF[-YF4-WF]++",
            ruleX = "+YF--ZF[3-WF--XF]+",
            ruleY = "-WF++XF[+++YF++ZF]-",
            ruleZ = "--YF++++WF[+ZF++++XF]--XF",
            drawLength = drawLength,
            theta = 36.0f // Degrees
        };

        ls.production = new StringBuilder("[X]++[X]++[X]++[X]++[X]");

        return ls;
    }

    // Build next penrose step
    private static void BuildProductionStep(PenroseLSystem ls)
    {
        StringBuilder newProduction = new StringBuilder();

        string production = ls.production.ToString();

        for (int i = 0; i < production.Length; i++)
        {
            char step = production[i];
            switch (step)
            {
                case 'W':
                    newProduction.Append(ls.ruleW);
                    break;
                case 'X':
                    newProduction.Append(ls.ruleX);
                    break;
                case 'Y':
                    newProduction.Append(ls.ruleY);
                    break;
                case 'Z':
                    newProduction.Append(ls.ruleZ);
                    break;
                default:
                    {
                        if (step != 'F')
                        {
                            newProduction.Append(step);
                        }
                    }
                    break;
            }
        }

        ls.drawLength *= 0.5f;
        ls.production = newProduction;
    }

    // Draw penrose tile lines
    private void DrawPenroseLSystem(PenroseLSystem ls)
    {
        Vector2 screenCenter = new Vector2(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);

        TurtleState turtle = new TurtleState
        {
            origin = new Vector2(0, 0),
            angle = -90.0f
        };

        int repeats = 1;
        string production = ls.production.ToString();
        int productionLength = production.Length;
        ls.steps += 12;

        if (ls.steps > productionLength)
        {
            ls.steps = productionLength;
        }

        for (int i = 0; i < ls.steps; i++)
        {
            char step = production[i];
            if (step == 'F')
            {
                for (int j = 0; j < repeats; j++)
                {
                    Vector2 startPosWorld = turtle.origin;
                    float radAngle = DEG2RAD * turtle.angle;
                    turtle.origin.X += ls.drawLength * MathF.Cos(radAngle);
                    turtle.origin.Y += ls.drawLength * MathF.Sin(radAngle);
                    Vector2 startPosScreen = new Vector2(startPosWorld.X + screenCenter.X, startPosWorld.Y + screenCenter.Y);
                    Vector2 endPosScreen = new Vector2(turtle.origin.X + screenCenter.X, turtle.origin.Y + screenCenter.Y);

                    DrawLineEx(startPosScreen, endPosScreen, 2, Fade(Color.Black, 0.2f));
                }

                repeats = 1;
            }
            else if (step == '+')
            {
                for (int j = 0; j < repeats; j++)
                {
                    turtle.angle += ls.theta;
                }

                repeats = 1;
            }
            else if (step == '-')
            {
                for (int j = 0; j < repeats; j++)
                {
                    turtle.angle += -ls.theta;
                }

                repeats = 1;
            }
            else if (step == '[')
            {
                PushTurtleState(turtle);
            }
            else if (step == ']')
            {
                turtle = PopTurtleState();
            }
            else if ((step >= 48) && (step <= 57))
            {
                repeats = (int)step - 48;
            }
        }

        turtleStack.Clear();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - penrose tile");

        SetTargetFPS(120);              // Set our game to run at 120 frames-per-second
        //---------------------------------------------------------------------------------------

        var game = new PenroseTile();
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
