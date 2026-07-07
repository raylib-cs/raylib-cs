/*******************************************************************************************
*
*   raylib [shaders] example - texture outline
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   Example originally created with raylib 4.0, last time updated with raylib 4.0
*
*   Example contributed by Serenity Skiff (@GoldenThumbs) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Serenity Skiff (@GoldenThumbs) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class TextureOutline : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Texture Outline";

    public string Title => "raylib [shaders] example - texture outline";

    private Texture2D texture;
    private Shader shdrOutline;
    private float outlineSize;
    private int outlineSizeLoc;

    public void Init()
    {
        texture = LoadTexture("resources/fudesumi.png");
        shdrOutline = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/outline.fs");

        outlineSize = 2.0f;

        // Normalized RED color
        var outlineColor = new[] { 1.0f, 0.0f, 0.0f, 1.0f };
        float[] textureSize = { (float)texture.Width, (float)texture.Height };

        // Get shader locations
        outlineSizeLoc = GetShaderLocation(shdrOutline, "outlineSize");
        var outlineColorLoc = GetShaderLocation(shdrOutline, "outlineColor");
        var textureSizeLoc = GetShaderLocation(shdrOutline, "textureSize");

        // Set shader values (they can be changed later)
        Raylib.SetShaderValue(
            shdrOutline,
            outlineSizeLoc,
            outlineSize,
            ShaderUniformDataType.Float
        );
        Raylib.SetShaderValue(
            shdrOutline,
            outlineColorLoc,
            outlineColor,
            ShaderUniformDataType.Vec4
        );
        Raylib.SetShaderValue(
            shdrOutline,
            textureSizeLoc,
            textureSize,
            ShaderUniformDataType.Vec2
        );
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        outlineSize += GetMouseWheelMove();
        if (outlineSize < 1.0f)
        {
            outlineSize = 1.0f;
        }

        Raylib.SetShaderValue(
            shdrOutline,
            outlineSizeLoc,
            outlineSize,
            ShaderUniformDataType.Float
        );
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginShaderMode(shdrOutline);
        DrawTexture(texture, GetScreenWidth() / 2 - texture.Width / 2, -30, Color.White);
        EndShaderMode();

        DrawText("Shader-based\ntexture\noutline", 10, 10, 20, Color.Gray);
        DrawText("Scroll mouse wheel to\nchange outline size", 10, 72, 20, Color.Gray);
        DrawText($"Outline size: {(int)outlineSize} px", 10, 120, 20, Color.Maroon);

        DrawFPS(710, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);
        UnloadShader(shdrOutline);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - texture outline");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new TextureOutline();
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
