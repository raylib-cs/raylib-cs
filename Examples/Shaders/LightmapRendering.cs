/*******************************************************************************************
*
*   raylib [shaders] example - lightmap rendering
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by Jussi Viitala (@nullstare) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Jussi Viitala (@nullstare) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Rlgl;

namespace Examples.Shaders;

public partial class LightmapRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MapSize = 16;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Lightmap Rendering";

    public string Title => "raylib [shaders] example - lightmap rendering";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Mesh mesh;
    private Shader shader;
    private Texture2D texture;
    private Texture2D light;
    private RenderTexture2D lightmap;
    private Material material;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(4.0f, 6.0f, 8.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        mesh = GenMeshPlane((float)MapSize, (float)MapSize, 1, 1);

        // GenMeshPlane doesn't generate texcoords2 so we will upload them separately
        mesh.AllocTexCoords2();

        // X                          // Y
        mesh.TexCoords2[0] = 0.0f;
        mesh.TexCoords2[1] = 0.0f;
        mesh.TexCoords2[2] = 1.0f;
        mesh.TexCoords2[3] = 0.0f;
        mesh.TexCoords2[4] = 0.0f;
        mesh.TexCoords2[5] = 1.0f;
        mesh.TexCoords2[6] = 1.0f;
        mesh.TexCoords2[7] = 1.0f;

        // Load a new texcoords2 attributes buffer
        mesh.VboId[(int)ShaderLocationIndex.VertexTexcoord02] =
            LoadVertexBuffer(mesh.TexCoords2, mesh.VertexCount * 2 * sizeof(float), false);
        EnableVertexArray(mesh.VaoId);

        // Index 5 is for texcoords2
        SetVertexAttribute(5, 2, Rlgl.FLOAT, false, 0, 0);
        EnableVertexAttribute(5);
        DisableVertexArray();

        // Load lightmap shader
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/lightmap.vs",
            $"resources/shaders/glsl{GlslVersion}/lightmap.fs"
        );

        texture = LoadTexture("resources/cubicmap_atlas.png");
        light = LoadTexture("resources/spark_flame.png");

        GenTextureMipmaps(ref texture);
        SetTextureFilter(texture, TextureFilter.Trilinear);

        lightmap = LoadRenderTexture(MapSize, MapSize);

        material = LoadMaterialDefault();
        material.Shader = shader;
        material.Maps[(int)MaterialMapIndex.Albedo].Texture = texture;
        material.Maps[(int)MaterialMapIndex.Metalness].Texture = lightmap.Texture;

        // Drawing to lightmap
        BeginTextureMode(lightmap);
        ClearBackground(Color.Black);

        BeginBlendMode(BlendMode.Additive);
        DrawTexturePro(
            light,
            new Rectangle(0, 0, (float)light.Width, (float)light.Height),
            new Rectangle(0, 0, 2.0f * MapSize, 2.0f * MapSize),
            new Vector2((float)MapSize, (float)MapSize),
            0.0f,
            Color.Red
        );
        DrawTexturePro(
            light,
            new Rectangle(0, 0, (float)light.Width, (float)light.Height),
            new Rectangle((float)MapSize * 0.8f, (float)MapSize / 2.0f, 2.0f * MapSize, 2.0f * MapSize),
            new Vector2((float)MapSize, (float)MapSize),
            0.0f,
            Color.Blue
        );
        DrawTexturePro(
            light,
            new Rectangle(0, 0, (float)light.Width, (float)light.Height),
            new Rectangle((float)MapSize * 0.8f, (float)MapSize * 0.8f, (float)MapSize, (float)MapSize),
            new Vector2((float)MapSize / 2.0f, (float)MapSize / 2.0f),
            0.0f,
            Color.Green
        );
        BeginBlendMode(BlendMode.Alpha);
        EndTextureMode();

        // NOTE: To enable trilinear filtering we need mipmaps available for texture
        GenTextureMipmaps(ref lightmap.Texture);
        SetTextureFilter(lightmap.Texture, TextureFilter.Trilinear);
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        DrawMesh(mesh, material, Matrix4x4.Identity);
        EndMode3D();

        DrawTexturePro(
            lightmap.Texture,
            new Rectangle(0, 0, -MapSize, -MapSize),
            new Rectangle((float)GetRenderWidth() - MapSize * 8 - 10, 10, (float)MapSize * 8, (float)MapSize * 8),
            new Vector2(0.0f, 0.0f),
            0.0f,
            Color.White
        );

        DrawText($"LIGHTMAP: {MapSize}x{MapSize} pixels", GetRenderWidth() - 130, 20 + MapSize * 8, 10, Color.Green);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadMesh(mesh);       // Unload the mesh
        UnloadShader(shader);   // Unload shader
        UnloadTexture(texture); // Unload texture
        UnloadTexture(light);   // Unload texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);  // Enable Multi Sampling Anti Aliasing 4x (if available)
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - lightmap rendering");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new LightmapRendering();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
