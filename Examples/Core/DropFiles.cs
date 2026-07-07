/*******************************************************************************************
*
*   raylib [core] example - drop files
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: This example only works on platforms that support drag & drop (Windows, Linux, OSX, Html5?)
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Collections.Generic;
using static Raylib_cs.Raylib;

namespace Examples.Core;

[ExcludeFromBrowser]
public partial class DropFiles : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int MaxFilepathRecorded = 4096;

    public string Name => "Core / Drop Files";

    public string Title => "raylib [core] example - drop files";

    private List<string> filePaths;

    public void Init()
    {
        // We will register a maximum of filepaths
        filePaths = new();
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsFileDropped())
        {
            var droppedFiles = GetDroppedFiles();

            for (var i = 0; i < droppedFiles.Length; i++)
            {
                if (filePaths.Count < (MaxFilepathRecorded - 1))
                {
                    filePaths.Add(droppedFiles[i]);
                }
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (filePaths.Count == 0)
        {
            DrawText("Drop your files to this window!", 100, 40, 20, Color.DarkGray);
        }
        else
        {
            DrawText("Dropped files:", 100, 40, 20, Color.DarkGray);

            for (var i = 0; i < filePaths.Count; i++)
            {
                if (i % 2 == 0)
                {
                    DrawRectangle(0, 85 + 40 * i, screenWidth, 40, Fade(Color.LightGray, 0.5f));
                }
                else
                {
                    DrawRectangle(0, 85 + 40 * i, screenWidth, 40, Fade(Color.LightGray, 0.3f));
                }

                DrawText(filePaths[i], 120, 100 + 40 * i, 10, Color.Gray);
            }

            DrawText("Drop new files...", 100, 110 + 40 * filePaths.Count, 20, Color.DarkGray);
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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - drop files");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DropFiles();
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
}
