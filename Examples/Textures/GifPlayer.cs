/*******************************************************************************************
*
*   raylib [textures] example - gif player
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class GifPlayer : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxFrameDelay = 20;
    private const int MinFrameDelay = 1;

    public string Name => "Textures / Gif Player";

    public string Title => "raylib [textures] example - gif player";

    private int animFrames;
    private Image imScarfyAnim;
    private Texture2D texScarfyAnim;
    private uint nextFrameDataOffset;  // Current byte offset to next frame in image.data
    private int currentAnimFrame;      // Current animation frame to load and draw
    private int frameDelay;            // Frame delay to switch between animation frames
    private int frameCounter;          // General frames counter

    public void Init()
    {
        animFrames = 0;

        // Load all GIF animation frames into a single Image
        // NOTE: GIF data is always loaded as RGBA (32bit) by default
        // NOTE: Frames are just appended one after another in image.data memory
        imScarfyAnim = LoadImageAnim("resources/scarfy_run.gif", out animFrames);

        // Load texture from image
        // NOTE: We will update this texture when required with next frame data
        // WARNING: It's not recommended to use this technique for sprites animation,
        // use spritesheets instead, like illustrated in textures_sprite_anim example
        texScarfyAnim = LoadTextureFromImage(imScarfyAnim);

        nextFrameDataOffset = 0;

        currentAnimFrame = 0;
        frameDelay = 8;
        frameCounter = 0;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        frameCounter++;
        if (frameCounter >= frameDelay)
        {
            // Move to next frame
            // NOTE: If final frame is reached we return to first frame
            currentAnimFrame++;
            if (currentAnimFrame >= animFrames)
            {
                currentAnimFrame = 0;
            }

            // Get memory offset position for next frame data in image.data
            nextFrameDataOffset = (uint)(imScarfyAnim.Width * imScarfyAnim.Height * 4 * currentAnimFrame);

            // Update GPU texture data with next frame image data
            // WARNING: Data size (frame size) and pixel format must match already created texture
            UpdateTexture(texScarfyAnim, (byte*)imScarfyAnim.Data + nextFrameDataOffset);

            frameCounter = 0;
        }

        // Control frames delay
        if (IsKeyPressed(KeyboardKey.Right))
        {
            frameDelay++;
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            frameDelay--;
        }

        if (frameDelay > MaxFrameDelay)
        {
            frameDelay = MaxFrameDelay;
        }
        else if (frameDelay < MinFrameDelay)
        {
            frameDelay = MinFrameDelay;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawText($"TOTAL GIF FRAMES:  {animFrames:D2}", 50, 30, 20, Color.LightGray);
        DrawText($"CURRENT FRAME: {currentAnimFrame:D2}", 50, 60, 20, Color.Gray);
        DrawText($"CURRENT FRAME IMAGE.DATA OFFSET: {nextFrameDataOffset:D2}", 50, 90, 20, Color.Gray);

        DrawText("FRAMES DELAY: ", 100, 305, 10, Color.DarkGray);
        DrawText($"{frameDelay:D2} frames", 620, 305, 10, Color.DarkGray);
        DrawText("PRESS RIGHT/LEFT KEYS to CHANGE SPEED!", 290, 350, 10, Color.DarkGray);

        for (var i = 0; i < MaxFrameDelay; i++)
        {
            if (i < frameDelay)
            {
                DrawRectangle(190 + 21 * i, 300, 20, 20, Color.Red);
            }
            DrawRectangleLines(190 + 21 * i, 300, 20, 20, Color.Maroon);
        }

        DrawTexture(texScarfyAnim, GetScreenWidth() / 2 - texScarfyAnim.Width / 2, 140, Color.White);

        DrawText("(c) Scarfy sprite by Eiden Marsal", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texScarfyAnim);   // Unload texture
        UnloadImage(imScarfyAnim);      // Unload image (contains all frames)
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - gif player");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new GifPlayer();
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
