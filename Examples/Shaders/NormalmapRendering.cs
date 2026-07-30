/*******************************************************************************************
*
*   raylib [shaders] example - normalmap rendering
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*        OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Jeremy Montgomery (@Sir_Irk) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jeremy Montgomery (@Sir_Irk) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Shaders;

public partial class NormalmapRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Normalmap Rendering";

    public string Title => "raylib [shaders] example - normalmap rendering";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Shader shader;
    private Model plane;
    private Vector3 lightPosition;
    private int lightPosLoc;
    private float specularExponent;
    private int specularExponentLoc;
    private int useNormalMap;
    private int useNormalMapLoc;

    public unsafe void Init()
    {
        camera = new();
        camera.Position = new Vector3(0.0f, 2.0f, -4.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load basic normal map lighting shader
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/normalmap.vs",
            $"resources/shaders/glsl{GlslVersion}/normalmap.fs"
        );

        // Get some required shader locations
        shader.Locs[(int)ShaderLocationIndex.MapNormal] = GetShaderLocation(shader, "normalMap");
        shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shader, "viewPos");

        // NOTE: "matModel" location name is automatically assigned on shader loading,
        // no need to get the location again if using that uniform name
        // shader.Locs[(int)ShaderLocationIndex.MatrixModel] = GetShaderLocation(shader, "matModel");

        // This example uses just 1 point light
        lightPosition = new Vector3(0.0f, 1.0f, 0.0f);
        lightPosLoc = GetShaderLocation(shader, "lightPos");

        // Load a plane model that has proper normals and tangents
        plane = LoadModel("resources/models/plane.glb");

        // Set the plane model's shader and texture maps
        plane.Materials[0].Shader = shader;
        plane.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = LoadTexture("resources/tiles_diffuse.png");
        plane.Materials[0].Maps[(int)MaterialMapIndex.Normal].Texture = LoadTexture("resources/tiles_normal.png");

        // Generate Mipmaps and use TRILINEAR filtering to help with texture aliasing
        GenTextureMipmaps(ref plane.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture);
        GenTextureMipmaps(ref plane.Materials[0].Maps[(int)MaterialMapIndex.Normal].Texture);

        SetTextureFilter(plane.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture, TextureFilter.Trilinear);
        SetTextureFilter(plane.Materials[0].Maps[(int)MaterialMapIndex.Normal].Texture, TextureFilter.Trilinear);

        // Specular exponent AKA shininess of the material
        specularExponent = 8.0f;
        specularExponentLoc = GetShaderLocation(shader, "specularExponent");

        // Allow toggling the normal map on and off for comparison purposes
        useNormalMap = 1;
        useNormalMapLoc = GetShaderLocation(shader, "useNormalMap");
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Move the light around on the X and Z axis using WASD keys
        Vector3 direction = new(0.0f, 0.0f, 0.0f);
        if (IsKeyDown(KeyboardKey.W))
        {
            direction = Vector3Add(direction, new Vector3(0.0f, 0.0f, 1.0f));
        }
        if (IsKeyDown(KeyboardKey.S))
        {
            direction = Vector3Add(direction, new Vector3(0.0f, 0.0f, -1.0f));
        }
        if (IsKeyDown(KeyboardKey.D))
        {
            direction = Vector3Add(direction, new Vector3(-1.0f, 0.0f, 0.0f));
        }
        if (IsKeyDown(KeyboardKey.A))
        {
            direction = Vector3Add(direction, new Vector3(1.0f, 0.0f, 0.0f));
        }

        direction = Vector3Normalize(direction);
        lightPosition = Vector3Add(lightPosition, Vector3Scale(direction, GetFrameTime() * 3.0f));

        // Increase/Decrease the specular exponent(shininess)
        if (IsKeyDown(KeyboardKey.Up))
        {
            specularExponent = Clamp(specularExponent + 40.0f * GetFrameTime(), 2.0f, 128.0f);
        }
        if (IsKeyDown(KeyboardKey.Down))
        {
            specularExponent = Clamp(specularExponent - 40.0f * GetFrameTime(), 2.0f, 128.0f);
        }

        // Toggle normal map on and off
        if (IsKeyPressed(KeyboardKey.N))
        {
            useNormalMap = (useNormalMap != 0) ? 0 : 1;
        }

        // Spin plane model at a constant rate
        plane.Transform = MatrixRotateY((float)GetTime() * 0.5f);

        // Update shader values
        Raylib.SetShaderValue(shader, lightPosLoc, lightPosition, ShaderUniformDataType.Vec3);

        Raylib.SetShaderValue(
            shader,
            shader.Locs[(int)ShaderLocationIndex.VectorView],
            camera.Position,
            ShaderUniformDataType.Vec3
        );

        Raylib.SetShaderValue(shader, specularExponentLoc, specularExponent, ShaderUniformDataType.Float);

        Raylib.SetShaderValue(shader, useNormalMapLoc, useNormalMap, ShaderUniformDataType.Int);
        //--------------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        BeginShaderMode(shader);

        DrawModel(plane, Vector3.Zero, 2.0f, Color.White);

        EndShaderMode();

        // Draw sphere to show light position
        DrawSphereWires(lightPosition, 0.2f, 8, 8, Color.Orange);

        EndMode3D();

        Color textColor = (useNormalMap != 0) ? Color.DarkGreen : Color.Red;
        string toggleStr = (useNormalMap != 0) ? "On" : "Off";
        DrawText($"Use key [N] to toggle normal map: {toggleStr}", 10, 10, 10, textColor);

        int yOffset = 24;
        DrawText("Use keys [W][A][S][D] to move the light", 10, 10 + yOffset * 1, 10, Color.Black);
        DrawText("Use keys [Up][Down] to change specular exponent", 10, 10 + yOffset * 2, 10, Color.Black);
        DrawText($"Specular Exponent: {specularExponent:F2}", 10, 10 + yOffset * 3, 10, Color.Blue);

        DrawFPS(screenWidth - 90, 10);

        EndDrawing();
        //--------------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);
        UnloadModel(plane);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - normalmap rendering");

        SetTargetFPS(60); // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new NormalmapRendering();
        game.Init();

        // Main game loop
        while (!WindowShouldClose()) // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow(); // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
