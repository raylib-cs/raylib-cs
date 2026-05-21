/*******************************************************************************************
*
*   raylib [core] example - Custom logging
*
*   This example has been created using raylib 2.1 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Example contributed by Pablo Marcos Oltra (@pamarcos) and reviewed by Ramon Santamaria (@raysan5)
*
*   Copyright (c) 2018 Pablo Marcos Oltra (@pamarcos) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Runtime.InteropServices;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public unsafe class CustomLogging
{
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void LogCustom(int logLevel, sbyte* text, sbyte* args)
    {
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

        Console.WriteLine($"Custom " + message);
        // Console.ResetColor();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        // First thing we do is setting our custom logger to ensure everything raylib logs
        // will use our own logger instead of its internal one
        SetTraceLogCallback(&LogCustom);

        InitWindow(screenWidth, screenHeight, "raylib [core] example - custom logging");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            // TODO: Update your variables here
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            DrawText("Check out the console output to see the custom logger in action!", 60, 200, 20, Color.LightGray);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        SetTraceLogCallback(&Logging.LogConsole);
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
