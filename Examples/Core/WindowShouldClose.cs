/*******************************************************************************************
*
*   raylib [core] example - window should close
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2013-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class WindowShouldClose : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Window Should Close";

    public string Title => "raylib [core] example - window should close";

    // The runner drives its loop off this instead of WindowShouldClose(), so the exit
    // confirmation (polled in Update) can intercept both the X-button and KEY_ESCAPE.
    public bool ShouldClose => exitWindow;

    private bool exitWindowRequested;   // Flag to request window to exit
    private bool exitWindow;    // Flag to set window to exit

    public void Init()
    {
        SetExitKey(KeyboardKey.Null);       // Disable KEY_ESCAPE to close window, X-button still works

        exitWindowRequested = false;
        exitWindow = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Detect if X-button or KEY_ESCAPE have been pressed to close window
        bool closeRequested = IsKeyPressed(KeyboardKey.Escape);
#if !BROWSER
        // WindowShouldClose() polls the OS window close button, but on the wasm target it calls
        // emscripten_sleep (ASYNCIFY is not enabled) and the canvas has no window chrome anyway,
        // so on web the confirmation is triggered by KEY_ESCAPE only.
        closeRequested |= Raylib.WindowShouldClose();
#endif
        if (closeRequested)
        {
            exitWindowRequested = true;
        }

        if (exitWindowRequested)
        {
            // A request for close window has been issued, we can save data before closing
            // or just show a message asking for confirmation

            if (IsKeyPressed(KeyboardKey.Y))
            {
                exitWindow = true;
            }
            else if (IsKeyPressed(KeyboardKey.N))
            {
                exitWindowRequested = false;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (exitWindowRequested)
        {
            DrawRectangle(0, 100, screenWidth, 200, Color.Black);
            DrawText("Are you sure you want to exit program? [Y/N]", 40, 180, 30, Color.White);
        }
        else
        {
            DrawText("Try to close the window to get confirmation message!", 120, 200, 20, Color.LightGray);
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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - window should close");

        SetTargetFPS(60);           // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new WindowShouldClose();
        game.Init();

        // Main game loop
        while (!game.exitWindow)
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
