/*******************************************************************************************
   *
   *   raylib [core] example - input gamepad
   *
   *   Example complexity rating: [★☆☆☆] 1/4
   *
   *   NOTE: This example requires a Gamepad connected to the system
   *         raylib is configured to work with the following gamepads:
   *                - Xbox 360 Controller (Xbox 360, Xbox One)
   *                - PLAYSTATION(R)3 Controller
   *         Check raylib.h for buttons configuration
   *
   *   Example originally created with raylib 1.1, last time updated with raylib 4.2
   *
   *   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
   *   BSD-like license that allows static linking with closed source software
   *
   *   Copyright (c) 2013-2025 Ramon Santamaria (@raysan5)
   *
   ********************************************************************************************/


using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class InputGamepad
{
    // NOTE: Gamepad name ID depends on drivers and OS
    // These are some possible names the gamepads could have.
    public const string XBOX_ALIAS_1 = "xbox";
    public const string XBOX_ALIAS_2 = "x-box";
    public const string PS_ALIAS_1 = "playstation";
    public const string PS_ALIAS_2 = "sony";

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

        int gamepad = 0;
        Rectangle vibrateButton = new Rectangle();

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            if (IsKeyPressed(KeyboardKey.Left) && gamepad > 0)
            {
                gamepad--;
            }

            if (IsKeyPressed(KeyboardKey.Right))
            {
                gamepad++;
            }

            Vector2 mousePosition = GetMousePosition();

            vibrateButton = new Rectangle(10, 70.0f + 20 * GetGamepadAxisCount(gamepad) + 20, 75, 24);
            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mousePosition, vibrateButton))
            {
                SetGamepadVibration(gamepad, 1.0f, 1.0f, 1.0f);
            }

            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            if (IsGamepadAvailable(gamepad))
            {
                string gamepadName = GetGamepadName_(gamepad);
                DrawText($"GP{gamepad}: {gamepadName}", 10, 10, 10, Color.Black);

                if (gamepadName.Contains(XBOX_ALIAS_1, StringComparison.OrdinalIgnoreCase) ||
                    gamepadName.Contains(XBOX_ALIAS_2, StringComparison.OrdinalIgnoreCase))
                {
                    DrawTexture(texXboxPad, 0, 0, Color.DarkGray);

                    // Draw buttons: xbox home
                    if (IsGamepadButtonDown(gamepad, GamepadButton.Middle))
                    {
                        DrawCircle(394, 89, 19, Color.Red);
                    }

                    // Draw buttons: basic
                    if (IsGamepadButtonDown(gamepad, GamepadButton.MiddleRight))
                    {
                        DrawCircle(436, 150, 9, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.MiddleLeft))
                    {
                        DrawCircle(352, 150, 9, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceLeft))
                    {
                        DrawCircle(501, 151, 15, Color.Blue);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceDown))
                    {
                        DrawCircle(536, 187, 15, Color.Lime);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceRight))
                    {
                        DrawCircle(572, 151, 15, Color.Maroon);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceUp))
                    {
                        DrawCircle(536, 115, 15, Color.Gold);
                    }

                    // Draw buttons: d-pad
                    DrawRectangle(317, 202, 19, 71, Color.Black);
                    DrawRectangle(293, 228, 69, 19, Color.Black);
                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceUp))
                    {
                        DrawRectangle(317, 202, 19, 26, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceDown))
                    {
                        DrawRectangle(317, 202 + 45, 19, 26, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceLeft))
                    {
                        DrawRectangle(292, 228, 25, 19, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceRight))
                    {
                        DrawRectangle(292 + 44, 228, 26, 19, Color.Red);
                    }

                    // Draw buttons: left-right back
                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftTrigger1))
                    {
                        DrawCircle(259, 61, 20, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightTrigger1))
                    {
                        DrawCircle(536, 61, 20, Color.Red);
                    }

                    // Draw axis: left joystick
                    DrawCircle(259, 152, 39, Color.Black);
                    DrawCircle(259, 152, 34, Color.LightGray);
                    DrawCircle(
                        259 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.LeftX) * 20),
                        152 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.LeftY) * 20),
                        25,
                        Color.Black
                    );

                    // Draw axis: right joystick
                    DrawCircle(461, 237, 38, Color.Black);
                    DrawCircle(461, 237, 33, Color.LightGray);
                    DrawCircle(
                        461 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.RightX) * 20),
                        237 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.RightY) * 20),
                        25, Color.Black
                    );

                    // Draw axis: left-right triggers
                    float leftTriggerX = GetGamepadAxisMovement(gamepad, GamepadAxis.LeftTrigger);
                    float rightTriggerX = GetGamepadAxisMovement(gamepad, GamepadAxis.RightTrigger);
                    DrawRectangle(170, 30, 15, 70, Color.Gray);
                    DrawRectangle(604, 30, 15, 70, Color.Gray);
                    DrawRectangle(170, 30, 15, (int)(((1.0f + leftTriggerX) / 2.0f) * 70), Color.Red);
                    DrawRectangle(604, 30, 15, (int)(((1.0f + rightTriggerX) / 2.0f) * 70), Color.Red);
                }
                else if (gamepadName.Contains(PS_ALIAS_1, StringComparison.OrdinalIgnoreCase) || gamepadName.Contains(PS_ALIAS_2, StringComparison.OrdinalIgnoreCase))
                {
                    DrawTexture(texPs3Pad, 0, 0, Color.DarkGray);

                    // Draw buttons: ps
                    if (IsGamepadButtonDown(gamepad, GamepadButton.Middle))
                    {
                        DrawCircle(396, 222, 13, Color.Red);
                    }

                    // Draw buttons: basic
                    if (IsGamepadButtonDown(gamepad, GamepadButton.MiddleLeft))
                    {
                        DrawRectangle(328, 170, 32, 13, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.MiddleRight))
                    {
                        DrawTriangle(
                            new Vector2(436, 168),
                            new Vector2(436, 185),
                            new Vector2(464, 177),
                            Color.Red
                        );
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceUp))
                    {
                        DrawCircle(557, 144, 13, Color.Lime);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceRight))
                    {
                        DrawCircle(586, 173, 13, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceDown))
                    {
                        DrawCircle(557, 203, 13, Color.Violet);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightFaceLeft))
                    {
                        DrawCircle(527, 173, 13, Color.Pink);
                    }

                    // Draw buttons: d-pad
                    DrawRectangle(225, 132, 24, 84, Color.Black);
                    DrawRectangle(195, 161, 84, 25, Color.Black);
                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceUp))
                    {
                        DrawRectangle(225, 132, 24, 29, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceDown))
                    {
                        DrawRectangle(225, 132 + 54, 24, 30, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceLeft))
                    {
                        DrawRectangle(195, 161, 30, 25, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftFaceRight))
                    {
                        DrawRectangle(195 + 54, 161, 30, 25, Color.Red);
                    }

                    // Draw buttons: left-right back buttons
                    if (IsGamepadButtonDown(gamepad, GamepadButton.LeftTrigger1))
                    {
                        DrawCircle(239, 82, 20, Color.Red);
                    }

                    if (IsGamepadButtonDown(gamepad, GamepadButton.RightTrigger1))
                    {
                        DrawCircle(557, 82, 20, Color.Red);
                    }

                    // Draw axis: left joystick
                    DrawCircle(319, 255, 35, Color.Black);
                    DrawCircle(319, 255, 31, Color.LightGray);
                    DrawCircle(
                        319 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.LeftX) * 20),
                        255 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.LeftY) * 20),
                        25,
                        Color.Black
                    );

                    // Draw axis: right joystick
                    DrawCircle(475, 255, 35, Color.Black);
                    DrawCircle(475, 255, 31, Color.LightGray);
                    DrawCircle(
                        475 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.RightX) * 20),
                        255 + (int)(GetGamepadAxisMovement(gamepad, GamepadAxis.RightY) * 20),
                        25,
                        Color.Black
                    );

                    // Draw axis: left-right triggers
                    float leftTriggerX = GetGamepadAxisMovement(gamepad, GamepadAxis.LeftTrigger);
                    float rightTriggerX = GetGamepadAxisMovement(gamepad, GamepadAxis.RightTrigger);
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

                DrawText($"DETECTED AXIS [{GetGamepadAxisCount(gamepad)}]:", 10, 50, 10, Color.Maroon);

                for (int i = 0; i < GetGamepadAxisCount(gamepad); i++)
                {
                    DrawText(
                        $"AXIS {i}: {GetGamepadAxisMovement(gamepad, (GamepadAxis)i)}",
                        20,
                        70 + 20 * i,
                        10,
                        Color.DarkGray
                    );
                }

                DrawRectangleRec(vibrateButton, Color.SkyBlue);
                DrawText("VIBRATE", (int)(vibrateButton.X + 14), (int)(vibrateButton.Y + 1), 10, Color.DarkGray);


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
                DrawText($"GP{gamepad}: NOT DETECTED", 10, 10, 10, Color.Gray);
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
