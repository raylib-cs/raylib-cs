/*******************************************************************************************
*
*   raylib [core] example - custom frame control
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: WARNING: This is an example for advanced users willing to have full control over
*   the frame processes. By default, EndDrawing() calls the following processes:
*       1. Draw remaining batch data: rlDrawRenderBatchActive()
*       2. SwapScreenBuffer()
*       3. Frame time control: WaitTime()
*       4. PollInputEvents()
*
*   To avoid steps 2, 3 and 4, flag SUPPORT_CUSTOM_FRAME_CONTROL can be enabled in
*   config.h (it requires recompiling raylib). This way those steps are up to the user
*
*   Note that enabling this flag invalidates some functions:
*       - GetFrameTime()
*       - SetTargetFPS()
*       - GetFPS()
*
*   Example originally created with raylib 4.0, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Core;

// NOTE: This example is intended to run against a raylib built with SUPPORT_CUSTOM_FRAME_CONTROL,
// where EndDrawing() does NOT swap buffers, wait or poll input, leaving those to the user code
// below. The stock raylib-cs native library is built WITHOUT that flag, so EndDrawing() still
// performs those steps and the manual SwapScreenBuffer()/WaitTime()/PollInputEvents() calls here
// run in addition to them. The port is kept faithful to upstream regardless.
[ExcludeFromBrowser("manual PollInputEvents/SwapScreenBuffer/WaitTime clashes with the emscripten main loop")]
public partial class CustomFrameControl : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Custom Frame Control";

    public string Title => "raylib [core] example - custom frame control";

    // Custom timming variables
    private double previousTime;    // Previous time measure
    private double currentTime;     // Current time measure
    private double updateDrawTime;  // Update + Draw time
    private double waitTime;        // Wait time (if target fps required)
    private float deltaTime;        // Frame time (Update + Draw + Wait time)

    private float timeCounter;      // Accumulative time counter (seconds)
    private float position;         // Circle position
    private bool pause;             // Pause control flag

    private int targetFPS;          // Our initial target fps

    public void Init()
    {
        previousTime = GetTime();
        currentTime = 0.0;
        updateDrawTime = 0.0;
        waitTime = 0.0;
        deltaTime = 0.0f;

        timeCounter = 0.0f;
        position = 0.0f;
        pause = false;

        targetFPS = 60;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
#if !BROWSER
        // NOTE: On non web platforms the PollInputEvents just works before the inputs checks
        PollInputEvents();          // Poll input events (SUPPORT_CUSTOM_FRAME_CONTROL)
#endif

        if (IsKeyPressed(KeyboardKey.Space)) pause = !pause;

        if (IsKeyPressed(KeyboardKey.Up)) targetFPS += 20;
        else if (IsKeyPressed(KeyboardKey.Down)) targetFPS -= 20;

        if (targetFPS < 0) targetFPS = 0;

        if (!pause)
        {
            position += 200 * deltaTime;  // We move at 200 pixels per second
            if (position >= GetScreenWidth()) position = 0;
            timeCounter += deltaTime;   // We count time (seconds)
        }

#if BROWSER
        // NOTE: On web platform for some reason the PollInputEvents only works after the inputs
        // check, so just call it after check all your inputs (on web)
        PollInputEvents();          // Poll input events (SUPPORT_CUSTOM_FRAME_CONTROL)
#endif
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (int i = 0; i < GetScreenWidth() / 200; i++) DrawRectangle(200 * i, 0, 1, GetScreenHeight(), Color.SkyBlue);

        DrawCircle((int)position, GetScreenHeight() / 2 - 25, 50, Color.Red);

        DrawText($"{timeCounter * 1000.0f:000} ms", (int)position - 40, GetScreenHeight() / 2 - 100, 20, Color.Maroon);
        DrawText($"PosX: {position:000}", (int)position - 50, GetScreenHeight() / 2 + 40, 20, Color.Black);

        DrawText("Circle is moving at a constant 200 pixels/sec,\nindependently of the frame rate.", 10, 10, 20, Color.DarkGray);
        DrawText("PRESS SPACE to PAUSE MOVEMENT", 10, GetScreenHeight() - 60, 20, Color.Gray);
        DrawText("PRESS UP | DOWN to CHANGE TARGET FPS", 10, GetScreenHeight() - 30, 20, Color.Gray);
        DrawText($"TARGET FPS: {targetFPS}", GetScreenWidth() - 220, 10, 20, Color.Lime);
        if (deltaTime != 0)
        {
            DrawText($"CURRENT FPS: {(int)(1.0f / deltaTime)}", GetScreenWidth() - 220, 40, 20, Color.Green);
        }

        EndDrawing();

        // NOTE: In case raylib is configured to SUPPORT_CUSTOM_FRAME_CONTROL,
        // Events polling, screen buffer swap and frame time control must be managed by the user

        SwapScreenBuffer();         // Flip the back buffer to screen (front buffer)

        currentTime = GetTime();
        updateDrawTime = currentTime - previousTime;

        if (targetFPS > 0)          // We want a fixed frame rate
        {
            waitTime = (1.0f / (float)targetFPS) - updateDrawTime;
            if (waitTime > 0.0)
            {
                WaitTime((float)waitTime);
                currentTime = GetTime();
                deltaTime = (float)(currentTime - previousTime);
            }
        }
        else deltaTime = (float)updateDrawTime;    // Framerate could be variable

        previousTime = currentTime;
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - custom frame control");

        // NOTE: Not calling SetTargetFPS(): this example manages frame timing manually with
        // WaitTime() (SetTargetFPS/GetFrameTime/GetFPS are invalidated by SUPPORT_CUSTOM_FRAME_CONTROL)
        //--------------------------------------------------------------------------------------

        var game = new CustomFrameControl();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
