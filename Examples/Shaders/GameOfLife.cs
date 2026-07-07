/*******************************************************************************************
*
*   raylib [shaders] example - game of life
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Jordi Santonja (@JordSant) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jordi Santonja (@JordSant)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public partial class GameOfLife : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Game of Life";

    public string Title => "raylib [shaders] example - game of life";

    // Interaction mode
    private enum InteractionMode
    {
        Run = 0,
        Pause,
        Draw,
    }

    // Struct to store example preset patterns
    private struct PresetPattern
    {
        public string Name;
        public Vector2 Position;

        public PresetPattern(string name, Vector2 position)
        {
            Name = name;
            Position = position;
        }
    }

    private const int menuWidth = 100;
    private const int windowWidth = screenWidth - menuWidth;
    private const int windowHeight = screenHeight;

    private const int worldWidth = 2048;
    private const int worldHeight = 2048;

    private const int randomTiles = 8;      // Random preset: divide the world to compute random points in each tile

    private static readonly PresetPattern[] presetPatterns =
    {
        new("Glider", new Vector2(0.5f, 0.5f)), new("R-pentomino", new Vector2(0.5f, 0.5f)), new("Acorn", new Vector2(0.5f, 0.5f)),
        new("Spaceships", new Vector2(0.1f, 0.5f)), new("Still lifes", new Vector2(0.5f, 0.5f)), new("Oscillators", new Vector2(0.5f, 0.5f)),
        new("Puffer train", new Vector2(0.1f, 0.5f)), new("Glider Gun", new Vector2(0.2f, 0.2f)), new("Breeder", new Vector2(0.1f, 0.5f)),
        new("Random", new Vector2(0.5f, 0.5f))
    };

    private static readonly int numberOfPresets = presetPatterns.Length;

    private Rectangle worldRectSource;
    private Rectangle worldRectDest;
    private Rectangle textureOnScreen;

    private int zoom;
    private float offsetX;
    private float offsetY;
    private int framesPerStep;
    private int frame;

    private int preset;
    private InteractionMode mode;
    private bool buttonZoomIn;
    private bool buttonZomOut;
    private bool buttonFaster;
    private bool buttonSlower;

    private Shader shdrGameOfLife;
    private int resolutionLoc;

    private RenderTexture2D world1;
    private RenderTexture2D world2;
    private RenderTexture2D currentWorld;
    private RenderTexture2D previousWorld;

    // Image to be used in DRAW mode, to be changed with mouse input
    private Image imageToDraw;
    private bool imageToDrawValid;

    // Static locals in the original loop, promoted to fields
    private Vector2 previousMousePosition;
    private int firstColor;

    private void FreeImageToDraw()
    {
        if (imageToDrawValid)
        {
            UnloadImage(imageToDraw);
            imageToDrawValid = false;
        }
    }

    public unsafe void Init()
    {
        worldRectSource = new Rectangle(0, 0, worldWidth, -worldHeight);
        worldRectDest = new Rectangle(0, 0, worldWidth, worldHeight);
        textureOnScreen = new Rectangle(0, 0, windowWidth, windowHeight);

        zoom = 1;
        offsetX = (worldWidth - windowWidth) / 2.0f;    // Centered on window
        offsetY = (worldHeight - windowHeight) / 2.0f;  // Centered on window
        framesPerStep = 1;
        frame = 0;

        preset = -1;                    // No button pressed for preset
        mode = InteractionMode.Run;     // Starting mode: running
        buttonZoomIn = false;           // Button states: false not pressed
        buttonZomOut = false;
        buttonFaster = false;
        buttonSlower = false;

        previousMousePosition = new Vector2(0.0f, 0.0f);
        firstColor = -1;
        imageToDrawValid = false;

        // Load shader
        shdrGameOfLife = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/game_of_life.fs");

        // Set shader uniform size of the world
        resolutionLoc = GetShaderLocation(shdrGameOfLife, "resolution");
        var resolution = new[] { (float)worldWidth, (float)worldHeight };
        Raylib.SetShaderValue(shdrGameOfLife, resolutionLoc, resolution, ShaderUniformDataType.Vec2);

        // Define two textures: the current world and the previous world
        world1 = LoadRenderTexture(worldWidth, worldHeight);
        world2 = LoadRenderTexture(worldWidth, worldHeight);
        BeginTextureMode(world2);
        ClearBackground(Color.RayWhite);
        EndTextureMode();

        var startPattern = LoadImage("resources/game_of_life/r_pentomino.png");
        UpdateTextureRec(
            world2.Texture,
            new Rectangle(worldWidth / 2.0f, worldHeight / 2.0f, startPattern.Width, startPattern.Height),
            startPattern.Data
        );
        UnloadImage(startPattern);

        // References to the two textures, to be swapped
        currentWorld = world2;
        previousWorld = world1;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        frame++;

        // Change zoom: both by buttons or by mouse wheel
        var mouseWheelMove = GetMouseWheelMove();
        if (buttonZoomIn || (buttonZomOut && (zoom > 1)) || (mouseWheelMove != 0.0f))
        {
            FreeImageToDraw();  // Zoom change: free the image to draw to be recreated again

            var centerX = offsetX + (windowWidth / 2.0f) / zoom;
            var centerY = offsetY + (windowHeight / 2.0f) / zoom;
            if (buttonZoomIn || (mouseWheelMove > 0.0f)) zoom *= 2;
            if ((buttonZomOut || (mouseWheelMove < 0.0f)) && (zoom > 1)) zoom /= 2;
            offsetX = centerX - (windowWidth / 2.0f) / zoom;
            offsetY = centerY - (windowHeight / 2.0f) / zoom;
        }

        // Change speed: number of frames per step
        if (buttonFaster && framesPerStep > 1) framesPerStep--;
        if (buttonSlower) framesPerStep++;

        // Mouse management
        if ((mode == InteractionMode.Run) || (mode == InteractionMode.Pause))
        {
            FreeImageToDraw();  // Free the image to draw: no longer needed in these modes

            // Pan with mouse left button
            var mousePosition = GetMousePosition();
            if (IsMouseButtonDown(MouseButton.Left) && (mousePosition.X < windowWidth))
            {
                offsetX -= (mousePosition.X - previousMousePosition.X) / zoom;
                offsetY -= (mousePosition.Y - previousMousePosition.Y) / zoom;
            }
            previousMousePosition = mousePosition;
        }
        else // MODE_DRAW
        {
            var offsetDecimalX = offsetX - MathF.Floor(offsetX);
            var offsetDecimalY = offsetY - MathF.Floor(offsetY);
            var sizeInWorldX = (int)(MathF.Ceiling((windowWidth + offsetDecimalX * zoom) / zoom));
            var sizeInWorldY = (int)(MathF.Ceiling((windowHeight + offsetDecimalY * zoom) / zoom));
            if (offsetX + sizeInWorldX >= worldWidth) sizeInWorldX = worldWidth - (int)MathF.Floor(offsetX);
            if (offsetY + sizeInWorldY >= worldHeight) sizeInWorldY = worldHeight - (int)MathF.Floor(offsetY);

            // Create image to draw if not created yet
            if (!imageToDrawValid)
            {
                var worldOnScreen = LoadRenderTexture(sizeInWorldX, sizeInWorldY);
                BeginTextureMode(worldOnScreen);
                DrawTexturePro(
                    currentWorld.Texture,
                    new Rectangle(MathF.Floor(offsetX), MathF.Floor(offsetY), sizeInWorldX, -sizeInWorldY),
                    new Rectangle(0, 0, sizeInWorldX, sizeInWorldY),
                    new Vector2(0, 0), 0.0f, Color.White
                );
                EndTextureMode();
                imageToDraw = LoadImageFromTexture(worldOnScreen.Texture);
                imageToDrawValid = true;

                UnloadRenderTexture(worldOnScreen);
            }

            var mousePosition = GetMousePosition();
            if (IsMouseButtonDown(MouseButton.Left) && (mousePosition.X < windowWidth))
            {
                var mouseX = (int)(mousePosition.X + offsetDecimalX * zoom) / zoom;
                var mouseY = (int)(mousePosition.Y + offsetDecimalY * zoom) / zoom;
                if (mouseX >= sizeInWorldX) mouseX = sizeInWorldX - 1;
                if (mouseY >= sizeInWorldY) mouseY = sizeInWorldY - 1;
                if (firstColor == -1) firstColor = (GetImageColor(imageToDraw, mouseX, mouseY).R < 5) ? 0 : 1;
                var prevColor = (GetImageColor(imageToDraw, mouseX, mouseY).R < 5) ? 0 : 1;

                ImageDrawPixel(ref imageToDraw, mouseX, mouseY, (firstColor != 0) ? Color.Black : Color.RayWhite);

                if (prevColor != firstColor)
                {
                    UpdateTextureRec(
                        currentWorld.Texture,
                        new Rectangle(MathF.Floor(offsetX), MathF.Floor(offsetY), sizeInWorldX, sizeInWorldY),
                        imageToDraw.Data
                    );
                }
            }
            else firstColor = -1;
        }

        // Load selected preset
        if (preset >= 0)
        {
            Image pattern;
            if (preset < numberOfPresets - 1)   // Preset with pattern image to load
            {
                pattern = preset switch
                {
                    0 => LoadImage("resources/game_of_life/glider.png"),
                    1 => LoadImage("resources/game_of_life/r_pentomino.png"),
                    2 => LoadImage("resources/game_of_life/acorn.png"),
                    3 => LoadImage("resources/game_of_life/spaceships.png"),
                    4 => LoadImage("resources/game_of_life/still_lifes.png"),
                    5 => LoadImage("resources/game_of_life/oscillators.png"),
                    6 => LoadImage("resources/game_of_life/puffer_train.png"),
                    7 => LoadImage("resources/game_of_life/glider_gun.png"),
                    8 => LoadImage("resources/game_of_life/breeder.png"),
                    _ => default,
                };
                BeginTextureMode(currentWorld);
                ClearBackground(Color.RayWhite);
                EndTextureMode();

                UpdateTextureRec(
                    currentWorld.Texture,
                    new Rectangle(
                        worldWidth * presetPatterns[preset].Position.X - pattern.Width / 2.0f,
                        worldHeight * presetPatterns[preset].Position.Y - pattern.Height / 2.0f,
                        pattern.Width, pattern.Height
                    ),
                    pattern.Data
                );
            }
            else    // Last preset: Random values
            {
                pattern = GenImageColor(worldWidth / randomTiles, worldHeight / randomTiles, Color.RayWhite);
                for (var i = 0; i < randomTiles; i++)
                {
                    for (var j = 0; j < randomTiles; j++)
                    {
                        ImageClearBackground(ref pattern, Color.RayWhite);
                        for (var x = 0; x < pattern.Width; x++)
                        {
                            for (var y = 0; y < pattern.Height; y++)
                            {
                                if (GetRandomValue(0, 100) < 15) ImageDrawPixel(ref pattern, x, y, Color.Black);
                            }
                        }
                        UpdateTextureRec(
                            currentWorld.Texture,
                            new Rectangle(pattern.Width * i, pattern.Height * j, pattern.Width, pattern.Height),
                            pattern.Data
                        );
                    }
                }
            }

            UnloadImage(pattern);

            mode = InteractionMode.Pause;
            offsetX = worldWidth * presetPatterns[preset].Position.X - (float)windowWidth / zoom / 2.0f;
            offsetY = worldHeight * presetPatterns[preset].Position.Y - (float)windowHeight / zoom / 2.0f;
        }

        // Check window draw inside world limits
        if (offsetX < 0) offsetX = 0;
        if (offsetY < 0) offsetY = 0;
        if (offsetX > worldWidth - (float)windowWidth / zoom) offsetX = worldWidth - (float)windowWidth / zoom;
        if (offsetY > worldHeight - (float)windowHeight / zoom) offsetY = worldHeight - (float)windowHeight / zoom;

        // Rectangles for drawing texture portion to screen
        var textureSourceToScreen = new Rectangle(offsetX, offsetY, (float)windowWidth / zoom, (float)windowHeight / zoom);
        //----------------------------------------------------------------------------------

        // Draw to texture
        //----------------------------------------------------------------------------------
        if ((mode == InteractionMode.Run) && ((frame % framesPerStep) == 0))
        {
            // Swap worlds
            var tempWorld = currentWorld;
            currentWorld = previousWorld;
            previousWorld = tempWorld;

            // Draw to texture
            BeginTextureMode(currentWorld);
            BeginShaderMode(shdrGameOfLife);
            DrawTexturePro(previousWorld.Texture, worldRectSource, worldRectDest, new Vector2(0, 0), 0.0f, Color.RayWhite);
            EndShaderMode();
            EndTextureMode();
        }
        //----------------------------------------------------------------------------------

        // Draw to screen
        //----------------------------------------------------------------------------------
        BeginDrawing();

        DrawTexturePro(currentWorld.Texture, textureSourceToScreen, textureOnScreen, new Vector2(0, 0), 0.0f, Color.White);

        DrawLine(windowWidth, 0, windowWidth, screenHeight, new Color(218, 218, 218, 255));
        DrawRectangle(windowWidth, 0, screenWidth - windowWidth, screenHeight, new Color(232, 232, 232, 255));

        DrawText("Conway's", 704, 4, 20, Color.DarkBlue);
        DrawText(" game of", 704, 19, 20, Color.DarkBlue);
        DrawText("  life", 708, 34, 20, Color.DarkBlue);
        DrawText("in raylib", 757, 42, 6, Color.Black);

        DrawText("Presets", 710, 58, 8, Color.Gray);
        preset = -1;

        // Draw GUI controls
        //------------------------------------------------------------------------------
        // NOTE: raygui is not bound in raylib-cs; controls are kept for reference. The
        // simulation still runs automatically and mouse wheel zoom / left-drag pan work.
        /*for (int i = 0; i < numberOfPresets; i++)
            if (GuiButton(new Rectangle( 710.0f, 70.0f + 18*i, 80.0f, 16.0f ), presetPatterns[i].Name)) preset = i;

        GuiToggleGroup(new Rectangle( 710, 258, 80, 16 ), "Run\nPause\nDraw", ref mode);*/

        DrawText($"Zoom: {zoom}x", 710, 316, 8, Color.Gray);
        /*buttonZoomIn = GuiButton(new Rectangle( 710, 328, 80, 16 ), "Zoom in");
        buttonZomOut = GuiButton(new Rectangle( 710, 346, 80, 16 ), "Zoom out");*/

        DrawText($"Speed: {framesPerStep} frame{((framesPerStep > 1) ? "s" : "")}", 710, 370, 8, Color.Gray);
        /*buttonFaster = GuiButton(new Rectangle( 710, 382, 80, 16 ), "Faster");
        buttonSlower = GuiButton(new Rectangle( 710, 400, 80, 16 ), "Slower");*/
        //------------------------------------------------------------------------------

        DrawFPS(712, 426);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shdrGameOfLife);
        UnloadRenderTexture(world1);
        UnloadRenderTexture(world2);

        FreeImageToDraw();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - game of life");

        SetTargetFPS(60);               // Set at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new GameOfLife();
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
