/*******************************************************************************************
*
*   raylib [audio] example - music stream
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Audio;

public partial class MusicStreamDemo : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Audio / Music Stream Demo";

    public string Title => "raylib [audio] example - music stream";

    public int TargetFps => 30;

    private Music music;
    private float timePlayed;
    private bool pause;
    private float pan;
    private float volume;

    public void Init()
    {
        InitAudioDevice();              // Initialize audio device

        music = LoadMusicStream("resources/audio/country.mp3");

        PlayMusicStream(music);

        timePlayed = 0.0f;              // Time played normalized [0.0f..1.0f]
        pause = false;                  // Music playing paused

        pan = 0.0f;                     // Default audio pan center [-1.0f..1.0f]
        SetMusicPan(music, pan);

        volume = 0.8f;                  // Default audio volume [0.0f..1.0f]
        SetMusicVolume(music, volume);
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

            if (pause)
            {
                PauseMusicStream(music);
            }
            else
            {
                ResumeMusicStream(music);
            }
        }

        // Set audio pan
        if (IsKeyDown(KeyboardKey.Left))
        {
            pan -= 0.05f;
            if (pan < -1.0f)
            {
                pan = -1.0f;
            }
            SetMusicPan(music, pan);
        }
        else if (IsKeyDown(KeyboardKey.Right))
        {
            pan += 0.05f;
            if (pan > 1.0f)
            {
                pan = 1.0f;
            }
            SetMusicPan(music, pan);
        }

        // Set audio volume
        if (IsKeyDown(KeyboardKey.Down))
        {
            volume -= 0.05f;
            if (volume < 0.0f)
            {
                volume = 0.0f;
            }
            SetMusicVolume(music, volume);
        }
        else if (IsKeyDown(KeyboardKey.Up))
        {
            volume += 0.05f;
            if (volume > 1.0f)
            {
                volume = 1.0f;
            }
            SetMusicVolume(music, volume);
        }

        // Get normalized time played for current music stream
        timePlayed = GetMusicTimePlayed(music) / GetMusicTimeLength(music);

        if (timePlayed > 1.0f)
        {
            timePlayed = 1.0f;   // Make sure time played is no longer than music
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("MUSIC SHOULD BE PLAYING!", 255, 150, 20, Color.LightGray);

        DrawText("LEFT-RIGHT for PAN CONTROL", 320, 74, 10, Color.DarkBlue);
        DrawRectangle(300, 100, 200, 12, Color.LightGray);
        DrawRectangleLines(300, 100, 200, 12, Color.Gray);
        DrawRectangle((int)(300 + (pan + 1.0f) / 2.0f * 200 - 5), 92, 10, 28, Color.DarkGray);

        DrawRectangle(200, 200, 400, 12, Color.LightGray);
        DrawRectangle(200, 200, (int)(timePlayed * 400.0f), 12, Color.Maroon);
        DrawRectangleLines(200, 200, 400, 12, Color.Gray);

        DrawText("PRESS SPACE TO RESTART MUSIC", 215, 250, 20, Color.LightGray);
        DrawText("PRESS P TO PAUSE/RESUME MUSIC", 208, 280, 20, Color.LightGray);

        DrawText("UP-DOWN for VOLUME CONTROL", 320, 334, 10, Color.DarkGreen);
        DrawRectangle(300, 360, 200, 12, Color.LightGray);
        DrawRectangleLines(300, 360, 200, 12, Color.Gray);
        DrawRectangle((int)(300 + volume * 200 - 5), 352, 10, 28, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadMusicStream(music);   // Unload music stream buffers from RAM

        CloseAudioDevice();         // Close audio device (music streaming is automatically stopped)
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - music stream");

        SetTargetFPS(30);               // Set our game to run at 30 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new MusicStreamDemo();
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
