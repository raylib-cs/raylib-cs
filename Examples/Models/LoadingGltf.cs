/*******************************************************************************************
*
*   raylib [models] example - loading gltf
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   LIMITATIONS:
*     - Only supports 1 armature per file, and skips loading it if there are multiple armatures
*     - Only supports linear interpolation (default method in Blender when checked
*       "Always Sample Animations" when exporting a GLTF file)
*     - Only supports translation/rotation/scale animation channel.path,
*       weights not considered (i.e. morph targets)
*
*   Example originally created with raylib 3.7, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2020-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public partial class LoadingGltf : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Loading GLTF";

    public string Title => "raylib [models] example - loading gltf";

    private Camera3D camera;
    private Model model;
    private Vector3 position;
    private unsafe ModelAnimation* anims;
    private int animCount;
    private int animIndex;
    private float animCurrentFrame;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(6.0f, 6.0f, 6.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 2.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load model
        model = LoadModel("resources/models/gltf/robot.glb");
        position = new(0.0f, 0.0f, 0.0f); // Set model world position

        // Load model animations
        anims = LoadModelAnimations("resources/models/gltf/robot.glb", ref animCount);

        // Animation playing variables
        animIndex = 0;          // Current animation playing
        animCurrentFrame = 0.0f;  // Current animation frame
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
        UpdateModelAnimation(model, anims[animIndex], (float)animCurrentFrame);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(
            model,
            position,
            1f,
            Color.White
        );

        DrawGrid(10, 1.0f);

        EndMode3D();
        DrawText($"Current animation: {anims[animIndex].NameToString()}", 10, 40, 20, Color.Maroon);
        DrawText("Use the LEFT/RIGHT keys to switch animation", 10, 10, 20, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(anims, animCount); // Unload model animations data
        UnloadModel(model);         // Unload model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - loading gltf");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LoadingGltf();
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
