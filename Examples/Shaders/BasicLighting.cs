/*******************************************************************************************
*
*   raylib [shaders] example - basic lighting
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example originally created with raylib 3.0, last time updated with raylib 4.2
*
*   Example contributed by Chris Camacho (@chriscamacho) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Chris Camacho (@chriscamacho) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using Examples.Shared;

namespace Examples.Shaders;

public class BasicLighting : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Basic Lighting";

    public string Title => "raylib [shaders] example - basic lighting";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Shader shader;
    private Light[] lights;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(2.0f, 4.0f, 6.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load basic lighting shader
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/lighting.vs",
            $"resources/shaders/glsl{GlslVersion}/lighting.fs"
        );
        // Get some required shader locations
        shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shader, "viewPos");
        // NOTE: "matModel" location name is automatically assigned on shader loading,
        // no need to get the location again if using that uniform name
        //shader.Locs[(int)ShaderLocationIndex.MatrixModel] = GetShaderLocation(shader, "matModel");

        // Ambient light level (some basic lighting)
        var ambientLoc = GetShaderLocation(shader, "ambient");
        var ambient = new[] { 0.1f, 0.1f, 0.1f, 1.0f };
        Raylib.SetShaderValue(shader, ambientLoc, ambient, ShaderUniformDataType.Vec4);

        // Create lights
        lights = new Light[4];
        lights[0] = Rlights.CreateLight(
            0,
            LightType.Point,
            new Vector3(-2, 1, -2),
            Vector3.Zero,
            Color.Yellow,
            shader
        );
        lights[1] = Rlights.CreateLight(
            1,
            LightType.Point,
            new Vector3(2, 1, 2),
            Vector3.Zero,
            Color.Red,
            shader
        );
        lights[2] = Rlights.CreateLight(
            2,
            LightType.Point,
            new Vector3(-2, 1, 2),
            Vector3.Zero,
            Color.Green,
            shader
        );
        lights[3] = Rlights.CreateLight(
            3,
            LightType.Point,
            new Vector3(2, 1, -2),
            Vector3.Zero,
            Color.Blue,
            shader
        );
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Update the shader with the camera view vector (points towards { 0.0f, 0.0f, 0.0f })
        Raylib.SetShaderValue(
            shader,
            shader.Locs[(int)ShaderLocationIndex.VectorView],
            camera.Position,
            ShaderUniformDataType.Vec3
        );

        // Check key inputs to enable/disable lights
        if (IsKeyPressed(KeyboardKey.Y))
        {
            lights[0].Enabled = !lights[0].Enabled;
        }
        if (IsKeyPressed(KeyboardKey.R))
        {
            lights[1].Enabled = !lights[1].Enabled;
        }
        if (IsKeyPressed(KeyboardKey.G))
        {
            lights[2].Enabled = !lights[2].Enabled;
        }
        if (IsKeyPressed(KeyboardKey.B))
        {
            lights[3].Enabled = !lights[3].Enabled;
        }

        // Update light values (actually, only enable/disable them)
        for (var i = 0; i < 4; i++)
        {
            Rlights.UpdateLightValues(shader, lights[i]);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        BeginShaderMode(shader);

        DrawPlane(Vector3.Zero, new Vector2(10.0f, 10.0f), Color.White);
        DrawCube(Vector3.Zero, 2.0f, 4.0f, 2.0f, Color.White);

        EndShaderMode();

        // Draw spheres to show where the lights are
        for (var i = 0; i < 4; i++)
        {
            if (lights[i].Enabled)
            {
                DrawSphereEx(lights[i].Position, 0.2f, 8, 8, lights[i].Color);
            }
            else
            {
                DrawSphereWires(lights[i].Position, 0.2f, 8, 8, ColorAlpha(lights[i].Color, 0.3f));
            }
        }

        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawFPS(10, 10);

        DrawText("Use keys [Y][R][G][B] to toggle lights", 10, 40, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);   // Unload shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);  // Enable Multi Sampling Anti Aliasing 4x (if available)
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - basic lighting");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new BasicLighting();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();          // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
