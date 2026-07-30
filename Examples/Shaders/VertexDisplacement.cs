/*******************************************************************************************
*
*   raylib [shaders] example - vertex displacement
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 4.5
*
*   Example contributed by Alex ZH (@ZzzhHe) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Alex ZH (@ZzzhHe)
*
********************************************************************************************/

using static Raylib_cs.Rlgl;

namespace Examples.Shaders;

public partial class VertexDisplacement : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Vertex Displacement";

    public string Title => "raylib [shaders] example - vertex displacement";

    private Camera3D camera;
    private Shader shader;
    private Texture2D perlinNoiseMap;
    private Model planeModel;
    private float time;

    public unsafe void Init()
    {
        // set up camera
        camera = new();
        camera.Position = new Vector3(20.0f, 5.0f, -20.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 60.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load vertex and fragment shaders
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/vertex_displacement.vs",
            $"resources/shaders/glsl{GlslVersion}/vertex_displacement.fs"
        );

        // Load perlin noise texture
        var perlinNoiseImage = GenImagePerlinNoise(512, 512, 0, 0, 1.0f);
        perlinNoiseMap = LoadTextureFromImage(perlinNoiseImage);
        UnloadImage(perlinNoiseImage);

        // Set shader uniform location
        var perlinNoiseMapLoc = GetShaderLocation(shader, "perlinNoiseMap");
        EnableShader(shader.Id);
        ActiveTextureSlot(1);
        EnableTexture(perlinNoiseMap.Id);
        SetUniformSampler(perlinNoiseMapLoc, 1);

        // Create a plane mesh and model
        var planeMesh = GenMeshPlane(50, 50, 50, 50);
        planeModel = LoadModelFromMesh(planeMesh);
        // Set plane model material
        var materials = planeModel.Materials;
        materials[0].Shader = shader;

        time = 0.0f;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Free); // Update camera

        time += GetFrameTime(); // Update time variable
        Raylib.SetShaderValue(shader, GetShaderLocation(shader, "time"), time, ShaderUniformDataType.Float); // Send time value to shader

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        BeginShaderMode(shader);
        // Draw plane model
        DrawModel(planeModel, new Vector3(0.0f, 0.0f, 0.0f), 1.0f, new Color(255, 255, 255, 255));
        EndShaderMode();

        EndMode3D();

        DrawText("Vertex displacement", 10, 10, 20, Color.DarkGray);
        DrawFPS(10, 40);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);
        UnloadModel(planeModel);
        UnloadTexture(perlinNoiseMap);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - vertex displacement");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new VertexDisplacement();
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
