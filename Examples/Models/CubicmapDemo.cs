/*******************************************************************************************
*
*   raylib [models] example - cubicmap rendering
*
*   Example complexity rating: [★★☆☆] 2/4
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

public partial class CubicmapDemo : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Cubicmap Demo";

    public string Title => "raylib [models] example - cubicmap rendering";

    private Camera3D camera;
    private Texture2D cubicmap;
    private Texture2D texture;
    private Model model;
    private Vector3 mapPosition;
    private bool pause;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(16.0f, 14.0f, 16.0f);     // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);          // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);              // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                    // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;       // Camera projection type

        var image = LoadImage("resources/cubicmap.png");      // Load cubicmap image (RAM)
        cubicmap = LoadTextureFromImage(image);                 // Convert image to texture to display (VRAM)

        var mesh = GenMeshCubicmap(image, new Vector3(1.0f, 1.0f, 1.0f));
        model = LoadModelFromMesh(mesh);

        // NOTE: By default each cube is mapped to one part of texture atlas
        texture = LoadTexture("resources/cubicmap_atlas.png");  // Load map texture

        // Set map diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        mapPosition = new(-16.0f, 0.0f, -8.0f);                 // Set model position

        UnloadImage(image);     // Unload cubesmap image from RAM, already uploaded to VRAM

        pause = false;          // Pause camera orbital rotation (and zoom)
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.P))
        {
            pause = !pause;
        }

        if (!pause)
        {
            UpdateCamera(ref camera, CameraMode.Orbital);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, mapPosition, 1.0f, Color.White);

        EndMode3D();

        Vector2 position = new(screenWidth - cubicmap.Width * 4 - 20, 20);
        DrawTextureEx(cubicmap, position, 0.0f, 4.0f, Color.White);
        DrawRectangleLines(
            screenWidth - cubicmap.Width * 4 - 20,
            20,
            cubicmap.Width * 4,
            cubicmap.Height * 4,
            Color.Green
        );

        DrawText("cubicmap image used to", 658, 90, 10, Color.Gray);
        DrawText("generate map 3d model", 658, 104, 10, Color.Gray);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(cubicmap);    // Unload cubicmap texture
        UnloadTexture(texture);     // Unload map texture
        UnloadModel(model);         // Unload map model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - cubicmap rendering");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new CubicmapDemo();
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
