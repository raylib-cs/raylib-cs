/*******************************************************************************************
*
*   raylib [models] example - tesseract view
*
*   NOTE: This example only works on platforms that support drag & drop (Windows, Linux, OSX, Html5?)
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Timothy van der Valk (@arceryz) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2025 Timothy van der Valk (@arceryz) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Models;

public partial class TesseractView : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Tesseract View";

    public string Title => "raylib [models] example - tesseract view";

    // Define the camera to look into our 3d world
    private Camera3D camera;

    // Find the coordinates by setting XYZW to +-1
    private Vector4[] tesseract;

    private float rotation;
    private Vector3[] transformed;
    private float[] wValues;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new Camera3D();
        camera.Position = new Vector3(4.0f, 4.0f, 4.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 0.0f, 1.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 50.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera mode type

        // Find the coordinates by setting XYZW to +-1
        tesseract = new Vector4[16]
        {
            new( 1,  1,  1, 1 ), new( 1,  1,  1, -1 ),
            new( 1,  1, -1, 1 ), new( 1,  1, -1, -1 ),
            new( 1, -1,  1, 1 ), new( 1, -1,  1, -1 ),
            new( 1, -1, -1, 1 ), new( 1, -1, -1, -1 ),
            new(-1,  1,  1, 1 ), new(-1,  1,  1, -1 ),
            new(-1,  1, -1, 1 ), new(-1,  1, -1, -1 ),
            new(-1, -1,  1, 1 ), new(-1, -1,  1, -1 ),
            new(-1, -1, -1, 1 ), new(-1, -1, -1, -1 ),
        };

        rotation = 0.0f;
        transformed = new Vector3[16];
        wValues = new float[16];
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        rotation = DEG2RAD * 45.0f * (float)GetTime();

        for (int i = 0; i < 16; i++)
        {
            Vector4 p = tesseract[i];

            // Rotate the XW part of the vector
            Vector2 rotXW = Vector2Rotate(new Vector2(p.X, p.W), rotation);
            p.X = rotXW.X;
            p.W = rotXW.Y;

            // Projection from XYZW to XYZ from perspective point (0, 0, 0, 3)
            // NOTE: Trace a ray from (0, 0, 0, 3) > p and continue until W = 0
            float c = 3.0f / (3.0f - p.W);
            p.X = c * p.X;
            p.Y = c * p.Y;
            p.Z = c * p.Z;

            // Split XYZ coordinate and W values later for drawing
            transformed[i] = new Vector3(p.X, p.Y, p.Z);
            wValues[i] = p.W;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        for (int i = 0; i < 16; i++)
        {
            // Draw spheres to indicate the W value
            DrawSphere(transformed[i], MathF.Abs(wValues[i] * 0.1f), Color.Red);

            for (int j = 0; j < 16; j++)
            {
                // Two lines are connected if they differ by 1 coordinate
                // This way we dont have to keep an edge list
                Vector4 v1 = tesseract[i];
                Vector4 v2 = tesseract[j];
                int diff = (v1.X == v2.X ? 1 : 0) + (v1.Y == v2.Y ? 1 : 0) + (v1.Z == v2.Z ? 1 : 0) + (v1.W == v2.W ? 1 : 0);

                // Draw only differing by 1 coordinate and the lower index only (duplicate lines)
                if (diff == 3 && i < j)
                {
                    DrawLine3D(transformed[i], transformed[j], Color.Maroon);
                }
            }
        }
        EndMode3D();

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
        InitWindow(screenWidth, screenHeight, "raylib [models] example - tesseract view");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new TesseractView();
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
