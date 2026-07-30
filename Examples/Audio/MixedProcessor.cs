/*******************************************************************************************
*
*   raylib [audio] example - mixed processor
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example contributed by hkc (@hatkidchan) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 hkc (@hatkidchan)
*
********************************************************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Examples.Audio;

[ExcludeFromBrowser("AttachAudioMixedProcessor callback is unreliable on the wasm audio backend")]
public unsafe partial class MixedProcessor : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Audio / Mixed Processor";

    public string Title => "raylib [audio] example - mixed processor";

    private static float exponent = 1.0f;                       // Audio exponentiation value
    private static readonly float[] averageVolume = new float[400];   // Average volume history

    private Music music;
    private Sound sound;

    //------------------------------------------------------------------------------------
    // Audio processing function
    //------------------------------------------------------------------------------------
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void ProcessAudio(void* buffer, uint frames)
    {
        float* samples = (float*)buffer;    // Samples internally stored as <float>s
        float average = 0.0f;               // Temporary average volume

        for (uint frame = 0; frame < frames; frame++)
        {
            float* left = &samples[frame * 2 + 0];
            float* right = &samples[frame * 2 + 1];

            *left = MathF.Pow(MathF.Abs(*left), exponent) * ((*left < 0.0f) ? -1.0f : 1.0f);
            *right = MathF.Pow(MathF.Abs(*right), exponent) * ((*right < 0.0f) ? -1.0f : 1.0f);

            average += MathF.Abs(*left) / frames;   // accumulating average volume
            average += MathF.Abs(*right) / frames;
        }

        // Moving history to the left
        for (int i = 0; i < 399; i++)
        {
            averageVolume[i] = averageVolume[i + 1];
        }

        averageVolume[399] = average;       // Adding last average value
    }

    public void Init()
    {
        InitAudioDevice();              // Initialize audio device

        exponent = 1.0f;
        Array.Clear(averageVolume, 0, averageVolume.Length);

        AttachAudioMixedProcessor(&ProcessAudio);

        music = LoadMusicStream("resources/audio/country.mp3");
        sound = LoadSound("resources/audio/coin.wav");

        PlayMusicStream(music);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateMusicStream(music);   // Update music buffer with new stream data

        // Modify processing variables
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Left))
        {
            exponent -= 0.05f;
        }

        if (IsKeyPressed(KeyboardKey.Right))
        {
            exponent += 0.05f;
        }

        if (exponent <= 0.5f)
        {
            exponent = 0.5f;
        }

        if (exponent >= 3.0f)
        {
            exponent = 3.0f;
        }

        if (IsKeyPressed(KeyboardKey.Space))
        {
            PlaySound(sound);
        }

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText("MUSIC SHOULD BE PLAYING!", 255, 150, 20, Color.LightGray);

        DrawText($"EXPONENT = {exponent:F2}", 215, 180, 20, Color.LightGray);

        DrawRectangle(199, 199, 402, 34, Color.LightGray);
        for (int i = 0; i < 400; i++)
        {
            DrawLine(201 + i, 232 - (int)(averageVolume[i] * 32), 201 + i, 232, Color.Maroon);
        }
        DrawRectangleLines(199, 199, 402, 34, Color.Gray);

        DrawText("PRESS SPACE TO PLAY OTHER SOUND", 200, 250, 20, Color.LightGray);
        DrawText("USE LEFT AND RIGHT ARROWS TO ALTER DISTORTION", 140, 280, 20, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadMusicStream(music);   // Unload music stream buffers from RAM

        DetachAudioMixedProcessor(&ProcessAudio);  // Disconnect audio processor

        CloseAudioDevice();         // Close audio device (music streaming is automatically stopped)
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - mixed processor");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new MixedProcessor();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();              // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
