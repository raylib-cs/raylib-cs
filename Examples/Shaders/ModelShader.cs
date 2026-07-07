/*******************************************************************************************
*
*   raylib [shaders] example - model shader
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3), to test this example
*         on OpenGL ES 2.0 platforms (Android, Raspberry Pi, HTML5), use #version 100 shaders
*         raylib comes with shaders ready for both versions, check raylib/shaders install folder
*
*   Example originally created with raylib 1.3, last time updated with raylib 3.7
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class ModelShader : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Model Shader";

    public string Title => "raylib [shaders] example - model shader";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    public bool CursorDisabled => true;

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private Shader shader;
    private Vector3 position;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(4.0f, 4.0f, 4.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 1.0f, -1.0f);     // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        model = LoadModel("resources/models/obj/watermill.obj");                   // Load OBJ model
        texture = LoadTexture("resources/models/obj/watermill_diffuse.png");   // Load model texture

        // Load shader for model
        // NOTE: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/grayscale.fs");

        Raylib.SetMaterialShader(ref model, 0, ref shader);                 // Set shader effect to 3d model
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture); // Bind texture to model

        position = new(0.0f, 0.0f, 0.0f);    // Set model position
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Free);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, position, 0.2f, Color.White);   // Draw 3d model with texture

        DrawGrid(10, 1.0f);     // Draw a grid

        EndMode3D();

        DrawText(
            "(c) Watermill 3D model by Alberto Cano",
            screenWidth - 210,
            screenHeight - 20,
            10,
            Color.Gray
        );

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);       // Unload shader
        UnloadTexture(texture);     // Unload texture
        UnloadModel(model);         // Unload model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);      // Enable Multi Sampling Anti Aliasing 4x (if available)

        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - model shader");

        DisableCursor();                    // Limit cursor to relative movement inside the window
        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ModelShader();
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
