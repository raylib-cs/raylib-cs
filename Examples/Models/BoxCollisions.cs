/*******************************************************************************************
*
*   raylib [models] example - box collisions
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.3, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public partial class BoxCollisions : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Box Collisions";

    public string Title => "raylib [models] example - box collisions";

    private Camera3D camera;

    private Vector3 playerPosition;
    private Vector3 playerSize;
    private Color playerColor;

    private Vector3 enemyBoxPos;
    private Vector3 enemyBoxSize;

    private Vector3 enemySpherePos;
    private float enemySphereSize;

    private bool collision;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.0f, 10.0f, 10.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        playerPosition = new(0.0f, 1.0f, 2.0f);
        playerSize = new(1.0f, 2.0f, 1.0f);
        playerColor = Color.Green;

        enemyBoxPos = new(-4.0f, 1.0f, 0.0f);
        enemyBoxSize = new(2.0f, 2.0f, 2.0f);

        enemySpherePos = new(4.0f, 0.0f, 0.0f);
        enemySphereSize = 1.5f;

        collision = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------

        // Move player
        if (IsKeyDown(KeyboardKey.Right))
        {
            playerPosition.X += 0.2f;
        }
        else if (IsKeyDown(KeyboardKey.Left))
        {
            playerPosition.X -= 0.2f;
        }
        else if (IsKeyDown(KeyboardKey.Down))
        {
            playerPosition.Z += 0.2f;
        }
        else if (IsKeyDown(KeyboardKey.Up))
        {
            playerPosition.Z -= 0.2f;
        }

        collision = false;

        // Check collisions player vs enemy-box
        BoundingBox box1 = new(
            playerPosition - (playerSize / 2),
            playerPosition + (playerSize / 2)
        );
        BoundingBox box2 = new(
            enemyBoxPos - (enemyBoxSize / 2),
            enemyBoxPos + (enemyBoxSize / 2)
        );

        if (CheckCollisionBoxes(box1, box2))
        {
            collision = true;
        }

        // Check collisions player vs enemy-sphere
        if (CheckCollisionBoxSphere(box1, enemySpherePos, enemySphereSize))
        {
            collision = true;
        }

        if (collision)
        {
            playerColor = Color.Red;
        }
        else
        {
            playerColor = Color.Green;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw enemy-box
        DrawCube(enemyBoxPos, enemyBoxSize.X, enemyBoxSize.Y, enemyBoxSize.Z, Color.Gray);
        DrawCubeWires(enemyBoxPos, enemyBoxSize.X, enemyBoxSize.Y, enemyBoxSize.Z, Color.DarkGray);

        // Draw enemy-sphere
        DrawSphere(enemySpherePos, enemySphereSize, Color.Gray);
        DrawSphereWires(enemySpherePos, enemySphereSize, 16, 16, Color.DarkGray);

        // Draw player
        DrawCubeV(playerPosition, playerSize, playerColor);

        DrawGrid(10, 1.0f);        // Draw a grid

        EndMode3D();

        DrawText("Move player with arrow keys to collide", 220, 40, 20, Color.Gray);

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
        InitWindow(screenWidth, screenHeight, "raylib [models] example - box collisions");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new BoxCollisions();
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
