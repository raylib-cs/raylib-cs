/*******************************************************************************************
*
*   raylib [shaders] example - custom uniform
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3), to test this example
*         on OpenGL ES 2.0 platforms (Android, Raspberry Pi, HTML5), use #version 100 shaders
*         raylib comes with shaders ready for both versions, check raylib/shaders install folder
*
*   Example originally created with raylib 1.3, last time updated with raylib 4.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class CustomUniform : IExample
{
#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shaders / Custom Uniform";

    public string Title => "raylib [shaders] example - custom uniform";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private Vector3 position;
    private Shader shader;
    private int swirlCenterLoc;
    private float[] swirlCenter;
    private RenderTexture2D target;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(8.0f, 8.0f, 8.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 1.5f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        model = LoadModel("resources/models/obj/barracks.obj");                   // Load OBJ model
        texture = LoadTexture("resources/models/obj/barracks_diffuse.png");   // Load model texture (diffuse map)

        // Set model diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        position = new(0.0f, 0.0f, 0.0f);                                    // Set model position

        // Load postprocessing shader
        // NOTE: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/swirl.fs");

        // Get variable (uniform) location on the shader to connect with the program
        // NOTE: If uniform variable could not be found in the shader, function returns -1
        swirlCenterLoc = GetShaderLocation(shader, "center");

        swirlCenter = new float[2] { (float)screenWidth / 2, (float)screenHeight / 2 };

        // Create a RenderTexture2D to be used for render to texture
        target = LoadRenderTexture(screenWidth, screenHeight);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        var mousePosition = GetMousePosition();

        swirlCenter[0] = mousePosition.X;
        swirlCenter[1] = screenHeight - mousePosition.Y;

        // Send new value to the shader to be used on drawing
        Raylib.SetShaderValue(shader, swirlCenterLoc, swirlCenter, ShaderUniformDataType.Vec2);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginTextureMode(target);       // Enable drawing to texture
        ClearBackground(Color.RayWhite);  // Clear texture background

        BeginMode3D(camera);        // Begin 3d mode drawing
        DrawModel(model, position, 0.5f, Color.White);   // Draw 3d model with texture
        DrawGrid(10, 1.0f);     // Draw a grid
        EndMode3D();                // End 3d mode drawing, returns to orthographic 2d mode

        DrawText("TEXT DRAWN IN RENDER TEXTURE", 200, 10, 30, Color.Red);
        EndTextureMode();               // End drawing to texture (now we have a texture available for next passes)

        BeginDrawing();
        ClearBackground(Color.RayWhite);  // Clear screen background

        // Enable shader using the custom uniform
        BeginShaderMode(shader);

        // NOTE: Render texture must be y-flipped due to default OpenGL coordinates (left-bottom)
        DrawTextureRec(
            target.Texture,
            new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
            new Vector2(0, 0),
            Color.White
        );

        EndShaderMode();

        // Draw some 2d text over drawn texture
        DrawText(
            "(c) Barracks 3D model by Alberto Cano",
            screenWidth - 220,
            screenHeight - 20,
            10,
            Color.Gray
        );

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);               // Unload shader
        UnloadTexture(texture);             // Unload texture
        UnloadModel(model);                 // Unload model
        UnloadRenderTexture(target);        // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);      // Enable Multi Sampling Anti Aliasing 4x (if available)

        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - custom uniform");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new CustomUniform();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                      // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
