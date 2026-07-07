/*******************************************************************************************
*
*   raylib [models] example - rotating cube
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Jopestpe (@jopestpe)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jopestpe (@jopestpe)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public partial class RotatingCube : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Rotating Cube";

    public string Title => "raylib [models] example - rotating cube";

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private float rotation;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.0f, 3.0f, 3.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load image to create texture for the cube
        model = LoadModelFromMesh(GenMeshCube(1.0f, 1.0f, 1.0f));
        var img = LoadImage("resources/cubicmap_atlas.png");
        var crop = ImageFromImage(img, new Rectangle(0, img.Height / 2.0f, img.Width / 2.0f, img.Height / 2.0f));
        texture = LoadTextureFromImage(crop);
        UnloadImage(img);
        UnloadImage(crop);

        model.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = texture;

        rotation = 0.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        rotation += 1.0f;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw model defining: position, size, rotation-axis, rotation (degrees), size, and tint-color
        DrawModelEx(model, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.5f, 1.0f, 0.0f),
            rotation, new Vector3(1.0f, 1.0f, 1.0f), Color.White);

        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture); // Unload texture
        UnloadModel(model);     // Unload model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - rotating cube");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new RotatingCube();
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
