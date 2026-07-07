/*******************************************************************************************
*
*   raylib [textures] example - npatch drawing
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example originally created with raylib 2.0, last time updated with raylib 2.5
*
*   Example contributed by Jorge A. Gomes (@overdev) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Jorge A. Gomes (@overdev) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class NpatchDrawing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / N-patch Drawing";

    public string Title => "raylib [textures] example - npatch drawing";

    private Texture2D nPatchTexture;

    private Vector2 mousePosition;
    private Vector2 origin;

    private Rectangle dstRec1;
    private Rectangle dstRec2;
    private Rectangle dstRecH;
    private Rectangle dstRecV;

    private NPatchInfo ninePatchInfo1;
    private NPatchInfo ninePatchInfo2;
    private NPatchInfo h3PatchInfo;
    private NPatchInfo v3PatchInfo;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
        nPatchTexture = LoadTexture("resources/ninepatch_button.png");

        mousePosition = new(0.0f, 0.0f);
        origin = new(0.0f, 0.0f);

        // Position and size of the n-patches
        dstRec1 = new(480.0f, 160.0f, 32.0f, 32.0f);
        dstRec2 = new(160.0f, 160.0f, 32.0f, 32.0f);
        dstRecH = new(160.0f, 93.0f, 32.0f, 32.0f);
        dstRecV = new(92.0f, 160.0f, 32.0f, 32.0f);

        // A 9-patch (NPATCH_NINE_PATCH) changes its sizes in both axis
        ninePatchInfo1 = new NPatchInfo
        {
            Source = new Rectangle(0.0f, 0.0f, 64.0f, 64.0f),
            Left = 12,
            Top = 40,
            Right = 12,
            Bottom = 12,
            Layout = NPatchLayout.NinePatch
        };
        ninePatchInfo2 = new NPatchInfo
        {
            Source = new Rectangle(0.0f, 128.0f, 64.0f, 64.0f),
            Left = 16,
            Top = 16,
            Right = 16,
            Bottom = 16,
            Layout = NPatchLayout.NinePatch
        };

        // A horizontal 3-patch (NPATCH_THREE_PATCH_HORIZONTAL) changes its sizes along the x axis only
        h3PatchInfo = new NPatchInfo
        {
            Source = new Rectangle(0.0f, 64.0f, 64.0f, 64.0f),
            Left = 8,
            Top = 8,
            Right = 8,
            Bottom = 8,
            Layout = NPatchLayout.ThreePatchHorizontal
        };

        // A vertical 3-patch (NPATCH_THREE_PATCH_VERTICAL) changes its sizes along the y axis only
        v3PatchInfo = new NPatchInfo
        {
            Source = new Rectangle(0.0f, 192.0f, 64.0f, 64.0f),
            Left = 6,
            Top = 6,
            Right = 6,
            Bottom = 6,
            Layout = NPatchLayout.ThreePatchVertical
        };
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        mousePosition = GetMousePosition();

        // Resize the n-patches based on mouse position
        dstRec1.Width = mousePosition.X - dstRec1.X;
        dstRec1.Height = mousePosition.Y - dstRec1.Y;
        dstRec2.Width = mousePosition.X - dstRec2.X;
        dstRec2.Height = mousePosition.Y - dstRec2.Y;
        dstRecH.Width = mousePosition.X - dstRecH.X;
        dstRecV.Height = mousePosition.Y - dstRecV.Y;

        // Set a minimum width and/or height
        dstRec1.Width = Math.Clamp(dstRec1.Width, 1.0f, 300.0f);
        dstRec1.Height = MathF.Max(dstRec1.Height, 1.0f);
        dstRec2.Width = Math.Clamp(dstRec2.Width, 1.0f, 300.0f);
        dstRec2.Height = MathF.Max(dstRec2.Height, 1.0f);
        dstRecH.Width = MathF.Max(dstRecH.Width, 1.0f);
        dstRecV.Height = MathF.Max(dstRecV.Height, 1.0f);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw the n-patches
        DrawTextureNPatch(nPatchTexture, ninePatchInfo2, dstRec2, origin, 0.0f, Color.White);
        DrawTextureNPatch(nPatchTexture, ninePatchInfo1, dstRec1, origin, 0.0f, Color.White);
        DrawTextureNPatch(nPatchTexture, h3PatchInfo, dstRecH, origin, 0.0f, Color.White);
        DrawTextureNPatch(nPatchTexture, v3PatchInfo, dstRecV, origin, 0.0f, Color.White);

        // Draw the source texture
        DrawRectangleLines(5, 88, 74, 266, Color.Blue);
        DrawTexture(nPatchTexture, 10, 93, Color.White);
        DrawText("TEXTURE", 15, 360, 10, Color.DarkGray);

        DrawText("Move the mouse to stretch or shrink the n-patches", 10, 20, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(nPatchTexture);       // Texture unloading
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - npatch drawing");

        SetTargetFPS(60);
        //---------------------------------------------------------------------------------------

        var game = new NpatchDrawing();
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
