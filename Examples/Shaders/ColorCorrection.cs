/*******************************************************************************************
*
*   raylib [shaders] example - color correction
*
*   Example complexity rating: [★★☆☆] 2/4
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

namespace Examples.Shaders;

public partial class ColorCorrection : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int MaxTextures = 4;

    public string Name => "Shaders / Color Correction";

    public string Title => "raylib [shaders] example - color correction";

    private Texture2D[] texture;
    private Shader shdrColorCorrection;
    private int imageIndex;
    private int resetButtonClicked;
    private float contrast;
    private float saturation;
    private float brightness;
    private int contrastLoc;
    private int saturationLoc;
    private int brightnessLoc;

    public void Init()
    {
        texture = new[]
        {
            LoadTexture("resources/parrots.png"),
            LoadTexture("resources/cat.png"),
            LoadTexture("resources/mandrill.png"),
            LoadTexture("resources/fudesumi.png")
        };

        shdrColorCorrection = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/color_correction.fs");

        imageIndex = 0;
        resetButtonClicked = 0;

        contrast = 0.0f;
        saturation = 0.0f;
        brightness = 0.0f;

        // Get shader locations
        contrastLoc = GetShaderLocation(shdrColorCorrection, "contrast");
        saturationLoc = GetShaderLocation(shdrColorCorrection, "saturation");
        brightnessLoc = GetShaderLocation(shdrColorCorrection, "brightness");

        // Set shader values (they can be changed later)
        Raylib.SetShaderValue(shdrColorCorrection, contrastLoc, contrast, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shdrColorCorrection, saturationLoc, saturation, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shdrColorCorrection, brightnessLoc, brightness, ShaderUniformDataType.Float);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Select texture to draw
        if (IsKeyPressed(KeyboardKey.One))
        {
            imageIndex = 0;
        }
        else if (IsKeyPressed(KeyboardKey.Two))
        {
            imageIndex = 1;
        }
        else if (IsKeyPressed(KeyboardKey.Three))
        {
            imageIndex = 2;
        }
        else if (IsKeyPressed(KeyboardKey.Four))
        {
            imageIndex = 3;
        }

        // Reset values to 0
        if (IsKeyPressed(KeyboardKey.R) || resetButtonClicked != 0)
        {
            contrast = 0.0f;
            saturation = 0.0f;
            brightness = 0.0f;
        }

        // Send the values to the shader
        Raylib.SetShaderValue(shdrColorCorrection, contrastLoc, contrast, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shdrColorCorrection, saturationLoc, saturation, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shdrColorCorrection, brightnessLoc, brightness, ShaderUniformDataType.Float);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginShaderMode(shdrColorCorrection);

        DrawTexture(texture[imageIndex], 580 / 2 - texture[imageIndex].Width / 2, GetScreenHeight() / 2 - texture[imageIndex].Height / 2, Color.White);

        EndShaderMode();

        DrawLine(580, 0, 580, GetScreenHeight(), new Color(218, 218, 218, 255));
        DrawRectangle(580, 0, GetScreenWidth(), GetScreenHeight(), new Color(232, 232, 232, 255));

        // Draw UI info text
        DrawText("Color Correction", 585, 40, 20, Color.Gray);

        DrawText("Picture", 602, 75, 10, Color.Gray);
        DrawText("Press [1] - [4] to Change Picture", 600, 230, 8, Color.Gray);
        DrawText("Press [R] to Reset Values", 600, 250, 8, Color.Gray);

        // Draw GUI controls
        //------------------------------------------------------------------------------
        // NOTE: raygui is not bound in raylib-cs; controls are kept for reference. Use
        // keyboard shortcuts ([1]-[4], [R]) to interact with the example.
        /*GuiToggleGroup(new Rectangle( 645, 70, 20, 20 ), "1;2;3;4", ref imageIndex);

        GuiSliderBar(new Rectangle( 645, 100, 120, 20 ), "Contrast", TextFormat("%.0f", contrast), ref contrast, -100.0f, 100.0f);
        GuiSliderBar(new Rectangle( 645, 130, 120, 20 ), "Saturation", TextFormat("%.0f", saturation), ref saturation, -100.0f, 100.0f);
        GuiSliderBar(new Rectangle( 645, 160, 120, 20 ), "Brightness", TextFormat("%.0f", brightness), ref brightness, -100.0f, 100.0f);

        resetButtonClicked = GuiButton(new Rectangle( 645, 190, 40, 20 ), "Reset");*/
        //------------------------------------------------------------------------------

        DrawFPS(710, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        for (var i = 0; i < MaxTextures; i++)
        {
            UnloadTexture(texture[i]);
        }
        UnloadShader(shdrColorCorrection);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - color correction");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ColorCorrection();
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
