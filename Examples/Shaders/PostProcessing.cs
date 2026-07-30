/*******************************************************************************************
*
*   raylib [shaders] example - postprocessing
*
*   Example complexity rating: [★★★☆] 3/4
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

namespace Examples.Shaders;

public class PostProcessing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Post Processing";

    public string Title => "raylib [shaders] example - postprocessing";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private enum PostproShader
    {
        FxGrayScale = 0,
        FxPosterization,
        FxDreamVision,
        FxPixelizer,
        FxCrossHatching,
        FxCrossStiching,
        FxPredatorView,
        FxScanLines,
        FxFishEye,
        FxSobel,
        FxBloom,
        FxBlur,
        //FX_FXAA
        Max
    }

    private string[] postproShaderText = new string[] {
            "GRAYSCALE",
            "POSTERIZATION",
            "DREAM_VISION",
            "PIXELIZER",
            "CROSS_HATCHING",
            "CROSS_STITCHING",
            "PREDATOR_VIEW",
            "SCANLINES",
            "FISHEYE",
            "SOBEL",
            "BLOOM",
            "BLUR",
            //"FXAA"
        };

    private Camera3D camera;
    private Model model;
    private Texture2D texture;
    private Vector3 position;
    private Shader[] shaders;
    private int currentShader;
    private RenderTexture2D target;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(2.0f, 3.0f, 2.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 1.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        model = LoadModel("resources/models/church.obj");                 // Load OBJ model
        texture = LoadTexture("resources/models/church_diffuse.png"); // Load model texture (diffuse map)

        // Set model diffuse texture
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        position = new(0.0f, 0.0f, 0.0f);           // Set model position

        // Load all postpro shaders
        // NOTE 1: All postpro shader use the base vertex shader (DEFAULT_VERTEX_SHADER)
        // NOTE 2: We load the correct shader depending on GLSL version
        shaders = new Shader[(int)PostproShader.Max];

        // NOTE: Defining null (NULL) for vertex shader forces usage of internal default vertex shader
        var shaderPath = $"resources/shaders/glsl{GlslVersion}";
        shaders[(int)PostproShader.FxGrayScale] = LoadShader(null, $"{shaderPath}/grayscale.fs");
        shaders[(int)PostproShader.FxPosterization] = LoadShader(null, $"{shaderPath}/posterization.fs");
        shaders[(int)PostproShader.FxDreamVision] = LoadShader(null, $"{shaderPath}/dream_vision.fs");
        shaders[(int)PostproShader.FxPixelizer] = LoadShader(null, $"{shaderPath}/pixelizer.fs");
        shaders[(int)PostproShader.FxCrossHatching] = LoadShader(null, $"{shaderPath}/cross_hatching.fs");
        shaders[(int)PostproShader.FxCrossStiching] = LoadShader(null, $"{shaderPath}/cross_stitching.fs");
        shaders[(int)PostproShader.FxPredatorView] = LoadShader(null, $"{shaderPath}/predator.fs");
        shaders[(int)PostproShader.FxScanLines] = LoadShader(null, $"{shaderPath}/scanlines.fs");
        shaders[(int)PostproShader.FxFishEye] = LoadShader(null, $"{shaderPath}/fisheye.fs");
        shaders[(int)PostproShader.FxSobel] = LoadShader(null, $"{shaderPath}/sobel.fs");
        shaders[(int)PostproShader.FxBloom] = LoadShader(null, $"{shaderPath}/bloom.fs");
        shaders[(int)PostproShader.FxBlur] = LoadShader(null, $"{shaderPath}/blur.fs");

        currentShader = (int)PostproShader.FxGrayScale;

        // Create a RenderTexture2D to be used for render to texture
        target = LoadRenderTexture(screenWidth, screenHeight);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        if (IsKeyPressed(KeyboardKey.Right))
        {
            currentShader++;
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            currentShader--;
        }

        if (currentShader >= (int)PostproShader.Max)
        {
            currentShader = 0;
        }
        else if (currentShader < 0)
        {
            currentShader = (int)PostproShader.Max - 1;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Enable drawing to texture
        BeginTextureMode(target);
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, position, 0.1f, Color.White);

        DrawGrid(10, 1.0f);

        EndMode3D();

        // End drawing to texture (now we have a texture available for next passes)
        EndTextureMode();

        // Render generated texture using selected postprocessing shader
        BeginShaderMode(shaders[currentShader]);

        // NOTE: Render texture must be y-flipped due to default OpenGL coordinates (left-bottom)
        DrawTextureRec(
            target.Texture,
            new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
            new Vector2(0, 0),
            Color.White
        );

        EndShaderMode();

        DrawRectangle(0, 9, 580, 30, Fade(Color.LightGray, 0.7f));

        DrawText("(c) Church 3D model by Alberto Cano", screenWidth - 200, screenHeight - 20, 10, Color.Gray);

        DrawText("CURRENT POSTPRO SHADER:", 10, 15, 20, Color.Black);
        DrawText(postproShaderText[currentShader], 330, 15, 20, Color.Red);
        DrawText("< >", 540, 10, 30, Color.DarkBlue);

        DrawFPS(700, 15);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // Unload all postpro shaders
        for (var i = 0; i < (int)PostproShader.Max; i++)
        {
            UnloadShader(shaders[i]);
        }

        UnloadTexture(texture);         // Unload texture
        UnloadModel(model);             // Unload model
        UnloadRenderTexture(target);    // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);      // Enable Multi Sampling Anti Aliasing 4x (if available)

        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - postprocessing");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new PostProcessing();
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
