/*******************************************************************************************
*
*   raylib [audio] example - module playing
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.5, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Audio;

public partial class ModulePlaying : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxCircles = 64;

    public string Name => "Audio / Module Playing";

    public string Title => "raylib [audio] example - module playing";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private struct CircleWave
    {
        public Vector2 Position;
        public float Radius;
        public float Alpha;
        public float Speed;
        public Color Color;
    }

    private Color[] colors;
    private CircleWave[] circles;
    private Music music;
    private float pitch;
    private float timePlayed;
    private bool pause;

    public void Init()
    {
        InitAudioDevice();                  // Initialize audio device

        colors = new Color[14] {
            Color.Orange,
            Color.Red,
            Color.Gold,
            Color.Lime,
            Color.Blue,
            Color.Violet,
            Color.Brown,
            Color.LightGray,
            Color.Pink,
            Color.Yellow,
            Color.Green,
            Color.SkyBlue,
            Color.Purple,
            Color.Beige
        };

        // Creates some circles for visual effect
        circles = new CircleWave[MaxCircles];

        for (var i = MaxCircles - 1; i >= 0; i--)
        {
            circles[i].Alpha = 0.0f;
            circles[i].Radius = GetRandomValue(10, 40);
            circles[i].Position.X = GetRandomValue((int)circles[i].Radius, screenWidth - (int)circles[i].Radius);
            circles[i].Position.Y = GetRandomValue((int)circles[i].Radius, screenHeight - (int)circles[i].Radius);
            circles[i].Speed = (float)GetRandomValue(1, 100) / 2000.0f;
            circles[i].Color = colors[GetRandomValue(0, 13)];
        }

        music = LoadMusicStream("resources/audio/mini1111.xm");
        music.Looping = false;
        pitch = 1.0f;

        PlayMusicStream(music);

        timePlayed = 0.0f;
        pause = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateMusicStream(music);      // Update music buffer with new stream data

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

            if (pause)
            {
                PauseMusicStream(music);
            }
            else
            {
                ResumeMusicStream(music);
            }
        }

        if (IsKeyDown(KeyboardKey.Down))
        {
            pitch -= 0.01f;
        }
        else if (IsKeyDown(KeyboardKey.Up))
        {
            pitch += 0.01f;
        }

        SetMusicPitch(music, pitch);

        // Get timePlayed scaled to bar dimensions
        timePlayed = GetMusicTimePlayed(music) / GetMusicTimeLength(music) * (screenWidth - 40);

        // Color circles animation
        for (var i = MaxCircles - 1; (i >= 0) && !pause; i--)
        {
            circles[i].Alpha += circles[i].Speed;
            circles[i].Radius += circles[i].Speed * 10.0f;

            if (circles[i].Alpha > 1.0f)
            {
                circles[i].Speed *= -1;
            }

            if (circles[i].Alpha <= 0.0f)
            {
                circles[i].Alpha = 0.0f;
                circles[i].Radius = GetRandomValue(10, 40);
                circles[i].Position.X = GetRandomValue(
                    (int)circles[i].Radius,
                    screenWidth - (int)circles[i].Radius
                );
                circles[i].Position.Y = GetRandomValue(
                    (int)circles[i].Radius,
                    screenHeight - (int)circles[i].Radius
                );
                circles[i].Color = colors[GetRandomValue(0, 13)];
                circles[i].Speed = (float)GetRandomValue(1, 100) / 2000.0f;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        for (var i = MaxCircles - 1; i >= 0; i--)
        {
            DrawCircleV(
                circles[i].Position,
                circles[i].Radius,
                Fade(circles[i].Color, circles[i].Alpha)
            );
        }

        // Draw time bar
        DrawRectangle(20, screenHeight - 20 - 12, screenWidth - 40, 12, Color.LightGray);
        DrawRectangle(20, screenHeight - 20 - 12, (int)timePlayed, 12, Color.Maroon);
        DrawRectangleLines(20, screenHeight - 20 - 12, screenWidth - 40, 12, Color.Gray);

        // Draw help instructions
        DrawRectangle(20, 20, 425, 145, Color.White);
        DrawRectangleLines(20, 20, 425, 145, Color.Gray);
        DrawText("PRESS SPACE TO RESTART MUSIC", 40, 40, 20, Color.Black);
        DrawText("PRESS P TO PAUSE/RESUME", 40, 70, 20, Color.Black);
        DrawText("PRESS UP/DOWN TO CHANGE SPEED", 40, 100, 20, Color.Black);
        DrawText($"SPEED: {pitch:F6}", 40, 130, 20, Color.Maroon);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadMusicStream(music);          // Unload music stream buffers from RAM

        CloseAudioDevice();     // Close audio device (music streaming is automatically stopped)
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);  // NOTE: Try to enable MSAA 4X

        InitWindow(screenWidth, screenHeight, "raylib [audio] example - module playing");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ModulePlaying();
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
