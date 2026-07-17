/*******************************************************************************************
*
*   raylib [textures] example - sprite explosion
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class SpriteExplosion : IExample
{
    private const int NumFramesPerLine = 5;
    private const int NumLines = 5;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Sprite Explosion";

    public string Title => "raylib [textures] example - sprite explosion";

    private Sound fxBoom;
    private Texture2D explosion;
    private int frameWidth;
    private int frameHeight;
    private int currentFrame;
    private int currentLine;
    private Rectangle frameRec;
    private Vector2 position;
    private bool active;
    private int framesCounter;

    public void Init()
    {
        InitAudioDevice();

        // Load explosion sound
        fxBoom = LoadSound("resources/audio/boom.wav");

        // Load explosion texture
        explosion = LoadTexture("resources/explosion.png");

        // Init variables for animation
        frameWidth = explosion.Width / NumFramesPerLine;   // Sprite one frame rectangle width
        frameHeight = explosion.Height / NumLines;         // Sprite one frame rectangle height
        currentFrame = 0;
        currentLine = 0;

        frameRec = new(0, 0, frameWidth, frameHeight);
        position = new(0.0f, 0.0f);

        active = false;
        framesCounter = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------

        // Check for mouse button pressed and activate explosion (if not active)
        if (IsMouseButtonPressed(MouseButton.Left) && !active)
        {
            position = GetMousePosition();
            active = true;

            position.X -= frameWidth / 2;
            position.Y -= frameHeight / 2;

            PlaySound(fxBoom);
        }

        // Compute explosion animation frames
        if (active)
        {
            framesCounter++;

            if (framesCounter > 2)
            {
                currentFrame++;

                if (currentFrame >= NumFramesPerLine)
                {
                    currentFrame = 0;
                    currentLine++;

                    if (currentLine >= NumLines)
                    {
                        currentLine = 0;
                        active = false;
                    }
                }

                framesCounter = 0;
            }
        }

        frameRec.X = frameWidth * currentFrame;
        frameRec.Y = frameHeight * currentLine;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw explosion required frame rectangle
        if (active)
        {
            DrawTextureRec(explosion, frameRec, position, Color.White);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(explosion);   // Unload texture
        UnloadSound(fxBoom);        // Unload sound

        CloseAudioDevice();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - sprite explosion");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SpriteExplosion();
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
