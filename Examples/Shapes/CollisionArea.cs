/*******************************************************************************************
*
*   raylib [shapes] example - collision area
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2013-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class CollisionArea : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Collision Area";

    public string Title => "raylib [shapes] example - collision area";

    private Rectangle boxA;
    private int boxASpeedX;
    private Rectangle boxB;
    private Rectangle boxCollision;
    private int screenUpperLimit;
    private bool pause;
    private bool collision;

    public void Init()
    {
        // Box A: Moving box
        boxA = new(10, GetScreenHeight() / 2.0f - 50, 200, 100);
        boxASpeedX = 4;

        // Box B: Mouse moved box
        boxB = new(GetScreenWidth() / 2.0f - 30, GetScreenHeight() / 2.0f - 30, 60, 60);

        boxCollision = new(); // Collision rectangle

        screenUpperLimit = 40;      // Top menu limits

        pause = false;             // Movement pause
        collision = false;         // Collision detection
    }

    public void Update()
    {
        // Update
        //-----------------------------------------------------
        // Move box if not paused
        if (!pause)
        {
            boxA.X += boxASpeedX;
        }

        // Bounce box on x screen limits
        if (((boxA.X + boxA.Width) >= GetScreenWidth()) || (boxA.X <= 0))
        {
            boxASpeedX *= -1;
        }

        // Update player-controlled-box (box02)
        boxB.X = GetMouseX() - boxB.Width / 2;
        boxB.Y = GetMouseY() - boxB.Height / 2;

        // Make sure Box B does not go out of move area limits
        if ((boxB.X + boxB.Width) >= GetScreenWidth())
        {
            boxB.X = GetScreenWidth() - boxB.Width;
        }
        else if (boxB.X <= 0)
        {
            boxB.X = 0;
        }

        if ((boxB.Y + boxB.Height) >= GetScreenHeight())
        {
            boxB.Y = GetScreenHeight() - boxB.Height;
        }
        else if (boxB.Y <= screenUpperLimit)
        {
            boxB.Y = screenUpperLimit;
        }

        // Check boxes collision
        collision = CheckCollisionRecs(boxA, boxB);

        // Get collision rectangle (only on collision)
        if (collision)
        {
            boxCollision = GetCollisionRec(boxA, boxB);
        }

        // Pause Box A movement
        if (IsKeyPressed(KeyboardKey.Space))
        {
            pause = !pause;
        }
        //-----------------------------------------------------

        // Draw
        //-----------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawRectangle(0, 0, screenWidth, screenUpperLimit, collision ? Color.Red : Color.Black);

        DrawRectangleRec(boxA, Color.Gold);
        DrawRectangleRec(boxB, Color.Blue);

        if (collision)
        {
            // Draw collision area
            DrawRectangleRec(boxCollision, Color.Lime);

            // Draw collision message
            var cx = GetScreenWidth() / 2 - MeasureText("COLLISION!", 20) / 2;
            var cy = screenUpperLimit / 2 - 10;
            DrawText("COLLISION!", cx, cy, 20, Color.Black);

            // Draw collision area
            var text = $"Collision Area: {(int)boxCollision.Width * (int)boxCollision.Height}";
            DrawText(text, GetScreenWidth() / 2 - 100, screenUpperLimit + 10, 20, Color.Black);
        }

        // Draw help instructions
        DrawText("Press SPACE to PAUSE/RESUME", 20, screenHeight - 35, 20, Color.LightGray);

        DrawFPS(10, 10);

        EndDrawing();
        //-----------------------------------------------------
    }

    public void Unload()
    {
    }

    public static int Main()
    {
        // Initialization
        //---------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - collision area");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //----------------------------------------------------------

        var game = new CollisionArea();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //---------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //----------------------------------------------------------

        return 0;
    }
}
