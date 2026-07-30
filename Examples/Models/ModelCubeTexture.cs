/*******************************************************************************************
*
*   raylib [models] example - textured cube
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Models;

public partial class ModelCubeTexture : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Model Cube Texture";

    public string Title => "raylib [models] example - textured cube";

    private Camera3D camera;
    private Texture2D texture;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.0f, 10.0f, 10.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load texture to be applied to the cubes sides
        texture = LoadTexture("resources/cubicmap_atlas.png");
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw cube with an applied texture
        DrawCubeTexture(texture, new Vector3(-2.0f, 2.0f, 0.0f), 2.0f, 4.0f, 2.0f, Color.White);

        // Draw cube with an applied texture, but only a defined rectangle piece of the texture
        DrawCubeTextureRec(
            texture,
            new Rectangle(0, texture.Height / 2, texture.Width / 2, texture.Height / 2),
            new Vector3(2.0f, 1.0f, 0.0f),
            2.0f,
            2.0f,
            2.0f,
            Color.White
        );

        DrawGrid(10, 1.0f);

        EndMode3D();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);
    }

    // Draw cube textured
    // NOTE: Cube position is the center position
    private static void DrawCubeTexture(
        Texture2D texture,
        Vector3 position,
        float width,
        float height,
        float length,
        Color color
    )
    {
        var x = position.X;
        var y = position.Y;
        var z = position.Z;

        // Set desired texture to be enabled while drawing following vertex data
        Rlgl.SetTexture(texture.Id);

        // Vertex data transformation can be defined with the commented lines,
        // but in this example we calculate the transformed vertex data directly when calling Rlgl.Vertex3f()
        // Rlgl.PushMatrix();
        // NOTE: Transformation is applied in inverse order (scale -> rotate -> translate)
        // Rlgl.Translatef(2.0f, 0.0f, 0.0f);
        // Rlgl.Rotatef(45, 0, 1, 0);
        // Rlgl.Scalef(2.0f, 2.0f, 2.0f);

        Rlgl.Begin(DrawMode.Quads);
        Rlgl.Color4ub(color.R, color.G, color.B, color.A);

        // Front Face
        // Normal Pointing Towards Viewer
        Rlgl.Normal3f(0.0f, 0.0f, 1.0f);
        Rlgl.TexCoord2f(0.0f, 0.0f);
        // Bottom Left Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f(1.0f, 0.0f);
        // Bottom Right Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f(1.0f, 1.0f);
        // Top Right Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(0.0f, 1.0f);
        // Top Left Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);

        // Back Face
        // Normal Pointing Away From Viewer
        Rlgl.Normal3f(0.0f, 0.0f, -1.0f);
        Rlgl.TexCoord2f(1.0f, 0.0f);
        // Bottom Right Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(1.0f, 1.0f);
        // Top Right Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(0.0f, 1.0f);
        // Top Left Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(0.0f, 0.0f);
        // Bottom Left Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);

        // Top Face
        // Normal Pointing Up
        Rlgl.Normal3f(0.0f, 1.0f, 0.0f);
        Rlgl.TexCoord2f(0.0f, 1.0f);
        // Top Left Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(0.0f, 0.0f);
        // Bottom Left Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(1.0f, 0.0f);
        // Bottom Right Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(1.0f, 1.0f);
        // Top Right Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);

        // Bottom Face
        // Normal Pointing Down
        Rlgl.Normal3f(0.0f, -1.0f, 0.0f);
        Rlgl.TexCoord2f(1.0f, 1.0f);
        // Top Right Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(0.0f, 1.0f);
        // Top Left Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(0.0f, 0.0f);
        // Bottom Left Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f(1.0f, 0.0f);
        // Bottom Right Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);

        // Right face
        // Normal Pointing Right
        Rlgl.Normal3f(1.0f, 0.0f, 0.0f);
        Rlgl.TexCoord2f(1.0f, 0.0f);
        // Bottom Right Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(1.0f, 1.0f);
        // Top Right Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(0.0f, 1.0f);
        // Top Left Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(0.0f, 0.0f);
        // Bottom Left Of The Texture and Quad
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);

        // Left Face
        // Normal Pointing Left
        Rlgl.Normal3f(-1.0f, 0.0f, 0.0f);
        Rlgl.TexCoord2f(0.0f, 0.0f);
        // Bottom Left Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(1.0f, 0.0f);
        // Bottom Right Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f(1.0f, 1.0f);
        // Top Right Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(0.0f, 1.0f);
        // Top Left Of The Texture and Quad
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
        Rlgl.End();
        //rlPopMatrix();

        Rlgl.SetTexture(0);
    }

    // Draw cube with texture piece applied to all faces
    private static void DrawCubeTextureRec(
        Texture2D texture,
        Rectangle source,
        Vector3 position,
        float width,
        float height,
        float length,
        Color color
    )
    {
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        var texWidth = (float)texture.Width;
        var texHeight = (float)texture.Height;

        // Set desired texture to be enabled while drawing following vertex data
        Rlgl.SetTexture(texture.Id);

        // We calculate the normalized texture coordinates for the desired texture-source-rectangle
        // It means converting from (tex.Width, tex.Height) coordinates to [0.0f, 1.0f] equivalent
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.Color4ub(color.R, color.G, color.B, color.A);

        // Front face
        Rlgl.Normal3f(0.0f, 0.0f, 1.0f);
        Rlgl.TexCoord2f(source.X / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);

        // Back face
        Rlgl.Normal3f(0.0f, 0.0f, -1.0f);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);

        // Top face
        Rlgl.Normal3f(0.0f, 1.0f, 0.0f);
        Rlgl.TexCoord2f(source.X / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);

        // Bottom face
        Rlgl.Normal3f(0.0f, -1.0f, 0.0f);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);

        // Right face
        Rlgl.Normal3f(1.0f, 0.0f, 0.0f);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);

        // Left face
        Rlgl.Normal3f(-1.0f, 0.0f, 0.0f);
        Rlgl.TexCoord2f(source.X / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight);
        Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);
        Rlgl.TexCoord2f((source.X + source.Width) / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);
        Rlgl.TexCoord2f(source.X / texWidth, source.Y / texHeight);
        Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);

        Rlgl.End();

        Rlgl.SetTexture(0);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - textured cube");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ModelCubeTexture();
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
}
