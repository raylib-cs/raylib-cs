/*******************************************************************************************
*
*   raylib [shaders] example - shapes textures
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
*   Example originally created with raylib 1.7, last time updated with raylib 3.7
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shaders;

public class ShapesTextures : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Shapes Textures";

    public string Title => "raylib [shaders] example - shapes textures";

    private Texture2D fudesumi;
    private Shader shader;

    public void Init()
    {
        fudesumi = LoadTexture("resources/fudesumi.png");

        // Load shader to be used on some parts drawing
        // NOTE 1: Using GLSL 330 shader version, on OpenGL ES 2.0 use GLSL 100 shader version
        // NOTE 2: Defining null (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/grayscale.fs");
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Start drawing with default shader
        DrawText("USING DEFAULT SHADER", 20, 40, 10, Color.Red);

        DrawCircle(80, 120, 35, Color.DarkBlue);
        DrawCircleGradient(new Vector2(80, 220), 60, Color.Green, Color.SkyBlue);
        DrawCircleLines(80, 340, 80, Color.DarkBlue);


        // Activate our custom shader to be applied on next shapes/textures drawings
        BeginShaderMode(shader);

        DrawText("USING CUSTOM SHADER", 190, 40, 10, Color.Red);

        DrawRectangle(250 - 60, 90, 120, 60, Color.Red);
        DrawRectangleGradientH(250 - 90, 170, 180, 130, Color.Maroon, Color.Gold);
        DrawRectangleLines(250 - 40, 320, 80, 60, Color.Orange);

        // Activate our default shader for next drawings
        EndShaderMode();

        DrawText("USING DEFAULT SHADER", 370, 40, 10, Color.Red);

        DrawTriangle(
            new Vector2(430, 80),
            new Vector2(430 - 60, 150),
            new Vector2(430 + 60, 150), Color.Violet
        );

        DrawTriangleLines(
            new Vector2(430, 160),
            new Vector2(430 - 20, 230),
            new Vector2(430 + 20, 230), Color.DarkBlue
        );

        DrawPoly(new Vector2(430, 320), 6, 80, 0, Color.Brown);

        // Activate our custom shader to be applied on next shapes/textures drawings
        BeginShaderMode(shader);

        // Using custom shader
        DrawTexture(fudesumi, 500, -30, Color.White);

        // Activate our default shader for next drawings
        EndShaderMode();

        DrawText("(c) Fudesumi sprite by Eiden Marsal", 380, screenHeight - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);       // Unload shader
        UnloadTexture(fudesumi);    // Unload texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - shapes textures");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ShapesTextures();
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
