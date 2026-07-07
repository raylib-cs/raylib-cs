/*******************************************************************************************
*
*   raylib [core] example - monitor detector
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Maicon Santana (@maiconpintoabreu) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Maicon Santana (@maiconpintoabreu)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

[ExcludeFromBrowser("GetMonitorCount() is not implemented on the wasm target")]
public partial class MonitorDetector : IExample
{
    private const int MaxMonitors = 10;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Monitor Detector";

    public string Title => "raylib [core] example - monitor detector";

    // Monitor info
    private struct MonitorInfo
    {
        public Vector2 Position;
        public string Name;
        public int Width;
        public int Height;
        public int PhysicalWidth;
        public int PhysicalHeight;
        public int RefreshRate;
    }

    private MonitorInfo[] monitors;
    private int currentMonitorIndex;
    private int monitorCount;

    public void Init()
    {
        monitors = new MonitorInfo[MaxMonitors];
        currentMonitorIndex = GetCurrentMonitor();
        monitorCount = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Variables to find the max x and Y to calculate the scale
        int maxWidth = 1;
        int maxHeight = 1;

        // Monitor offset is to fix when monitor position x is negative
        int monitorOffsetX = 0;

        // Rebuild monitors array every frame
        monitorCount = GetMonitorCount();
        for (int i = 0; i < monitorCount; i++)
        {
            monitors[i] = new MonitorInfo
            {
                Position = GetMonitorPosition(i),
                Name = GetMonitorName_(i),
                Width = GetMonitorWidth(i),
                Height = GetMonitorHeight(i),
                PhysicalWidth = GetMonitorPhysicalWidth(i),
                PhysicalHeight = GetMonitorPhysicalHeight(i),
                RefreshRate = GetMonitorRefreshRate(i)
            };

            if (monitors[i].Position.X < monitorOffsetX)
            {
                monitorOffsetX = -(int)monitors[i].Position.X;
            }

            int width = (int)monitors[i].Position.X + monitors[i].Width;
            int height = (int)monitors[i].Position.Y + monitors[i].Height;

            if (maxWidth < width)
            {
                maxWidth = width;
            }
            if (maxHeight < height)
            {
                maxHeight = height;
            }
        }

        if (IsKeyPressed(KeyboardKey.Enter) && (monitorCount > 1))
        {
            currentMonitorIndex += 1;

            // Set index to 0 if the last one
            if (currentMonitorIndex == monitorCount)
            {
                currentMonitorIndex = 0;
            }

            SetWindowMonitor(currentMonitorIndex); // Move window to currentMonitorIndex
        }
        else
        {
            currentMonitorIndex = GetCurrentMonitor(); // Get currentMonitorIndex if manually moved
        }

        float monitorScale = 0.6f;

        if (maxHeight > (maxWidth + monitorOffsetX))
        {
            monitorScale *= ((float)screenHeight / (float)maxHeight);
        }
        else
        {
            monitorScale *= ((float)screenWidth / (float)(maxWidth + monitorOffsetX));
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText("Press [Enter] to move window to next monitor available", 20, 20, 20, Color.DarkGray);

        DrawRectangleLines(20, 60, screenWidth - 40, screenHeight - 100, Color.DarkGray);

        // Draw Monitor Rectangles with information inside
        for (int i = 0; i < monitorCount; i++)
        {
            // Calculate retangle position and size using monitorScale
            Rectangle rec = new Rectangle(
                (monitors[i].Position.X + monitorOffsetX) * monitorScale + 140,
                monitors[i].Position.Y * monitorScale + 80,
                monitors[i].Width * monitorScale,
                monitors[i].Height * monitorScale
            );

            // Draw monitor name and information inside the rectangle
            DrawText($"[{i}] {monitors[i].Name}", (int)rec.X + 10, (int)rec.Y + (int)(100 * monitorScale), (int)(120 * monitorScale), Color.Blue);
            DrawText(
                $"Resolution: [{monitors[i].Width}px x {monitors[i].Height}px]\nRefreshRate: [{monitors[i].RefreshRate}hz]\nPhysical Size: [{monitors[i].PhysicalWidth}mm x {monitors[i].PhysicalHeight}mm]\nPosition: {monitors[i].Position.X,3:F0} x {monitors[i].Position.Y,3:F0}",
                (int)rec.X + 10, (int)rec.Y + (int)(200 * monitorScale), (int)(120 * monitorScale), Color.DarkGray);

            // Highlight current monitor
            if (i == currentMonitorIndex)
            {
                DrawRectangleLinesEx(rec, 5, Color.Red);
                Vector2 windowPosition = new Vector2((GetWindowPosition().X + monitorOffsetX) * monitorScale + 140, GetWindowPosition().Y * monitorScale + 80);

                // Draw window position based on monitors
                DrawRectangleV(windowPosition, new Vector2(screenWidth * monitorScale, screenHeight * monitorScale), Fade(Color.Green, 0.5f));
            }
            else
            {
                DrawRectangleLinesEx(rec, 5, Color.Gray);
            }
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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - monitor detector");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new MonitorDetector();
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
