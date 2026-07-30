/*******************************************************************************************
*
*   raylib [audio] example - raw stream
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 1.6, last time updated with raylib 6.0
*
*   Example created by Ramon Santamaria (@raysan5) and reviewed by James Hofmann (@triplefox)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2026 Ramon Santamaria (@raysan5) and James Hofmann (@triplefox)
*
********************************************************************************************/

namespace Examples.Audio;

public unsafe partial class RawStream : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int BUFFER_SIZE = 4096;
    private const int SAMPLE_RATE = 44100;

    public string Name => "Audio / Raw Stream";

    public string Title => "raylib [audio] example - raw stream";

    public int TargetFps => 30;

    private float[] buffer;
    private AudioStream stream;
    private float pan;
    private int sineFrequency;
    private int newSineFrequency;
    private int sineIndex;
    private double sineStartTime;

    public void Init()
    {
        InitAudioDevice();

        // Set the number of samples the stream will keep in memory at a time to BUFFER_SIZE
        SetAudioStreamBufferSizeDefault(BUFFER_SIZE);
        buffer = new float[BUFFER_SIZE];

        // Init raw audio stream (sample rate: 44100, sample size: 32bit-float, channels: 1-mono)
        stream = LoadAudioStream(SAMPLE_RATE, 32, 1);
        pan = 0.0f;
        SetAudioStreamPan(stream, pan);
        PlayAudioStream(stream);

        sineFrequency = 440;
        newSineFrequency = 440;
        sineIndex = 0;
        sineStartTime = 0.0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------

        if (IsKeyDown(KeyboardKey.Up))
        {
            newSineFrequency += 10;
            if (newSineFrequency > 12500)
            {
                newSineFrequency = 12500;
            }
        }

        if (IsKeyDown(KeyboardKey.Down))
        {
            newSineFrequency -= 10;
            if (newSineFrequency < 20)
            {
                newSineFrequency = 20;
            }
        }

        if (IsKeyDown(KeyboardKey.Left))
        {
            pan -= 0.01f;
            if (pan < -1.0f)
            {
                pan = -1.0f;
            }

            SetAudioStreamPan(stream, pan);
        }

        if (IsKeyDown(KeyboardKey.Right))
        {
            pan += 0.01f;
            if (pan > 1.0f)
            {
                pan = 1.0f;
            }

            SetAudioStreamPan(stream, pan);
        }

        if (IsAudioStreamProcessed(stream))
        {
            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                int wl = SAMPLE_RATE / sineFrequency;
                buffer[i] = MathF.Sin(2 * MathF.PI * sineIndex / wl);
                sineIndex++;

                if (sineIndex >= wl)
                {
                    sineFrequency = newSineFrequency;
                    sineIndex = 0;
                    sineStartTime = GetTime();
                }
            }

            fixed (float* bufferPtr = buffer)
            {
                UpdateAudioStream(stream, bufferPtr, BUFFER_SIZE);
            }
        }

        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText($"sine frequency: {sineFrequency}", screenWidth - 220, 10, 20, Color.Red);
        DrawText($"pan: {pan:F2}", screenWidth - 220, 30, 20, Color.Red);
        DrawText("Up/down to change frequency", 10, 10, 20, Color.DarkGray);
        DrawText("Left/right to pan", 10, 30, 20, Color.DarkGray);

        int windowStart = (int)((GetTime() - sineStartTime) * SAMPLE_RATE);
        int windowSize = (int)(0.1f * SAMPLE_RATE);
        int wavelength = SAMPLE_RATE / sineFrequency;

        // Draw a sine wave with the same frequency as the one being sent to the audio stream
        for (int i = 0; i < screenWidth; i++)
        {
            int t0 = windowStart + i * windowSize / screenWidth;
            int t1 = windowStart + (i + 1) * windowSize / screenWidth;
            Vector2 startPos = new Vector2(i, 250 + 50 * MathF.Sin(2 * MathF.PI * t0 / wavelength));
            Vector2 endPos = new Vector2(i + 1, 250 + 50 * MathF.Sin(2 * MathF.PI * t1 / wavelength));
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

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - raw stream");

        SetTargetFPS(30);
        //--------------------------------------------------------------------------------------

        var game = new RawStream();
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
