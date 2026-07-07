/*******************************************************************************************
*
*   raylib [textures] example - screen buffer
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Agnis Aldiņš (@nezvers) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Agnis Aldiņš (@nezvers)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class ScreenBuffer : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxColors = 256;
    private const int ScaleFactor = 2;

    private const int imageWidth = screenWidth / ScaleFactor;
    private const int imageHeight = screenHeight / ScaleFactor;
    private const int flameWidth = screenWidth / ScaleFactor;

    public string Name => "Textures / Screen Buffer";

    public string Title => "raylib [textures] example - screen buffer";

    private Color[] palette;
    private byte[] indexBuffer;
    private byte[] flameRootBuffer;
    private Image screenImage;
    private Texture2D screenTexture;

    public void Init()
    {
        palette = new Color[MaxColors];
        indexBuffer = new byte[imageWidth * imageWidth];
        flameRootBuffer = new byte[flameWidth];

        screenImage = GenImageColor(imageWidth, imageHeight, Color.Black);
        screenTexture = LoadTextureFromImage(screenImage);

        // Generate flame color palette
        for (var i = 0; i < MaxColors; i++)
        {
            var t = (float)i / (float)(MaxColors - 1);
            var hue = t * t;
            var saturation = t;
            var value = t;

            palette[i] = ColorFromHSV(250.0f + 150.0f * hue, saturation, value);
        }
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Grow flameRoot
        for (var x = 2; x < flameWidth; x++)
        {
            var flame = (int)flameRootBuffer[x];
            flame += GetRandomValue(0, 2);
            flameRootBuffer[x] = (flame > 255) ? (byte)255 : (byte)flame;
        }

        // Transfer flameRoot to indexBuffer
        for (var x = 0; x < flameWidth; x++)
        {
            var i = x + (imageHeight - 1) * imageWidth;
            indexBuffer[i] = flameRootBuffer[x];
        }

        // Clear top row, because it can't move any higher
        for (var x = 0; x < imageWidth; x++)
        {
            if (indexBuffer[x] != 0)
            {
                indexBuffer[x] = 0;
            }
        }

        // Skip top row, it is already cleared
        for (var y = 1; y < imageHeight; y++)
        {
            for (var x = 0; x < imageWidth; x++)
            {
                var i = x + y * imageWidth;
                int colorIndex = indexBuffer[i];

                if (colorIndex != 0)
                {
                    // Move pixel a row above
                    indexBuffer[i] = 0;
                    var moveX = GetRandomValue(0, 2) - 1;
                    var newX = x + moveX;

                    if ((newX > 0) && (newX < imageWidth))
                    {
                        var iabove = i - imageWidth + moveX;
                        var decay = GetRandomValue(0, 3);
                        colorIndex -= (decay < colorIndex) ? decay : colorIndex;
                        indexBuffer[iabove] = (byte)colorIndex;
                    }
                }
            }
        }

        // Update screenImage with palette colors
        for (var y = 1; y < imageHeight; y++)
        {
            for (var x = 0; x < imageWidth; x++)
            {
                var i = x + y * imageWidth;
                int colorIndex = indexBuffer[i];
                var col = palette[colorIndex];

                ImageDrawPixel(ref screenImage, x, y, col);
            }
        }

        UpdateTexture(screenTexture, screenImage.Data);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawTextureEx(screenTexture, new Vector2(0, 0), 0.0f, 2.0f, Color.White);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(screenTexture);
        UnloadImage(screenImage);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - screen buffer");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ScreenBuffer();
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
