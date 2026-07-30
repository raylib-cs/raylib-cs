/*******************************************************************************************
*
*   raylib [shaders] example - eratosthenes sieve
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: Sieve of Eratosthenes, the earliest known (ancient Greek) prime number sieve
*
*       "Sift the twos and sift the threes,
*        The Sieve of Eratosthenes.
*        When the multiples sublime,
*        the numbers that are left are prime."
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example originally created with raylib 2.5, last time updated with raylib 4.0
*
*   Example contributed by ProfJski (@ProfJski) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 ProfJski (@ProfJski) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shaders;

public class Eratosthenes : IExample
{
#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shaders / Eratosthenes";

    public string Title => "raylib [shaders] example - eratosthenes sieve";

    private RenderTexture2D target;
    private Shader shader;

    public void Init()
    {
        target = LoadRenderTexture(screenWidth, screenHeight);

        // Load Eratosthenes shader
        // NOTE: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/eratosthenes.fs");
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Nothing to do here, everything is happening in the shader
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginTextureMode(target);       // Enable drawing to texture
        ClearBackground(Color.Black);   // Clear the render texture

        // Draw a rectangle in shader mode to be used as shader canvas
        // NOTE: Rectangle uses font white character texture coordinates,
        // so shader can not be applied here directly because input vertexTexCoord
        // do not represent full screen coordinates (space where want to apply shader)
        DrawRectangle(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Black);
        EndTextureMode();               // End drawing to texture (now we have a blank texture available for the shader)

        BeginDrawing();
        ClearBackground(Color.RayWhite);  // Clear screen background

        BeginShaderMode(shader);
        // NOTE: Render texture must be y-flipped due to default OpenGL coordinates (left-bottom)
        DrawTextureRec(
            target.Texture,
            new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
            new Vector2(0.0f, 0.0f),
            Color.White
        );
        EndShaderMode();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);
        UnloadRenderTexture(target);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - eratosthenes sieve");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new Eratosthenes();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
