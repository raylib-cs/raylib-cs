/*******************************************************************************************
*
*   raylib [shaders] example - raymarching rendering
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: This example requires raylib OpenGL 3.3 for shaders support and only #version 330
*         is currently supported. OpenGL ES 2.0 platforms are not supported at the moment
*
*   Example originally created with raylib 2.0, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.ConfigFlags;

namespace Examples.Shaders;

public class Raymarching : IExample
{
#if BROWSER
    public const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    public const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Raymarching";

    public string Title => "raylib [shaders] example - raymarching rendering";

    public ConfigFlags ConfigFlags => ConfigFlags.ResizableWindow;

    public bool CursorDisabled => true;

    private int screenWidth;
    private int screenHeight;

    private Camera3D camera;
    private Shader shader;
    private int viewEyeLoc;
    private int viewCenterLoc;
    private int runTimeLoc;
    private int resolutionLoc;
    private float runTime;

    public void Init()
    {
        screenWidth = GetScreenWidth();
        screenHeight = GetScreenHeight();

        camera = new();
        camera.Position = new Vector3(2.5f, 2.5f, 3.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.7f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 65.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load raymarching shader
        // NOTE: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/raymarching.fs");

        // Get shader locations for required uniforms
        viewEyeLoc = GetShaderLocation(shader, "viewEye");
        viewCenterLoc = GetShaderLocation(shader, "viewCenter");
        runTimeLoc = GetShaderLocation(shader, "runTime");
        resolutionLoc = GetShaderLocation(shader, "resolution");

        float[] resolution = { (float)screenWidth, (float)screenHeight };
        Raylib.SetShaderValue(shader, resolutionLoc, resolution, ShaderUniformDataType.Vec2);

        runTime = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.FirstPerson);

        var deltaTime = GetFrameTime();
        runTime += deltaTime;

        // Set shader required uniform values
        Raylib.SetShaderValue(shader, viewEyeLoc, camera.Position, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(shader, viewCenterLoc, camera.Target, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(shader, runTimeLoc, runTime, ShaderUniformDataType.Float);

        // Check if screen is resized
        if (IsWindowResized())
        {
            screenWidth = GetScreenWidth();
            screenHeight = GetScreenHeight();
            var resolution = new float[] { (float)screenWidth, (float)screenHeight };
            Raylib.SetShaderValue(shader, resolutionLoc, resolution, ShaderUniformDataType.Vec2);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // We only draw a white full-screen rectangle,
        // frame is generated in shader using raymarching
        BeginShaderMode(shader);
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

        DrawText(
            "(c) Raymarching shader by Iñigo Quilez. MIT License.",
            screenWidth - 280,
            screenHeight - 20,
            10,
            Color.Black
        );

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);           // Unload shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ResizableWindow);
        InitWindow(800, 450, "raylib [shaders] example - raymarching rendering");

        DisableCursor();                    // Limit cursor to relative movement inside the window
        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Raymarching();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                  // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
