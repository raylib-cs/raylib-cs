/*******************************************************************************************
*
*   raylib [textures] example - sprite button
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
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

public partial class SpriteButton : IExample
{
    // Number of frames (rectangles) for the button sprite texture
    public const int NumFrames = 3;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Sprite Button";

    public string Title => "raylib [textures] example - sprite button";

    private Sound fxButton;
    private Texture2D button;
    private int frameHeight;
    private Rectangle sourceRec;
    private Rectangle btnBounds;
    private int btnState;
    private bool btnAction;
    private Vector2 mousePoint;

    public void Init()
    {
        InitAudioDevice();      // Initialize audio device

        fxButton = LoadSound("resources/audio/buttonfx.wav");   // Load button sound
        button = LoadTexture("resources/button.png"); // Load button texture

        // Define frame rectangle for drawing
        frameHeight = button.Height / NumFrames;
        sourceRec = new(0, 0, button.Width, frameHeight);

        // Define button bounds on screen
        btnBounds = new(
            screenWidth / 2 - button.Width / 2,
            screenHeight / 2 - button.Height / NumFrames / 2,
            button.Width,
            frameHeight
        );

        // Button state: 0-NORMAL, 1-MOUSE_HOVER, 2-PRESSED
        btnState = 0;

        // Button action should be activated
        btnAction = false;

        mousePoint = new(0.0f, 0.0f);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        mousePoint = GetMousePosition();
        btnAction = false;

        // Check button state
        if (CheckCollisionPointRec(mousePoint, btnBounds))
        {
            if (IsMouseButtonDown(MouseButton.Left))
            {
                btnState = 2;
            }
            else
            {
                btnState = 1;
            }

            if (IsMouseButtonReleased(MouseButton.Left))
            {
                btnAction = true;
            }
        }
        else
        {
            btnState = 0;
        }

        if (btnAction)
        {
            PlaySound(fxButton);
            // TODO: Any desired action
        }

        // Calculate button frame rectangle to draw depending on button state
        sourceRec.Y = btnState * frameHeight;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawTextureRec(button, sourceRec, new Vector2(btnBounds.X, btnBounds.Y), Color.White); // Draw button frame

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(button);  // Unload button texture
        UnloadSound(fxButton);  // Unload sound

        CloseAudioDevice();     // Close audio device
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - sprite button");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new SpriteButton();
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
