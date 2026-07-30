/*******************************************************************************************
*
*   raylib [shaders] example - texture waves
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3), to test this example
*         on OpenGL ES 2.0 platforms (Android, Raspberry Pi, HTML5), use #version 100 shaders
*         raylib comes with shaders ready for both versions, check raylib/shaders install folder
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.7
*
*   Example contributed by Anata (@anatagawa) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Anata (@anatagawa) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shaders;

public class TextureWaves : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Texture Waves";

    public string Title => "raylib [shaders] example - texture waves";

    private Texture2D texture;
    private Shader shader;
    private int secondsLoc;
    private float seconds;

    public void Init()
    {
        // Load texture texture to apply shaders
        texture = LoadTexture("resources/space.png");

        // Load shader and setup location points and values
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/wave.fs");

        secondsLoc = GetShaderLocation(shader, "seconds");
        var freqXLoc = GetShaderLocation(shader, "freqX");
        var freqYLoc = GetShaderLocation(shader, "freqY");
        var ampXLoc = GetShaderLocation(shader, "ampX");
        var ampYLoc = GetShaderLocation(shader, "ampY");
        var speedXLoc = GetShaderLocation(shader, "speedX");
        var speedYLoc = GetShaderLocation(shader, "speedY");

        // Shader uniform values that can be updated at any time
        var freqX = 25.0f;
        var freqY = 25.0f;
        var ampX = 5.0f;
        var ampY = 5.0f;
        var speedX = 8.0f;
        var speedY = 8.0f;

        float[] screenSize = { (float)GetScreenWidth(), (float)GetScreenHeight() };
        Raylib.SetShaderValue(
            shader,
            GetShaderLocation(shader, "size"),
            screenSize,
            ShaderUniformDataType.Vec2
        );
        Raylib.SetShaderValue(shader, freqXLoc, freqX, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, freqYLoc, freqY, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, ampXLoc, ampX, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, ampYLoc, ampY, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, speedXLoc, speedX, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, speedYLoc, speedY, ShaderUniformDataType.Float);

        seconds = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        seconds += GetFrameTime();

        Raylib.SetShaderValue(shader, secondsLoc, seconds, ShaderUniformDataType.Float);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginShaderMode(shader);

        DrawTexture(texture, 0, 0, Color.White);
        DrawTexture(texture, texture.Width, 0, Color.White);

        EndShaderMode();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);         // Unload shader
        UnloadTexture(texture);       // Unload texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - texture waves");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new TextureWaves();
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
}
