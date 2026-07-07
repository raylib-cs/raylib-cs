/*******************************************************************************************
*
*   raylib [audio] example - stream effects
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 5.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Raylib_cs.Raylib;

namespace Examples.Audio;

[ExcludeFromBrowser("AttachAudioStreamProcessor is unreliable on the wasm audio backend")]
public unsafe partial class StreamEffects : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Audio / Stream Effects";

    public string Title => "raylib [audio] example - stream effects";

    //----------------------------------------------------------------------------------
    // Global Variables Definition
    //----------------------------------------------------------------------------------
    private static float[] delayBuffer = null;
    private static uint delayBufferSize = 0;
    private static uint delayReadIndex = 2;
    private static uint delayWriteIndex = 0;

    // Low-pass filter state (was a function-local static in the C example)
    private static readonly float[] low = { 0.0f, 0.0f };

    private Music music;
    private float timePlayed;
    private bool pause;
    private bool enableEffectLPF;
    private bool enableEffectDelay;

    public void Init()
    {
        InitAudioDevice();              // Initialize audio device

        music = LoadMusicStream("resources/audio/country.mp3");

        // Allocate buffer for the delay effect
        delayBufferSize = 48000 * 2;    // 1 second delay (device sampleRate*channels)
        delayBuffer = new float[delayBufferSize];
        delayReadIndex = 2;
        delayWriteIndex = 0;
        low[0] = 0.0f;
        low[1] = 0.0f;

        PlayMusicStream(music);

        timePlayed = 0.0f;              // Time played normalized [0.0f..1.0f]
        pause = false;                  // Music playing paused

        enableEffectLPF = false;        // Enable effect low-pass-filter
        enableEffectDelay = false;      // Enable effect delay (1 second)
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateMusicStream(music);   // Update music buffer with new stream data

        // Restart music playing (stop and play)
        if (IsKeyPressed(KeyboardKey.Space))
        {
            StopMusicStream(music);
            PlayMusicStream(music);
        }

        // Pause/Resume music playing
        if (IsKeyPressed(KeyboardKey.P))
        {
            pause = !pause;

            if (pause) PauseMusicStream(music);
            else ResumeMusicStream(music);
        }

        // Add/Remove effect: lowpass filter
        if (IsKeyPressed(KeyboardKey.F))
        {
            enableEffectLPF = !enableEffectLPF;
            if (enableEffectLPF) AttachAudioStreamProcessor(music.Stream, &AudioProcessEffectLPF);
            else DetachAudioStreamProcessor(music.Stream, &AudioProcessEffectLPF);
        }

        // Add/Remove effect: delay
        if (IsKeyPressed(KeyboardKey.D))
        {
            enableEffectDelay = !enableEffectDelay;
            if (enableEffectDelay) AttachAudioStreamProcessor(music.Stream, &AudioProcessEffectDelay);
            else DetachAudioStreamProcessor(music.Stream, &AudioProcessEffectDelay);
        }

        // Get normalized time played for current music stream
        timePlayed = GetMusicTimePlayed(music) / GetMusicTimeLength(music);

        if (timePlayed > 1.0f) timePlayed = 1.0f;   // Make sure time played is no longer than music
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText("MUSIC SHOULD BE PLAYING!", 245, 150, 20, Color.LightGray);

        DrawRectangle(200, 180, 400, 12, Color.LightGray);
        DrawRectangle(200, 180, (int)(timePlayed * 400.0f), 12, Color.Maroon);
        DrawRectangleLines(200, 180, 400, 12, Color.Gray);

        DrawText("PRESS SPACE TO RESTART MUSIC", 215, 230, 20, Color.LightGray);
        DrawText("PRESS P TO PAUSE/RESUME MUSIC", 208, 260, 20, Color.LightGray);

        DrawText($"PRESS F TO TOGGLE LPF EFFECT: {(enableEffectLPF ? "ON" : "OFF")}", 200, 320, 20, Color.Gray);
        DrawText($"PRESS D TO TOGGLE DELAY EFFECT: {(enableEffectDelay ? "ON" : "OFF")}", 180, 350, 20, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadMusicStream(music);   // Unload music stream buffers from RAM

        CloseAudioDevice();         // Close audio device (music streaming is automatically stopped)

        delayBuffer = null;         // Free delay buffer
    }

    //------------------------------------------------------------------------------------
    // Module Functions Definition
    //------------------------------------------------------------------------------------
    // Audio effect: lowpass filter
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void AudioProcessEffectLPF(void* buffer, uint frames)
    {
        const float cutoff = 70.0f / 44100.0f; // 70 Hz lowpass filter
        const float k = cutoff / (cutoff + 0.1591549431f); // RC filter formula

        // Converts the buffer data before using it
        float* bufferData = (float*)buffer;
        for (uint i = 0; i < frames * 2; i += 2)
        {
            float l = bufferData[i];
            float r = bufferData[i + 1];

            low[0] += k * (l - low[0]);
            low[1] += k * (r - low[1]);
            bufferData[i] = low[0];
            bufferData[i + 1] = low[1];
        }
    }

    // Audio effect: delay
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void AudioProcessEffectDelay(void* buffer, uint frames)
    {
        float* bufferData = (float*)buffer;
        fixed (float* delay = delayBuffer)
        {
            for (uint i = 0; i < frames * 2; i += 2)
            {
                float leftDelay = delay[delayReadIndex++];    // ERROR: Reading buffer -> WHY??? Maybe thread related???
                float rightDelay = delay[delayReadIndex++];

                if (delayReadIndex == delayBufferSize) delayReadIndex = 0;

                bufferData[i] = 0.5f * bufferData[i] + 0.5f * leftDelay;
                bufferData[i + 1] = 0.5f * bufferData[i + 1] + 0.5f * rightDelay;

                delay[delayWriteIndex++] = bufferData[i];
                delay[delayWriteIndex++] = bufferData[i + 1];
                if (delayWriteIndex == delayBufferSize) delayWriteIndex = 0;
            }
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - stream effects");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new StreamEffects();
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
