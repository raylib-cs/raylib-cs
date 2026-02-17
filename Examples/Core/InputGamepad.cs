/*******************************************************************************************
*
*   raylib [core] example - Gamepad input
*
*   NOTE: This example requires a Gamepad connected to the system
*         raylib is configured to work with the following gamepads:
*                - Xbox 360 Controller (Xbox 360, Xbox One)
*                - PLAYSTATION(R)3 Controller
*         Check raylib.h for buttons configuration
*
*   This example has been created using raylib 1.6 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2013-2016 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class InputGamepad
{
    // NOTE: Gamepad name ID depends on drivers and OS
    // These are some possible names the gamepads could have.
    public const string XBOX360_LEGACY_NAME_ID = "Xbox Controller";
    public const string XBOX360_NAME_ID = "Xbox 360 Controller";
    public const string XBOX360_NAME_ID_RPI = "Microsoft X-Box 360 pad";
    public const string PS3_NAME_ID = "PLAYSTATION(R)3 Controller";

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        // Set MSAA 4X hint before windows creation
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [core] example - gamepad input");

        Texture2D texPs3Pad = LoadTexture("resources/ps3.png");
        Texture2D texXboxPad = LoadTexture("resources/xbox.png");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            // ...
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            if (IsGamepadAvailable(0))
            {
                string gamepadName = GetGamepadName_(0);
                DrawText($"GP1: {gamepadName}", 10, 10, 10, Color.Black);

                if (gamepadName == XBOX360_LEGACY_NAME_ID ||
                    gamepadName == XBOX360_NAME_ID ||
                    gamepadName == XBOX360_NAME_ID_RPI)
                {
                    DrawTexture(texXboxPad, 0, 0, Color.DarkGray);

                    // Draw buttons: xbox home
                    if (IsGamepadButtonDown(0, GamepadButton.Middle))
                    {
                        DrawCircle(394, 89, 19, Color.Red);
                    }

                    // Draw buttons: basic
                    if (IsGamepadButtonDown(0, GamepadButton.MiddleRight))
                    {
                        DrawCircle(436, 150, 9, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.MiddleLeft))
                    {
                        DrawCircle(352, 150, 9, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceLeft))
                    {
                        DrawCircle(501, 151, 15, Color.Blue);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceDown))
                    {
                        DrawCircle(536, 187, 15, Color.Lime);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceRight))
                    {
                        DrawCircle(572, 151, 15, Color.Maroon);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceUp))
                    {
                        DrawCircle(536, 115, 15, Color.Gold);
                    }

                    // Draw buttons: d-pad
                    DrawRectangle(317, 202, 19, 71, Color.Black);
                    DrawRectangle(293, 228, 69, 19, Color.Black);
                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceUp))
                    {
                        DrawRectangle(317, 202, 19, 26, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceDown))
                    {
                        DrawRectangle(317, 202 + 45, 19, 26, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceLeft))
                    {
                        DrawRectangle(292, 228, 25, 19, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceRight))
                    {
                        DrawRectangle(292 + 44, 228, 26, 19, Color.Red);
                    }

                    // Draw buttons: left-right back
                    if (IsGamepadButtonDown(0, GamepadButton.LeftTrigger1))
                    {
                        DrawCircle(259, 61, 20, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightTrigger1))
                    {
                        DrawCircle(536, 61, 20, Color.Red);
                    }

                    // Draw axis: left joystick
                    DrawCircle(259, 152, 39, Color.Black);
                    DrawCircle(259, 152, 34, Color.LightGray);
                    DrawCircle(
                        259 + (int)(GetGamepadAxisMovement(0, GamepadAxis.LeftX) * 20),
                        152 + (int)(GetGamepadAxisMovement(0, GamepadAxis.LeftY) * 20),
                        25,
                        Color.Black
                    );

                    // Draw axis: right joystick
                    DrawCircle(461, 237, 38, Color.Black);
                    DrawCircle(461, 237, 33, Color.LightGray);
                    DrawCircle(
                        461 + (int)(GetGamepadAxisMovement(0, GamepadAxis.RightX) * 20),
                        237 + (int)(GetGamepadAxisMovement(0, GamepadAxis.RightY) * 20),
                        25, Color.Black
                    );

                    // Draw axis: left-right triggers
                    float leftTriggerX = GetGamepadAxisMovement(0, GamepadAxis.LeftTrigger);
                    float rightTriggerX = GetGamepadAxisMovement(0, GamepadAxis.RightTrigger);
                    DrawRectangle(170, 30, 15, 70, Color.Gray);
                    DrawRectangle(604, 30, 15, 70, Color.Gray);
                    DrawRectangle(170, 30, 15, (int)(((1.0f + leftTriggerX) / 2.0f) * 70), Color.Red);
                    DrawRectangle(604, 30, 15, (int)(((1.0f + rightTriggerX) / 2.0f) * 70), Color.Red);
                }
                else if (gamepadName == PS3_NAME_ID)
                {
                    DrawTexture(texPs3Pad, 0, 0, Color.DarkGray);

                    // Draw buttons: ps
                    if (IsGamepadButtonDown(0, GamepadButton.Middle))
                    {
                        DrawCircle(396, 222, 13, Color.Red);
                    }

                    // Draw buttons: basic
                    if (IsGamepadButtonDown(0, GamepadButton.MiddleLeft))
                    {
                        DrawRectangle(328, 170, 32, 13, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.MiddleRight))
                    {
                        DrawTriangle(
                            new Vector2(436, 168),
                            new Vector2(436, 185),
                            new Vector2(464, 177),
                            Color.Red
                        );
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceUp))
                    {
                        DrawCircle(557, 144, 13, Color.Lime);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceRight))
                    {
                        DrawCircle(586, 173, 13, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceDown))
                    {
                        DrawCircle(557, 203, 13, Color.Violet);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightFaceLeft))
                    {
                        DrawCircle(527, 173, 13, Color.Pink);
                    }

                    // Draw buttons: d-pad
                    DrawRectangle(225, 132, 24, 84, Color.Black);
                    DrawRectangle(195, 161, 84, 25, Color.Black);
                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceUp))
                    {
                        DrawRectangle(225, 132, 24, 29, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceDown))
                    {
                        DrawRectangle(225, 132 + 54, 24, 30, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceLeft))
                    {
                        DrawRectangle(195, 161, 30, 25, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.LeftFaceRight))
                    {
                        DrawRectangle(195 + 54, 161, 30, 25, Color.Red);
                    }

                    // Draw buttons: left-right back buttons
                    if (IsGamepadButtonDown(0, GamepadButton.LeftTrigger1))
                    {
                        DrawCircle(239, 82, 20, Color.Red);
                    }

                    if (IsGamepadButtonDown(0, GamepadButton.RightTrigger1))
                    {
                        DrawCircle(557, 82, 20, Color.Red);
                    }

                    // Draw axis: left joystick
                    DrawCircle(319, 255, 35, Color.Black);
                    DrawCircle(319, 255, 31, Color.LightGray);
                    DrawCircle(
                        319 + (int)(GetGamepadAxisMovement(0, GamepadAxis.LeftX) * 20),
                        255 + (int)(GetGamepadAxisMovement(0, GamepadAxis.LeftY) * 20),
                        25,
                        Color.Black
                    );

                    // Draw axis: right joystick
                    DrawCircle(475, 255, 35, Color.Black);
                    DrawCircle(475, 255, 31, Color.LightGray);
                    DrawCircle(
                        475 + (int)(GetGamepadAxisMovement(0, GamepadAxis.RightX) * 20),
                        255 + (int)(GetGamepadAxisMovement(0, GamepadAxis.RightY) * 20),
                        25,
                        Color.Black
                    );

                    // Draw axis: left-right triggers
                    float leftTriggerX = GetGamepadAxisMovement(0, GamepadAxis.LeftTrigger);
                    float rightTriggerX = GetGamepadAxisMovement(0, GamepadAxis.RightTrigger);
                    DrawRectangle(169, 48, 15, 70, Color.Gray);
                    DrawRectangle(611, 48, 15, 70, Color.Gray);
                    DrawRectangle(169, 48, 15, (int)(((1.0f - leftTriggerX) / 2.0f) * 70), Color.Red);
                    DrawRectangle(611, 48, 15, (int)(((1.0f - rightTriggerX) / 2.0f) * 70), Color.Red);
                }
                else
                {
                    DrawText("- GENERIC GAMEPAD -", 280, 180, 20, Color.Gray);
                    // TODO: Draw generic gamepad
                }

                DrawText($"DETECTED AXIS [{GetGamepadAxisCount(0)}]:", 10, 50, 10, Color.Maroon);

                for (int i = 0; i < GetGamepadAxisCount(0); i++)
                {
                    DrawText(
                        $"AXIS {i}: {GetGamepadAxisMovement(0, (GamepadAxis)i)}",
                        20,
                        70 + 20 * i,
                        10,
                        Color.DarkGray
                    );
                }

                if (GetGamepadButtonPressed() != (int)GamepadButton.Unknown)
                {
                    DrawText($"DETECTED BUTTON: {GetGamepadButtonPressed()}", 10, 430, 10, Color.Red);
                }
                else
                {
                    DrawText("DETECTED BUTTON: NONE", 10, 430, 10, Color.Gray);
                }
            }
            else
            {
                DrawText("GP1: NOT DETECTED", 10, 10, 10, Color.Gray);
                DrawTexture(texXboxPad, 0, 0, Color.LightGray);
            }

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadTexture(texPs3Pad);
        UnloadTexture(texXboxPad);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
