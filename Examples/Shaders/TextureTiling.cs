/*******************************************************************************************
*
*   raylib [shaders] example - texture tiling
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example demonstrates how to tile a texture on a 3D model using raylib
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by Luis Almeida (@luis605) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Luis Almeida (@luis605)
*
********************************************************************************************/

namespace Examples.Shaders;

public class TextureTiling : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Texture Tiling";

    public string Title => "raylib [shaders] example - texture tiling";

    public bool CursorDisabled => true;

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private Shader shader;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(4.0f, 4.0f, 4.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load a cube model
        var cube = GenMeshCube(1.0f, 1.0f, 1.0f);
        model = LoadModelFromMesh(cube);

        // Load a texture and assign to cube model
        texture = LoadTexture("resources/cubicmap_atlas.png");
        model.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = texture;

        // Set the texture tiling using a shader
        var tiling = new[] { 3.0f, 3.0f };
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/tiling.fs");
        SetTextureWrap(texture, TextureWrap.Repeat);
        Raylib.SetShaderValue(shader, GetShaderLocation(shader, "tiling"), tiling, ShaderUniformDataType.Vec2);
        model.Materials[0].Shader = shader;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Free);

        if (IsKeyPressed(KeyboardKey.Z))
        {
            camera.Target = new Vector3(0.0f, 0.5f, 0.0f);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        BeginShaderMode(shader);
        DrawModel(model, new Vector3(0.0f, 0.0f, 0.0f), 2.0f, Color.White);
        EndShaderMode();

        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawText("Use mouse to rotate the camera", 10, 10, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(model);         // Unload model
        UnloadShader(shader);       // Unload shader
        UnloadTexture(texture);     // Unload texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - texture tiling");

        DisableCursor();                    // Limit cursor to relative movement inside the window

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new TextureTiling();
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
