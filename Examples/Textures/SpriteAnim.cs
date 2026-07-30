/*******************************************************************************************
*
*   raylib [textures] example - sprite animation
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.3, last time updated with raylib 1.3
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class SpriteAnim : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int MaxFrameSpeed = 15;
    public const int MinFrameSpeed = 1;

    public string Name => "Textures / Sprite Anim";

    public string Title => "raylib [textures] example - sprite animation";

    private Texture2D scarfy;
    private Vector2 position;
    private Rectangle frameRec;
    private int currentFrame;
    private int framesCounter;
    private int framesSpeed;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
        scarfy = LoadTexture("resources/scarfy.png");        // Texture loading

        position = new(350.0f, 280.0f);
        frameRec = new(0.0f, 0.0f, (float)scarfy.Width / 6, (float)scarfy.Height);
        currentFrame = 0;

        framesCounter = 0;
        framesSpeed = 8;            // Number of spritesheet frames shown by second
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        framesCounter++;

        if (framesCounter >= (60 / framesSpeed))
        {
            framesCounter = 0;
            currentFrame++;

            if (currentFrame > 5)
            {
                currentFrame = 0;
            }

            frameRec.X = (float)currentFrame * (float)scarfy.Width / 6;
        }

        if (IsKeyPressed(KeyboardKey.Right))
        {
            framesSpeed++;
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            framesSpeed--;
        }

        framesSpeed = Math.Clamp(framesSpeed, MinFrameSpeed, MaxFrameSpeed);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawTexture(scarfy, 15, 40, Color.White);
        DrawRectangleLines(15, 40, scarfy.Width, scarfy.Height, Color.Lime);
        DrawRectangleLines(
            15 + (int)frameRec.X,
            40 + (int)frameRec.Y,
            (int)frameRec.Width,
            (int)frameRec.Height,
            Color.Red
        );

        DrawText("FRAME SPEED: ", 165, 210, 10, Color.DarkGray);
        DrawText($"{framesSpeed:D2} FPS", 575, 210, 10, Color.DarkGray);
        DrawText("PRESS RIGHT/LEFT KEYS to CHANGE SPEED!", 290, 240, 10, Color.DarkGray);

        for (var i = 0; i < MaxFrameSpeed; i++)
        {
            if (i < framesSpeed)
            {
                DrawRectangle(250 + 21 * i, 205, 20, 20, Color.Red);
            }
            DrawRectangleLines(250 + 21 * i, 205, 20, 20, Color.Maroon);
        }

        DrawTextureRec(scarfy, frameRec, position, Color.White);  // Draw part of the texture

        DrawText("(c) Scarfy sprite by Eiden Marsal", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(scarfy);       // Texture unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - sprite animation");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SpriteAnim();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
