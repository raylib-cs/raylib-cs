/*******************************************************************************************
*
*   raylib [core] example - custom logging
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example contributed by Pablo Marcos Oltra (@pamarcos) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Pablo Marcos Oltra (@pamarcos) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Runtime.InteropServices;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public unsafe partial class CustomLogging : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Custom Logging";

    public string Title => "raylib [core] example - custom logging";

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void LogCustom(int logLevel, sbyte* text, sbyte* args)
    {
#if BROWSER
        // WebAssembly can't invoke the C varargs vsprintf/vsnprintf that Logging.GetLogMessage relies on
        // (the wasm runtime traps with "function signature mismatch"), so log the raw, unformatted text.
        string message = Marshal.PtrToStringUTF8(new IntPtr(text)) ?? string.Empty;
#else
        var message = Logging.GetLogMessage(new IntPtr(text), new IntPtr(args));

        /*Console.ForegroundColor = (TraceLogLevel)logLevel switch
        {
            TraceLogLevel.LOG_ALL => ConsoleColor.White,
            TraceLogLevel.LOG_TRACE => ConsoleColor.Black,
            TraceLogLevel.LOG_DEBUG => ConsoleColor.Blue,
            TraceLogLevel.LOG_INFO => ConsoleColor.Black,
            TraceLogLevel.LOG_WARNING => ConsoleColor.DarkYellow,
            TraceLogLevel.LOG_ERROR => ConsoleColor.Red,
            TraceLogLevel.LOG_FATAL => ConsoleColor.Red,
            TraceLogLevel.LOG_NONE => ConsoleColor.White,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };*/
#endif

        Console.WriteLine($"Custom " + message);
        // Console.ResetColor();
    }

    public void Init()
    {
        // First thing we do is setting our custom logger to ensure everything raylib logs
        // will use our own logger instead of its internal one
        SetTraceLogCallback(&LogCustom);
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("Check out the console output to see the custom logger in action!", 60, 200, 20, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
#if !BROWSER
        // Restore the default formatting logger on desktop. On WebAssembly we leave the (wasm-safe)
        // custom logger in place — Logging.LogConsole formats via vsprintf, which traps on wasm.
        SetTraceLogCallback(&Logging.LogConsole);
#endif
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - custom logging");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new CustomLogging();
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
