/*******************************************************************************************
*
*   raylib [core] example - 3d picking
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class Picking3d : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Picking 3D";

    public string Title => "raylib [core] example - 3d picking";

    private Camera3D camera;
    private Vector3 cubePosition;
    private Vector3 cubeSize;
    private Ray ray;                    // Picking line ray
    private RayCollision collision;     // Ray collision hit info

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new Camera3D();
        camera.Position = new Vector3(10.0f, 10.0f, 10.0f); // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        cubePosition = new(0.0f, 1.0f, 0.0f);
        cubeSize = new(2.0f, 2.0f, 2.0f);

        ray = new();                    // Picking line ray
        collision = new();              // Ray collision hit info
    }

    public void Update()
    {
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

        if (IsMouseButtonPressed(MouseButton.Left))
        {
            if (!collision.Hit)
            {
                ray = GetScreenToWorldRay(GetMousePosition(), camera);

                // Check collision between ray and box
                BoundingBox box = new(
                    cubePosition - cubeSize / 2,
                    cubePosition + cubeSize / 2
                );
                collision = GetRayCollisionBox(ray, box);
            }
            else
            {
                collision.Hit = false;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        if (collision.Hit)
        {
            DrawCube(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, Color.Red);
            DrawCubeWires(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, Color.Maroon);

            DrawCubeWires(cubePosition, cubeSize.X + 0.2f, cubeSize.Y + 0.2f, cubeSize.Z + 0.2f, Color.Green);
        }
        else
        {
            DrawCube(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, Color.Gray);
            DrawCubeWires(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, Color.DarkGray);
        }

        DrawRay(ray, Color.Maroon);
        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawText("Try clicking on the box with your mouse!", 240, 10, 20, Color.DarkGray);

        if (collision.Hit)
        {
            var posX = (screenWidth - MeasureText("BOX SELECTED", 30)) / 2;
            DrawText("BOX SELECTED", posX, (int)(screenHeight * 0.1f), 30, Color.Green);
        }

        DrawText("Right click mouse to toggle camera controls", 10, 430, 10, Color.Gray);

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
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d picking");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Picking3d();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
