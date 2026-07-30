/*******************************************************************************************
*
*   raylib [core] example - window letterbox
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 4.0
*
*   Example contributed by Anata (@anatagawa) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Anata (@anatagawa) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class WindowLetterbox : IExample
{
    private const int windowWidth = 800;
    private const int windowHeight = 450;

    private const int gamescreenWidth = 640;
    private const int gamescreenHeight = 480;

    public string Name => "Core / Window Letterbox";

    public string Title => "raylib [core] example - window letterbox";

    public ConfigFlags ConfigFlags => ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint;

    private RenderTexture2D target;
    private Color[] colors;

    public void Init()
    {
        // Render texture initialization, used to hold the rendering result so we can easily resize it
        target = LoadRenderTexture(gamescreenWidth, gamescreenHeight);
        SetTextureFilter(target.Texture, TextureFilter.Bilinear);  // Texture scale filter to use

        colors = new Color[10];
        for (var i = 0; i < 10; i++)
        {
            colors[i] = new Color(GetRandomValue(100, 250), GetRandomValue(50, 150), GetRandomValue(10, 100), 255);
        }
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Compute required framebuffer scaling
        var scale = MathF.Min(
            (float)GetScreenWidth() / gamescreenWidth,
            (float)GetScreenHeight() / gamescreenHeight
        );

        if (IsKeyPressed(KeyboardKey.Space))
        {
            // Recalculate random colors for the bars
            for (var i = 0; i < 10; i++)
            {
                colors[i] = new Color(
                    GetRandomValue(100, 250),
                    GetRandomValue(50, 150),
                    GetRandomValue(10, 100),
                    255
                );
            }
        }

        // Update virtual mouse (clamped mouse value behind game screen)
        var mouse = GetMousePosition();
        var virtualMouse = Vector2.Zero;
        virtualMouse.X = (mouse.X - (GetScreenWidth() - (gamescreenWidth * scale)) * 0.5f) / scale;
        virtualMouse.Y = (mouse.Y - (GetScreenHeight() - (gamescreenHeight * scale)) * 0.5f) / scale;

        Vector2 max = new((float)gamescreenWidth, (float)gamescreenHeight);
        virtualMouse = Vector2.Clamp(virtualMouse, Vector2.Zero, max);

        // Apply the same transformation as the virtual mouse to the real mouse (i.e. to work with raygui)
        //SetMouseOffset(-(GetScreenWidth() - (gamescreenWidth*scale))*0.5f, -(GetScreenHeight() - (gamescreenHeight*scale))*0.5f);
        //SetMouseScale(1/scale, 1/scale);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Draw everything in the render texture, note this will not be rendered on screen, yet
        BeginTextureMode(target);
        ClearBackground(Color.RayWhite);  // Clear render texture background color

        for (var i = 0; i < 10; i++)
        {
            DrawRectangle(0, (gamescreenHeight / 10) * i, gamescreenWidth, gamescreenHeight / 10, colors[i]);
        }

        DrawText(
            "If executed inside a window,\nyou can resize the window,\nand see the screen scaling!",
            10,
            25,
            20,
            Color.White
        );

        DrawText($"Default Mouse: [{(int)mouse.X} , {(int)mouse.Y}]", 350, 25, 20, Color.Green);
        DrawText($"Virtual Mouse: [{(int)virtualMouse.X} , {(int)virtualMouse.Y}]", 350, 55, 20, Color.Yellow);

        EndTextureMode();

        BeginDrawing();
        ClearBackground(Color.Black);     // Clear screen background

        // Draw render texture to screen, properly scaled
        Rectangle sourceRec = new(
            0.0f,
            0.0f,
            (float)target.Texture.Width,
            (float)-target.Texture.Height
        );
        Rectangle destRec = new(
            (GetScreenWidth() - ((float)gamescreenWidth * scale)) * 0.5f,
            (GetScreenHeight() - ((float)gamescreenHeight * scale)) * 0.5f,
            (float)gamescreenWidth * scale,
            (float)gamescreenHeight * scale
        );
        DrawTexturePro(target.Texture, sourceRec, destRec, new Vector2(0, 0), 0.0f, Color.White);

        EndDrawing();
        //--------------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(target);        // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        // Enable config flags for resizable window and vertical synchro
        SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
        InitWindow(windowWidth, windowHeight, "raylib [core] example - window letterbox");
        SetWindowMinSize(320, 240);

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new WindowLetterbox();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                      // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
