/*******************************************************************************************
*
*   raylib [models] example - mesh picking
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 1.7, last time updated with raylib 4.0
*
*   Example contributed by Joel Davis (@joeld42) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2017-2025 Joel Davis (@joeld42) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Models;

public partial class MeshPicking : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Mesh Picking";

    public string Title => "raylib [models] example - mesh picking";

    private Camera3D camera;
    private Ray ray;
    private Model tower;
    private Texture2D texture;
    private Vector3 towerPos;
    private BoundingBox towerBBox;
    private Vector3 g0;
    private Vector3 g1;
    private Vector3 g2;
    private Vector3 g3;
    private Vector3 ta;
    private Vector3 tb;
    private Vector3 tc;
    private Vector3 bary;
    private Vector3 sp;
    private float sr;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(20.0f, 20.0f, 20.0f);
        camera.Target = new Vector3(0.0f, 8.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.6f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Picking ray
        ray = new();

        tower = LoadModel("resources/models/obj/turret.obj");
        texture = LoadTexture("resources/models/obj/turret_diffuse.png");
        Raylib.SetMaterialTexture(ref tower, 0, MaterialMapIndex.Albedo, ref texture);

        towerPos = new(0.0f, 0.0f, 0.0f);
        towerBBox = GetMeshBoundingBox(tower.Meshes[0]);

        // Ground quad
        g0 = new(-50.0f, 0.0f, -50.0f);
        g1 = new(-50.0f, 0.0f, 50.0f);
        g2 = new(50.0f, 0.0f, 50.0f);
        g3 = new(50.0f, 0.0f, -50.0f);

        // Test triangle
        ta = new(-25.0f, 0.5f, 0.0f);
        tb = new(-4.0f, 2.5f, 1.0f);
        tc = new(-8.0f, 6.5f, 0.0f);

        bary = new(0.0f, 0.0f, 0.0f);

        // Test sphere
        sp = new(-30.0f, 5.0f, 5.0f);
        sr = 4.0f;
    }

    public unsafe void Update()
    {
        //----------------------------------------------------------------------------------
        // Update
        //----------------------------------------------------------------------------------
        if (IsCursorHidden())
        {
            UpdateCamera(ref camera, CameraMode.FirstPerson);
        }

        // Toggle camera controls
        if (IsMouseButtonPressed(MouseButton.Right))
        {
            if (IsCursorHidden())
            {
                EnableCursor();
            }
            else
            {
                DisableCursor();
            }
        }

        // Display information about closest hit
        RayCollision collision = new();
        var hitObjectName = "None";
        collision.Distance = float.MaxValue;
        collision.Hit = false;
        var cursorColor = Color.White;

        // Get ray and test against objects
        ray = GetScreenToWorldRay(GetMousePosition(), camera);

        // Check ray collision against ground quad
        var groundHitInfo = GetRayCollisionQuad(ray, g0, g1, g2, g3);
        if (groundHitInfo.Hit && (groundHitInfo.Distance < collision.Distance))
        {
            collision = groundHitInfo;
            cursorColor = Color.Green;
            hitObjectName = "Ground";
        }

        // Check ray collision against test triangle
        var triHitInfo = GetRayCollisionTriangle(ray, ta, tb, tc);
        if (triHitInfo.Hit && (triHitInfo.Distance < collision.Distance))
        {
            collision = triHitInfo;
            cursorColor = Color.Purple;
            hitObjectName = "Triangle";

            bary = Vector3Barycenter(collision.Point, ta, tb, tc);
        }

        // Check ray collision against test sphere
        var sphereHitInfo = GetRayCollisionSphere(ray, sp, sr);
        if ((sphereHitInfo.Hit) && (sphereHitInfo.Distance < collision.Distance))
        {
            collision = sphereHitInfo;
            cursorColor = Color.Orange;
            hitObjectName = "Sphere";
        }

        // Check ray collision against bounding box first, before trying the full ray-mesh test
        var boxHitInfo = GetRayCollisionBox(ray, towerBBox);
        if (boxHitInfo.Hit && boxHitInfo.Distance < collision.Distance)
        {
            collision = boxHitInfo;
            cursorColor = Color.Orange;
            hitObjectName = "Box";

            // Check ray collision against model meshes
            RayCollision meshHitInfo = new();
            for (var m = 0; m < tower.MeshCount; m++)
            {
                // NOTE: We consider the model.Transform for the collision check but
                // it can be checked against any transform matrix, used when checking against same
                // model drawn multiple times with multiple transforms
                meshHitInfo = GetRayCollisionMesh(ray, tower.Meshes[m], tower.Transform);
                if (meshHitInfo.Hit)
                {
                    // Save the closest hit mesh
                    if ((!collision.Hit) || (collision.Distance > meshHitInfo.Distance))
                    {
                        collision = meshHitInfo;
                    }
                    break;
                }
            }

            if (meshHitInfo.Hit)
            {
                collision = meshHitInfo;
                cursorColor = Color.Orange;
                hitObjectName = "Mesh";
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw the tower
        // WARNING: If scale is different than 1.0f,
        // not considered by GetRayCollisionModel()
        DrawModel(tower, towerPos, 1.0f, Color.White);

        // Draw the test triangle
        DrawLine3D(ta, tb, Color.Purple);
        DrawLine3D(tb, tc, Color.Purple);
        DrawLine3D(tc, ta, Color.Purple);

        // Draw the test sphere
        DrawSphereWires(sp, sr, 8, 8, Color.Purple);

        // Draw the mesh bbox if we hit it
        if (boxHitInfo.Hit)
        {
            DrawBoundingBox(towerBBox, Color.Lime);
        }

        // If we hit something, draw the cursor at the hit point
        if (collision.Hit)
        {
            DrawCube(collision.Point, 0.3f, 0.3f, 0.3f, cursorColor);
            DrawCubeWires(collision.Point, 0.3f, 0.3f, 0.3f, Color.Red);

            var normalEnd = collision.Point + collision.Normal;
            DrawLine3D(collision.Point, normalEnd, Color.Red);
        }

        DrawRay(ray, Color.Maroon);

        DrawGrid(10, 10.0f);

        EndMode3D();

        // Draw some debug GUI text
        DrawText($"Hit Object: {hitObjectName}", 10, 50, 10, Color.Black);

        if (collision.Hit)
        {
            var ypos = 70;

            DrawText($"Distance: {collision.Distance}", 10, ypos, 10, Color.Black);

            DrawText($"Hit Pos: {collision.Point}", 10, ypos + 15, 10, Color.Black);

            DrawText($"Hit Norm: {collision.Normal}", 10, ypos + 30, 10, Color.Black);

            if (triHitInfo.Hit && hitObjectName == "Triangle")
            {
                DrawText($"Barycenter: {bary}", 10, ypos + 45, 10, Color.Black);
            }
        }

        DrawText("Right click mouse to toggle camera controls", 10, 430, 10, Color.Gray);

        DrawText("(c) Turret 3D model by Alberto Cano", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(tower);
        UnloadTexture(texture);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - mesh picking");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new MeshPicking();
        game.Init();

        //----------------------------------------------------------------------------------
        // Main game loop
        //--------------------------------------------------------------------------------------
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
