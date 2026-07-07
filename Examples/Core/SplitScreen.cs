/*******************************************************************************************
*
*   raylib [core] example - 3d camera split screen
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 3.7, last time updated with raylib 4.0
*
*   Example contributed by Jeffery Myers (@JeffM2501) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Jeffery Myers (@JeffM2501)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class SplitScreen : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private Camera3D CameraPlayer1;
    private Camera3D CameraPlayer2;

    private RenderTexture2D screenPlayer1;
    private RenderTexture2D screenPlayer2;
    private Rectangle splitScreenRect;

    public string Name => "Core / Split Screen";

    public string Title => "raylib [core] example - 3d camera split screen";

    // Scene drawing
    private void DrawScene()
    {
        var count = 5;
        float spacing = 4;

        // Draw scene: grid of cube trees on a plane to make a "world"
        DrawPlane(new Vector3(0, 0, 0), new Vector2(50, 50), Color.Beige); // Simple world plane

        for (var x = -count * spacing; x <= count * spacing; x += spacing)
        {
            for (var z = -count * spacing; z <= count * spacing; z += spacing)
            {
                DrawCube(new Vector3(x, 1.5f, z), 1, 1, 1, Color.Lime);
                DrawCube(new Vector3(x, 0.5f, z), 0.25f, 1, 0.25f, Color.Brown);
            }
        }

        // Draw a cube at each player's position
        DrawCube(CameraPlayer1.Position, 1, 1, 1, Color.Red);
        DrawCube(CameraPlayer2.Position, 1, 1, 1, Color.Blue);
    }

    public void Init()
    {
        // Setup player 1 camera and screen
        CameraPlayer1.FovY = 45.0f;
        CameraPlayer1.Up.Y = 1.0f;
        CameraPlayer1.Target.Y = 1.0f;
        CameraPlayer1.Position.Z = -3.0f;
        CameraPlayer1.Position.Y = 1.0f;

        screenPlayer1 = LoadRenderTexture(screenWidth / 2, screenHeight);

        // Setup player two camera and screen
        CameraPlayer2.FovY = 45.0f;
        CameraPlayer2.Up.Y = 1.0f;
        CameraPlayer2.Target.Y = 3.0f;
        CameraPlayer2.Position.X = -3.0f;
        CameraPlayer2.Position.Y = 3.0f;

        screenPlayer2 = LoadRenderTexture(screenWidth / 2, screenHeight);

        // Build a flipped rectangle the size of the split view to use for drawing later
        splitScreenRect = new(
            0.0f,
            0.0f,
            (float)screenPlayer1.Texture.Width,
            (float)-screenPlayer1.Texture.Height
        );
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // If anyone moves this frame, how far will they move based on the time since the last frame
        // this moves things at 10 world units per second, regardless of the actual FPS
        var offsetThisFrame = 10.0f * GetFrameTime();

        // Move Player1 forward and backwards (no turning)
        if (IsKeyDown(KeyboardKey.W))
        {
            CameraPlayer1.Position.Z += offsetThisFrame;
            CameraPlayer1.Target.Z += offsetThisFrame;
        }
        else if (IsKeyDown(KeyboardKey.S))
        {
            CameraPlayer1.Position.Z -= offsetThisFrame;
            CameraPlayer1.Target.Z -= offsetThisFrame;
        }

        // Move Player2 forward and backwards (no turning)
        if (IsKeyDown(KeyboardKey.Up))
        {
            CameraPlayer2.Position.X += offsetThisFrame;
            CameraPlayer2.Target.X += offsetThisFrame;
        }
        else if (IsKeyDown(KeyboardKey.Down))
        {
            CameraPlayer2.Position.X -= offsetThisFrame;
            CameraPlayer2.Target.X -= offsetThisFrame;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Draw Player1 view to the render texture
        BeginTextureMode(screenPlayer1);
        ClearBackground(Color.SkyBlue);

        BeginMode3D(CameraPlayer1);
        DrawScene();
        EndMode3D();

        DrawRectangle(0, 0, GetScreenWidth() / 2, 40, Fade(Color.RayWhite, 0.8f));
        DrawText("PLAYER1: W/S to move", 10, 10, 20, Color.Maroon);
        EndTextureMode();

        // Draw Player2 view to the render texture
        BeginTextureMode(screenPlayer2);
        ClearBackground(Color.SkyBlue);

        BeginMode3D(CameraPlayer2);
        DrawScene();
        EndMode3D();

        DrawRectangle(0, 0, GetScreenWidth() / 2, 40, Fade(Color.RayWhite, 0.8f));
        DrawText("PLAYER2: UP/DOWN to move", 10, 10, 20, Color.DarkBlue);
        EndTextureMode();

        // Draw both views render textures to the screen side by side
        BeginDrawing();
        ClearBackground(Color.Black);

        DrawTextureRec(screenPlayer1.Texture, splitScreenRect, new Vector2(0, 0), Color.White);
        DrawTextureRec(screenPlayer2.Texture, splitScreenRect, new Vector2(screenWidth / 2.0f, 0), Color.White);

        DrawRectangle(GetScreenWidth() / 2 - 2, 0, 4, GetScreenHeight(), Color.LightGray);
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(screenPlayer1); // Unload render texture
        UnloadRenderTexture(screenPlayer2); // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d camera split screen");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SplitScreen();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
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
