/*******************************************************************************************
*
*   raylib [core] example - 3d camera mode
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.0, last time updated with raylib 1.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2014-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class Camera3dMode : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private Camera3D camera;
    private Vector3 cubePosition;

    public string Name => "Core / Camera 3D Mode";

    public string Title => "raylib [core] example - 3d camera mode";

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.0f, 10.0f, 10.0f);  // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera mode type

        cubePosition = new(0.0f, 0.0f, 0.0f);
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawCube(cubePosition, 2.0f, 2.0f, 2.0f, Color.Red);
        DrawCubeWires(cubePosition, 2.0f, 2.0f, 2.0f, Color.Maroon);

        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawText("Welcome to the third dimension!", 10, 40, 20, Color.DarkGray);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d camera mode");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Camera3dMode();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
