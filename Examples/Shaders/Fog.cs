/*******************************************************************************************
*
*   raylib [shaders] example - fog rendering
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.7
*
*   Example contributed by Chris Camacho (@chriscamacho) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Chris Camacho (@chriscamacho) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using Examples.Shared;

namespace Examples.Shaders;

public class Fog : IExample
{
#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shaders / Fog";

    public string Title => "raylib [shaders] example - fog rendering";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Model modelA;
    private Model modelB;
    private Model modelC;
    private Texture2D texture;
    private Shader shader;
    private int fogDensityLoc;
    private float fogDensity;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(2.0f, 2.0f, 6.0f);
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load models and texture
        modelA = LoadModelFromMesh(GenMeshTorus(0.4f, 1.0f, 16, 32));
        modelB = LoadModelFromMesh(GenMeshCube(1.0f, 1.0f, 1.0f));
        modelC = LoadModelFromMesh(GenMeshSphere(0.5f, 32, 32));
        texture = LoadTexture("resources/texel_checker.png");

        // Assign texture to default model material
        Raylib.SetMaterialTexture(ref modelA, 0, MaterialMapIndex.Albedo, ref texture);
        Raylib.SetMaterialTexture(ref modelB, 0, MaterialMapIndex.Albedo, ref texture);
        Raylib.SetMaterialTexture(ref modelC, 0, MaterialMapIndex.Albedo, ref texture);

        // Load shader and set up some uniforms
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/lighting.vs",
            $"resources/shaders/glsl{GlslVersion}/fog.fs"
        );
        shader.Locs[(int)ShaderLocationIndex.MatrixModel] = GetShaderLocation(shader, "matModel");
        shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shader, "viewPos");

        // Ambient light level
        var ambientLoc = GetShaderLocation(shader, "ambient");
        Raylib.SetShaderValue(
            shader,
            ambientLoc,
            new float[] { 0.2f, 0.2f, 0.2f, 1.0f },
            ShaderUniformDataType.Vec4
        );

        var fogColor = ColorNormalize(Color.Gray);
        var fogColorLoc = GetShaderLocation(shader, "fogColor");
        Raylib.SetShaderValue(shader, fogColorLoc, fogColor, ShaderUniformDataType.Vec4);

        fogDensity = 0.15f;
        fogDensityLoc = GetShaderLocation(shader, "fogDensity");
        Raylib.SetShaderValue(shader, fogDensityLoc, fogDensity, ShaderUniformDataType.Float);

        // NOTE: All models share the same shader
        Raylib.SetMaterialShader(ref modelA, 0, ref shader);
        Raylib.SetMaterialShader(ref modelB, 0, ref shader);
        Raylib.SetMaterialShader(ref modelC, 0, ref shader);

        // Using just 1 point lights
        Rlights.CreateLight(0, LightType.Point, new Vector3(0, 2, 6), Vector3.Zero, Color.White, shader);
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        if (IsKeyDown(KeyboardKey.Up))
        {
            fogDensity += 0.001f;
            if (fogDensity > 1.0f)
            {
                fogDensity = 1.0f;
            }
        }

        if (IsKeyDown(KeyboardKey.Down))
        {
            fogDensity -= 0.001f;
            if (fogDensity < 0.0f)
            {
                fogDensity = 0.0f;
            }
        }

        Raylib.SetShaderValue(shader, fogDensityLoc, fogDensity, ShaderUniformDataType.Float);

        // Rotate the torus
        modelA.Transform = MatrixMultiply(modelA.Transform, MatrixRotateX(-0.025f));
        modelA.Transform = MatrixMultiply(modelA.Transform, MatrixRotateZ(0.012f));

        // Update the light shader with the camera view position
        Raylib.SetShaderValue(
            shader,
            shader.Locs[(int)ShaderLocationIndex.VectorView],
            camera.Position,
            ShaderUniformDataType.Vec3
        );
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.Gray);

        BeginMode3D(camera);

        // Draw the three models
        DrawModel(modelA, Vector3.Zero, 1.0f, Color.White);
        DrawModel(modelB, new Vector3(-2.6f, 0, 0), 1.0f, Color.White);
        DrawModel(modelC, new Vector3(2.6f, 0, 0), 1.0f, Color.White);

        for (var i = -20; i < 20; i += 2)
        {
            DrawModel(modelA, new Vector3(i, 0, 2), 1.0f, Color.White);
        }

        EndMode3D();

        DrawText(
            $"Use KEY_UP/KEY_DOWN to change fog density [{fogDensity:F2}]",
            10,
            10,
            20,
            Color.RayWhite
        );

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(modelA);
        UnloadModel(modelB);
        UnloadModel(modelC);

        UnloadTexture(texture);
        UnloadShader(shader);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        // Enable Multi Sampling Anti Aliasing 4x (if available)
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - fog rendering");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new Fog();
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
