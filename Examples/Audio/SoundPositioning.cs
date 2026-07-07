/*******************************************************************************************
*
*   raylib [audio] example - sound positioning
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Le Juez Victor (@Bigfoot71) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Le Juez Victor (@Bigfoot71)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Audio;

public partial class SoundPositioning : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Audio / Sound Positioning";

    public string Title => "raylib [audio] example - sound positioning";

    public bool CursorDisabled => true;

    private Sound sound;
    private Camera3D camera;

    public void Init()
    {
        InitAudioDevice();

        sound = LoadSound("resources/audio/coin.wav");

        camera = new Camera3D
        {
            Position = new Vector3(0, 5, 5),
            Target = new Vector3(0, 0, 0),
            Up = new Vector3(0, 1, 0),
            FovY = 60,
            Projection = CameraProjection.Perspective
        };
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Free);

        float th = (float)GetTime();

        Vector3 spherePos = new Vector3(
            5.0f * MathF.Cos(th),
            0.0f,
            5.0f * MathF.Sin(th)
        );

        SetSoundPosition(camera, sound, spherePos, 1.0f);

        if (!IsSoundPlaying(sound)) PlaySound(sound);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        DrawGrid(10, 2);
        DrawSphere(spherePos, 0.5f, Color.Red);
        EndMode3D();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadSound(sound);
        CloseAudioDevice();     // Close audio device
    }

    //------------------------------------------------------------------------------------
    // Module Functions Definition
    //------------------------------------------------------------------------------------
    // Set sound 3d position
    private static void SetSoundPosition(Camera3D listener, Sound sound, Vector3 position, float maxDist)
    {
        // Calculate direction vector and distance between listener and sound source
        Vector3 direction = Vector3.Subtract(position, listener.Position);
        float distance = direction.Length();

        // Apply logarithmic distance attenuation and clamp between 0-1
        float attenuation = 1.0f / (1.0f + (distance / maxDist));
        attenuation = Math.Clamp(attenuation, 0.0f, 1.0f);

        // Calculate normalized vectors for spatial positioning
        Vector3 normalizedDirection = Vector3.Normalize(direction);
        Vector3 forward = Vector3.Normalize(Vector3.Subtract(listener.Target, listener.Position));
        Vector3 right = Vector3.Normalize(Vector3.Cross(listener.Up, forward));

        // Reduce volume for sounds behind the listener
        float dotProduct = Vector3.Dot(forward, normalizedDirection);
        if (dotProduct < 0.0f) attenuation *= (1.0f + dotProduct * 0.5f);

        // Set stereo panning based on sound position relative to listener
        float pan = 0.5f + 0.5f * Vector3.Dot(normalizedDirection, right);

        // Apply final sound properties
        SetSoundVolume(sound, attenuation);
        SetSoundPan(sound, pan);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [audio] example - sound positioning");

        DisableCursor();

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new SoundPositioning();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
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
