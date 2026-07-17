/*******************************************************************************************
*
*   raylib [core] example - storage values
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.4, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class StorageValues : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;
    private const string storageDataFile = "storage.data";

    // NOTE: Storage positions must start with 0, directly related to file memory layout
    private enum StorageData
    {
        Score,
        HiScore
    }

    public string Name => "Core / Storage Values";

    public string Title => "raylib [core] example - storage values";

    private int score = 0;
    private int hiscore = 0;
    private int framesCounter = 0;

    public void Init()
    {
        score = 0;
        hiscore = 0;
        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.R))
        {
            score = GetRandomValue(1000, 2000);
            hiscore = GetRandomValue(2000, 4000);
        }

        if (IsKeyPressed(KeyboardKey.Enter))
        {
            SaveStorageValue(storageDataFile, (int)StorageData.Score, score);
            SaveStorageValue(storageDataFile, (int)StorageData.HiScore, hiscore);
        }
        else if (IsKeyPressed(KeyboardKey.Space))
        {
            // NOTE: If requested position could not be found, value 0 is returned
            score = LoadStorageValue(storageDataFile, (int)StorageData.Score);
            hiscore = LoadStorageValue(storageDataFile, (int)StorageData.HiScore);
        }

        framesCounter++;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText($"SCORE: {score}", 280, 130, 40, Color.Maroon);
        DrawText($"HI-SCORE: {hiscore}", 210, 200, 50, Color.Black);

        DrawText($"frames: {framesCounter}", 10, 10, 20, Color.Lime);

        DrawText("Press R to generate random numbers", 220, 40, 20, Color.LightGray);
        DrawText("Press ENTER to SAVE values", 250, 310, 20, Color.LightGray);
        DrawText("Press SPACE to LOAD values", 252, 350, 20, Color.LightGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - storage values");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new StorageValues();
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

    // Save integer value to storage file (to defined position)
    // NOTE: Storage positions is directly related to file memory layout (4 bytes each integer)
    private static unsafe bool SaveStorageValue(string fileName, int position, int value)
    {
        using var fileNameBuffer = fileName.ToUtf8Buffer();

        var success = false;
        var dataSize = 0;
        var newDataSize = 0;

        var fileData = LoadFileData(fileNameBuffer.AsPointer(), &dataSize);
        byte* newFileData = null;

        if (fileData != null)
        {
            if (dataSize <= (position * sizeof(int)))
            {
                // Increase data size up to position and store value
                newDataSize = (position + 1) * sizeof(int);
                newFileData = (byte*)MemRealloc(fileData, (uint)newDataSize);

                if (newFileData != null)
                {
                    // RL_REALLOC succeded
                    var dataPtr = (int*)newFileData;
                    dataPtr[position] = value;
                }
                else
                {
                    // RL_REALLOC failed
                    var positionInBytes = position * sizeof(int);
                    TraceLog(
                        TraceLogLevel.Warning,
                        @$"FILEIO: [{fileName}] Failed to realloc data ({dataSize}),
                            position in bytes({positionInBytes}) bigger than actual file size"
                    );

                    // We store the old size of the file
                    newFileData = fileData;
                    newDataSize = dataSize;
                }
            }
            else
            {
                // Store the old size of the file
                newFileData = fileData;
                newDataSize = dataSize;

                // Replace value on selected position
                var dataPtr = (int*)newFileData;
                dataPtr[position] = value;
            }

            success = SaveFileData(fileNameBuffer.AsPointer(), newFileData, newDataSize);
            MemFree(newFileData);

            TraceLog(TraceLogLevel.Info, $"FILEIO: [{fileName}] Saved storage value: {value}");
        }
        else
        {
            TraceLog(TraceLogLevel.Info, $"FILEIO: [{fileName}] File created successfully");

            dataSize = (position + 1) * sizeof(int);
            fileData = (byte*)MemAlloc((uint)dataSize);
            var dataPtr = (int*)fileData;
            dataPtr[position] = value;

            success = SaveFileData(fileNameBuffer.AsPointer(), fileData, dataSize);
            UnloadFileData(fileData);

            TraceLog(TraceLogLevel.Info, $"FILEIO: [{fileName}] Saved storage value: {value}");
        }

        return success;
    }

    // Load integer value from storage file (from defined position)
    // NOTE: If requested position could not be found, value 0 is returned
    private static unsafe int LoadStorageValue(string fileName, uint position)
    {
        using var fileNameBuffer = fileName.ToUtf8Buffer();

        var value = 0;
        var dataSize = 0;
        var fileData = LoadFileData(fileNameBuffer.AsPointer(), &dataSize);

        if (fileData != null)
        {
            if (dataSize < (position * 4))
            {
                TraceLog(
                    TraceLogLevel.Warning,
                    $"FILEIO: [{fileName}] Failed to find storage position: {value}"
                );
            }
            else
            {
                var dataPtr = (int*)fileData;
                value = dataPtr[position];
            }

            UnloadFileData(fileData);

            TraceLog(TraceLogLevel.Info, $"FILEIO: [{fileName}] Loaded storage value: {value}");
        }

        return value;
    }
}
