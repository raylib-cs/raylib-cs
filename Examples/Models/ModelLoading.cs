/*******************************************************************************************
*
*   raylib [models] example - Models loading
*
*   raylib supports multiple models file formats:
*
*     - OBJ > Text file, must include vertex position-texcoords-normals information,
*             if files references some .mtl materials file, it will be loaded (or try to)
*     - GLTF > Modern text/binary file format, includes lot of information and it could
*              also reference external files, raylib will try loading mesh and materials data
*     - IQM > Binary file format including mesh vertex data but also animation data,
*             raylib can load .iqm animations.
*
*   This example has been created using raylib 2.6 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2014-2019 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public class ModelLoading
{
    public unsafe static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [models] example - models loading");

        // Define the camera to look into our 3d world
        Camera3D camera = new();
        camera.Position = new Vector3(50.0f, 50.0f, 50.0f);
        camera.Target = new Vector3(0.0f, 10.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        Model model = LoadModel("resources/models/obj/castle.obj");
        Texture2D texture = LoadTexture("resources/models/obj/castle_diffuse.png");

        // Set map diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        Vector3 position = new(0.0f, 0.0f, 0.0f);
        BoundingBox bounds = GetMeshBoundingBox(model.Meshes[0]);

        // NOTE: bounds are calculated from the original size of the model,
        // if model is scaled on drawing, bounds must be also scaled

        bool selected = false;

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            UpdateCamera(ref camera, CameraMode.Orbital);

            if (IsFileDropped())
            {
                string[] droppedFiles = Raylib.GetDroppedFiles();

                if (droppedFiles.Length == 1)
                {
                    if (IsFileExtension(droppedFiles[0], ".obj") ||
                        IsFileExtension(droppedFiles[0], ".gltf") ||
                        IsFileExtension(droppedFiles[0], ".glb") ||
                        IsFileExtension(droppedFiles[0], ".vox") ||
                        IsFileExtension(droppedFiles[0], ".iqm") ||
                        IsFileExtension(droppedFiles[0], ".m3d")
                    )
                    {
                        UnloadModel(model);
                        model = LoadModel(droppedFiles[0]);

                        // Set current map diffuse texture
                        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

                        bounds = GetMeshBoundingBox(model.Meshes[0]);

                        // TODO: Move camera position from target enough distance to visualize model properly
                    }
                    else if (IsFileExtension(droppedFiles[0], ".png"))
                    {
                        // Unload model texture and load new one
                        UnloadTexture(texture);
                        texture = LoadTexture(droppedFiles[0]);
                        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);
                    }
                }
            }

            // Select model on mouse click
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                // Check collision between ray and box
                if (GetRayCollisionBox(GetScreenToWorldRay(GetMousePosition(), camera), bounds).Hit)
                {
                    selected = !selected;
                }
                else
                {
                    selected = false;
                }
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            BeginMode3D(camera);

            DrawModel(model, position, 1.0f, Color.White);

            DrawGrid(20, 10.0f);

            if (selected)
            {
                DrawBoundingBox(bounds, Color.Green);
            }

            EndMode3D();

            DrawText("Drag & drop model to load mesh/texture.", 10, GetScreenHeight() - 20, 10, Color.DarkGray);
            if (selected)
            {
                DrawText("MODEL SELECTED", GetScreenWidth() - 110, 10, 10, Color.Green);
            }

            DrawText("(c) Castle 3D model by Alberto Cano", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

            DrawFPS(10, 10);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadTexture(texture);
        UnloadModel(model);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
