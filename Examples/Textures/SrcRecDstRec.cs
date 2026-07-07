/*******************************************************************************************
*
*   raylib [textures] example - srcrec dstrec
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 1.3, last time updated with raylib 1.3
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class SrcRecDstRec : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Src and Dst Rectangles";

    public string Title => "raylib [textures] example - srcrec dstrec";

    private Texture2D scarfy;
    private Rectangle sourceRec;
    private Rectangle destRec;
    private Vector2 origin;
    private int rotation;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)

        scarfy = LoadTexture("resources/scarfy.png");        // Texture loading

        var frameWidth = scarfy.Width / 6;
        var frameHeight = scarfy.Height;

        // Source rectangle (part of the texture to use for drawing)
        sourceRec = new(0, 0, frameWidth, frameHeight);

        // Destination rectangle (screen rectangle where drawing part of texture)
        destRec = new(screenWidth / 2, screenHeight / 2, frameWidth * 2, frameHeight * 2);

        // Origin of the texture (rotation/scale point), it's relative to destination rectangle size
        origin = new(frameWidth, frameHeight);

        rotation = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        rotation++;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // NOTE: Using DrawTexturePro() we can easily rotate and scale the part of the texture we draw
        // sourceRec defines the part of the texture we use for drawing
        // destRec defines the rectangle where our texture part will fit (scaling it to fit)
        // origin defines the point of the texture used as reference for rotation and scaling
        // rotation defines the texture rotation (using origin as rotation point)
        DrawTexturePro(scarfy, sourceRec, destRec, origin, rotation, Color.White);

        DrawLine((int)destRec.X, 0, (int)destRec.X, screenHeight, Color.Gray);
        DrawLine(0, (int)destRec.Y, screenWidth, (int)destRec.Y, Color.Gray);

        DrawText("(c) Scarfy sprite by Eiden Marsal", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(scarfy);        // Texture unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - srcrec dstrec");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new SrcRecDstRec();
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
