/*******************************************************************************************
*
*   raylib [models] example - loading
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   NOTE: raylib supports multiple models file formats:
*
*     - OBJ  > Text file format. Must include vertex position-texcoords-normals information,
*              if .obj references some .mtl materials file, it will be tried to be loaded
*     - GLTF/GLB > Text/binary file formats. Includes lot of information and it could
*              also reference external files, mesh and materials data will be tried to be loaded
*     - IQM  > Binary file format. Includes mesh vertex data but also animation data,
*              meshes and animation data can be loaded
*     - VOX  > Binary file format. MagikaVoxel mesh format:
*              https://github.com/ephtracy/voxel-model/blob/master/MagicaVoxel-file-format-vox.txt
*     - M3D  > Binary file format. Model 3D format:
*              https://bztsrc.gitlab.io/model3d
*
*   Example originally created with raylib 2.0, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public partial class ModelLoading : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Model Loading";

    public string Title => "raylib [models] example - loading";

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private Vector3 position;
    private BoundingBox bounds;
    private bool selected;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(50.0f, 50.0f, 50.0f); // Camera position
        camera.Target = new Vector3(0.0f, 12.0f, 0.0f);     // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera mode type

        model = LoadModel("resources/models/obj/castle.obj");                 // Load model
        texture = LoadTexture("resources/models/obj/castle_diffuse.png");     // Load model texture

        // Set map diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        position = new(0.0f, 0.0f, 0.0f);                  // Set model position
        bounds = GetMeshBoundingBox(model.Meshes[0]);      // Set model bounds

        // NOTE: bounds are calculated from the original size of the model,
        // if model is scaled on drawing, bounds must be also scaled

        selected = false;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

#if BROWSER
        // NOTE: drag-and-drop model loading is not supported in the browser host; default loaded model is kept.
#else
        // Load new models/textures on drag&drop
        if (IsFileDropped())
        {
            var droppedFiles = Raylib.GetDroppedFiles();

            if (droppedFiles.Length == 1) // Only support one file dropped
            {
                if (IsFileExtension(droppedFiles[0], ".obj") ||
                    IsFileExtension(droppedFiles[0], ".gltf") ||
                    IsFileExtension(droppedFiles[0], ".glb") ||
                    IsFileExtension(droppedFiles[0], ".vox") ||
                    IsFileExtension(droppedFiles[0], ".iqm") ||
                    IsFileExtension(droppedFiles[0], ".m3d")       // Model file formats supported
                )
                {
                    UnloadModel(model);                         // Unload previous model
                    model = LoadModel(droppedFiles[0]);         // Load new model

                    // Set current map diffuse texture
                    Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

                    bounds = GetMeshBoundingBox(model.Meshes[0]);

                    // Move camera position from target enough distance to visualize model properly
                    camera.Position.X = bounds.Max.X + 10.0f;
                    camera.Position.Y = bounds.Max.Y + 10.0f;
                    camera.Position.Z = bounds.Max.Z + 10.0f;
                }
                else if (IsFileExtension(droppedFiles[0], ".png"))  // Texture file formats supported
                {
                    // Unload current model texture and load new one
                    UnloadTexture(texture);
                    texture = LoadTexture(droppedFiles[0]);
                    Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);
                }
            }
        }
#endif

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

        DrawModel(model, position, 1.0f, Color.White);  // Draw 3d model with texture

        DrawGrid(20, 10.0f);        // Draw a grid

        if (selected)
        {
            DrawBoundingBox(bounds, Color.Green);   // Draw selection box
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

    public void Unload()
    {
        UnloadTexture(texture);
        UnloadModel(model);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - loading");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ModelLoading();
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
