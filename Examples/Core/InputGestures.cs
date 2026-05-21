/*******************************************************************************************
*
*   raylib [core] example - input gestures
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.4, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class InputGestures
{
    public const int MaxGestureStrings = 20;

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - input gestures");

        Vector2 touchPosition = new(0, 0);
        Rectangle touchArea = new(220, 10, screenWidth - 230, screenHeight - 20);

        int gesturesCount = 0;
        string[] gestureStrings = new string[MaxGestureStrings];

        Gesture currentGesture = Gesture.None;
        Gesture lastGesture = Gesture.None;

        // SetGesturesEnabled(0b0000000000001001);   // Enable only some gestures to be detected

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            lastGesture = currentGesture;
            currentGesture = GetGestureDetected();
            touchPosition = GetTouchPosition(0);

            if (CheckCollisionPointRec(touchPosition, touchArea) && (currentGesture != Gesture.None))
            {
                if (currentGesture != lastGesture)
                {
                    // Store gesture string
                    switch ((Gesture)currentGesture)
                    {
                        case Gesture.Tap:
                            gestureStrings[gesturesCount] = "GESTURE TAP";
                            break;
                        case Gesture.DoubleTap:
                            gestureStrings[gesturesCount] = "GESTURE DOUBLETAP";
                            break;
                        case Gesture.Hold:
                            gestureStrings[gesturesCount] = "GESTURE HOLD";
                            break;
                        case Gesture.Drag:
                            gestureStrings[gesturesCount] = "GESTURE DRAG";
                            break;
                        case Gesture.SwipeRight:
                            gestureStrings[gesturesCount] = "GESTURE SWIPE RIGHT";
                            break;
                        case Gesture.SwipeLeft:
                            gestureStrings[gesturesCount] = "GESTURE SWIPE LEFT";
                            break;
                        case Gesture.SwipeUp:
                            gestureStrings[gesturesCount] = "GESTURE SWIPE UP";
                            break;
                        case Gesture.SwipeDown:
                            gestureStrings[gesturesCount] = "GESTURE SWIPE DOWN";
                            break;
                        case Gesture.PinchIn:
                            gestureStrings[gesturesCount] = "GESTURE PINCH IN";
                            break;
                        case Gesture.PinchOut:
                            gestureStrings[gesturesCount] = "GESTURE PINCH OUT";
                            break;
                        default:
                            break;
                    }

                    gesturesCount++;

                    // Reset gestures strings
                    if (gesturesCount >= MaxGestureStrings)
                    {
                        for (int i = 0; i < MaxGestureStrings; i++)
                        {
                            gestureStrings[i] = " ";
                        }
                        gesturesCount = 0;
                    }
                }
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawRectangleRec(touchArea, Color.Gray);
            DrawRectangle(225, 15, screenWidth - 240, screenHeight - 30, Color.RayWhite);

            DrawText("GESTURES TEST AREA", screenWidth - 270, screenHeight - 40, 20, ColorAlpha(Color.Gray, 0.5f));

            for (int i = 0; i < gesturesCount; i++)
            {
                if (i % 2 == 0)
                {
                    DrawRectangle(10, 30 + 20 * i, 200, 20, ColorAlpha(Color.LightGray, 0.5f));
                }
                else
                {
                    DrawRectangle(10, 30 + 20 * i, 200, 20, ColorAlpha(Color.LightGray, 0.3f));
                }

                if (i < gesturesCount - 1)
                {
                    DrawText(gestureStrings[i], 35, 36 + 20 * i, 10, Color.DarkGray);
                }
                else
                {
                    DrawText(gestureStrings[i], 35, 36 + 20 * i, 10, Color.Maroon);
                }
            }

            DrawRectangleLines(10, 29, 200, screenHeight - 50, Color.Gray);
            DrawText("DETECTED GESTURES", 50, 15, 10, Color.Gray);

            if (currentGesture != Gesture.None)
            {
                DrawCircleV(touchPosition, 30, Color.Maroon);
            }

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------
        return 0;
    }
}
