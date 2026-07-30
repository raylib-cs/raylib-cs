/*******************************************************************************************
*
*   raylib [textures] example - tiled drawing
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 3.0, last time updated with raylib 4.2
*
*   Example contributed by Vlad Adrian (@demizdor) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2020-2025 Vlad Adrian (@demizdor) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class DrawTiled : IExample
{
    private const int OptWidth = 220;
    private const int MarginSize = 8;
    private const int ColorSize = 16;

    public string Name => "Textures / Draw Tiled";

    public string Title => "raylib [textures] example - tiled drawing";

    public ConfigFlags ConfigFlags => ConfigFlags.ResizableWindow;

    private int screenWidth;
    private int screenHeight;

    private Texture2D texPattern;

    // Coordinates for all patterns inside the texture
    private Rectangle[] recPattern;

    // Setup colors
    private Color[] colors;
    private Rectangle[] colorRec;

    private int activePattern;
    private int activeCol;
    private float scale;
    private float rotation;

    public void Init()
    {
        screenWidth = 800;
        screenHeight = 450;

        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)
        texPattern = LoadTexture("resources/patterns.png");
        SetTextureFilter(texPattern, TextureFilter.Bilinear); // Makes the texture smoother when upscaled

        // Coordinates for all patterns inside the texture
        recPattern = new[] {
            new Rectangle(3, 3, 66, 66),
            new Rectangle(75, 3, 100, 100),
            new Rectangle(3, 75, 66, 66),
            new Rectangle(7, 156, 50, 50),
            new Rectangle(85, 106, 90, 45),
            new Rectangle(75, 154, 100, 60)
        };

        // Setup colors
        colors = new[]
        {
            Color.Black,
            Color.Maroon,
            Color.Orange,
            Color.Blue,
            Color.Purple,
            Color.Beige,
            Color.Lime,
            Color.Red,
            Color.DarkGray,
            Color.SkyBlue
        };
        colorRec = new Rectangle[colors.Length];

        // Calculate rectangle for each color
        for (int i = 0, x = 0, y = 0; i < colors.Length; i++)
        {
            colorRec[i].X = 2 + MarginSize + x;
            colorRec[i].Y = 22 + 256 + MarginSize + y;
            colorRec[i].Width = ColorSize * 2;
            colorRec[i].Height = ColorSize;

            if (i == (colors.Length / 2 - 1))
            {
                x = 0;
                y += ColorSize + MarginSize;
            }
            else
            {
                x += (ColorSize * 2 + MarginSize);
            }
        }

        activePattern = 0;
        activeCol = 0;
        scale = 1.0f;
        rotation = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        screenWidth = GetScreenWidth();
        screenHeight = GetScreenHeight();

        // Handle mouse
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            var mouse = GetMousePosition();

            // Check which pattern was clicked and set it as the active pattern
            for (var i = 0; i < recPattern.Length; i++)
            {
                Rectangle rec = new(
                    2 + MarginSize + recPattern[i].X,
                    40 + MarginSize + recPattern[i].Y,
                    recPattern[i].Width,
                    recPattern[i].Height
                );
                if (CheckCollisionPointRec(mouse, rec))
                {
                    activePattern = i;
                    break;
                }
            }

            // Check to see which color was clicked and set it as the active color
            for (var i = 0; i < colors.Length; ++i)
            {
                if (CheckCollisionPointRec(mouse, colorRec[i]))
                {
                    activeCol = i;
                    break;
                }
            }
        }

        // Handle keys: change scale
        if (IsKeyPressed(KeyboardKey.Up))
        {
            scale += 0.25f;
        }
        if (IsKeyPressed(KeyboardKey.Down))
        {
            scale -= 0.25f;
        }
        if (scale > 10.0f)
        {
            scale = 10.0f;
        }
        else if (scale <= 0.0f)
        {
            scale = 0.25f;
        }

        // Handle keys: change rotation
        if (IsKeyPressed(KeyboardKey.Left))
        {
            rotation -= 25.0f;
        }
        if (IsKeyPressed(KeyboardKey.Right))
        {
            rotation += 25.0f;
        }

        // Handle keys: reset
        if (IsKeyPressed(KeyboardKey.Space))
        {
            rotation = 0.0f;
            scale = 1.0f;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw the tiled area
        var source = recPattern[activePattern];
        Rectangle dest = new(
            OptWidth + MarginSize,
            MarginSize,
            screenWidth - OptWidth - 2 * MarginSize,
            screenHeight - 2 * MarginSize
        );
        DrawTextureTiled(texPattern, source, dest, Vector2.Zero, rotation, scale, colors[activeCol]);

        // Draw options
        var color = ColorAlpha(Color.LightGray, 0.5f);
        DrawRectangle(MarginSize, MarginSize, OptWidth - MarginSize, screenHeight - 2 * MarginSize, color);

        DrawText("Select Pattern", 2 + MarginSize, 30 + MarginSize, 10, Color.Black);
        DrawTexture(texPattern, 2 + MarginSize, 40 + MarginSize, Color.Black);
        DrawRectangle(
            2 + MarginSize + (int)recPattern[activePattern].X,
            40 + MarginSize + (int)recPattern[activePattern].Y,
            (int)recPattern[activePattern].Width,
            (int)recPattern[activePattern].Height,
            ColorAlpha(Color.DarkBlue, 0.3f)
        );

        DrawText("Select Color", 2 + MarginSize, 10 + 256 + MarginSize, 10, Color.Black);
        for (var i = 0; i < colors.Length; i++)
        {
            DrawRectangleRec(colorRec[i], colors[i]);
            if (activeCol == i)
            {
                DrawRectangleLinesEx(colorRec[i], 3, ColorAlpha(Color.White, 0.5f));
            }
        }

        DrawText("Scale (UP/DOWN to change)", 2 + MarginSize, 80 + 256 + MarginSize, 10, Color.Black);
        DrawText($"{scale:F2}x", 2 + MarginSize, 92 + 256 + MarginSize, 20, Color.Black);

        DrawText("Rotation (LEFT/RIGHT to change)", 2 + MarginSize, 122 + 256 + MarginSize, 10, Color.Black);
        DrawText($"{rotation:F0} degrees", 2 + MarginSize, 134 + 256 + MarginSize, 20, Color.Black);

        DrawText("Press [SPACE] to reset", 2 + MarginSize, 164 + 256 + MarginSize, 10, Color.DarkBlue);

        // Draw FPS
        DrawText($"{GetFPS()} FPS", 2 + MarginSize, 2 + MarginSize, 20, Color.Black);
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texPattern);  // Unload texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.ResizableWindow); // Make the window resizable
        InitWindow(800, 450, "raylib [textures] example - tiled drawing");

        SetTargetFPS(60);
        //---------------------------------------------------------------------------------------

        var game = new DrawTiled();
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

    // Draw part of a texture (defined by a rectangle) with rotation and scale tiled into dest.
    private static void DrawTextureTiled(
        Texture2D texture,
        Rectangle source,
        Rectangle dest,
        Vector2 origin,
        float rotation,
        float scale,
        Color tint
    )
    {
        if ((texture.Id <= 0) || (scale <= 0.0f))
        {
            // Wanna see a infinite loop?!...just delete this line!
            return;
        }

        if ((source.Width == 0) || (source.Height == 0))
        {
            return;
        }

        int tileWidth = (int)(source.Width * scale), tileHeight = (int)(source.Height * scale);
        if ((dest.Width < tileWidth) && (dest.Height < tileHeight))
        {
            // Can fit only one tile
            DrawTexturePro(
                texture,
                new Rectangle(
                    source.X,
                    source.Y,
                    ((float)dest.Width / tileWidth) * source.Width,
                    ((float)dest.Height / tileHeight) * source.Height
                ),
                new Rectangle(dest.X, dest.Y, dest.Width, dest.Height), origin, rotation, tint
            );
        }
        else if (dest.Width <= tileWidth)
        {
            // Tiled vertically (one column)
            var dy = 0;
            for (; dy + tileHeight < dest.Height; dy += tileHeight)
            {
                DrawTexturePro(
                    texture,
                    new Rectangle(
                        source.X,
                        source.Y,
                        ((float)dest.Width / tileWidth) * source.Width,
                        source.Height
                    ),
                    new Rectangle(dest.X, dest.Y + dy, dest.Width, (float)tileHeight),
                    origin,
                    rotation,
                    tint
                );
            }

            // Fit last tile
            if (dy < dest.Height)
            {
                DrawTexturePro(
                    texture,
                    new Rectangle(
                        source.X,
                        source.Y,
                        ((float)dest.Width / tileWidth) * source.Width,
                        ((float)(dest.Height - dy) / tileHeight) * source.Height
                    ),
                    new Rectangle(dest.X, dest.Y + dy, dest.Width, dest.Height - dy),
                    origin,
                    rotation,
                    tint
                );
            }
        }
        else if (dest.Height <= tileHeight)
        {
            // Tiled horizontally (one row)
            var dx = 0;
            for (; dx + tileWidth < dest.Width; dx += tileWidth)
            {
                DrawTexturePro(
                    texture,
                    new Rectangle(
                        source.X,
                        source.Y,
                        source.Width,
                        ((float)dest.Height / tileHeight) * source.Height
                    ),
                    new Rectangle(dest.X + dx, dest.Y, (float)tileWidth, dest.Height),
                    origin,
                    rotation,
                    tint
                );
            }

            // Fit last tile
            if (dx < dest.Width)
            {
                DrawTexturePro(
                    texture,
                    new Rectangle(
                        source.X,
                        source.Y,
                        ((float)(dest.Width - dx) / tileWidth) * source.Width,
                        ((float)dest.Height / tileHeight) * source.Height
                    ),
                    new Rectangle(
                        dest.X + dx,
                        dest.Y,
                        dest.Width - dx,
                        dest.Height
                    ),
                    origin,
                    rotation,
                    tint
                );
            }
        }
        else
        {
            // Tiled both horizontally and vertically (rows and columns)
            var dx = 0;
            for (; dx + tileWidth < dest.Width; dx += tileWidth)
            {
                var dy = 0;
                for (; dy + tileHeight < dest.Height; dy += tileHeight)
                {
                    DrawTexturePro(
                        texture,
                        source,
                        new Rectangle(
                            dest.X + dx,
                            dest.Y + dy,
                            (float)tileWidth,
                            (float)tileHeight
                        ),
                        origin,
                        rotation,
                        tint
                    );
                }

                if (dy < dest.Height)
                {
                    DrawTexturePro(
                        texture,
                        new Rectangle(
                            source.X,
                            source.Y,
                            source.Width,
                            ((float)(dest.Height - dy) / tileHeight) * source.Height
                        ),
                        new Rectangle(
                            dest.X + dx,
                            dest.Y + dy,
                            (float)tileWidth, dest.Height - dy
                        ),
                        origin,
                        rotation,
                        tint
                    );
                }
            }

            // Fit last column of tiles
            if (dx < dest.Width)
            {
                var dy = 0;
                for (; dy + tileHeight < dest.Height; dy += tileHeight)
                {
                    DrawTexturePro(
                        texture,
                        new Rectangle(
                            source.X,
                            source.Y,
                            ((float)(dest.Width - dx) / tileWidth) * source.Width,
                            source.Height
                        ),
                        new Rectangle(
                            dest.X + dx,
                            dest.Y + dy,
                            dest.Width - dx,
                            (float)tileHeight
                        ),
                        origin,
                        rotation,
                        tint
                    );
                }

                // Draw final tile in the bottom right corner
                if (dy < dest.Height)
                {
                    DrawTexturePro(
                        texture,
                        new Rectangle(
                            source.X,
                            source.Y,
                            ((float)(dest.Width - dx) / tileWidth) * source.Width,
                            ((float)(dest.Height - dy) / tileHeight) * source.Height
                        ),
                        new Rectangle(
                            dest.X + dx,
                            dest.Y + dy,
                            dest.Width - dx,
                            dest.Height - dy
                        ),
                        origin,
                        rotation,
                        tint
                    );
                }
            }
        }
    }
}
