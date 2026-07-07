/*******************************************************************************************
*
*   raylib [models] example - mesh generation
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.8, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2017-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public partial class MeshGeneration : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Mesh Generation";

    public string Title => "raylib [models] example - mesh generation";

    private Texture2D texture;
    private Model[] models;
    private Camera3D camera;
    private Vector3 position;
    private int currentModel;

    public void Init()
    {
        // We generate a checked image for texturing
        var isChecked = GenImageChecked(2, 2, 1, 1, Color.Red, Color.Green);
        texture = LoadTextureFromImage(isChecked);
        UnloadImage(isChecked);

        models = new Model[9];

        models[0] = LoadModelFromMesh(GenMeshPlane(2, 2, 4, 3));
        models[1] = LoadModelFromMesh(GenMeshCube(2.0f, 1.0f, 2.0f));
        models[2] = LoadModelFromMesh(GenMeshSphere(2, 32, 32));
        models[3] = LoadModelFromMesh(GenMeshHemiSphere(2, 16, 16));
        models[4] = LoadModelFromMesh(GenMeshCylinder(1, 2, 16));
        models[5] = LoadModelFromMesh(GenMeshTorus(0.25f, 4.0f, 16, 32));
        models[6] = LoadModelFromMesh(GenMeshKnot(1.0f, 2.0f, 16, 128));
        models[7] = LoadModelFromMesh(GenMeshPoly(5, 2.0f));
        models[8] = LoadModelFromMesh(GenMeshCustom());

        // NOTE: Generated meshes could be exported using ExportMesh()

        // Set checked texture as default diffuse component for all models material
        for (var i = 0; i < models.Length; i++)
        {
            // Set map diffuse texture
            Raylib.SetMaterialTexture(ref models[i], 0, MaterialMapIndex.Albedo, ref texture);
        }

        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(5.0f, 5.0f, 5.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Model drawing position
        position = new(0.0f, 0.0f, 0.0f);

        currentModel = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        if (IsMouseButtonPressed(MouseButton.Left))
        {
            currentModel = (currentModel + 1) % models.Length; // Cycle between the textures
        }

        if (IsKeyPressed(KeyboardKey.Right))
        {
            currentModel++;
            if (currentModel >= models.Length)
            {
                currentModel = 0;
            }
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            currentModel--;
            if (currentModel < 0)
            {
                currentModel = models.Length - 1;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(models[currentModel], position, 1.0f, Color.White);
        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawRectangle(30, 400, 310, 30, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(30, 400, 310, 30, Fade(Color.DarkBlue, 0.5f));
        DrawText("MOUSE LEFT BUTTON to CYCLE PROCEDURAL MODELS", 40, 410, 10, Color.Blue);

        switch (currentModel)
        {
            case 0:
                DrawText("PLANE", 680, 10, 20, Color.DarkBlue);
                break;
            case 1:
                DrawText("CUBE", 680, 10, 20, Color.DarkBlue);
                break;
            case 2:
                DrawText("SPHERE", 680, 10, 20, Color.DarkBlue);
                break;
            case 3:
                DrawText("HEMISPHERE", 640, 10, 20, Color.DarkBlue);
                break;
            case 4:
                DrawText("CYLINDER", 680, 10, 20, Color.DarkBlue);
                break;
            case 5:
                DrawText("TORUS", 680, 10, 20, Color.DarkBlue);
                break;
            case 6:
                DrawText("KNOT", 680, 10, 20, Color.DarkBlue);
                break;
            case 7:
                DrawText("POLY", 680, 10, 20, Color.DarkBlue);
                break;
            case 8:
                DrawText("Custom (triangle)", 580, 10, 20, Color.DarkBlue);
                break;
            default:
                break;
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture); // Unload texture

        // Unload models data (GPU VRAM)
        for (var i = 0; i < models.Length; i++)
        {
            UnloadModel(models[i]);
        }
    }

    // Generate a simple triangle mesh from code
    private static Mesh GenMeshCustom()
    {
        Mesh mesh = new(3, 1);
        mesh.AllocVertices();
        mesh.AllocTexCoords();
        mesh.AllocNormals();
        var vertices = mesh.VerticesAs<Vector3>();
        var texcoords = mesh.TexCoordsAs<Vector2>();
        var normals = mesh.NormalsAs<Vector3>();

        // Vertex at (0, 0, 0)
        vertices[0] = new(0, 0, 0);
        normals[0] = new(0, 1, 0);
        texcoords[0] = new(0, 0);

        // Vertex at (1, 0, 2)
        vertices[1] = new(1, 0, 2);
        normals[1] = new(0, 1, 0);
        texcoords[1] = new(0.5f, 1.0f);

        // Vertex at (2, 0, 0)
        vertices[2] = new(2, 0, 0);
        normals[2] = new(0, 1, 0);
        texcoords[2] = new(1, 0);

        // Upload mesh data from CPU (RAM) to GPU (VRAM) memory
        UploadMesh(ref mesh, false);

        return mesh;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - mesh generation");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new MeshGeneration();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();          // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
