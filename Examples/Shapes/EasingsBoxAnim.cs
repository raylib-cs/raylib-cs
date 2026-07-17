/*******************************************************************************************
*
*   raylib [shapes] example - easings box
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using Examples.Shared;

namespace Examples.Shapes;

public partial class EasingsBoxAnim : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Easings Box Anim";

    public string Title => "raylib [shapes] example - easings box";

    // Box variables to be animated with easings
    private Rectangle rec;
    private float rotation;
    private float alpha;

    private int state;
    private int framesCounter;

    public void Init()
    {
        rec = new(GetScreenWidth() / 2.0f, -100, 100, 100);
        rotation = 0.0f;
        alpha = 1.0f;

        state = 0;
        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        switch (state)
        {
            case 0:     // Move box down to center of screen
                framesCounter++;

                // NOTE: Remember that 3rd parameter of easing function refers to
                // desired value variation, do not confuse it with expected final value!
                rec.Y = Easings.EaseElasticOut(framesCounter, -100, GetScreenHeight() / 2.0f + 100, 120);

                if (framesCounter >= 120)
                {
                    framesCounter = 0;
                    state = 1;
                }
                break;
            case 1:     // Scale box to an horizontal bar
                framesCounter++;
                rec.Height = Easings.EaseBounceOut(framesCounter, 100, -90, 120);
                rec.Width = Easings.EaseBounceOut(framesCounter, 100, GetScreenWidth(), 120);

                if (framesCounter >= 120)
                {
                    framesCounter = 0;
                    state = 2;
                }
                break;
            case 2:     // Rotate horizontal bar rectangle
                framesCounter++;
                rotation = Easings.EaseQuadOut(framesCounter, 0.0f, 270.0f, 240);

                if (framesCounter >= 240)
                {
                    framesCounter = 0;
                    state = 3;
                }
                break;
            case 3:     // Increase bar size to fill all screen
                framesCounter++;
                rec.Height = Easings.EaseCircOut(framesCounter, 10, GetScreenWidth(), 120);

                if (framesCounter >= 120)
                {
                    framesCounter = 0;
                    state = 4;
                }
                break;
            case 4:     // Fade out animation
                framesCounter++;
                alpha = Easings.EaseSineOut(framesCounter, 1.0f, -1.0f, 160);

                if (framesCounter >= 160)
                {
                    framesCounter = 0;
                    state = 5;
                }
                break;
            default:
                break;
        }

        // Reset animation at any moment
        if (IsKeyPressed(KeyboardKey.Space))
        {
            rec = new Rectangle(GetScreenWidth() / 2.0f, -100, 100, 100);
            rotation = 0.0f;
            alpha = 1.0f;
            state = 0;
            framesCounter = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawRectanglePro(
            rec,
            new Vector2(rec.Width / 2, rec.Height / 2),
            rotation,
            Fade(Color.Black, alpha)
        );
        DrawText("PRESS [SPACE] TO RESET BOX ANIMATION!", 10, GetScreenHeight() - 25, 20, Color.LightGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - easings box");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new EasingsBoxAnim();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
