/*******************************************************************************************
*
*   raylib [models] example - loading m3d
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by bzt (@bztsrc) and reviewed by Ramon Santamaria (@raysan5)
*
*   NOTES:
*     - Model3D (M3D) fileformat specs: https://gitlab.com/bztsrc/model3d
*     - Bender M3D exported: https://gitlab.com/bztsrc/model3d/-/tree/master/blender
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 bzt (@bztsrc)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public partial class LoadingM3d : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Loading M3D";

    public string Title => "raylib [models] example - loading m3d";

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
        camera.Position = new Vector3(1.5f, 1.5f, 1.5f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.4f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load model
        model = LoadModel("resources/models/m3d/cesium_man.m3d");             // Load the animated model mesh and basic data
        position = new Vector3(0.0f, 0.0f, 0.0f); // Set model position

        // Load animation data
        animCount = 0;
        anims = LoadModelAnimations("resources/models/m3d/cesium_man.m3d", ref animCount);

        // Animation playing variables
        animIndex = 0;         // Current animation playing
        animCurrentFrame = 0.0f;      // Current animation frame (supporting interpolated frames)
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
        animCurrentFrame += 1.0f;
        if (animCurrentFrame >= anims[animIndex].KeyFrameCount)
        {
            animCurrentFrame = 0.0f;
        }
        UpdateModelAnimation(model, anims[animIndex], animCurrentFrame);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw 3d model with texture
        if (!IsKeyDown(KeyboardKey.Space))
        {
            DrawModel(model, position, 1.0f, Color.White);
        }
        else
        {
            // Draw the animated skeleton
            DrawModelSkeleton(model.Skeleton, anims[animIndex].KeyframePoses[(int)animCurrentFrame], 1.0f, Color.Red);
        }

        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawText($"Current animation: {anims[animIndex].NameToString()}", 10, 10, 20, Color.LightGray);
        DrawText("Press SPACE to draw skeleton", 10, 40, 20, Color.Maroon);
        DrawText("(c) CesiumMan model by KhronosGroup", GetScreenWidth() - 210, GetScreenHeight() - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(anims, animCount);   // Unload model animations data
        UnloadModel(model);                        // Unload model
    }

    // Draw model skeleton
    private static unsafe void DrawModelSkeleton(ModelSkeleton skeleton, Transform* pose, float scale, Color color)
    {
        // Loop to (boneCount - 1) because the last one is a special "no bone" bone,
        // needed to workaround buggy models without a -1, a cube is always drawn at the origin
        for (var i = 0; i < skeleton.BoneCount - 1; i++)
        {
            // Display the frame-pose skeleton
            DrawCube(pose[i].Translation, scale * 0.05f, scale * 0.05f, scale * 0.05f, color);

            if (skeleton.Bones[i].Parent >= 0)
            {
                DrawLine3D(pose[i].Translation, pose[skeleton.Bones[i].Parent].Translation, color);
            }
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - loading m3d");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LoadingM3d();
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
