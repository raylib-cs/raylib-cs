/*******************************************************************************************
*
*   raylib [shaders] example - cel shading
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example contributed by Gleb A (@ggrizzly) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Gleb A (@ggrizzly)
*
********************************************************************************************/

using static Raylib_cs.Raymath;
using static Raylib_cs.Rlgl;
using Examples.Shared;

namespace Examples.Shaders;

public partial class CelShading : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxLights = 4;

    // rlgl cull face modes (rlgl.h: RL_CULL_FACE_FRONT = 0, RL_CULL_FACE_BACK = 1)
    private const int CullFaceFront = 0;
    private const int CullFaceBack = 1;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Cel Shading";

    public string Title => "raylib [shaders] example - cel shading";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Model model;
    private Shader celShader;
    private Shader defaultShader;
    private Shader outlineShader;
    private float numBands;
    private int numBandsLoc;
    private int outlineThicknessLoc;
    private Light[] lights;
    private bool celEnabled;
    private bool outlineEnabled;

    public unsafe void Init()
    {
        camera = new();
        camera.Position = new Vector3(9.0f, 6.0f, 9.0f);
        camera.Target = new Vector3(0.0f, 1.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load model
        model = LoadModel("resources/models/old_car_new.glb");

        // Load cel shader
        celShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/cel.vs",
            $"resources/shaders/glsl{GlslVersion}/cel.fs"
        );
        celShader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(celShader, "viewPos");

        // Apply cel shader to model, keep copy of default shader
        defaultShader = model.Materials[0].Shader;
        model.Materials[0].Shader = celShader;

        // numBands: controls toon quantization steps (2 = hard binary, 20 = near-smooth)
        numBands = 10.0f;
        numBandsLoc = GetShaderLocation(celShader, "numBands");
        Raylib.SetShaderValue(celShader, numBandsLoc, numBands, ShaderUniformDataType.Float);

        // Inverted-hull outline shader: draws back faces extruded along normals
        outlineShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/outline_hull.vs",
            $"resources/shaders/glsl{GlslVersion}/outline_hull.fs"
        );
        outlineThicknessLoc = GetShaderLocation(outlineShader, "outlineThickness");

        // Single directional white light, angled so toon bands are visible on the model sides.
        // Spins opposite to CAMERA_ORBITAL (0.5 rad/s) so lighting changes as you watch.
        lights = new Light[MaxLights];
        lights[0] = Rlights.CreateLight(
            0,
            LightType.Directorional,
            new Vector3(50.0f, 50.0f, 50.0f),
            Vector3.Zero,
            Color.White,
            celShader
        );

        celEnabled = true;
        outlineEnabled = true;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        Raylib.SetShaderValue(
            celShader,
            celShader.Locs[(int)ShaderLocationIndex.VectorView],
            camera.Position,
            ShaderUniformDataType.Vec3
        );

        // [Z] Toggle cel shading on/off
        if (IsKeyPressed(KeyboardKey.Z))
        {
            celEnabled = !celEnabled;
            if (celEnabled)
            {
                model.Materials[0].Shader = celShader;      // Apply cel shader to model
            }
            else
            {
                model.Materials[0].Shader = defaultShader;  // Apply default shader to model
            }
        }

        // [C] Toggle outline on/off
        if (IsKeyPressed(KeyboardKey.C))
        {
            outlineEnabled = !outlineEnabled;
        }

        // [Q/E] Decrease/increase toon band count (press or hold to repeat)
        if (IsKeyPressed(KeyboardKey.E) || IsKeyPressedRepeat(KeyboardKey.E))
        {
            numBands = Clamp(numBands + 1.0f, 2.0f, 20.0f);
        }
        if (IsKeyPressed(KeyboardKey.Q) || IsKeyPressedRepeat(KeyboardKey.Q))
        {
            numBands = Clamp(numBands - 1.0f, 2.0f, 20.0f);
        }
        Raylib.SetShaderValue(celShader, numBandsLoc, numBands, ShaderUniformDataType.Float);

        // Spin light opposite to CAMERA_ORBITAL (0.5 rad/s), angled 45 degrees off vertical
        float t = (float)GetTime();
        lights[0].Position = new Vector3(MathF.Sin(-t * 0.3f) * 5.0f, 5.0f, MathF.Cos(-t * 0.3f) * 5.0f);

        for (var i = 0; i < MaxLights; i++)
        {
            Rlights.UpdateLightValues(celShader, lights[i]);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        if (outlineEnabled)
        {
            // Outline pass: cull front faces, draw extruded back faces as silhouette
            float thickness = 0.005f;
            Raylib.SetShaderValue(outlineShader, outlineThicknessLoc, thickness, ShaderUniformDataType.Float);

            SetCullFace(CullFaceFront);

            model.Materials[0].Shader = outlineShader;

            DrawModel(model, Vector3.Zero, 0.75f, Color.White);

            if (celEnabled)
            {
                model.Materials[0].Shader = celShader;      // Apply cel shader to model
            }
            else
            {
                model.Materials[0].Shader = defaultShader;  // Apply default shader to model
            }

            SetCullFace(CullFaceBack);
        }

        DrawModel(model, Vector3.Zero, 0.75f, Color.White);
        DrawSphereEx(lights[0].Position, 0.2f, 50, 50, Color.Yellow);  // Light position indicator
        DrawGrid(10, 10.0f);

        EndMode3D();

        DrawFPS(10, 10);
        DrawText($"Cel: {(celEnabled ? "ON" : "OFF")}  [Z]", 10, 65, 20, celEnabled ? Color.DarkGreen : Color.DarkGray);
        DrawText($"Outline: {(outlineEnabled ? "ON" : "OFF")}  [C]", 10, 90, 20, outlineEnabled ? Color.DarkGreen : Color.DarkGray);
        DrawText($"Bands: {numBands:0}  [Q/E]", 10, 115, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(model);
        UnloadShader(celShader);
        UnloadShader(outlineShader);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - cel shading");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new CelShading();
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
