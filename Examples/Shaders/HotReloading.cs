/*******************************************************************************************
*
*   raylib [shaders] example - hot reloading
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 for shaders support and only #version 330
*         is currently supported. OpenGL ES 2.0 platforms are not supported at the moment
*
*   Example originally created with raylib 3.0, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2020-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class HotReloading : IExample
{
#if BROWSER
    const int GlslVersion = 100;   // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shaders / Hot Reloading";

    public string Title => "raylib [shaders] example - hot reloading";

    private string fragShaderFileName;
    private Shader shader;
    private int resolutionLoc;
    private int mouseLoc;
    private int timeLoc;
    private float[] resolution;
    private float totalTime;
#if !BROWSER
    private long fragShaderFileModTime;
    private bool shaderAutoReloading;
#endif

    public void Init()
    {
        fragShaderFileName = $"resources/shaders/glsl{GlslVersion}/reload.fs";
#if !BROWSER
        fragShaderFileModTime = GetFileModTime(fragShaderFileName);
#endif

        // Load raymarching shader
        // NOTE: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, fragShaderFileName);

        // Get shader locations for required uniforms
        resolutionLoc = GetShaderLocation(shader, "resolution");
        mouseLoc = GetShaderLocation(shader, "mouse");
        timeLoc = GetShaderLocation(shader, "time");

        resolution = new[] { (float)screenWidth, (float)screenHeight };
        Raylib.SetShaderValue(shader, resolutionLoc, resolution, ShaderUniformDataType.Vec2);

        totalTime = 0.0f;
#if !BROWSER
        shaderAutoReloading = false;
#endif
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        totalTime += GetFrameTime();
        var mouse = GetMousePosition();
        var mousePos = new[] { mouse.X, mouse.Y };

        // Set shader required uniform values
        Raylib.SetShaderValue(shader, timeLoc, totalTime, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, mouseLoc, mousePos, ShaderUniformDataType.Vec2);

#if !BROWSER
        // Hot shader reloading
        if (shaderAutoReloading || (IsMouseButtonPressed(MouseButton.Left)))
        {
            var currentFragShaderModTime = GetFileModTime(fragShaderFileName);

            // Check if shader file has been modified
            if (currentFragShaderModTime != fragShaderFileModTime)
            {
                // Try reloading updated shader
                var updatedShader = LoadShader(null, fragShaderFileName);

                // It was correctly loaded
                if (updatedShader.Id != Rlgl.GetShaderIdDefault())
                {
                    UnloadShader(shader);
                    shader = updatedShader;

                    // Get shader locations for required uniforms
                    resolutionLoc = GetShaderLocation(shader, "resolution");
                    mouseLoc = GetShaderLocation(shader, "mouse");
                    timeLoc = GetShaderLocation(shader, "time");

                    // Reset required uniforms
                    Raylib.SetShaderValue(
                        shader,
                        resolutionLoc,
                        resolution,
                        ShaderUniformDataType.Vec2
                    );
                }

                fragShaderFileModTime = currentFragShaderModTime;
            }
        }

        if (IsKeyPressed(KeyboardKey.A))
        {
            shaderAutoReloading = !shaderAutoReloading;
        }
#endif
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // We only draw a white full-screen rectangle, frame is generated in shader
        BeginShaderMode(shader);
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

#if BROWSER
        DrawText("Shader generates the frame in real time", 10, 10, 10, Color.Black);
#else
        var info = $"PRESS [A] to TOGGLE SHADER AUTOLOADING: {(shaderAutoReloading ? "AUTO" : "MANUAL")}";
        DrawText(info, 10, 10, 10, shaderAutoReloading ? Color.Red : Color.Black);
        if (!shaderAutoReloading)
        {
            DrawText("MOUSE CLICK to SHADER RE-LOADING", 10, 30, 10, Color.Black);
        }

        var lastModification = DateTimeOffset.FromUnixTimeSeconds(fragShaderFileModTime).LocalDateTime.ToString();
        DrawText($"Shader last modification: {lastModification}", 10, 430, 10, Color.Black);
#endif

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - hot reloading");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new HotReloading();
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
