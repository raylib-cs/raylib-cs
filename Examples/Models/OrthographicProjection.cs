/*******************************************************************************************
*
*   raylib [models] example - orthographic projection
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 2.0, last time updated with raylib 3.7
*
*   Example contributed by Max Danielsson (@autious) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Max Danielsson (@autious) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public partial class OrthographicProjection : IExample
{
    public const float FOVY_PERSPECTIVE = 45.0f;
    public const float WIDTH_ORTHOGRAPHIC = 10.0f;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Orthographic Projection";

    public string Title => "raylib [models] example - orthographic projection";

    private Camera3D camera;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.0f, 10.0f, 10.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = FOVY_PERSPECTIVE;
        camera.Projection = CameraProjection.Perspective;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space))
        {
            if (camera.Projection == CameraProjection.Perspective)
            {
                camera.FovY = WIDTH_ORTHOGRAPHIC;
                camera.Projection = CameraProjection.Orthographic;
            }
            else
            {
                camera.FovY = FOVY_PERSPECTIVE;
                camera.Projection = CameraProjection.Perspective;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawCube(new Vector3(-4.0f, 0.0f, 2.0f), 2.0f, 5.0f, 2.0f, Color.Red);
        DrawCubeWires(new Vector3(-4.0f, 0.0f, 2.0f), 2.0f, 5.0f, 2.0f, Color.Gold);
        DrawCubeWires(new Vector3(-4.0f, 0.0f, -2.0f), 3.0f, 6.0f, 2.0f, Color.Maroon);

        DrawSphere(new Vector3(-1.0f, 0.0f, -2.0f), 1.0f, Color.Green);
        DrawSphereWires(new Vector3(1.0f, 0.0f, 2.0f), 2.0f, 16, 16, Color.Lime);

        DrawCylinder(new Vector3(4.0f, 0.0f, -2.0f), 1.0f, 2.0f, 3.0f, 4, Color.SkyBlue);
        DrawCylinderWires(new Vector3(4.0f, 0.0f, -2.0f), 1.0f, 2.0f, 3.0f, 4, Color.DarkBlue);
        DrawCylinderWires(new Vector3(4.5f, -1.0f, 2.0f), 1.0f, 1.0f, 2.0f, 6, Color.Brown);

        DrawCylinder(new Vector3(1.0f, 0.0f, -4.0f), 0.0f, 1.5f, 3.0f, 8, Color.Gold);
        DrawCylinderWires(new Vector3(1.0f, 0.0f, -4.0f), 0.0f, 1.5f, 3.0f, 8, Color.Pink);

        DrawGrid(10, 1.0f);        // Draw a grid

        EndMode3D();

        DrawText("Press Spacebar to switch camera type", 10, GetScreenHeight() - 30, 20, Color.DarkGray);

        if (camera.Projection == CameraProjection.Orthographic)
        {
            DrawText("ORTHOGRAPHIC", 10, 40, 20, Color.Black);
        }
        else if (camera.Projection == CameraProjection.Perspective)
        {
            DrawText("PERSPECTIVE", 10, 40, 20, Color.Black);
        }

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
        InitWindow(screenWidth, screenHeight, "raylib [models] example - orthographic projection");

        SetTargetFPS(60);   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new OrthographicProjection();
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
