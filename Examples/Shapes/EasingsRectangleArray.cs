/*******************************************************************************************
*
*   raylib [shapes] example - easings rectangles
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires 'easings.h' library, provided on raylib/src. Just copy
*   the library to same directory as example or make sure it's available on include path
*
*   Example originally created with raylib 2.0, last time updated with raylib 2.5
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

public partial class EasingsRectangleArray : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int RecsWidth = 50;
    public const int RecsHeight = 50;
    public const int MaxRecsX = 800 / RecsWidth;
    public const int MaxRecsY = 450 / RecsHeight;

    // At 60 fps = 4 seconds
    public const int PlayTimeInFrames = 240;

    public string Name => "Shapes / Easings Rectangle Array";

    public string Title => "raylib [shapes] example - easings rectangles";

    private Rectangle[] recs;
    private float rotation;
    private int framesCounter;
    private int state;                  // Rectangles animation state: 0-Playing, 1-Finished

    public void Init()
    {
        recs = new Rectangle[MaxRecsX * MaxRecsY];

        for (var y = 0; y < MaxRecsY; y++)
        {
            for (var x = 0; x < MaxRecsX; x++)
            {
                recs[y * MaxRecsX + x].X = RecsWidth / 2.0f + RecsWidth * x;
                recs[y * MaxRecsX + x].Y = RecsHeight / 2.0f + RecsHeight * y;
                recs[y * MaxRecsX + x].Width = RecsWidth;
                recs[y * MaxRecsX + x].Height = RecsHeight;
            }
        }

        rotation = 0.0f;
        framesCounter = 0;
        state = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (state == 0)
        {
            framesCounter++;

            for (var i = 0; i < MaxRecsX * MaxRecsY; i++)
            {
                recs[i].Height = Easings.EaseCircOut(framesCounter, RecsHeight, -RecsHeight, PlayTimeInFrames);
                recs[i].Width = Easings.EaseCircOut(framesCounter, RecsWidth, -RecsWidth, PlayTimeInFrames);

                if (recs[i].Height < 0)
                {
                    recs[i].Height = 0;
                }
                if (recs[i].Width < 0)
                {
                    recs[i].Width = 0;
                }

                // Finish playing
                if ((recs[i].Height == 0) && (recs[i].Width == 0))
                {
                    state = 1;
                }
                rotation = Easings.EaseLinearIn(framesCounter, 0.0f, 360.0f, PlayTimeInFrames);
            }
        }
        else if ((state == 1) && IsKeyPressed(KeyboardKey.Space))
        {
            // When animation has finished, press space to restart
            framesCounter = 0;

            for (var i = 0; i < MaxRecsX * MaxRecsY; i++)
            {
                recs[i].Height = RecsHeight;
                recs[i].Width = RecsWidth;
            }

            state = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        if (state == 0)
        {
            for (var i = 0; i < MaxRecsX * MaxRecsY; i++)
            {
                DrawRectanglePro(
                    recs[i],
                    new Vector2(recs[i].Width / 2, recs[i].Height / 2),
                    rotation,
                    Color.Red
                );
            }
        }
        else if (state == 1)
        {
            DrawText("PRESS [SPACE] TO PLAY AGAIN!", 240, 200, 20, Color.Gray);
        }

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - easings rectangles");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new EasingsRectangleArray();
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
