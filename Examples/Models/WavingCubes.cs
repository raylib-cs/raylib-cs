/*******************************************************************************************
*
*   raylib [models] example - waving cubes
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.7
*
*   Example contributed by Codecat (@codecat) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Codecat (@codecat) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public partial class WavingCubes : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Specify the amount of blocks in each direction
    private const int numBlocks = 15;

    public string Name => "Models / Waving Cubes";

    public string Title => "raylib [models] example - waving cubes";

    private Camera3D camera;

    public void Init()
    {
        // Initialize the camera
        camera = new();
        camera.Position = new Vector3(30.0f, 20.0f, 30.0f); // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 70.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var time = GetTime();

        // Calculate time scale for cube position and size
        var scale = (2.0f + (float)Math.Sin(time)) * 0.7f;

        // Move camera around the scene
        var cameraTime = time * 0.3;
        camera.Position.X = (float)Math.Cos(cameraTime) * 40.0f;
        camera.Position.Z = (float)Math.Sin(cameraTime) * 40.0f;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawGrid(10, 5.0f);

        for (var x = 0; x < numBlocks; x++)
        {
            for (var y = 0; y < numBlocks; y++)
            {
                for (var z = 0; z < numBlocks; z++)
                {
                    // Scale of the blocks depends on x/y/z positions
                    var blockScale = (x + y + z) / 30.0f;

                    // Scatter makes the waving effect by adding blockScale over time
                    var scatter = (float)Math.Sin(blockScale * 20.0f + (float)(time * 4.0f));

                    // Calculate the cube position
                    Vector3 cubePos = new(
                        (float)(x - numBlocks / 2) * (scale * 3.0f) + scatter,
                        (float)(y - numBlocks / 2) * (scale * 2.0f) + scatter,
                        (float)(z - numBlocks / 2) * (scale * 3.0f) + scatter
                    );

                    // Pick a color with a hue depending on cube position for the rainbow color effect
                    // NOTE: This function is quite costly to be done per cube and frame,
                    // pre-catching the results into a separate array could improve performance
                    var cubeColor = ColorFromHSV((float)(((x + y + z) * 18) % 360), 0.75f, 0.9f);

                    // Calculate cube size
                    var cubeSize = (2.4f - scale) * blockScale;

                    // And finally, draw the cube!
                    DrawCube(cubePos, cubeSize, cubeSize, cubeSize, cubeColor);
                }
            }
        }

        EndMode3D();

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
        InitWindow(screenWidth, screenHeight, "raylib [models] example - waving cubes");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new WavingCubes();
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
