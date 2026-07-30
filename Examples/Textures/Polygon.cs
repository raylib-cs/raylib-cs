/*******************************************************************************************
*
*   raylib [textures] example - polygon drawing
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 3.7, last time updated with raylib 3.7
*
*   Example contributed by Chris Camacho (@chriscamacho) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Chris Camacho (@chriscamacho) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class Polygon : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Textured Polygon";

    public string Title => "raylib [textures] example - polygon drawing";

    private Vector2[] texcoords;
    private Vector2[] points;
    private Vector2[] positions;
    private Texture2D texture;
    private float angle;

    public void Init()
    {
        texcoords = new[] {
            new Vector2(0.75f, 0),
            new Vector2(0.25f, 0),
            new Vector2(0, 0.5f),
            new Vector2(0, 0.75f),
            new Vector2(0.25f, 1),
            new Vector2(0.375f, 0.875f),
            new Vector2(0.625f, 0.875f),
            new Vector2(0.75f, 1),
            new Vector2(1, 0.75f),
            new Vector2(1, 0.5f),
            // Close the poly
            new Vector2(0.75f, 0)
        };

        points = new Vector2[11];

        // Define the base poly vertices from the UV's
        // NOTE: They can be specified in any other way
        for (var i = 0; i < points.Length; i++)
        {
            points[i].X = (texcoords[i].X - 0.5f) * 256.0f;
            points[i].Y = (texcoords[i].Y - 0.5f) * 256.0f;
        }

        // Define the vertices drawing position
        // NOTE: Initially same as points but updated every frame
        positions = new Vector2[points.Length];
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] = points[i];
        }

        texture = LoadTexture("resources/cat.png");
        angle = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        angle += 1;
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] = Raymath.Vector2Rotate(points[i], angle * Raylib.DEG2RAD);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("textured polygon", 20, 20, 20, Color.DarkGray);
        Vector2 center = new(screenWidth / 2, screenHeight / 2);
        DrawTexturePoly(texture, center, positions, texcoords, positions.Length, Color.White);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);
    }

    // Draw textured polygon, defined by vertex and texture coordinates
    // NOTE: Polygon center must have straight line path to all points
    // without crossing perimeter, points must be in anticlockwise order
    private static void DrawTexturePoly(
        Texture2D texture,
        Vector2 center,
        Vector2[] points,
        Vector2[] texcoords,
        int pointCount,
        Color tint
    )
    {
        Rlgl.SetTexture(texture.Id);
        Rlgl.Begin(DrawMode.Triangles);

        Rlgl.Color4ub(tint.R, tint.G, tint.B, tint.A);

        for (var i = 0; i < pointCount - 1; i++)
        {
            Rlgl.TexCoord2f(0.5f, 0.5f);
            Rlgl.Vertex2f(center.X, center.Y);

            Rlgl.TexCoord2f(texcoords[i].X, texcoords[i].Y);
            Rlgl.Vertex2f(points[i].X + center.X, points[i].Y + center.Y);

            Rlgl.TexCoord2f(texcoords[i + 1].X, texcoords[i + 1].Y);
            Rlgl.Vertex2f(points[i + 1].X + center.X, points[i + 1].Y + center.Y);
        }
        Rlgl.End();

        Rlgl.SetTexture(0);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - polygon drawing");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new Polygon();
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
