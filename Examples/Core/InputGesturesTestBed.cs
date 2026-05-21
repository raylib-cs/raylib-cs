/*******************************************************************************************
 *
 *   raylib [core] example - input gestures testbed
 *
 *   Example complexity rating: [★★★☆] 3/4
 *
 *   Example originally created with raylib 5.0, last time updated with raylib 6.0
 *
 *   Example contributed by ubkp (@ubkp) and reviewed by Ramon Santamaria (@raysan5)
 *
 *   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
 *   BSD-like license that allows static linking with closed source software
 *
 *   Copyright (c) 2023-2025 ubkp (@ubkp)
 *
 ********************************************************************************************/

using System.Numerics;

namespace Examples.Core;

using static Raylib_cs.Raylib;

public class InputGesturesTestBed
{
    public const int GESTURE_LOG_SIZE = 20;
    public const int MAX_TOUCH_COUNT = 32;

    //------------------------------------------------------------------------------------
    // Program main entry point
    //------------------------------------------------------------------------------------

    public static int Main()
    {
// Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - input gestures testbed");

        Vector2 messagePosition = new Vector2( 160, 7 );

        // Last gesture variables definitions
        Gesture lastGesture = 0;
        Vector2 lastGesturePosition = new Vector2( 165, 130 );

        // Gesture log variables definitions
        // NOTE: The gesture log uses an array (as an inverted circular queue) to store the performed gestures
        string[] gestureLog = new string[GESTURE_LOG_SIZE + 1];
        for (int  i= 0; i < GESTURE_LOG_SIZE; i++)
        {
            gestureLog[i] = new string(new char[12]);
        }
        ;
        // NOTE: The index for the inverted circular queue (moving from last to first direction, then looping around)
        int gestureLogIndex = GESTURE_LOG_SIZE;
        Gesture previousGesture = 0;

        // Log mode values:
        // - 0 shows repeated events
        // - 1 hides repeated events
        // - 2 shows repeated events but hide hold events
        // - 3 hides repeated events and hide hold events
        int logMode = 1;

        Color gestureColor = new Color(0, 0, 0, 255 );
        Rectangle logButton1 = new Rectangle( 53, 7, 48, 26 );
        Rectangle logButton2 = new Rectangle( 108, 7, 36, 26 );
        Vector2 gestureLogPosition = new Vector2( 10, 10 );

        // Protractor variables definitions
        float angleLength = 90.0f;
        float currentAngleDegrees = 0.0f;
        Vector2 finalVector = new Vector2( 0.0f, 0.0f );
        Vector2 protractorPosition = new Vector2( 266.0f, 315.0f );

        SetTargetFPS(60); // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose()) // Detect window close button or ESC key
        {
            // Update
            //--------------------------------------------------------------------------------------
            // Handle common gestures data
            int i, ii; // Iterators that will be reused by all for loops
            Gesture currentGesture = GetGestureDetected();
            float currentDragDegrees = GetGestureDragAngle();
            float currentPitchDegrees = GetGesturePinchAngle();
            int touchCount = GetTouchPointCount();

            // Handle last gesture
            if ((currentGesture != 0) && ((int)currentGesture != 4) && (currentGesture != previousGesture))
            {
                lastGesture = currentGesture; // Filter the meaningful gestures (1, 2, 8 to 512) for the display
            }

            // Handle gesture log
            if (IsMouseButtonReleased(MouseButton.Left))
            {
                if (CheckCollisionPointRec(GetMousePosition(), logButton1))
                {
                    switch (logMode)
                    {
                        case 3: logMode = 2; break;
                        case 2: logMode = 3; break;
                        case 1: logMode = 0; break;
                        default: logMode = 1; break;
                    }
                }
                else if (CheckCollisionPointRec(GetMousePosition(), logButton2))
                {
                    switch (logMode)
                    {
                        case 3: logMode = 1; break;
                        case 2: logMode = 0; break;
                        case 1: logMode = 3; break;
                        default: logMode = 2; break;
                    }
                }
            }

            int fillLog = 0; // Gate variable to be used to allow or not the gesture log to be filled
            if (currentGesture != 0)
            {
                if (logMode == 3) // 3 hides repeated events and hide hold events
                {
                    if ((((int)currentGesture != 4) && (currentGesture != previousGesture)) || ((int)currentGesture < 3))
                    {
                        fillLog = 1;
                    }
                }
                else if (logMode == 2) // 2 shows repeated events but hide hold events
                {
                    if ((int)currentGesture != 4)
                    {
                        fillLog = 1;
                    }
                }
                else if (logMode == 1) // 1 hides repeated events
                {
                    if (currentGesture != previousGesture)
                    {
                        fillLog = 1;
                    }
                }
                else // 0 shows repeated events
                {
                    fillLog = 1;
                }
            }

            if (fillLog > 0) // If one of the conditions from logMode was met, fill the gesture log
            {
                previousGesture = currentGesture;
                gestureColor = GetGestureColor((int)currentGesture);
                if (gestureLogIndex <= 0)
                {
                    gestureLogIndex = GESTURE_LOG_SIZE;
                }
                gestureLogIndex--;

                // Copy the gesture respective name to the gesture log array
                gestureLog[gestureLogIndex] = GetGestureName((int)currentGesture);
            }

            // Handle protractor
            if ((int)currentGesture > 255)
            {
                currentAngleDegrees = currentPitchDegrees; // Pinch In and Pinch Out
            }
            else if ((int)currentGesture > 15)
            {
                currentAngleDegrees = currentDragDegrees; // Swipe Right, Swipe Left, Swipe Up and Swipe Down
            }
            else if (currentGesture > 0)
            {
                currentAngleDegrees = 0.0f; // Tap, Doubletap, Hold and Grab
            }

            float currentAngleRadians =
                ((currentAngleDegrees + 90.0f) * MathF.PI / 180); // Convert the current angle to Radians
            // Calculate the final vector for display
            finalVector = new Vector2(
                (angleLength * MathF.Sin(currentAngleRadians)) + protractorPosition.X,
                (angleLength * MathF.Cos(currentAngleRadians)) + protractorPosition.Y
            )
            ;

            // Handle touch and mouse pointer points
            Vector2[] touchPosition = new Vector2[MAX_TOUCH_COUNT];

            Vector2 mousePosition = Vector2.Zero;
            if (currentGesture != Gesture.None)
            {
                if (touchCount != 0)
                {
                    for (i = 0; i < touchCount; i++)
                    {
                        touchPosition[i] = GetTouchPosition(i); // Fill the touch positions
                    }
                }
                else
                {
                    mousePosition = GetMousePosition();
                }
            }
            //--------------------------------------------------------------------------------------

            // Draw
            //--------------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            // Draw common elements
            DrawText("*", (int)messagePosition.X + 5, (int)messagePosition.Y + 5, 10, Color.Black);
            DrawText("Example optimized for Web/HTML5\non Smartphones with Touch Screen.", (int)messagePosition.X + 15,
                (int)messagePosition.Y + 5, 10, Color.Black);
            DrawText("*", (int)messagePosition.X + 5, (int)messagePosition.Y + 35, 10, Color.Black);
            DrawText("While running on Desktop Web Browsers,\ninspect and turn on Touch Emulation.",
                (int)messagePosition.X + 15, (int)messagePosition.Y + 35, 10, Color.Black);

            // Draw last gesture
            DrawText("Last gesture", (int)lastGesturePosition.X + 33, (int)lastGesturePosition.Y - 47, 20, Color.Black);
            DrawText("Swipe         Tap       Pinch  Touch", (int)lastGesturePosition.X + 17,
                (int)lastGesturePosition.Y - 18, 10, Color.Black);
            DrawRectangle((int)lastGesturePosition.X + 20, (int)lastGesturePosition.Y, 20, 20,
                lastGesture == Gesture.SwipeUp ? Color.Red : Color.LightGray);
            DrawRectangle((int)lastGesturePosition.X, (int)lastGesturePosition.Y + 20, 20, 20,
                lastGesture == Gesture.SwipeLeft ? Color.Red : Color.LightGray);
            DrawRectangle((int)lastGesturePosition.X + 40, (int)lastGesturePosition.Y + 20, 20, 20,
                lastGesture == Gesture.SwipeRight ? Color.Red : Color.LightGray);
            DrawRectangle((int)lastGesturePosition.X + 20, (int)lastGesturePosition.Y + 40, 20, 20,
                lastGesture == Gesture.SwipeDown ? Color.Red : Color.LightGray);
            DrawCircle((int)lastGesturePosition.X + 80, (int)lastGesturePosition.Y + 16, 10,
                lastGesture == Gesture.Tap ? Color.Blue : Color.LightGray);
            DrawRing(new Vector2(
                lastGesturePosition.X + 103, lastGesturePosition.Y + 16
            ), 6.0f, 11.0f, 0.0f, 360.0f, 0, lastGesture == Gesture.Drag ? Color.Lime : Color.LightGray);
            DrawCircle((int)lastGesturePosition.X + 80, (int)lastGesturePosition.Y + 43, 10,
                lastGesture == Gesture.DoubleTap ? Color.SkyBlue : Color.LightGray);
            DrawCircle((int)lastGesturePosition.X + 103, (int)lastGesturePosition.Y + 43, 10,
                lastGesture == Gesture.DoubleTap ? Color.SkyBlue : Color.LightGray);
            DrawTriangle(new Vector2(
                lastGesturePosition.X + 122, lastGesturePosition.Y + 16
            ), new Vector2(
                lastGesturePosition.X + 137, lastGesturePosition.Y + 26
            ), new Vector2(
                lastGesturePosition.X + 137, lastGesturePosition.Y + 6
            ), lastGesture == Gesture.PinchOut ? Color.Orange : Color.LightGray);
            DrawTriangle(new Vector2(
                lastGesturePosition.X + 147, lastGesturePosition.Y + 6
            ), new Vector2(
                lastGesturePosition.X + 147, lastGesturePosition.Y + 26
            ), new Vector2(
                lastGesturePosition.X + 162, lastGesturePosition.Y + 16
            ), lastGesture == Gesture.PinchOut ? Color.Orange : Color.Gray);
            DrawTriangle(new Vector2(
                lastGesturePosition.X + 125, lastGesturePosition.Y + 33
            ), new Vector2(
                lastGesturePosition.X + 125, lastGesturePosition.Y + 53
            ), new Vector2(
                lastGesturePosition.X + 140, lastGesturePosition.Y + 43
            ), lastGesture == Gesture.PinchIn ? Color.Violet : Color.LightGray);
            DrawTriangle(new Vector2(
                lastGesturePosition.X + 144, lastGesturePosition.Y + 43
            ), new Vector2(
                lastGesturePosition.X + 159, lastGesturePosition.Y + 53
            ), new Vector2(
                lastGesturePosition.X + 159, lastGesturePosition.Y + 33
            ), lastGesture == Gesture.PinchIn ? Color.Violet : Color.LightGray);
            for (i = 0; i < 4; i++)
            {
                DrawCircle((int)lastGesturePosition.X + 180, (int)lastGesturePosition.Y + 7 + i * 15, 5,
                    touchCount <= i ? Color.LightGray : gestureColor);
            }

            // Draw gesture log
            DrawText("Log", (int)gestureLogPosition.X, (int)gestureLogPosition.Y, 20, Color.Black);

            // Loop in both directions to print the gesture log array in the inverted order (and looping around if the index started somewhere in the middle)
            for (i = 0, ii = gestureLogIndex; i < GESTURE_LOG_SIZE; i++, ii = (ii + 1) % GESTURE_LOG_SIZE)
            {
                DrawText(gestureLog[ii], (int)gestureLogPosition.X, (int)gestureLogPosition.Y + 410 - i * 20, 20,
                    (i == 0 ? gestureColor : Color.LightGray));
            }

            Color logButton1Color, logButton2Color;
            switch (logMode)
            {
                case 3:
                    logButton1Color = Color.Maroon;
                    logButton2Color = Color.Maroon;
                    break;
                case 2:
                    logButton1Color = Color.Gray;
                    logButton2Color = Color.Maroon;
                    break;
                case 1:
                    logButton1Color = Color.Maroon;
                    logButton2Color = Color.Gray;
                    break;
                default:
                    logButton1Color = Color.Gray;
                    logButton2Color = Color.Gray;
                    break;
            }

            DrawRectangleRec(logButton1, logButton1Color);
            DrawText("Hide", (int)logButton1.X + 7, (int)logButton1.Y + 3, 10, Color.White);
            DrawText("Repeat", (int)logButton1.X + 7, (int)logButton1.Y + 13, 10, Color.White);
            DrawRectangleRec(logButton2, logButton2Color);
            DrawText("Hide", (int)logButton1.X + 62, (int)logButton1.Y + 3, 10, Color.White);
            DrawText("Hold", (int)logButton1.X + 62, (int)logButton1.Y + 13, 10, Color.White);

            // Draw protractor
            DrawText("Angle", (int)protractorPosition.X + 55, (int)protractorPosition.Y + 76, 10, Color.Black);

            // Note: Official it's using raylibs functions for string manipulation. But in C# it will end up in an unsafe handling.
            string angleString = currentAngleDegrees.ToString("F3");
            int angleStringDot = angleString.IndexOf('.');
            string angleStringTrim =  angleString.Substring(0, angleStringDot + 3);

            DrawText(angleStringTrim, (int)protractorPosition.X + 55, (int)protractorPosition.Y + 92, 20, gestureColor);
            DrawCircleV(protractorPosition, 80.0f, Color.White);
            DrawLineEx(new Vector2(
                protractorPosition.X - 90, protractorPosition.Y
            ), new Vector2(
                protractorPosition.X + 90, protractorPosition.Y
            ), 3.0f, Color.LightGray);
            DrawLineEx(new Vector2(
                protractorPosition.X, protractorPosition.Y - 90
            ), new Vector2(
                protractorPosition.X, protractorPosition.Y + 90
            ), 3.0f, Color.LightGray);
            DrawLineEx(new Vector2(
                protractorPosition.X - 80, protractorPosition.Y - 45
            ), new Vector2(
                protractorPosition.X + 80, protractorPosition.Y + 45
            ), 3.0f, Color.Green);
            DrawLineEx(new Vector2(
                protractorPosition.X - 80, protractorPosition.Y + 45
            ), new Vector2(
                protractorPosition.X + 80, protractorPosition.Y - 45
            ), 3.0f, Color.Green);
            DrawText("0", (int)protractorPosition.X + 96, (int)protractorPosition.Y - 9, 20, Color.Black);
            DrawText("30", (int)protractorPosition.X + 74, (int)protractorPosition.Y - 68, 20, Color.Black);
            DrawText("90", (int)protractorPosition.X - 11, (int)protractorPosition.Y - 110, 20, Color.Black);
            DrawText("150", (int)protractorPosition.X - 100, (int)protractorPosition.Y - 68, 20, Color.Black);
            DrawText("180", (int)protractorPosition.X - 124, (int)protractorPosition.Y - 9, 20, Color.Black);
            DrawText("210", (int)protractorPosition.X - 100, (int)protractorPosition.Y + 50, 20, Color.Black);
            DrawText("270", (int)protractorPosition.X - 18, (int)protractorPosition.Y + 92, 20, Color.Black);
            DrawText("330", (int)protractorPosition.X + 72, (int)protractorPosition.Y + 50, 20, Color.Black);
            if (currentAngleDegrees != 0.0f)
            {
                DrawLineEx(protractorPosition, finalVector, 3.0f, gestureColor);
            }

            // Draw touch and mouse pointer points
            if (currentGesture != Gesture.None)
            {
                if (touchCount != 0)
                {
                    for (i = 0; i < touchCount; i++)
                    {
                        DrawCircleV(touchPosition[i], 50.0f, Fade(gestureColor, 0.5f));
                        DrawCircleV(touchPosition[i], 5.0f, gestureColor);
                    }

                    if (touchCount == 2)
                    {
                        DrawLineEx(touchPosition[0], touchPosition[1], (((int)currentGesture == 512) ? 8.0f : 12.0f),
                            gestureColor);
                    }
                }
                else
                {
                    DrawCircleV(mousePosition, 35.0f, Fade(gestureColor, 0.5f));
                    DrawCircleV(mousePosition, 5.0f, gestureColor);
                }
            }

            EndDrawing();
            //--------------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow(); // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }

    static string GetGestureName(int gesture)
    {
        switch (gesture)
        {
            case 0: return "None";
            case 1: return "Tap";
            case 2: return "Double Tap";
            case 4: return "Hold";
            case 8: return "Drag";
            case 16: return "Swipe Right";
            case 32: return "Swipe Left";
            case 64: return "Swipe Up";
            case 128: return "Swipe Down";
            case 256: return "Pinch In";
            case 512: return "Pinch Out";
            default: return "Unknown";
        }
    }

// Get color for gesture value
    static Color GetGestureColor(int gesture)
    {
        switch (gesture)
        {
            case 0: return Color.Black;
            case 1: return Color.Blue;
            case 2: return Color.SkyBlue;
            case 4: return Color.Black;
            case 8: return Color.Lime;
            case 16: return Color.Red;
            case 32: return Color.Red;
            case 64: return Color.Red;
            case 128: return Color.Red;
            case 256: return Color.Violet;
            case 512: return Color.Orange;
            default: return Color.Black;
        }
    }
}
