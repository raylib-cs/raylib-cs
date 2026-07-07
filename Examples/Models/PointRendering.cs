/*******************************************************************************************
*
*   raylib [models] example - point rendering
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.0
*
*   Example contributed by Reese Gallagher (@satchelfrost) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2025 Reese Gallagher (@satchelfrost)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

[ExcludeFromBrowser("rlEnablePointMode needs glPolygonMode, which OpenGL ES/WebGL lacks (renders as triangles)")]
public partial class PointRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxPoints = 10000000;     // 10 million
    private const int MinPoints = 1000;         // 1 thousand

    public string Name => "Models / Point Rendering";

    public string Title => "raylib [models] example - point rendering";

    private static readonly Random Rand = new();

    private Camera3D camera;
    private Vector3 position;
    private bool useDrawModelPoints;
    private bool numPointsChanged;
    private int numPoints;
    private Mesh mesh;
    private Model model;

    public void Init()
    {
        camera = new()
        {
            Position = new Vector3(3.0f, 3.0f, 3.0f),
            Target = new Vector3(0.0f, 0.0f, 0.0f),
            Up = new Vector3(0.0f, 1.0f, 0.0f),
            FovY = 45.0f,
            Projection = CameraProjection.Perspective
        };

        position = new Vector3(0.0f, 0.0f, 0.0f);
        useDrawModelPoints = true;
        numPointsChanged = false;
        numPoints = 1000;

        mesh = GenMeshPoints(numPoints);
        model = LoadModelFromMesh(mesh);
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        if (IsKeyPressed(KeyboardKey.Space))
        {
            useDrawModelPoints = !useDrawModelPoints;
        }
        if (IsKeyPressed(KeyboardKey.Up))
        {
            numPoints = (numPoints * 10 > MaxPoints) ? MaxPoints : numPoints * 10;
            numPointsChanged = true;
        }
        if (IsKeyPressed(KeyboardKey.Down))
        {
            numPoints = (numPoints / 10 < MinPoints) ? MinPoints : numPoints / 10;
            numPointsChanged = true;
        }

        // Upload a different point cloud size
        if (numPointsChanged)
        {
            UnloadModel(model);
            mesh = GenMeshPoints(numPoints);
            model = LoadModelFromMesh(mesh);
            numPointsChanged = false;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.Black);

        BeginMode3D(camera);

        // The new method only uploads the points once to the GPU
        if (useDrawModelPoints)
        {
            DrawModelPoints(model, position, 1.0f, Color.White);
        }
        else
        {
            // The old method must continually draw the "points" (lines)
            for (var i = 0; i < numPoints; i++)
            {
                Vector3 pos = new(
                    mesh.Vertices[i * 3 + 0],
                    mesh.Vertices[i * 3 + 1],
                    mesh.Vertices[i * 3 + 2]
                );
                Color color = new(
                    mesh.Colors[i * 4 + 0],
                    mesh.Colors[i * 4 + 1],
                    mesh.Colors[i * 4 + 2],
                    mesh.Colors[i * 4 + 3]
                );

                DrawPoint3D(pos, color);
            }
        }

        // Draw a unit sphere for reference
        DrawSphereWires(position, 1.0f, 10, 10, Color.Yellow);

        EndMode3D();

        // Draw UI text
        DrawText($"Point Count: {numPoints}", 10, screenHeight - 50, 40, Color.White);
        DrawText("UP - Increase points", 10, 40, 20, Color.White);
        DrawText("DOWN - Decrease points", 10, 70, 20, Color.White);
        DrawText("SPACE - Drawing function", 10, 100, 20, Color.White);

        if (useDrawModelPoints)
        {
            DrawText("Using: DrawModelPoints()", 10, 130, 20, Color.Green);
        }
        else
        {
            DrawText("Using: DrawPoint3D()", 10, 130, 20, Color.Red);
        }

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(model);
    }

    // Generate a spherical point cloud
    private static unsafe Mesh GenMeshPoints(int numPoints)
    {
        Mesh mesh = new(numPoints, 1);
        mesh.AllocVertices();
        mesh.AllocColors();

        // REF: https://en.wikipedia.org/wiki/Spherical_coordinate_system
        for (var i = 0; i < numPoints; i++)
        {
            var theta = MathF.PI * (float)Rand.NextDouble();
            var phi = 2.0f * MathF.PI * (float)Rand.NextDouble();
            var r = 10.0f * (float)Rand.NextDouble();

            mesh.Vertices[i * 3 + 0] = r * MathF.Sin(theta) * MathF.Cos(phi);
            mesh.Vertices[i * 3 + 1] = r * MathF.Sin(theta) * MathF.Sin(phi);
            mesh.Vertices[i * 3 + 2] = r * MathF.Cos(theta);

            var color = ColorFromHSV(r * 360.0f, 1.0f, 1.0f);

            mesh.Colors[i * 4 + 0] = color.R;
            mesh.Colors[i * 4 + 1] = color.G;
            mesh.Colors[i * 4 + 2] = color.B;
            mesh.Colors[i * 4 + 3] = color.A;
        }

        // Upload mesh data from CPU (RAM) to GPU (VRAM) memory
        UploadMesh(ref mesh, false);

        return mesh;
    }

    // Draw a model points
    // WARNING: OpenGL ES 2.0 does not support point mode drawing
    private static void DrawModelPoints(Model model, Vector3 position, float scale, Color tint)
    {
        Rlgl.EnablePointMode();
        Rlgl.DisableBackfaceCulling();

        DrawModel(model, position, scale, tint);

        Rlgl.EnableBackfaceCulling();
        Rlgl.DisablePointMode();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - point rendering");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new PointRendering();
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
