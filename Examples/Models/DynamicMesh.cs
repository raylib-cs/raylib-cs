namespace Examples.Models;

public partial class DynamicMesh : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int triangleRows = 48;
    private const int vertexRows = triangleRows + 1;

    public string Name => "Models / Dynamic Mesh";

    public string Title => "raylib [models] example - dynamic mesh";

    private Camera3D camera;
    private Mesh dynamicMesh;
    private Texture2D texture;
    private Color[] pixels;
    private Material material;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = Vector3.One * 1.5f;
        camera.Target = camera.Position + new Vector3(1f, -0.25f, 1f);
        camera.Up = Vector3.UnitY;
        camera.FovY = 60.0f;
        camera.Projection = CameraProjection.Perspective;

        // Generate a dynamic mesh using utils to allocate/access mesh attribute data
        dynamicMesh = new(vertexRows * vertexRows, triangleRows * triangleRows * 2);
        dynamicMesh.AllocVertices();
        dynamicMesh.AllocTexCoords();
        dynamicMesh.AllocIndices();
        var indices = dynamicMesh.IndicesAs<ushort>();
        for (int z = 0, i = 0; z < triangleRows; z++)
        {
            for (var x = 0; x < triangleRows; x++, i += 6)
            {
                indices[i + 0] = (ushort)(x + (z * vertexRows));
                indices[i + 1] = (ushort)(indices[i] + vertexRows);
                indices[i + 2] = (ushort)(indices[i] + 1);
                indices[i + 3] = (ushort)(indices[i] + 1);
                indices[i + 4] = (ushort)(indices[i] + vertexRows);
                indices[i + 5] = (ushort)(indices[i] + vertexRows + 1);
            }
        }
        UploadMesh(ref dynamicMesh, true);

        // Allocate the texture
        var image = GenImageColor(triangleRows, triangleRows, Color.Blank);
        texture = LoadTextureFromImage(image);
        pixels = new Color[texture.Width * texture.Height];
        UnloadImage(image);

        // Load the material
        material = LoadMaterialDefault();
        SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, texture);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var time = (float)GetTime();
        Random random = new(42);

        var vertices = dynamicMesh.VerticesAs<Vector3>();
        var texcoords = dynamicMesh.TexCoordsAs<Vector2>();

        for (int z = 0, i = 0; z < vertexRows; z++)
        {
            for (var x = 0; x < vertexRows; x++, i++)
            {
                var noiseX = SmoothNoise(time + random.Next(10000));
                var noiseZ = SmoothNoise(time + random.Next(10000));
                vertices[i].X = x + noiseX - .5f;
                vertices[i].Y = (noiseX + noiseZ) / 2;
                vertices[i].Z = z + noiseZ - .5f;
                texcoords[i].X = (x - noiseZ) / triangleRows;
                texcoords[i].Y = (z - noiseX) / triangleRows;
            }
        }
        UpdateMeshBuffer<Vector3>(dynamicMesh, Mesh.VboIdIndexVertices, vertices, 0);
        UpdateMeshBuffer<Vector2>(dynamicMesh, Mesh.VboIdIndexTexCoords, texcoords, 0);

        for (int y = 0, i = 0; y < texture.Height; y++)
        {
            for (var x = 0; x < texture.Width; x++, i++)
            {
                pixels[i] = new(32, 178, 170, 255);
                pixels[i] = ColorBrightness(pixels[i], (SmoothNoise(time + random.Next(10000)) / 8) - (1 / 16f));
                pixels[i] = ColorAlpha(pixels[i], (triangleRows - new Vector2(x, y).Length()) / triangleRows);
            }
        }
        UpdateTexture(texture, pixels);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        DrawMesh(dynamicMesh, material, Matrix4x4.Identity);
        EndMode3D();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadMaterial(material);
        // Raylib.UnloadTexture(texture); <- No need to unload the texture. UnloadMaterial(Material) already unloaded it for us
        UnloadMesh(dynamicMesh);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - dynamic mesh");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new DynamicMesh();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }

    private static float SmoothNoise(float value)
    {
        return ((MathF.Sin(value) + MathF.Cos(value * MathF.E)) / 4) + .5f;
    }
}
