/*******************************************************************************************
*
*   raylib [models] example - basic voxel
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Tim Little (@timlittle) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Tim Little (@timlittle)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public partial class BasicVoxel : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int WorldSize = 8;   // Size of our voxel world (8x8x8 cubes)

    public string Name => "Models / Basic Voxel";

    public string Title => "raylib [models] example - basic voxel";

    public bool CursorDisabled => true;

    private Camera3D camera;
    private Model cubeModel;
    private bool[,,] voxels;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world (first person)
        camera = new();
        camera.Position = new Vector3(-2.0f, 0.0f, -2.0f);  // Camera position at ground level
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Create a cube model
        var cubeMesh = GenMeshCube(1.0f, 1.0f, 1.0f);       // Create a unit cube mesh
        cubeModel = LoadModelFromMesh(cubeMesh);            // Convert mesh to a model
        cubeModel.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Color = Color.Beige;

        // Initialize voxel world - fill with voxels
        voxels = new bool[WorldSize, WorldSize, WorldSize];
        for (var x = 0; x < WorldSize; x++)
        {
            for (var y = 0; y < WorldSize; y++)
            {
                for (var z = 0; z < WorldSize; z++)
                {
                    voxels[x, y, z] = true;
                }
            }
        }
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.FirstPerson);

        // Handle voxel removal with mouse click
        // This method is quite inefficient. Ray marching through the voxel grid using DDA would be faster, but more complex.
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            // Cast a ray from the screen center (where crosshair would be)
            Vector2 screenCenter = new(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);
            var ray = GetScreenToWorldRay(screenCenter, camera);

            // Check ray collision with all voxels
            var closestDistance = 99999.0f;
            Vector3 closestVoxelPosition = new(-1, -1, -1);
            var voxelFound = false;
            for (var x = 0; x < WorldSize; x++)
            {
                for (var y = 0; y < WorldSize; y++)
                {
                    for (var z = 0; z < WorldSize; z++)
                    {
                        if (!voxels[x, y, z])
                        {
                            continue; // Skip empty voxels
                        }

                        // Build a bounding box for this voxel
                        Vector3 position = new(x, y, z);
                        BoundingBox box = new(
                            new Vector3(position.X - 0.5f, position.Y - 0.5f, position.Z - 0.5f),
                            new Vector3(position.X + 0.5f, position.Y + 0.5f, position.Z + 0.5f)
                        );

                        // Check ray-box collision
                        var collision = GetRayCollisionBox(ray, box);
                        if (collision.Hit && (collision.Distance < closestDistance))
                        {
                            closestDistance = collision.Distance;
                            closestVoxelPosition = new Vector3(x, y, z);
                            voxelFound = true;
                        }
                    }
                }
            }

            // Remove the closest voxel if one was hit
            if (voxelFound)
            {
                voxels[(int)closestVoxelPosition.X,
                       (int)closestVoxelPosition.Y,
                       (int)closestVoxelPosition.Z] = false;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawGrid(10, 1.0f);

        // Draw all voxels
        for (var x = 0; x < WorldSize; x++)
        {
            for (var y = 0; y < WorldSize; y++)
            {
                for (var z = 0; z < WorldSize; z++)
                {
                    if (!voxels[x, y, z])
                    {
                        continue;
                    }

                    Vector3 position = new(x, y, z);
                    DrawModel(cubeModel, position, 1.0f, Color.Beige);
                    DrawCubeWires(position, 1.0f, 1.0f, 1.0f, Color.Black);
                }
            }
        }

        EndMode3D();

        // Draw reference point for raycasting to delete blocks
        DrawCircle(GetScreenWidth() / 2, GetScreenHeight() / 2, 4, Color.Red);

        DrawText("Left-click a voxel to remove it!", 10, 10, 20, Color.DarkGray);
        DrawText("WASD to move, mouse to look around", 10, 35, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(cubeModel);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - basic voxel");

        DisableCursor(); // Lock mouse to window center

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new BasicVoxel();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
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
