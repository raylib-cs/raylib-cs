/*******************************************************************************************
*
*   raylib [models] example - loading iqm
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.5
*
*   Example contributed by Culacant (@culacant) and reviewed by Ramon Santamaria (@raysan5)
*
*   NOTES: To export an IQM model from blender, make sure it is not posed, the vertices need
*   to be in the same position as they would be in edit mode and the scale of the models is
*   set to 0; scaling can be set from the export menu
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Culacant (@culacant) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public partial class LoadingIqm : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Loading IQM";

    public string Title => "raylib [models] example - loading iqm";

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private Vector3 position;
    private unsafe ModelAnimation* anims;
    private int animCount;
    private int animIndex;
    private float animCurrentFrame;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(10.0f, 10.0f, 10.0f); // Camera position
        camera.Target = new Vector3(0.0f, 4.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera mode type

        model = LoadModel("resources/models/iqm/guy.iqm");                    // Load the animated model mesh and basic data
        texture = LoadTexture("resources/models/iqm/guytex.png");             // Load model texture and set material
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Diffuse, ref texture);     // Set model material map texture
        position = new(0.0f, 0.0f, 0.0f); // Set model position

        // Load animation data
        anims = LoadModelAnimations("resources/models/iqm/guyanim.iqm", ref animCount);

        // Animation playing variables
        animIndex = 0;          // Current animation playing
        animCurrentFrame = 0.0f;      // Current animation frame (supporting interpolated frames)
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Play animation when spacebar is held down
        animCurrentFrame += 1.0f;
        UpdateModelAnimation(model, anims[0], animCurrentFrame);

        if (animCurrentFrame >= anims[0].KeyFrameCount)
        {
            animCurrentFrame = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModelEx(
            model,
            position,
            new Vector3(1.0f, 0.0f, 0.0f),
            -90.0f,
            new Vector3(1.0f, 1.0f, 1.0f),
            Color.White
        );

        DrawGrid(10, 1.0f);

        EndMode3D();
        DrawText($"Current animation: {anims[animIndex].NameToString()}", 10, 10, 20, Color.Maroon);
        DrawText("(c) Guy IQM 3D model by @culacant", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadTexture(texture);                    // Unload texture
        UnloadModelAnimations(anims, animCount);   // Unload model animations data
        UnloadModel(model);                        // Unload model
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - loading iqm");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LoadingIqm();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
