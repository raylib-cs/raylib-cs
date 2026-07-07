/*******************************************************************************************
*
*   raylib [shapes] example - rlgl triangle
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Robin (@RobinsAviary) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Robin (@RobinsAviary)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Rlgl;

namespace Examples.Shapes;

public partial class RlglTriangle : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / RLGL Triangle";

    public string Title => "raylib [shapes] example - rlgl triangle";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    // Starting postions and rendered triangle positions
    private Vector2[] startingPositions;
    private Vector2[] trianglePositions;

    // Currently selected vertex, -1 means none
    private int triangleIndex;
    private bool linesMode;
    private float handleRadius;

    public void Init()
    {
        // Starting postions and rendered triangle positions
        startingPositions = [new(400.0f, 150.0f), new(300.0f, 300.0f), new(500.0f, 300.0f)];
        trianglePositions = [startingPositions[0], startingPositions[1], startingPositions[2]];

        // Currently selected vertex, -1 means none
        triangleIndex = -1;
        linesMode = false;
        handleRadius = 8.0f;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Space)) linesMode = !linesMode;

        // Check selected vertex
        for (int i = 0; i < 3; i++)
        {
            // If the mouse is within the handle circle
            if (CheckCollisionPointCircle(GetMousePosition(), trianglePositions[i], handleRadius) &&
                IsMouseButtonDown(MouseButton.Left))
            {
                triangleIndex = i;
                break;
            }
        }

        // If the user has selected a vertex, offset it by the mouse's delta this frame
        if (triangleIndex != -1)
        {
            Vector2 mouseDelta = GetMouseDelta();
            trianglePositions[triangleIndex].X += mouseDelta.X;
            trianglePositions[triangleIndex].Y += mouseDelta.Y;
        }

        // Reset index on release
        if (IsMouseButtonReleased(MouseButton.Left)) triangleIndex = -1;

        // Enable/disable backface culling (2-sided triangles, slower to render)
        if (IsKeyPressed(KeyboardKey.Left)) EnableBackfaceCulling();
        if (IsKeyPressed(KeyboardKey.Right)) DisableBackfaceCulling();

        // Reset triangle vertices to starting positions and reset backface culling
        if (IsKeyPressed(KeyboardKey.R))
        {
            trianglePositions[0] = startingPositions[0];
            trianglePositions[1] = startingPositions[1];
            trianglePositions[2] = startingPositions[2];

            EnableBackfaceCulling();
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        if (linesMode)
        {
            // Draw triangle with lines
            Begin(DrawMode.Lines);
            // Three lines, six points
            // Define color for next vertex
            Color4ub(255, 0, 0, 255);
            // Define vertex
            Vertex2f(trianglePositions[0].X, trianglePositions[0].Y);
            Color4ub(0, 255, 0, 255);
            Vertex2f(trianglePositions[1].X, trianglePositions[1].Y);

            Color4ub(0, 255, 0, 255);
            Vertex2f(trianglePositions[1].X, trianglePositions[1].Y);
            Color4ub(0, 0, 255, 255);
            Vertex2f(trianglePositions[2].X, trianglePositions[2].Y);

            Color4ub(0, 0, 255, 255);
            Vertex2f(trianglePositions[2].X, trianglePositions[2].Y);
            Color4ub(255, 0, 0, 255);
            Vertex2f(trianglePositions[0].X, trianglePositions[0].Y);
            End();
        }
        else
        {
            // Draw triangle as a triangle
            Begin(DrawMode.Triangles);
            // One triangle, three points
            // Define color for next vertex
            Color4ub(255, 0, 0, 255);
            // Define vertex
            Vertex2f(trianglePositions[0].X, trianglePositions[0].Y);
            Color4ub(0, 255, 0, 255);
            Vertex2f(trianglePositions[1].X, trianglePositions[1].Y);
            Color4ub(0, 0, 255, 255);
            Vertex2f(trianglePositions[2].X, trianglePositions[2].Y);
            End();
        }

        // Render the vertex handles, reacting to mouse movement/input
        for (int i = 0; i < 3; i++)
        {
            // Draw handle fill focused by mouse
            if (CheckCollisionPointCircle(GetMousePosition(), trianglePositions[i], handleRadius))
                DrawCircleV(trianglePositions[i], handleRadius, ColorAlpha(Color.DarkGray, 0.5f));

            // Draw handle fill selected
            if (i == triangleIndex) DrawCircleV(trianglePositions[i], handleRadius, Color.DarkGray);

            // Draw handle outline
            DrawCircleLinesV(trianglePositions[i], handleRadius, Color.Black);
        }

        // Draw controls
        DrawText("SPACE: Toggle lines mode", 10, 10, 20, Color.DarkGray);
        DrawText("LEFT-RIGHT: Toggle backface culling", 10, 40, 20, Color.DarkGray);
        DrawText("MOUSE: Click and drag vertex points", 10, 70, 20, Color.DarkGray);
        DrawText("R: Reset triangle to start positions", 10, 100, 20, Color.DarkGray);

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
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - rlgl triangle");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new RlglTriangle();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
