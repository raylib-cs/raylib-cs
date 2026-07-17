/*******************************************************************************************
*
*   raylib [shaders] example - texture rendering
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.0, last time updated with raylib 3.7
*
*   Example contributed by Michał Ciesielski (@ciessielski) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Michał Ciesielski (@ciessielski) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class TextureDrawing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Texture Drawing";

    public string Title => "raylib [shaders] example - texture rendering";

    private Texture2D texture;
    private Shader shader;
    private float time;
    private int timeLoc;

    public void Init()
    {
        var imBlank = GenImageColor(1024, 1024, Color.Blank);
        texture = LoadTextureFromImage(imBlank);  // Load blank texture to fill on shader
        UnloadImage(imBlank);

        // NOTE: Using GLSL 330 shader version, on OpenGL ES 2.0 use GLSL 100 shader version
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/cubes_panning.fs");

        time = 0.0f;
        timeLoc = GetShaderLocation(shader, "uTime");
        Raylib.SetShaderValue(shader, timeLoc, time, ShaderUniformDataType.Float);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        time = (float)GetTime();
        Raylib.SetShaderValue(shader, timeLoc, time, ShaderUniformDataType.Float);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginShaderMode(shader);    // Enable our custom shader for next shapes/textures drawings
        DrawTexture(texture, 0, 0, Color.White);  // Drawing BLANK texture, all rendering magic happens on shader
        EndShaderMode();            // Disable our custom shader, return to default shader

        DrawText("BACKGROUND is PAINTED and ANIMATED on SHADER!", 10, 10, 20, Color.Maroon);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);
        UnloadTexture(texture);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - texture rendering");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new TextureDrawing();
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
