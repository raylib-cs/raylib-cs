/*******************************************************************************************
*
*   raylib [audio] example - sound multi
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.0
*
*   Example contributed by Jeffery Myers (@JeffM2501) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Jeffery Myers (@JeffM2501)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Audio;

public partial class SoundMulti : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_SOUNDS = 10;

    public string Name => "Audio / Sound Multi";

    public string Title => "raylib [audio] example - sound multi";

    private Sound[] soundArray = new Sound[MAX_SOUNDS];
    private int currentSound;

    public void Init()
    {
        InitAudioDevice();      // Initialize audio device

        // Load audio file into the first slot as the 'source' sound,
        // this sound owns the sample data
        soundArray[0] = LoadSound("resources/audio/sound.wav");

        // Load an alias of the sound into slots 1-9. These do not own the sound data, but can be played
        for (int i = 1; i < MAX_SOUNDS; i++) soundArray[i] = LoadSoundAlias(soundArray[0]);

        currentSound = 0;               // Set the sound list to the start
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            PlaySound(soundArray[currentSound]);    // Play the next open sound slot
            currentSound++;                         // Increment the sound slot

            // If the sound slot is out of bounds, go back to 0
            if (currentSound >= MAX_SOUNDS) currentSound = 0;

            // NOTE: Another approach would be to look at the list for the first sound
            // that is not playing and use that slot
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText("Press SPACE to PLAY a WAV sound!", 200, 180, 20, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        for (int i = 1; i < MAX_SOUNDS; i++) UnloadSoundAlias(soundArray[i]); // Unload sound aliases
        UnloadSound(soundArray[0]); // Unload source sound data

        CloseAudioDevice();     // Close audio device
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - sound multi");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SoundMulti();
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
