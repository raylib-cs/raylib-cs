/*******************************************************************************************
*
*   raylib [core] example - highdpi testbed
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Ramon Santamaria (@raysan5) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

[ExcludeFromBrowser("fullscreen/borderless/monitor APIs are meaningless on the fixed wasm canvas")]
public partial class HighDpiTestbed : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / High DPI Testbed";

    public string Title => "raylib [core] example - highdpi testbed";

    public ConfigFlags ConfigFlags => ConfigFlags.ResizableWindow | ConfigFlags.HighDpiWindow;

    private Vector2 scaleDpi;
    private Vector2 mousePos;
    private int currentMonitor;
    private Vector2 windowPos;

    private int gridSpacing;   // Grid spacing in pixels

    public void Init()
    {
        scaleDpi = GetWindowScaleDPI();
        mousePos = GetMousePosition();
        currentMonitor = GetCurrentMonitor();
        windowPos = GetWindowPosition();

        gridSpacing = 40;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        mousePos = GetMousePosition();
        currentMonitor = GetCurrentMonitor();
        scaleDpi = GetWindowScaleDPI();
        windowPos = GetWindowPosition();

        if (IsKeyPressed(KeyboardKey.Space))
        {
            ToggleBorderlessWindowed();
        }
        if (IsKeyPressed(KeyboardKey.F))
        {
            ToggleFullscreen();
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw grid
        for (int h = 0; h < GetScreenHeight() / gridSpacing + 1; h++)
        {
            DrawText($"{h * gridSpacing:D2}", 4, h * gridSpacing - 4, 10, Color.Gray);
            DrawLine(24, h * gridSpacing, GetScreenWidth(), h * gridSpacing, Color.LightGray);
        }
        for (int v = 0; v < GetScreenWidth() / gridSpacing + 1; v++)
        {
            DrawText($"{v * gridSpacing:D2}", v * gridSpacing - 10, 4, 10, Color.Gray);
            DrawLine(v * gridSpacing, 20, v * gridSpacing, GetScreenHeight(), Color.LightGray);
        }

        // Draw UI info
        DrawText($"CURRENT MONITOR: {currentMonitor + 1}/{GetMonitorCount()} ({GetMonitorWidth(currentMonitor)}x{GetMonitorHeight(currentMonitor)})", 50, 50, 20, Color.DarkGray);
        DrawText($"WINDOW POSITION: {(int)windowPos.X}x{(int)windowPos.Y}", 50, 90, 20, Color.DarkGray);
        DrawText($"SCREEN SIZE: {GetScreenWidth()}x{GetScreenHeight()}", 50, 130, 20, Color.DarkGray);
        DrawText($"RENDER SIZE: {GetRenderWidth()}x{GetRenderHeight()}", 50, 170, 20, Color.DarkGray);
        DrawText($"SCALE FACTOR: {scaleDpi.X:F2}x{scaleDpi.Y:F2}", 50, 210, 20, Color.Gray);

        // Draw reference rectangles, top-left and bottom-right corners
        DrawRectangle(0, 0, 30, 60, Color.Red);
        DrawRectangle(GetScreenWidth() - 30, GetScreenHeight() - 60, 30, 60, Color.Blue);

        // Draw mouse position
        DrawCircleV(GetMousePosition(), 20, Color.Maroon);
        DrawRectangleRec(new Rectangle(mousePos.X - 25, mousePos.Y, 50, 2), Color.Black);
        DrawRectangleRec(new Rectangle(mousePos.X, mousePos.Y - 25, 2, 50), Color.Black);
        DrawText($"[{GetMouseX()},{GetMouseY()}]", (int)mousePos.X - 44,
            (mousePos.Y > GetScreenHeight() - 60) ? (int)mousePos.Y - 46 : (int)mousePos.Y + 30, 20, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // TODO: Unload all loaded resources at this point
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.HighDpiWindow);
        InitWindow(screenWidth, screenHeight, "raylib [core] example - highdpi testbed");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new HighDpiTestbed();
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
