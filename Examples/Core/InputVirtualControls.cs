/*******************************************************************************************
 *
 *   raylib [core] example - input virtual controls
 *
 *   Example complexity rating: [★★☆☆] 2/4
 *
 *   Example originally created with raylib 5.0, last time updated with raylib 5.0
 *
 *   Example contributed by GreenSnakeLinux (@GreenSnakeLinux),
 *   reviewed by Ramon Santamaria (@raysan5), oblerion (@oblerion) and danilwhale (@danilwhale)
 *
 *   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
 *   BSD-like license that allows static linking with closed source software
 *
 *   Copyright (c) 2024-2025 GreenSnakeLinux (@GreenSnakeLinux) and Ramon Santamaria (@raysan5)
 *
 ********************************************************************************************/

using System.Numerics;

namespace Examples.Core;

using static Raylib_cs.Raylib;

public enum PadButton
{
    BUTTON_NONE = -1,
    BUTTON_UP,
    BUTTON_LEFT,
    BUTTON_RIGHT,
    BUTTON_DOWN,
    BUTTON_MAX
}

public class InputVirtualControls
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - input virtual controls");

        Vector2 padPosition = new Vector2(100, 350);
        float buttonRadius = 30;

        Vector2[] buttonPositions =
        [
            new Vector2(
                padPosition.X,padPosition.Y - buttonRadius * 1.5f
            ), // Up
            new Vector2(
                padPosition.X - buttonRadius * 1.5f, padPosition.Y
            ), // Left
            new Vector2(
                padPosition.X + buttonRadius * 1.5f, padPosition.Y
            ), // Right
            new Vector2(
                padPosition.X, padPosition.Y + buttonRadius * 1.5f
            ) // Down
        ];

        Vector2[][] arrowTris = [
            // Up
            [
                new Vector2(
                    buttonPositions[0].X, buttonPositions[0].Y - 12
                ),
                new Vector2(
                    buttonPositions[0].X - 9, buttonPositions[0].Y + 9
                ),
                new Vector2(
                    buttonPositions[0].X + 9, buttonPositions[0].Y + 9
                )
            ],
            // Left
            [
                new Vector2(
                    buttonPositions[1].X + 9, buttonPositions[1].Y - 9
                ),
                new Vector2(
                    buttonPositions[1].X - 12, buttonPositions[1].Y
                ),
                new Vector2(
                    buttonPositions[1].X + 9, buttonPositions[1].Y + 9
                )
            ],
            // Right
            [
                new Vector2(
                    buttonPositions[2].X + 12, buttonPositions[2].Y
                ),
                new Vector2(
                    buttonPositions[2].X - 9, buttonPositions[2].Y - 9
                ),
                new Vector2(
                    buttonPositions[2].X - 9, buttonPositions[2].Y + 9
                )
            ],
            // Down
            [
                new Vector2(
                    buttonPositions[3].X - 9, buttonPositions[3].Y - 9
                ),
                new Vector2(
                    buttonPositions[3].X, buttonPositions[3].Y + 12
                ),
                new Vector2(
                    buttonPositions[3].X + 9, buttonPositions[3].Y - 9
                )
            ]
        ]
        ;

        Color[] buttonLabelColors = [
            Color.Yellow, // Up
            Color.Blue, // Left
            Color.Red, // Right
            Color.Green // Down
        ];

        int pressedButton = (int)PadButton.BUTTON_NONE;
        Vector2 inputPosition = new Vector2(0, 0);

        Vector2 playerPosition = new Vector2((float)screenWidth / 2, (float)screenHeight / 2);
        float playerSpeed = 75f;

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose()) // Detect window close button or ESC key
        {
            // Update
            //--------------------------------------------------------------------------
            if ((GetTouchPointCount() > 0))
            {
                inputPosition = GetTouchPosition(0); // Use touch position
            }
            else
            {
                inputPosition = GetMousePosition(); // Use mouse position
            }

            // Reset pressed button to none
            pressedButton = (int)PadButton.BUTTON_NONE;

            // Make sure user is pressing left mouse button if they're from desktop
            if ((GetTouchPointCount() > 0) ||
                ((GetTouchPointCount() == 0) && IsMouseButtonDown(MouseButton.Left)))
            {
                // Find nearest D-Pad button to the input position
                for (int i = 0; i < (int)PadButton.BUTTON_MAX; i++)
                {
                    float distX = MathF.Abs(buttonPositions[i].X - inputPosition.X);
                    float distY = MathF.Abs(buttonPositions[i].Y - inputPosition.Y);

                    if ((distX + distY < buttonRadius))
                    {
                        pressedButton = i;
                        break;
                    }
                }
            }

            // Move player according to pressed button
            switch ((PadButton)pressedButton)
            {
                case PadButton.BUTTON_UP:
                    playerPosition.Y -= playerSpeed * GetFrameTime();
                    break;
                case PadButton.BUTTON_LEFT:
                    playerPosition.X -= playerSpeed * GetFrameTime();
                    break;
                case PadButton.BUTTON_RIGHT:
                    playerPosition.X += playerSpeed * GetFrameTime();
                    break;
                case PadButton.BUTTON_DOWN:
                    playerPosition.Y += playerSpeed * GetFrameTime();
                    break;
                default:
                    break;
            }

            ;
            //--------------------------------------------------------------------------

            // Draw
            //--------------------------------------------------------------------------
            BeginDrawing();

            ClearBackground(Color.RayWhite);

            // Draw world
            DrawCircleV(playerPosition, 50, Color.Maroon);

            // Draw GUI
            for (int i = 0; i < (int)PadButton.BUTTON_MAX; i++)
            {
                DrawCircleV(buttonPositions[i], buttonRadius, (i == pressedButton) ? Color.DarkGray : Color.Black);

                DrawTriangle(
                    arrowTris[i][0],
                    arrowTris[i][1],
                    arrowTris[i][2],
                    buttonLabelColors[i]
                );
            }

            DrawText("move the player with D-Pad buttons", 10, 10, 20, Color.DarkGray);

            EndDrawing();
            //--------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow(); // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
