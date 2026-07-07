/*******************************************************************************************
*
*   raylib [shapes] example - math angle rotation
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 5.6
*
*   Example contributed by Kris (@krispy-snacc) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Kris (@krispy-snacc)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class MathAngleRotation : IExample
{
    private const int screenWidth = 720;
    private const int screenHeight = 400;

    public string Name => "Shapes / Math Angle Rotation";

    public string Title => "raylib [shapes] example - math angle rotation";

    public int Width => screenWidth;

    public int Height => screenHeight;

    private Vector2 center;
    private const float lineLength = 150.0f;

    // Predefined angles for fixed lines
    private int[] angles;
    private int numAngles;

    private float totalAngle; // Animated rotation angle

    public void Init()
    {
        center = new Vector2(screenWidth/2.0f, screenHeight/2.0f);

        // Predefined angles for fixed lines
        angles = new[] { 0, 30, 60, 90 };
        numAngles = angles.Length;

        totalAngle = 0.0f; // Animated rotation angle
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        totalAngle += 1.0f; // degrees per frame
        if (totalAngle >= 360.0f) totalAngle -= 360.0f;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
            ClearBackground(Color.White);

            DrawText("Fixed angles + rotating line", 10, 10, 20, Color.LightGray);

            // Draw fixed-angle lines with colorful gradient
            for (int i = 0; i < numAngles; i++)
            {
                float rad = angles[i]*DEG2RAD;
                Vector2 end = new Vector2(center.X + MathF.Cos(rad)*lineLength,
                                          center.Y + MathF.Sin(rad)*lineLength);

                // Gradient color from green → cyan → blue → magenta
                Color col;
                switch(i)
                {
                    case 0: col = Color.Green; break;
                    case 1: col = Color.Orange; break;
                    case 2: col = Color.Blue; break;
                    case 3: col = Color.Magenta; break;
                    default: col = Color.White; break;
                }

                DrawLineEx(center, end, 5.0f, col);

                // Draw angle label slightly offset along the line
                Vector2 textPos = new Vector2(center.X + MathF.Cos(rad)*(lineLength + 20),
                                              center.Y + MathF.Sin(rad)*(lineLength + 20));
                DrawText($"{angles[i]}°", (int)textPos.X, (int)textPos.Y, 20, col);
            }

            // Draw animated rotating line with changing color
            float animRad = totalAngle*DEG2RAD;
            Vector2 animEnd = new Vector2(center.X + MathF.Cos(animRad)*lineLength,
                                          center.Y + MathF.Sin(animRad)*lineLength);

            // Cycle through HSV colors for animated line
            Color animCol = ColorFromHSV(totalAngle % 360.0f, 0.8f, 0.9f);
            DrawLineEx(center, animEnd, 5.0f, animCol);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - math angle rotation");
        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new MathAngleRotation();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
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
