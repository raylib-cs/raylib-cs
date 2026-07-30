/*******************************************************************************************
*
*   raylib [models] example - first person maze
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public unsafe partial class FirstPersonMaze : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / First Person Maze";

    public string Title => "raylib [models] example - first person maze";

    public bool CursorDisabled => true;

    private Camera3D camera;
    private Texture2D cubicmap;
    private Texture2D texture;
    private Model model;
    private Color* mapPixels;
    private Vector3 mapPosition;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.2f, 0.4f, 0.2f);    // Camera position
        camera.Target = new Vector3(0.185f, 0.4f, 0.0f);    // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        var imMap = LoadImage("resources/cubicmap.png");      // Load cubicmap image (RAM)
        cubicmap = LoadTextureFromImage(imMap);                 // Convert image to texture to display (VRAM)
        var mesh = GenMeshCubicmap(imMap, new Vector3(1.0f, 1.0f, 1.0f));
        model = LoadModelFromMesh(mesh);

        // NOTE: By default each cube is mapped to one part of texture atlas
        texture = LoadTexture("resources/cubicmap_atlas.png");  // Load map texture

        // Set map diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        // Get map image data to be used for collision detection
        mapPixels = LoadImageColors(imMap);
        UnloadImage(imMap);             // Unload image from RAM

        mapPosition = new(-16.0f, 0.0f, -8.0f);  // Set model position
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var oldCamPos = camera.Position;    // Store old camera position

        UpdateCamera(ref camera, CameraMode.FirstPerson);

        // Check player collision (we simplify to 2D collision detection)
        Vector2 playerPos = new(camera.Position.X, camera.Position.Z);
        var playerRadius = 0.1f;  // Collision radius (player is modelled as a cilinder for collision)

        var playerCellX = (int)(playerPos.X - mapPosition.X + 0.5f);
        var playerCellY = (int)(playerPos.Y - mapPosition.Z + 0.5f);

        // Out-of-limits security check
        if (playerCellX < 0)
        {
            playerCellX = 0;
        }
        else if (playerCellX >= cubicmap.Width)
        {
            playerCellX = cubicmap.Width - 1;
        }

        if (playerCellY < 0)
        {
            playerCellY = 0;
        }
        else if (playerCellY >= cubicmap.Height)
        {
            playerCellY = cubicmap.Height - 1;
        }

        // Check map collisions using image data and player position against surrounding cells only
        for (var y = playerCellY - 1; y <= playerCellY + 1; y++)
        {
            // Avoid map accessing out of bounds
            if ((y >= 0) && (y < cubicmap.Height))
            {
                for (var x = playerCellX - 1; x <= playerCellX + 1; x++)
                {
                    // NOTE: Collision: Only checking R channel for white pixel
                    if (((x >= 0) && (x < cubicmap.Width)) &&
                        (mapPixels[y * cubicmap.Width + x].R == 255) &&
                        (CheckCollisionCircleRec(playerPos, playerRadius,
                        new Rectangle(mapPosition.X - 0.5f + x * 1.0f, mapPosition.Z - 0.5f + y * 1.0f, 1.0f, 1.0f))))
                    {
                        // Collision detected, reset camera position
                        camera.Position = oldCamPos;
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        DrawModel(model, mapPosition, 1.0f, Color.White);                     // Draw maze map
        EndMode3D();

        DrawTextureEx(cubicmap, new Vector2(GetScreenWidth() - cubicmap.Width * 4 - 20, 20), 0.0f, 4.0f, Color.White);
        DrawRectangleLines(GetScreenWidth() - cubicmap.Width * 4 - 20, 20, cubicmap.Width * 4, cubicmap.Height * 4, Color.Green);

        // Draw player position radar
        DrawRectangle(GetScreenWidth() - cubicmap.Width * 4 - 20 + playerCellX * 4, 20 + playerCellY * 4, 4, 4, Color.Red);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadImageColors(mapPixels);   // Unload color array

        UnloadTexture(cubicmap);        // Unload cubicmap texture
        UnloadTexture(texture);         // Unload map texture
        UnloadModel(model);             // Unload map model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - first person maze");

        DisableCursor();                // Limit cursor to relative movement inside the window

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new FirstPersonMaze();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                  // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
