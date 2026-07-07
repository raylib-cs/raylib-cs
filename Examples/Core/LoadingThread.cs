/*******************************************************************************************
*
*   raylib [core] example - loading thread
*
*   NOTE: raylib is NOT thread-safe: the loading thread only updates plain data
*   (progress counter and loaded flag); all raylib calls happen on the main thread.
*
*   Example originally created with raylib 2.5 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Diagnostics;
using System.Threading;
using static Raylib_cs.Raylib;

namespace Examples.Core;

[ExcludeFromBrowser("System.Threading.Thread is unsupported on single-threaded wasm")]
public partial class LoadingThread : IExample
{
    const int screenWidth = 800;
    const int screenHeight = 450;

    public string Name => "Core / Loading Thread";

    public string Title => "raylib [core] example - loading thread";

    enum State
    {
        Waiting,
        Loading,
        Finished
    }

    // Loading data thread; a Thread can only be started once, so a fresh one
    // is created for every load
    Thread loadingThread;

    // Data loaded completion indicator; volatile so the main thread sees the
    // background thread's writes
    volatile bool dataLoaded;

    // Data progress accumulator (0..500, the progress bar width in pixels)
    volatile int dataProgress;

    State state;
    int framesCounter;

    public void Init()
    {
        loadingThread = null;
        dataLoaded = false;
        dataProgress = 0;

        state = State.Waiting;
        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        switch (state)
        {
            case State.Waiting:
                if (IsKeyPressed(KeyboardKey.Enter))
                {
                    loadingThread = new Thread(LoadDataThread) { IsBackground = true };
                    loadingThread.Start();
                    TraceLog(TraceLogLevel.Info, "Loading thread initialized successfully");

                    state = State.Loading;
                }
                break;

            case State.Loading:
                framesCounter++;
                if (dataLoaded)
                {
                    framesCounter = 0;
                    loadingThread.Join();
                    TraceLog(TraceLogLevel.Info, "Loading thread terminated");

                    state = State.Finished;
                }
                break;

            case State.Finished:
                if (IsKeyPressed(KeyboardKey.Enter))
                {
                    // Reset everything to launch again
                    dataLoaded = false;
                    dataProgress = 0;

                    state = State.Waiting;
                }
                break;

            default:
                break;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        switch (state)
        {
            case State.Waiting:
                DrawText("PRESS ENTER to START LOADING DATA", 150, 170, 20, Color.DarkGray);
                break;

            case State.Loading:
                DrawRectangle(150, 200, dataProgress, 60, Color.SkyBlue);
                if ((framesCounter / 15) % 2 == 0)
                {
                    DrawText("LOADING DATA...", 240, 210, 40, Color.DarkBlue);
                }
                break;

            case State.Finished:
                DrawRectangle(150, 200, 500, 60, Color.Lime);
                DrawText("DATA LOADED!", 250, 210, 40, Color.Green);
                break;

            default:
                break;
        }

        DrawRectangleLines(150, 200, 500, 60, Color.DarkGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - loading thread");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LoadingThread();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();          // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }

    // Loading data thread function definition
    void LoadDataThread()
    {
        int timeCounter = 0;                        // Time counted in ms
        var stopwatch = Stopwatch.StartNew();

        // We simulate data loading with a time counter for 5 seconds
        while (timeCounter < 5000)
        {
            timeCounter = (int)stopwatch.ElapsedMilliseconds;

            // We accumulate time over a global variable to be used in
            // main thread as a progress bar
            dataProgress = timeCounter / 10;
        }

        // When data has finished loading, we set global variable
        dataLoaded = true;
    }
}
