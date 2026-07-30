/*******************************************************************************************
*
*   raylib [models] example - loading vox
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 4.0, last time updated with raylib 4.0
*
*   Example contributed by Johann Nadalutti (@procfxgen) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Johann Nadalutti (@procfxgen) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raymath;
using Examples.Shared;

namespace Examples.Models;

public partial class LoadingVox : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxVoxFiles = 4;
    private const int MaxLights = 4;

#if BROWSER
    private const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Models / Loading VOX";

    public string Title => "raylib [models] example - loading vox";

    private static readonly string[] VoxFileNames =
    {
        "resources/models/vox/chr_knight.vox",
        "resources/models/vox/chr_sword.vox",
        "resources/models/vox/monu9.vox",
        "resources/models/vox/fez.vox"
    };

    private Camera3D camera;
    private Model[] models;
    private int currentModel;
    private Vector3 modelpos;
    private Vector3 camerarot;
    private Shader shader;
    private Light[] lights;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(10.0f, 10.0f, 10.0f); // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load MagicaVoxel files
        models = new Model[MaxVoxFiles];

        for (var i = 0; i < MaxVoxFiles; i++)
        {
            // Load VOX file and measure time
            var t0 = GetTime() * 1000.0;
            models[i] = LoadModel(VoxFileNames[i]);
            var t1 = GetTime() * 1000.0;

            TraceLog(TraceLogLevel.Info, $"[{VoxFileNames[i]}] Model file loaded in {t1 - t0:0.000} ms");

            // Compute model translation matrix to center model on draw position (0, 0 , 0)
            var bb = GetModelBoundingBox(models[i]);
            Vector3 center = new();
            center.X = bb.Min.X + ((bb.Max.X - bb.Min.X) / 2);
            center.Z = bb.Min.Z + ((bb.Max.Z - bb.Min.Z) / 2);

            var matTranslate = MatrixTranslate(-center.X, 0, -center.Z);
            models[i].Transform = matTranslate;
        }

        currentModel = 0;
        modelpos = new Vector3(0, 0, 0);
        camerarot = new Vector3(0, 0, 0);

        // Load voxel shader
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/voxel_lighting.vs",
            $"resources/shaders/glsl{GlslVersion}/voxel_lighting.fs"
        );

        // Get some required shader locations
        shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shader, "viewPos");
        // NOTE: "matModel" location name is automatically assigned on shader loading,
        // no need to get the location again if using that uniform name
        //shader.Locs[(int)ShaderLocationIndex.MatrixModel] = GetShaderLocation(shader, "matModel");

        // Ambient light level (some basic lighting)
        var ambientLoc = GetShaderLocation(shader, "ambient");
        Raylib.SetShaderValue(shader, ambientLoc, new[] { 0.1f, 0.1f, 0.1f, 1.0f }, ShaderUniformDataType.Vec4);

        // Assign out lighting shader to model
        for (var i = 0; i < MaxVoxFiles; i++)
        {
            for (var j = 0; j < models[i].MaterialCount; j++)
            {
                models[i].Materials[j].Shader = shader;
            }
        }

        // Create lights
        lights = new Light[MaxLights];
        lights[0] = Rlights.CreateLight(0, LightType.Point, new Vector3(-20, 20, -20), Vector3.Zero, Color.Gray, shader);
        lights[1] = Rlights.CreateLight(1, LightType.Point, new Vector3(20, -20, 20), Vector3.Zero, Color.Gray, shader);
        lights[2] = Rlights.CreateLight(2, LightType.Point, new Vector3(-20, 20, 20), Vector3.Zero, Color.Gray, shader);
        lights[3] = Rlights.CreateLight(3, LightType.Point, new Vector3(20, -20, -20), Vector3.Zero, Color.Gray, shader);
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsMouseButtonDown(MouseButton.Middle))
        {
            var mouseDelta = GetMouseDelta();
            camerarot.X = mouseDelta.X * 0.05f;
            camerarot.Y = mouseDelta.Y * 0.05f;
        }
        else
        {
            camerarot.X = 0;
            camerarot.Y = 0;
        }

        // Update camere movement, custom controls
        UpdateCameraPro(ref camera,
            new Vector3(
                (IsKeyDown(KeyboardKey.W) || IsKeyDown(KeyboardKey.Up) ? 0.1f : 0.0f) - (IsKeyDown(KeyboardKey.S) || IsKeyDown(KeyboardKey.Down) ? 0.1f : 0.0f), // Move forward-backward
                (IsKeyDown(KeyboardKey.D) || IsKeyDown(KeyboardKey.Right) ? 0.1f : 0.0f) - (IsKeyDown(KeyboardKey.A) || IsKeyDown(KeyboardKey.Left) ? 0.1f : 0.0f), // Move right-left
                0.0f), // Move up-down
            camerarot, // Camera rotation
            GetMouseWheelMove() * -2.0f); // Move to target (zoom)

        // Cycle between models on mouse click
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            currentModel = (currentModel + 1) % MaxVoxFiles;
        }

        // Update the shader with the camera view vector (points towards { 0.0f, 0.0f, 0.0f })
        Raylib.SetShaderValue(shader, shader.Locs[(int)ShaderLocationIndex.VectorView], camera.Position, ShaderUniformDataType.Vec3);

        // Update light values (actually, only enable/disable them)
        for (var i = 0; i < MaxLights; i++)
        {
            Rlights.UpdateLightValues(shader, lights[i]);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw 3D model
        BeginMode3D(camera);

        DrawModel(models[currentModel], modelpos, 1.0f, Color.White);
        DrawGrid(10, 1.0f);

        // Draw spheres to show where the lights are
        for (var i = 0; i < MaxLights; i++)
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

        EndMode3D();

        // Display info
        DrawRectangle(10, 40, 340, 70, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(10, 40, 340, 70, Fade(Color.DarkBlue, 0.5f));
        DrawText("- MOUSE LEFT BUTTON: CYCLE VOX MODELS", 20, 50, 10, Color.Blue);
        DrawText("- MOUSE MIDDLE BUTTON: ZOOM OR ROTATE CAMERA", 20, 70, 10, Color.Blue);
        DrawText("- UP-DOWN-LEFT-RIGHT KEYS: MOVE CAMERA", 20, 90, 10, Color.Blue);
        DrawText($"VOX model file: {GetFileName(VoxFileNames[currentModel])}", 10, 10, 20, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // Unload models data (GPU VRAM)
        for (var i = 0; i < MaxVoxFiles; i++)
        {
            UnloadModel(models[i]);
        }

        UnloadShader(shader);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - loading vox");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LoadingVox();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
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
