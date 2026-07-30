/*******************************************************************************************
*
*   raylib [audio] example - amp envelope
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Arbinda Rizki Muhammad (@arbipink) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Arbinda Rizki Muhammad (@arbipink)
*
********************************************************************************************/

namespace Examples.Audio;

public unsafe partial class AmpEnvelope : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int BUFFER_SIZE = 4096;
    private const int SAMPLE_RATE = 44100;

    // Wave state
    private enum ADSRState
    {
        Idle,
        Attack,
        Decay,
        Sustain,
        Release
    }

    // Grouping all ADSR parameters and state into a struct
    private struct Envelope
    {
        public float AttackTime;
        public float DecayTime;
        public float SustainLevel;
        public float ReleaseTime;
        public float CurrentValue;
        public ADSRState State;
    }

    public string Name => "Audio / Amp Envelope";

    public string Title => "raylib [audio] example - amp envelope";

    private float[] buffer;
    private AudioStream stream;
    private float audioTime;
    private Envelope env;

    public void Init()
    {
        InitAudioDevice();

        // Set the number of samples the stream will keep in memory at a time to BUFFER_SIZE
        SetAudioStreamBufferSizeDefault(BUFFER_SIZE);
        buffer = new float[BUFFER_SIZE];

        // Init raw audio stream (sample rate: 44100, sample size: 32bit-float, channels: 1-mono)
        stream = LoadAudioStream(SAMPLE_RATE, 32, 1);

        // Init Phase
        audioTime = 0.0f;

        // Initialize the struct
        env = new Envelope
        {
            AttackTime = 1.0f,
            DecayTime = 1.0f,
            SustainLevel = 0.5f,
            ReleaseTime = 1.0f,
            CurrentValue = 0.0f,
            State = ADSRState.Idle
        };
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            env.State = ADSRState.Attack;
        }

        if (IsKeyReleased(KeyboardKey.Space) && (env.State != ADSRState.Idle))
        {
            env.State = ADSRState.Release;
        }

        if (IsAudioStreamProcessed(stream))
        {
            if ((env.State != ADSRState.Idle) || (env.CurrentValue > 0.0f))
            {
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    UpdateEnvelope(ref env);
                    FillAudioBuffer(i, buffer, env.CurrentValue, ref audioTime);
                }
            }
            else
            {
                // Clear buffer if silent to avoid looping noise
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    buffer[i] = 0;
                }

                audioTime = 0.0f;
            }

            fixed (float* bufferPtr = buffer)
            {
                UpdateAudioStream(stream, bufferPtr, BUFFER_SIZE);
            }
        }

        if (!IsAudioStreamPlaying(stream))
        {
            PlayAudioStream(stream);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // NOTE: raygui is not bound in raylib-cs, so the sliders below are left as reference.
        // The envelope keeps its default parameters (Attack/Decay/Release = 1.0s, Sustain = 0.5).
        //GuiSliderBar(new Rectangle( 100, 60, 400, 30 ), "Attack (s)", TextFormat("%2.2fs", env.AttackTime), ref env.AttackTime, 0.1f, 3.0f);
        //GuiSliderBar(new Rectangle( 100, 100, 400, 30 ), "Decay (s)", TextFormat("%2.2fs", env.DecayTime), ref env.DecayTime, 0.1f, 3.0f);
        //GuiSliderBar(new Rectangle( 100, 140, 400, 30 ), "Sustain", TextFormat("%2.2f", env.SustainLevel), ref env.SustainLevel, 0.0f, 1.0f);
        //GuiSliderBar(new Rectangle( 100, 180, 400, 30 ), "Release (s)", TextFormat("%2.2fs", env.ReleaseTime), ref env.ReleaseTime, 0.1f, 3.0f);

        DrawADSRGraph(ref env, new Rectangle(100, 250, 400, 100));

        DrawCircleV(new Vector2(520, 350 - (env.CurrentValue * 100)), 5, Color.Maroon);
        DrawText($"Current Gain: {env.CurrentValue:F2}", 535, (int)(345 - (env.CurrentValue * 100)), 10, Color.Maroon);

        DrawText("Press SPACE to PLAY the sound!", 200, 400, 20, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadAudioStream(stream);
        CloseAudioDevice();
    }

    //------------------------------------------------------------------------------------
    // Module Functions Definition
    //------------------------------------------------------------------------------------
    private static void FillAudioBuffer(int i, float[] buffer, float envelopeValue, ref float audioTime)
    {
        int frequency = 440;
        buffer[i] = envelopeValue * MathF.Sin(2.0f * MathF.PI * frequency * audioTime);
        audioTime += (1.0f / SAMPLE_RATE);
    }

    private static void UpdateEnvelope(ref Envelope env)
    {
        // Calculate the time delta for ONE sample (1/44100)
        float sampleTime = 1.0f / SAMPLE_RATE;

        switch (env.State)
        {
            case ADSRState.Attack:
                {
                    env.CurrentValue += (1.0f / env.AttackTime) * sampleTime;
                    if (env.CurrentValue >= 1.0f)
                    {
                        env.CurrentValue = 1.0f;
                        env.State = ADSRState.Decay;
                    }
                }
                break;
            case ADSRState.Decay:
                {
                    env.CurrentValue -= ((1.0f - env.SustainLevel) / env.DecayTime) * sampleTime;
                    if (env.CurrentValue <= env.SustainLevel)
                    {
                        env.CurrentValue = env.SustainLevel;
                        env.State = ADSRState.Sustain;
                    }
                }
                break;
            case ADSRState.Sustain:
                {
                    env.CurrentValue = env.SustainLevel;
                }
                break;
            case ADSRState.Release:
                {
                    env.CurrentValue -= (env.SustainLevel / env.ReleaseTime) * sampleTime;
                    if (env.CurrentValue <= 0.001f) // Use a small threshold to avoid infinite tail
                    {
                        env.CurrentValue = 0.0f;
                        env.State = ADSRState.Idle;
                    }
                }
                break;
            default:
                break;
        }
    }

    private static void DrawADSRGraph(ref Envelope env, Rectangle bounds)
    {
        DrawRectangleRec(bounds, Fade(Color.LightGray, 0.3f));
        DrawRectangleLinesEx(bounds, 1, Color.Gray);

        // Fixed visual width for sustain stage since it's an amplitude not a time value
        float sustainWidth = 1.0f;

        // Total time to visualize (sum of A, D, R + a padding for Sustain)
        float totalTime = env.AttackTime + env.DecayTime + sustainWidth + env.ReleaseTime;

        float scaleX = bounds.Width / totalTime;
        float scaleY = bounds.Height;

        Vector2 start = new Vector2(bounds.X, bounds.Y + bounds.Height);
        Vector2 peak = new Vector2(start.X + (env.AttackTime * scaleX), bounds.Y);
        Vector2 sustain = new Vector2(peak.X + (env.DecayTime * scaleX), bounds.Y + (1.0f - env.SustainLevel) * scaleY);
        Vector2 rel = new Vector2(sustain.X + (sustainWidth * scaleX), sustain.Y);
        Vector2 end = new Vector2(rel.X + (env.ReleaseTime * scaleX), bounds.Y + bounds.Height);

        DrawLineV(start, peak, Color.SkyBlue);
        DrawLineV(peak, sustain, Color.Blue);
        DrawLineV(sustain, rel, Color.DarkBlue);
        DrawLineV(rel, end, Color.Orange);

        DrawText("ADSR Visualizer", (int)bounds.X, (int)bounds.Y - 20, 10, Color.DarkGray);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - amp envelope");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new AmpEnvelope();
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
