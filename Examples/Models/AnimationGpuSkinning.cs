/*******************************************************************************************
*
*   raylib [models] example - animation gpu skinning
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by Daniel Holden (@orangeduck) and reviewed by Ramon Santamaria (@raysan5)
*
*   WARNING: GPU skinning must be enabled in raylib with a compilation flag,
*   if not enabled, CPU skinning will be used instead
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2025 Daniel Holden (@orangeduck)
*
********************************************************************************************/

namespace Examples.Models;

public partial class AnimationGpuSkinning : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    private const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Models / Animation GPU Skinning";

    public string Title => "raylib [models] example - animation gpu skinning";

    private Camera3D camera;
    private Model model;
    private Vector3 position;
    private Shader skinningShader;
    private unsafe ModelAnimation* anims;
    private int animCount;
    private int animIndex;
    private int animCurrentFrame;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(5.0f, 5.0f, 5.0f); // Camera position
        camera.Target = new Vector3(0.0f, 1.0f, 0.0f);  // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);      // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                            // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective; // Camera projection type

        // Load gltf model
        model = LoadModel("resources/models/gltf/greenman.glb"); // Load character model
        position = new Vector3(0.0f, 0.0f, 0.0f); // Set model position

        // Load skinning shader
        // NOTE: It must be a valid shader, following raylib attribs/uniform conventions for GPU skinning
        skinningShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/skinning.vs",
            $"resources/shaders/glsl{GlslVersion}/skinning.fs"
        );

        // Skinning shader could be required to be assigned to all materials shaders, just to make
        // sure required uniforms are being updated for the mesh using that material (and shader)
        model.Materials[1].Shader = skinningShader; // Just assigning to materials[1] for this model

        // Load gltf model animations
        animCount = 0;
        anims = LoadModelAnimations("resources/models/gltf/greenman.glb", ref animCount);

        // Animation playing variables
        animIndex = 0;         // Current animation playing
        animCurrentFrame = 0;  // Current animation frame
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Select current animation
        if (IsKeyPressed(KeyboardKey.Right))
        {
            animIndex = (animIndex + 1) % animCount;
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            animIndex = (animIndex + animCount - 1) % animCount;
        }

        // Update model animation
        animCurrentFrame = (animCurrentFrame + 1) % anims[animIndex].KeyFrameCount;
        UpdateModelAnimation(model, anims[animIndex], animCurrentFrame);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, position, 1.0f, Color.White);

        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawText($"Current animation: {anims[animIndex].NameToString()}", 10, 40, 20, Color.Maroon);
        DrawText("Use the LEFT/RIGHT keys to switch animation", 10, 10, 20, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(anims, animCount); // Unload model animation
        UnloadModel(model);             // Unload model and meshes/material
        UnloadShader(skinningShader);   // Unload GPU skinning shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - animation gpu skinning");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new AnimationGpuSkinning();
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
