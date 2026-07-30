/*******************************************************************************************
*
*   raylib [core] example - highdpi demo
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.5
*
*   Example contributed by Jonathan Marler (@marler8997) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jonathan Marler (@marler8997)
*
********************************************************************************************/

namespace Examples.Core;

[ExcludeFromBrowser("monitor/DPI APIs are meaningless on the fixed wasm canvas")]
public partial class HighDpiDemo : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / High DPI Demo";

    public string Title => "raylib [core] example - highdpi demo";

    public ConfigFlags ConfigFlags => ConfigFlags.HighDpiWindow | ConfigFlags.ResizableWindow;

    private int logicalGridDescY;
    private int logicalGridLabelY;
    private int logicalGridTop;
    private int logicalGridBottom;
    private int pixelGridTop;
    private int pixelGridBottom;
    private int pixelGridLabelY;
    private int pixelGridDescY;
    private int cellSize;
    private float cellSizePx;

    public void Init()
    {
        SetWindowMinSize(450, 450);

        logicalGridDescY = 120;
        logicalGridLabelY = logicalGridDescY + 30;
        logicalGridTop = logicalGridLabelY + 30;
        logicalGridBottom = logicalGridTop + 80;
        pixelGridTop = logicalGridBottom - 20;
        pixelGridBottom = pixelGridTop + 80;
        pixelGridLabelY = pixelGridBottom + 30;
        pixelGridDescY = pixelGridLabelY + 30;
        cellSize = 50;
        cellSizePx = (float)cellSize;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        int monitorCount = GetMonitorCount();

        if ((monitorCount > 1) && IsKeyPressed(KeyboardKey.N))
        {
            SetWindowMonitor((GetCurrentMonitor() + 1) % monitorCount);
        }

        int currentMonitor = GetCurrentMonitor();
        Vector2 dpiScale = GetWindowScaleDPI();
        cellSizePx = ((float)cellSize) / dpiScale.X;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        int windowCenter = GetScreenWidth() / 2;
        DrawTextCenter($"Dpi Scale: {dpiScale.X:F6}", windowCenter, 30, 40, Color.DarkGray);
        DrawTextCenter($"Monitor: {currentMonitor + 1}/{monitorCount} ([N] next monitor)", windowCenter, 70, 20, Color.LightGray);
        DrawTextCenter($"Window is {GetScreenWidth()} \"logical points\" wide", windowCenter, logicalGridDescY, 20, Color.Orange);

        bool odd = true;
        for (int i = cellSize; i < GetScreenWidth(); i += cellSize, odd = !odd)
        {
            if (odd)
            {
                DrawRectangle(i, logicalGridTop, cellSize, logicalGridBottom - logicalGridTop, Color.Orange);
            }

            DrawTextCenter($"{i}", i, logicalGridLabelY, 10, Color.LightGray);
            DrawLine(i, logicalGridLabelY + 10, i, logicalGridBottom, Color.Gray);
        }

        odd = true;
        const int minTextSpace = 30;
        int lastTextX = -minTextSpace;
        for (int i = cellSize; i < GetRenderWidth(); i += cellSize, odd = !odd)
        {
            int x = (int)(((float)i) / dpiScale.X);
            if (odd)
            {
                DrawRectangle(x, pixelGridTop, (int)cellSizePx, pixelGridBottom - pixelGridTop, new Color(0, 121, 241, 100));
            }

            DrawLine(x, pixelGridTop, (int)(((float)i) / dpiScale.X), pixelGridLabelY - 10, Color.Gray);

            if ((x - lastTextX) >= minTextSpace)
            {
                DrawTextCenter($"{i}", x, pixelGridLabelY, 10, Color.LightGray);
                lastTextX = x;
            }
        }

        DrawTextCenter($"Window is {GetRenderWidth()} \"physical pixels\" wide", windowCenter, pixelGridDescY, 20, Color.Blue);

        string text = "Can you see this?";
        Vector2 size = MeasureTextEx(GetFontDefault(), text, 20, 3);
        Vector2 pos = new Vector2(GetScreenWidth() - size.X - 5, GetScreenHeight() - size.Y - 5);
        DrawTextEx(GetFontDefault(), text, pos, 20, 3, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    // Draw text centered on the given position
    private static void DrawTextCenter(string text, int x, int y, int fontSize, Color color)
    {
        Vector2 size = MeasureTextEx(GetFontDefault(), text, (float)fontSize, 3);
        Vector2 pos = new Vector2(x - size.X / 2, y - size.Y / 2);
        DrawTextEx(GetFontDefault(), text, pos, (float)fontSize, 3, color);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.HighDpiWindow | ConfigFlags.ResizableWindow);
        InitWindow(screenWidth, screenHeight, "raylib [core] example - highdpi demo");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new HighDpiDemo();
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
