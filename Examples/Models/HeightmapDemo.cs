/*******************************************************************************************
*
*   raylib [models] example - heightmap rendering
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.8, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public partial class HeightmapDemo : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Heightmap Demo";

    public string Title => "raylib [models] example - heightmap rendering";

    private Camera3D camera;
    private Texture2D texture;
    private Model model;
    private Vector3 mapPosition;

    public void Init()
    {
        // Define our custom camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(18.0f, 21.0f, 18.0f);     // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);          // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);              // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                    // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;       // Camera projection type

        var image = LoadImage("resources/heightmap.png");     // Load heightmap image (RAM)
        texture = LoadTextureFromImage(image);                  // Convert image to texture (VRAM)

        var mesh = GenMeshHeightmap(image, new Vector3(16, 8, 16)); // Generate heightmap mesh (RAM and VRAM)
        model = LoadModelFromMesh(mesh);                        // Load model from generated mesh

        // Set map diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        mapPosition = new(-8.0f, 0.0f, -8.0f);                  // Define model position

        UnloadImage(image);             // Unload heightmap image from RAM, already uploaded to VRAM
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, mapPosition, 1.0f, Color.Red);

        DrawGrid(20, 1.0f);

        EndMode3D();

        DrawTexture(texture, screenWidth - texture.Width - 20, 20, Color.White);
        DrawRectangleLines(screenWidth - texture.Width - 20, 20, texture.Width, texture.Height, Color.Green);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);     // Unload texture
        UnloadModel(model);         // Unload model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - heightmap rendering");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new HeightmapDemo();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
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
