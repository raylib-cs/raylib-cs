/*******************************************************************************************
*
*   raylib [shapes] example - easings testbed
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example contributed by Juan Miguel López (@flashback-fx) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Juan Miguel López (@flashback-fx) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using Examples.Shared;

namespace Examples.Shapes;

public partial class EasingsTestbed : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int FONT_SIZE = 20;

    private const float D_STEP = 20.0f;
    private const float D_STEP_FINE = 2.0f;
    private const float D_MIN = 1.0f;
    private const float D_MAX = 10000.0f;

    public string Name => "Shapes / Easings Testbed";

    public string Title => "raylib [shapes] example - easings testbed";

    // Easing types
    private const int EASE_LINEAR_NONE = 0;
    private const int NUM_EASING_TYPES = 27;
    private const int EASING_NONE = NUM_EASING_TYPES;

    // NoEase function, used when "no easing" is selected for any axis
    // It just ignores all parameters besides b
    private static float NoEase(float t, float b, float c, float d)
    {
        // Hack to avoid compiler warning (about unused variables)
        float burn = t + b + c + d;
        d += burn;

        return b;
    }

    // Easing functions reference data
    private string[] easingNames;
    private Func<float, float, float, float, float>[] easingFuncs;

    private Vector2 ballPosition;
    private float t;                // Current time (in any unit measure, but same unit as duration)
    private float d;                // Total time it should take to complete (duration)
    private bool paused;
    private bool boundedT;          // If true, t will stop when d >= td, otherwise t will keep adding td to its value every loop

    private int easingX;            // Easing selected for x axis
    private int easingY;            // Easing selected for y axis

    public void Init()
    {
        easingNames = new string[]
        {
            "EaseLinearNone", "EaseLinearIn", "EaseLinearOut", "EaseLinearInOut",
            "EaseSineIn", "EaseSineOut", "EaseSineInOut",
            "EaseCircIn", "EaseCircOut", "EaseCircInOut",
            "EaseCubicIn", "EaseCubicOut", "EaseCubicInOut",
            "EaseQuadIn", "EaseQuadOut", "EaseQuadInOut",
            "EaseExpoIn", "EaseExpoOut", "EaseExpoInOut",
            "EaseBackIn", "EaseBackOut", "EaseBackInOut",
            "EaseBounceOut", "EaseBounceIn", "EaseBounceInOut",
            "EaseElasticIn", "EaseElasticOut", "EaseElasticInOut",
            "None",
        };

        easingFuncs = new Func<float, float, float, float, float>[]
        {
            Easings.EaseLinearNone, Easings.EaseLinearIn, Easings.EaseLinearOut, Easings.EaseLinearInOut,
            Easings.EaseSineIn, Easings.EaseSineOut, Easings.EaseSineInOut,
            Easings.EaseCircIn, Easings.EaseCircOut, Easings.EaseCircInOut,
            Easings.EaseCubicIn, Easings.EaseCubicOut, Easings.EaseCubicInOut,
            Easings.EaseQuadIn, Easings.EaseQuadOut, Easings.EaseQuadInOut,
            Easings.EaseExpoIn, Easings.EaseExpoOut, Easings.EaseExpoInOut,
            Easings.EaseBackIn, Easings.EaseBackOut, Easings.EaseBackInOut,
            Easings.EaseBounceOut, Easings.EaseBounceIn, Easings.EaseBounceInOut,
            Easings.EaseElasticIn, Easings.EaseElasticOut, Easings.EaseElasticInOut,
            NoEase,
        };

        ballPosition = new Vector2(100.0f, 100.0f);

        t = 0.0f;
        d = 300.0f;
        paused = true;
        boundedT = true;

        easingX = EASING_NONE;
        easingY = EASING_NONE;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.T))
        {
            boundedT = !boundedT;
        }

        // Choose easing for the X axis
        if (IsKeyPressed(KeyboardKey.Right))
        {
            easingX++;

            if (easingX > EASING_NONE)
            {
                easingX = 0;
            }
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            if (easingX == 0)
            {
                easingX = EASING_NONE;
            }
            else
            {
                easingX--;
            }
        }

        // Choose easing for the Y axis
        if (IsKeyPressed(KeyboardKey.Down))
        {
            easingY++;

            if (easingY > EASING_NONE)
            {
                easingY = 0;
            }
        }
        else if (IsKeyPressed(KeyboardKey.Up))
        {
            if (easingY == 0)
            {
                easingY = EASING_NONE;
            }
            else
            {
                easingY--;
            }
        }

        // Change d (duration) value
        if (IsKeyPressed(KeyboardKey.W) && (d < D_MAX - D_STEP))
        {
            d += D_STEP;
        }
        else if (IsKeyPressed(KeyboardKey.Q) && (d > D_MIN + D_STEP))
        {
            d -= D_STEP;
        }

        if (IsKeyDown(KeyboardKey.S) && (d < D_MAX - D_STEP_FINE))
        {
            d += D_STEP_FINE;
        }
        else if (IsKeyDown(KeyboardKey.A) && (d > D_MIN + D_STEP_FINE))
        {
            d -= D_STEP_FINE;
        }

        // Play, pause and restart controls
        if (IsKeyPressed(KeyboardKey.Space) || IsKeyPressed(KeyboardKey.T) ||
            IsKeyPressed(KeyboardKey.Right) || IsKeyPressed(KeyboardKey.Left) ||
            IsKeyPressed(KeyboardKey.Down) || IsKeyPressed(KeyboardKey.Up) ||
            IsKeyPressed(KeyboardKey.W) || IsKeyPressed(KeyboardKey.Q) ||
            IsKeyDown(KeyboardKey.S) || IsKeyDown(KeyboardKey.A) ||
            (IsKeyPressed(KeyboardKey.Enter) && (boundedT == true) && (t >= d)))
        {
            t = 0.0f;
            ballPosition.X = 100.0f;
            ballPosition.Y = 100.0f;
            paused = true;
        }

        if (IsKeyPressed(KeyboardKey.Enter))
        {
            paused = !paused;
        }

        // Movement computation
        if (!paused && ((boundedT && t < d) || !boundedT))
        {
            ballPosition.X = easingFuncs[easingX](t, 100.0f, 700.0f - 170.0f, d);
            ballPosition.Y = easingFuncs[easingY](t, 100.0f, 400.0f - 170.0f, d);
            t += 1.0f;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw information text
        DrawText($"Easing x: {easingNames[easingX]}", 20, FONT_SIZE, FONT_SIZE, Color.LightGray);
        DrawText($"Easing y: {easingNames[easingY]}", 20, FONT_SIZE * 2, FONT_SIZE, Color.LightGray);
        DrawText($"t ({(boundedT == true ? 'b' : 'u')}) = {t:F2} d = {d:F2}", 20, FONT_SIZE * 3, FONT_SIZE, Color.LightGray);

        // Draw instructions text
        DrawText("Use ENTER to play or pause movement, use SPACE to restart", 20, GetScreenHeight() - FONT_SIZE * 2, FONT_SIZE, Color.LightGray);
        DrawText("Use Q and W or A and S keys to change duration", 20, GetScreenHeight() - FONT_SIZE * 3, FONT_SIZE, Color.LightGray);
        DrawText("Use LEFT or RIGHT keys to choose easing for the x axis", 20, GetScreenHeight() - FONT_SIZE * 4, FONT_SIZE, Color.LightGray);
        DrawText("Use UP or DOWN keys to choose easing for the y axis", 20, GetScreenHeight() - FONT_SIZE * 5, FONT_SIZE, Color.LightGray);

        // Draw ball
        DrawCircleV(ballPosition, 16.0f, Color.Maroon);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - easings testbed");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new EasingsTestbed();
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
