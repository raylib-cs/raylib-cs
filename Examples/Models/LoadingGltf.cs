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

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public class LoadingGltf
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [models] example - loading gltf");

        // Define the camera to look into our 3d world
        Camera3D camera = new();
        camera.Position = new Vector3(6.0f, 6.0f, 6.0f);
        camera.Target = new Vector3(0.0f, 2.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        Model model = LoadModel("resources/models/gltf/robot.glb");
        Vector3 position = new(0.0f, 0.0f, 0.0f);

        // Load animation data
        var anims = LoadModelAnimations("resources/models/gltf/robot.glb");

        // Animation playing variables
        int animIndex = 0;
        float animCurrentFrame = 0.0f;

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            UpdateCamera(ref camera, CameraMode.Orbital);

            if (IsKeyPressed(KeyboardKey.Right))
            {
                animIndex = (animIndex + 1) % anims.Length;
            }
            else if (IsKeyPressed(KeyboardKey.Left))
            {
                animIndex = (animIndex + anims.Length - 1) % anims.Length;
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

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadModelAnimations(anims);
        UnloadModel(model);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}

