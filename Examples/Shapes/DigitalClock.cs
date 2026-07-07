/*******************************************************************************************
*
*   raylib [shapes] example - digital clock
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Hamza RAHAL (@hmz-rhl) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Hamza RAHAL (@hmz-rhl) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class DigitalClock : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int CLOCK_ANALOG = 0;
    private const int CLOCK_DIGITAL = 1;

    public string Name => "Shapes / Digital Clock";

    public string Title => "raylib [shapes] example - digital clock";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    // Clock hand type
    private struct ClockHand
    {
        public int value;          // Time value

        // Visual elements
        public float angle;        // Hand angle
        public int length;         // Hand length
        public int thickness;      // Hand thickness
        public Color color;        // Hand color
    }

    // Clock hands
    private struct Clock
    {
        public ClockHand second;   // Clock hand for seconds
        public ClockHand minute;   // Clock hand for minutes
        public ClockHand hour;     // Clock hand for hours
    }

    private int clockMode;
    private Clock clock;

    public void Init()
    {
        clockMode = CLOCK_DIGITAL;

        // Initialize clock
        // NOTE: Includes visual info for analog clock
        clock = new Clock();

        clock.second.angle = 45;
        clock.second.length = 140;
        clock.second.thickness = 3;
        clock.second.color = Color.Maroon;

        clock.minute.angle = 10;
        clock.minute.length = 130;
        clock.minute.thickness = 7;
        clock.minute.color = Color.DarkGray;

        clock.hour.angle = 0;
        clock.hour.length = 100;
        clock.hour.thickness = 7;
        clock.hour.color = Color.Black;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            // Toggle clock mode
            if (clockMode == CLOCK_DIGITAL) clockMode = CLOCK_ANALOG;
            else if (clockMode == CLOCK_ANALOG) clockMode = CLOCK_DIGITAL;
        }

        UpdateClock(); // Update clock required data: value and angle
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw clock in selected mode
        if (clockMode == CLOCK_ANALOG) DrawClockAnalog(clock, new Vector2(400, 240));
        else if (clockMode == CLOCK_DIGITAL)
        {
            DrawClockDigital(clock, new Vector2(30, 60));

            // Draw clock using default raylib font
            string clockTime = $"{clock.hour.value:D2}:{clock.minute.value:D2}:{clock.second.value:D2}";
            DrawText(clockTime, GetScreenWidth() / 2 - MeasureText(clockTime, 150) / 2, 300, 150, Color.Black);
        }

        DrawText($"Press [SPACE] to switch clock mode: {((clockMode == CLOCK_DIGITAL) ? "DIGITAL CLOCK" : "ANALOGUE CLOCK")}",
            10, 10, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    // Update clock time
    private void UpdateClock()
    {
        DateTime timeinfo = DateTime.Now;

        // Updating time data
        clock.second.value = timeinfo.Second;
        clock.minute.value = timeinfo.Minute;
        clock.hour.value = timeinfo.Hour;

        clock.hour.angle = (timeinfo.Hour % 12) * 180.0f / 6.0f;
        clock.hour.angle += (timeinfo.Minute % 60) * 30 / 60.0f;
        clock.hour.angle -= 90;

        clock.minute.angle = (timeinfo.Minute % 60) * 6.0f;
        clock.minute.angle += (timeinfo.Second % 60) * 6 / 60.0f;
        clock.minute.angle -= 90;

        clock.second.angle = (timeinfo.Second % 60) * 6.0f;
        clock.second.angle -= 90;
    }

    // Draw analog clock
    // Parameter: position, refers to center position
    private static void DrawClockAnalog(Clock clock, Vector2 position)
    {
        // Draw clock base
        DrawCircleV(position, clock.second.length + 40.0f, Color.LightGray);
        DrawCircleV(position, 12.0f, Color.Gray);

        // Draw clock minutes/seconds lines
        for (int i = 0; i < 60; i++)
        {
            DrawLineEx(new Vector2(position.X + (clock.second.length + ((i % 5) != 0 ? 10 : 6)) * MathF.Cos((6.0f * i - 90.0f) * DEG2RAD),
                position.Y + (clock.second.length + ((i % 5) != 0 ? 10 : 6)) * MathF.Sin((6.0f * i - 90.0f) * DEG2RAD)),
                new Vector2(position.X + (clock.second.length + 20) * MathF.Cos((6.0f * i - 90.0f) * DEG2RAD),
                position.Y + (clock.second.length + 20) * MathF.Sin((6.0f * i - 90.0f) * DEG2RAD)), ((i % 5) != 0 ? 1.0f : 3.0f), Color.DarkGray);
        }

        // Draw hand seconds
        DrawRectanglePro(new Rectangle(position.X, position.Y, (float)clock.second.length, (float)clock.second.thickness),
            new Vector2(0.0f, clock.second.thickness / 2.0f), clock.second.angle, clock.second.color);

        // Draw hand minutes
        DrawRectanglePro(new Rectangle(position.X, position.Y, (float)clock.minute.length, (float)clock.minute.thickness),
            new Vector2(0.0f, clock.minute.thickness / 2.0f), clock.minute.angle, clock.minute.color);

        // Draw hand hours
        DrawRectanglePro(new Rectangle(position.X, position.Y, (float)clock.hour.length, (float)clock.hour.thickness),
            new Vector2(0.0f, clock.hour.thickness / 2.0f), clock.hour.angle, clock.hour.color);
    }

    // Draw digital clock
    // PARAM: position, refers to top-left corner
    private static void DrawClockDigital(Clock clock, Vector2 position)
    {
        // Draw clock using custom 7-segments display (made of shapes)
        DrawDisplayValue(new Vector2(position.X, position.Y), clock.hour.value / 10, Color.Red, Fade(Color.LightGray, 0.3f));
        DrawDisplayValue(new Vector2(position.X + 120, position.Y), clock.hour.value % 10, Color.Red, Fade(Color.LightGray, 0.3f));

        DrawCircle((int)position.X + 240, (int)position.Y + 70, 12, (clock.second.value % 2) != 0 ? Color.Red : Fade(Color.LightGray, 0.3f));
        DrawCircle((int)position.X + 240, (int)position.Y + 150, 12, (clock.second.value % 2) != 0 ? Color.Red : Fade(Color.LightGray, 0.3f));

        DrawDisplayValue(new Vector2(position.X + 260, position.Y), clock.minute.value / 10, Color.Red, Fade(Color.LightGray, 0.3f));
        DrawDisplayValue(new Vector2(position.X + 380, position.Y), clock.minute.value % 10, Color.Red, Fade(Color.LightGray, 0.3f));

        DrawCircle((int)position.X + 500, (int)position.Y + 70, 12, (clock.second.value % 2) != 0 ? Color.Red : Fade(Color.LightGray, 0.3f));
        DrawCircle((int)position.X + 500, (int)position.Y + 150, 12, (clock.second.value % 2) != 0 ? Color.Red : Fade(Color.LightGray, 0.3f));

        DrawDisplayValue(new Vector2(position.X + 520, position.Y), clock.second.value / 10, Color.Red, Fade(Color.LightGray, 0.3f));
        DrawDisplayValue(new Vector2(position.X + 640, position.Y), clock.second.value % 10, Color.Red, Fade(Color.LightGray, 0.3f));
    }

    // Draw 7-segment display with value
    private static void DrawDisplayValue(Vector2 position, int value, Color colorOn, Color colorOff)
    {
        switch (value)
        {
            case 0: Draw7SDisplay(position, 0b00111111, colorOn, colorOff); break;
            case 1: Draw7SDisplay(position, 0b00000110, colorOn, colorOff); break;
            case 2: Draw7SDisplay(position, 0b01011011, colorOn, colorOff); break;
            case 3: Draw7SDisplay(position, 0b01001111, colorOn, colorOff); break;
            case 4: Draw7SDisplay(position, 0b01100110, colorOn, colorOff); break;
            case 5: Draw7SDisplay(position, 0b01101101, colorOn, colorOff); break;
            case 6: Draw7SDisplay(position, 0b01111101, colorOn, colorOff); break;
            case 7: Draw7SDisplay(position, 0b00000111, colorOn, colorOff); break;
            case 8: Draw7SDisplay(position, 0b01111111, colorOn, colorOff); break;
            case 9: Draw7SDisplay(position, 0b01101111, colorOn, colorOff); break;
            default: break;
        }
    }

    // Draw seven segments display
    // Parameter: position, refers to top-left corner of display
    // Parameter: segments, defines in binary the segments to be activated
    private static void Draw7SDisplay(Vector2 position, int segments, Color colorOn, Color colorOff)
    {
        int segmentLen = 60;
        int segmentThick = 20;
        float offsetYAdjust = segmentThick * 0.3f; // HACK: Adjust gap space between segment limits

        // Segment A
        DrawDisplaySegment(new Vector2(position.X + segmentThick + segmentLen / 2.0f, position.Y + segmentThick),
            segmentLen, segmentThick, false, (segments & 0b00000001) != 0 ? colorOn : colorOff);
        // Segment B
        DrawDisplaySegment(new Vector2(position.X + segmentThick + segmentLen + segmentThick / 2.0f, position.Y + 2 * segmentThick + segmentLen / 2.0f - offsetYAdjust),
            segmentLen, segmentThick, true, (segments & 0b00000010) != 0 ? colorOn : colorOff);
        // Segment C
        DrawDisplaySegment(new Vector2(position.X + segmentThick + segmentLen + segmentThick / 2.0f, position.Y + 4 * segmentThick + segmentLen + segmentLen / 2.0f - 3 * offsetYAdjust),
            segmentLen, segmentThick, true, (segments & 0b00000100) != 0 ? colorOn : colorOff);
        // Segment D
        DrawDisplaySegment(new Vector2(position.X + segmentThick + segmentLen / 2.0f, position.Y + 5 * segmentThick + 2 * segmentLen - 4 * offsetYAdjust),
            segmentLen, segmentThick, false, (segments & 0b00001000) != 0 ? colorOn : colorOff);
        // Segment E
        DrawDisplaySegment(new Vector2(position.X + segmentThick / 2.0f, position.Y + 4 * segmentThick + segmentLen + segmentLen / 2.0f - 3 * offsetYAdjust),
            segmentLen, segmentThick, true, (segments & 0b00010000) != 0 ? colorOn : colorOff);
        // Segment F
        DrawDisplaySegment(new Vector2(position.X + segmentThick / 2.0f, position.Y + 2 * segmentThick + segmentLen / 2.0f - offsetYAdjust),
            segmentLen, segmentThick, true, (segments & 0b00100000) != 0 ? colorOn : colorOff);
        // Segment G
        DrawDisplaySegment(new Vector2(position.X + segmentThick + segmentLen / 2.0f, position.Y + 3 * segmentThick + segmentLen - 2 * offsetYAdjust),
            segmentLen, segmentThick, false, (segments & 0b01000000) != 0 ? colorOn : colorOff);
    }

    // Draw one 7-segment display segment, horizontal or vertical
    private static void DrawDisplaySegment(Vector2 center, int length, int thick, bool vertical, Color color)
    {
        if (!vertical)
        {
            // Horizontal segment points
            /*
                 3___________________________5
                /                             \
               /1             x               6\
               \                               /
                \2___________________________4/
            */
            Vector2[] segmentPointsH = new Vector2[6]
            {
                new Vector2(center.X - length / 2.0f - thick / 2.0f, center.Y),  // Point 1
                new Vector2(center.X - length / 2.0f, center.Y + thick / 2.0f),  // Point 2
                new Vector2(center.X - length / 2.0f, center.Y - thick / 2.0f),  // Point 3
                new Vector2(center.X + length / 2.0f, center.Y + thick / 2.0f),  // Point 4
                new Vector2(center.X + length / 2.0f, center.Y - thick / 2.0f),  // Point 5
                new Vector2(center.X + length / 2.0f + thick / 2.0f, center.Y),  // Point 6
            };

            DrawTriangleStrip(segmentPointsH, 6, color);
        }
        else
        {
            // Vertical segment points
            Vector2[] segmentPointsV = new Vector2[6]
            {
                new Vector2(center.X, center.Y - length / 2.0f - thick / 2.0f),  // Point 1
                new Vector2(center.X - thick / 2.0f, center.Y - length / 2.0f),  // Point 2
                new Vector2(center.X + thick / 2.0f, center.Y - length / 2.0f),  // Point 3
                new Vector2(center.X - thick / 2.0f, center.Y + length / 2.0f),  // Point 4
                new Vector2(center.X + thick / 2.0f, center.Y + length / 2.0f),  // Point 5
                new Vector2(center.X, center.Y + (float)length / 2 + thick / 2.0f),  // Point 6
            };

            DrawTriangleStrip(segmentPointsV, 6, color);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - digital clock");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DigitalClock();
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
