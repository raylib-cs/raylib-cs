/*******************************************************************************************
*
*   raylib [shaders] example - simple mask
*
*   Example complexity rating: [★★☆☆] 2/4
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
********************************************************************************************
*
*   After a model is loaded it has a default material, this material can be
*   modified in place rather than creating one from scratch...
*   While all of the maps have particular names, they can be used for any purpose
*   except for three maps that are applied as cubic maps (see below)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Shaders;

public class SimpleMask : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Simple Mask";

    public string Title => "raylib [shaders] example - simple mask";

    public bool CursorDisabled => true;

    private Camera3D camera;
    private Model model1;
    private Model model2;
    private Model model3;
    private Shader shader;
    private Texture2D texDiffuse;
    private Texture2D texMask;
    private int shaderFrame;
    private int framesCounter;
    private Vector3 rotation;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.0f, 1.0f, 2.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Define our three models to show the shader on
        var torus = GenMeshTorus(.3f, 1, 16, 32);
        model1 = LoadModelFromMesh(torus);

        var cube = GenMeshCube(.8f, .8f, .8f);
        model2 = LoadModelFromMesh(cube);

        // Generate model to be shaded just to see the gaps in the other two
        var sphere = GenMeshSphere(1, 16, 16);
        model3 = LoadModelFromMesh(sphere);

        // Load the shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/mask.fs");

        // Load and apply the diffuse texture (colour map)
        texDiffuse = LoadTexture("resources/plasma.png");

        var materials = model1.Materials;
        var maps = materials[0].Maps;
        model1.Materials[0].Maps[(int)MaterialMapIndex.Albedo].Texture = texDiffuse;

        materials = model2.Materials;
        maps = materials[0].Maps;
        maps[(int)MaterialMapIndex.Albedo].Texture = texDiffuse;

        // Using MATERIAL_MAP_EMISSION as a spare slot to use for 2nd texture
        // NOTE: Don't use MATERIAL_MAP_IRRADIANCE, MATERIAL_MAP_PREFILTER or  MATERIAL_MAP_CUBEMAP as they are bound as cube maps
        texMask = LoadTexture("resources/mask.png");

        materials = model1.Materials;
        maps = (MaterialMap*)materials[0].Maps;
        maps[(int)MaterialMapIndex.Emission].Texture = texMask;

        materials = model2.Materials;
        maps = (MaterialMap*)materials[0].Maps;
        maps[(int)MaterialMapIndex.Emission].Texture = texMask;

        var locs = shader.Locs;
        locs[(int)ShaderLocationIndex.MapEmission] = GetShaderLocation(shader, "mask");

        // Frame is incremented each frame to animate the shader
        shaderFrame = GetShaderLocation(shader, "frame");

        // Apply the shader to the two models
        materials = model1.Materials;
        materials[0].Shader = shader;

        materials = (Material*)model2.Materials;
        materials[0].Shader = shader;

        framesCounter = 0;
        rotation = new(0, 0, 0);    // Model rotation angles
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.FirstPerson);

        framesCounter++;
        rotation.X += 0.01f;
        rotation.Y += 0.005f;
        rotation.Z -= 0.0025f;

        // Send frames counter to shader for animation
        Raylib.SetShaderValue(shader, shaderFrame, framesCounter, ShaderUniformDataType.Int);

        // Rotate one of the models
        model1.Transform = MatrixRotateXYZ(rotation);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.DarkBlue);

        BeginMode3D(camera);

        DrawModel(model1, new Vector3(0.5f, 0, 0), 1, Color.White);
        DrawModelEx(model2, new Vector3(-.5f, 0, 0), new Vector3(1, 1, 0), 50, new Vector3(1, 1, 1), Color.White);
        DrawModel(model3, new Vector3(0, 0, -1.5f), 1, Color.White);
        DrawGrid(10, 1.0f);        // Draw a grid

        EndMode3D();

        var frameText = $"Frame: {framesCounter}";
        DrawRectangle(16, 698, MeasureText(frameText, 20) + 8, 42, Color.Blue);
        DrawText(frameText, 20, 700, 20, Color.White);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(model1);
        UnloadModel(model2);
        UnloadModel(model3);

        UnloadTexture(texDiffuse);  // Unload default diffuse texture
        UnloadTexture(texMask);     // Unload texture mask

        UnloadShader(shader);       // Unload shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - simple mask");

        DisableCursor();                    // Limit cursor to relative movement inside the window
        SetTargetFPS(60);                   // Set  to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SimpleMask();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
