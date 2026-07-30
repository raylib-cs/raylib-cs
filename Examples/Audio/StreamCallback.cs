/*******************************************************************************************
*
*   raylib [audio] example - stream callback
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example created by Dan Hoang (@dan-hoang) and reviewed by Ramon Santamaria (@raysan5)
*
*   NOTE: Example sends a wave to the audio device,
*     user gets the choice of four waves: sine, square, triangle, and sawtooth
*     A stream is set up to play to the audio device; stream is hooked to a callback that
*     generates a wave, that is determined by user choice
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Dan Hoang (@dan-hoang)
*
********************************************************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Examples.Audio;

[ExcludeFromBrowser("SetAudioStreamCallback is unreliable on the wasm audio backend")]
public unsafe partial class StreamCallback : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int BUFFER_SIZE = 4096;
    private const int SAMPLE_RATE = 44100;

    // Wave type
    private enum WaveType
    {
        Sine,
        Square,
        Triangle,
        Sawtooth
    }

    public string Name => "Audio / Stream Callback";

    public string Title => "raylib [audio] example - stream callback";

    public int TargetFps => 30;

    private static int waveFrequency = 440;
    private static int newWaveFrequency = 440;
    private static int waveIndex = 0;

    // Buffer to keep the last second of uploaded audio,
    // part of which will be drawn on the screen
    private static readonly float[] buffer = new float[SAMPLE_RATE];

    private static readonly string[] waveTypesAsString = { "sine", "square", "triangle", "sawtooth" };

    private AudioStream stream;
    private WaveType waveType;

    public void Init()
    {
        InitAudioDevice();

        waveFrequency = 440;
        newWaveFrequency = 440;
        waveIndex = 0;
        Array.Clear(buffer, 0, buffer.Length);

        // Set the number of samples the stream will keep in memory at a time to BUFFER_SIZE
        SetAudioStreamBufferSizeDefault(BUFFER_SIZE);

        // Init raw audio stream (sample rate: 44100, sample size: 32bit-float, channels: 1-mono)
        stream = LoadAudioStream(SAMPLE_RATE, 32, 1);
        PlayAudioStream(stream);

        // Configure it so that the callback for waveType is called whenever stream is out of samples
        waveType = WaveType.Sine;
        SetWaveCallback();
    }

    // Attach the callback matching the current waveType to the stream
    private void SetWaveCallback()
    {
        switch (waveType)
        {
            case WaveType.Sine:
                SetAudioStreamCallback(stream, &SineCallback);
                break;
            case WaveType.Square:
                SetAudioStreamCallback(stream, &SquareCallback);
                break;
            case WaveType.Triangle:
                SetAudioStreamCallback(stream, &TriangleCallback);
                break;
            case WaveType.Sawtooth:
                SetAudioStreamCallback(stream, &SawtoothCallback);
                break;
        }
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyDown(KeyboardKey.Up))
        {
            newWaveFrequency += 10;
            if (newWaveFrequency > 12500)
            {
                newWaveFrequency = 12500;
            }
        }

        if (IsKeyDown(KeyboardKey.Down))
        {
            newWaveFrequency -= 10;
            if (newWaveFrequency < 20)
            {
                newWaveFrequency = 20;
            }
        }

        if (IsKeyPressed(KeyboardKey.Left))
        {
            if (waveType == WaveType.Sine)
            {
                waveType = WaveType.Sawtooth;
            }
            else if (waveType == WaveType.Square)
            {
                waveType = WaveType.Sine;
            }
            else if (waveType == WaveType.Triangle)
            {
                waveType = WaveType.Square;
            }
            else
            {
                waveType = WaveType.Triangle;
            }

            SetWaveCallback();
        }

        if (IsKeyPressed(KeyboardKey.Right))
        {
            if (waveType == WaveType.Sine)
            {
                waveType = WaveType.Square;
            }
            else if (waveType == WaveType.Square)
            {
                waveType = WaveType.Triangle;
            }
            else if (waveType == WaveType.Triangle)
            {
                waveType = WaveType.Sawtooth;
            }
            else
            {
                waveType = WaveType.Sine;
            }

            SetWaveCallback();
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);
        DrawText($"frequency: {newWaveFrequency}", screenWidth - 220, 10, 20, Color.Red);
        DrawText($"wave type: {waveTypesAsString[(int)waveType]}", screenWidth - 220, 30, 20, Color.Red);
        DrawText("Up/down to change frequency", 10, 10, 20, Color.DarkGray);
        DrawText("Left/right to change wave type", 10, 30, 20, Color.DarkGray);

        // Draw the last 10 ms of uploaded audio
        for (int i = 0; i < screenWidth; i++)
        {
            Vector2 startPos = new Vector2(i, 250 - 50 * buffer[WaveSampleIndex(i)]);
            Vector2 endPos = new Vector2(i + 1, 250 - 50 * buffer[WaveSampleIndex(i + 1)]);
            DrawLineV(startPos, endPos, Color.Red);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadAudioStream(stream);  // Close raw audio stream and delete buffers from RAM
        CloseAudioDevice();         // Close audio device (music streaming is automatically stopped)
    }

    // Maps a screen column to a sample index in the last 10 ms of the buffer. The final column
    // maps one sample past the end of the buffer; upstream C reads out of bounds there, so we
    // clamp to the last valid sample (visually identical, but safe under C# bounds checking).
    private static int WaveSampleIndex(int column)
    {
        int index = SAMPLE_RATE - SAMPLE_RATE / 100 + column * SAMPLE_RATE / 100 / screenWidth;
        return index < SAMPLE_RATE ? index : SAMPLE_RATE - 1;
    }

    //------------------------------------------------------------------------------------
    // Module Functions Definition
    //------------------------------------------------------------------------------------
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void SineCallback(void* framesOut, uint frameCount)
    {
        int fc = (int)frameCount;
        int wavelength = SAMPLE_RATE / waveFrequency;
        float* frames = (float*)framesOut;

        // Synthesize the sine wave
        for (int i = 0; i < fc; i++)
        {
            frames[i] = MathF.Sin(2 * MathF.PI * waveIndex / wavelength);

            waveIndex++;

            if (waveIndex >= wavelength)
            {
                waveFrequency = newWaveFrequency;
                waveIndex = 0;
            }
        }

        // Save the synthesized samples for later drawing
        for (int i = 0; i < SAMPLE_RATE - fc; i++)
        {
            buffer[i] = buffer[i + fc];
        }

        for (int i = 0; i < fc; i++)
        {
            buffer[SAMPLE_RATE - fc + i] = frames[i];
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void SquareCallback(void* framesOut, uint frameCount)
    {
        int fc = (int)frameCount;
        int wavelength = SAMPLE_RATE / waveFrequency;
        float* frames = (float*)framesOut;

        // Synthesize the square wave
        for (int i = 0; i < fc; i++)
        {
            frames[i] = (waveIndex < wavelength / 2) ? 1 : -1;
            waveIndex++;

            if (waveIndex >= wavelength)
            {
                waveFrequency = newWaveFrequency;
                waveIndex = 0;
            }
        }

        // Save the synthesized samples for later drawing
        for (int i = 0; i < SAMPLE_RATE - fc; i++)
        {
            buffer[i] = buffer[i + fc];
        }

        for (int i = 0; i < fc; i++)
        {
            buffer[SAMPLE_RATE - fc + i] = frames[i];
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void TriangleCallback(void* framesOut, uint frameCount)
    {
        int fc = (int)frameCount;
        int wavelength = SAMPLE_RATE / waveFrequency;
        float* frames = (float*)framesOut;

        // Synthesize the triangle wave
        for (int i = 0; i < fc; i++)
        {
            frames[i] = (waveIndex < wavelength / 2) ? (-1 + 2.0f * waveIndex / (wavelength / 2)) : (1 - 2.0f * (waveIndex - wavelength / 2) / (wavelength / 2));
            waveIndex++;

            if (waveIndex >= wavelength)
            {
                waveFrequency = newWaveFrequency;
                waveIndex = 0;
            }
        }

        // Save the synthesized samples for later drawing
        for (int i = 0; i < SAMPLE_RATE - fc; i++)
        {
            buffer[i] = buffer[i + fc];
        }

        for (int i = 0; i < fc; i++)
        {
            buffer[SAMPLE_RATE - fc + i] = frames[i];
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void SawtoothCallback(void* framesOut, uint frameCount)
    {
        int fc = (int)frameCount;
        int wavelength = SAMPLE_RATE / waveFrequency;
        float* frames = (float*)framesOut;

        // Synthesize the sawtooth wave
        for (int i = 0; i < fc; i++)
        {
            frames[i] = -1 + 2.0f * waveIndex / wavelength;
            waveIndex++;

            if (waveIndex >= wavelength)
            {
                waveFrequency = newWaveFrequency;
                waveIndex = 0;
            }
        }

        // Save the synthesized samples for later drawing
        for (int i = 0; i < SAMPLE_RATE - fc; i++)
        {
            buffer[i] = buffer[i + fc];
        }

        for (int i = 0; i < fc; i++)
        {
            buffer[SAMPLE_RATE - fc + i] = frames[i];
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - stream callback");

        SetTargetFPS(30);
        //--------------------------------------------------------------------------------------

        var game = new StreamCallback();
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
