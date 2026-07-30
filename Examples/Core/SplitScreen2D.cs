/*******************************************************************************************
*
*   raylib [core] example - 2d camera split screen
*
*   Example complexity rating: [★★★★] 4/4
*
*   Addapted from the core_3d_camera_split_screen example:
*       https://github.com/raysan5/raylib/blob/master/examples/core/core_3d_camera_split_screen.c
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by Gabriel dos Santos Sanches (@gabrielssanches) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Gabriel dos Santos Sanches (@gabrielssanches)
*
********************************************************************************************/

namespace Examples.Core;

public partial class SplitScreen2D : IExample
{
    private const int PLAYER_SIZE = 40;

    private const int screenWidth = 800;
    private const int screenHeight = 440;

    public string Name => "Core / 2D Camera Split Screen";

    public string Title => "raylib [core] example - 2d camera split screen";

    public int Width => screenWidth;

    public int Height => screenHeight;

    private Rectangle player1;
    private Rectangle player2;
    private Camera2D camera1;
    private Camera2D camera2;
    private RenderTexture2D screenCamera1;
    private RenderTexture2D screenCamera2;
    private Rectangle splitScreenRect;

    public void Init()
    {
        player1 = new Rectangle(200, 200, PLAYER_SIZE, PLAYER_SIZE);
        player2 = new Rectangle(250, 200, PLAYER_SIZE, PLAYER_SIZE);

        camera1 = new Camera2D();
        camera1.Target = new Vector2(player1.X, player1.Y);
        camera1.Offset = new Vector2(200.0f, 200.0f);
        camera1.Rotation = 0.0f;
        camera1.Zoom = 1.0f;

        camera2 = new Camera2D();
        camera2.Target = new Vector2(player2.X, player2.Y);
        camera2.Offset = new Vector2(200.0f, 200.0f);
        camera2.Rotation = 0.0f;
        camera2.Zoom = 1.0f;

        screenCamera1 = LoadRenderTexture(screenWidth / 2, screenHeight);
        screenCamera2 = LoadRenderTexture(screenWidth / 2, screenHeight);

        // Build a flipped rectangle the size of the split view to use for drawing later
        splitScreenRect = new Rectangle(0.0f, 0.0f, (float)screenCamera1.Texture.Width, (float)-screenCamera1.Texture.Height);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyDown(KeyboardKey.S))
        {
            player1.Y += 3.0f;
        }
        else if (IsKeyDown(KeyboardKey.W))
        {
            player1.Y -= 3.0f;
        }

        if (IsKeyDown(KeyboardKey.D))
        {
            player1.X += 3.0f;
        }
        else if (IsKeyDown(KeyboardKey.A))
        {
            player1.X -= 3.0f;
        }

        if (IsKeyDown(KeyboardKey.Up))
        {
            player2.Y -= 3.0f;
        }
        else if (IsKeyDown(KeyboardKey.Down))
        {
            player2.Y += 3.0f;
        }

        if (IsKeyDown(KeyboardKey.Right))
        {
            player2.X += 3.0f;
        }
        else if (IsKeyDown(KeyboardKey.Left))
        {
            player2.X -= 3.0f;
        }

        camera1.Target = new Vector2(player1.X, player1.Y);
        camera2.Target = new Vector2(player2.X, player2.Y);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginTextureMode(screenCamera1);
        ClearBackground(Color.RayWhite);

        BeginMode2D(camera1);

        // Draw full scene with first camera
        for (int i = 0; i < screenWidth / PLAYER_SIZE + 1; i++)
        {
            DrawLineV(new Vector2((float)PLAYER_SIZE * i, 0), new Vector2((float)PLAYER_SIZE * i, (float)screenHeight), Color.LightGray);
        }

        for (int i = 0; i < screenHeight / PLAYER_SIZE + 1; i++)
        {
            DrawLineV(new Vector2(0, (float)PLAYER_SIZE * i), new Vector2((float)screenWidth, (float)PLAYER_SIZE * i), Color.LightGray);
        }

        for (int i = 0; i < screenWidth / PLAYER_SIZE; i++)
        {
            for (int j = 0; j < screenHeight / PLAYER_SIZE; j++)
            {
                DrawText($"[{i},{j}]", 10 + PLAYER_SIZE * i, 15 + PLAYER_SIZE * j, 10, Color.LightGray);
            }
        }

        DrawRectangleRec(player1, Color.Red);
        DrawRectangleRec(player2, Color.Blue);
        EndMode2D();

        DrawRectangle(0, 0, GetScreenWidth() / 2, 30, Fade(Color.RayWhite, 0.6f));
        DrawText("PLAYER1: W/S/A/D to move", 10, 10, 10, Color.Maroon);

        EndTextureMode();

        BeginTextureMode(screenCamera2);
        ClearBackground(Color.RayWhite);

        BeginMode2D(camera2);

        // Draw full scene with second camera
        for (int i = 0; i < screenWidth / PLAYER_SIZE + 1; i++)
        {
            DrawLineV(new Vector2((float)PLAYER_SIZE * i, 0), new Vector2((float)PLAYER_SIZE * i, (float)screenHeight), Color.LightGray);
        }

        for (int i = 0; i < screenHeight / PLAYER_SIZE + 1; i++)
        {
            DrawLineV(new Vector2(0, (float)PLAYER_SIZE * i), new Vector2((float)screenWidth, (float)PLAYER_SIZE * i), Color.LightGray);
        }

        for (int i = 0; i < screenWidth / PLAYER_SIZE; i++)
        {
            for (int j = 0; j < screenHeight / PLAYER_SIZE; j++)
            {
                DrawText($"[{i},{j}]", 10 + PLAYER_SIZE * i, 15 + PLAYER_SIZE * j, 10, Color.LightGray);
            }
        }

        DrawRectangleRec(player1, Color.Red);
        DrawRectangleRec(player2, Color.Blue);

        EndMode2D();

        DrawRectangle(0, 0, GetScreenWidth() / 2, 30, Fade(Color.RayWhite, 0.6f));
        DrawText("PLAYER2: UP/DOWN/LEFT/RIGHT to move", 10, 10, 10, Color.DarkBlue);

        EndTextureMode();

        // Draw both views render textures to the screen side by side
        BeginDrawing();
        ClearBackground(Color.Black);

        DrawTextureRec(screenCamera1.Texture, splitScreenRect, new Vector2(0, 0), Color.White);
        DrawTextureRec(screenCamera2.Texture, splitScreenRect, new Vector2(screenWidth / 2.0f, 0), Color.White);

        DrawRectangle(GetScreenWidth() / 2 - 2, 0, 4, GetScreenHeight(), Color.LightGray);
        EndDrawing();
    }

    public void Unload()
    {
        UnloadRenderTexture(screenCamera1); // Unload render texture
        UnloadRenderTexture(screenCamera2); // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 2d camera split screen");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SplitScreen2D();
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
