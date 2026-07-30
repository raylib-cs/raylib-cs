/*******************************************************************************************
*
*   raylib [shapes] example - starfield effect
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 6.0
*
*   Example contributed by JP Mortiboys (@themushroompirates) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 JP Mortiboys (@themushroompirates)
*
********************************************************************************************/

using static Raylib_cs.Raymath;    // Required for: Lerp()

namespace Examples.Shapes;

public partial class StarfieldEffect : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int STAR_COUNT = 420;

    public string Name => "Shapes / Starfield Effect";

    public string Title => "raylib [shapes] example - starfield effect";

    private Color bgColor;

    // Speed at which we fly forward
    private float speed;

    // We're either drawing lines or circles
    private bool drawLines;

    private Vector3[] stars;
    private Vector2[] starsScreenPos;

    public void Init()
    {
        bgColor = ColorLerp(Color.DarkBlue, Color.Black, 0.69f);

        // Speed at which we fly forward
        speed = 10.0f / 9.0f;

        // We're either drawing lines or circles
        drawLines = true;

        stars = new Vector3[STAR_COUNT];
        starsScreenPos = new Vector2[STAR_COUNT];

        // Setup the stars with a random position
        for (int i = 0; i < STAR_COUNT; i++)
        {
            stars[i].X = (float)GetRandomValue(-screenWidth / 2, (int)screenWidth / 2);
            stars[i].Y = (float)GetRandomValue(-screenHeight / 2, (int)screenHeight / 2);
            stars[i].Z = 1.0f;
        }
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Change speed based on mouse
        float mouseMove = GetMouseWheelMove();
        if ((int)mouseMove != 0)
        {
            speed += 2.0f * mouseMove / 9.0f;
        }

        if (speed < 0.0f)
        {
            speed = 0.1f;
        }
        else if (speed > 2.0f)
        {
            speed = 2.0f;
        }

        // Toggle lines / points with space bar
        if (IsKeyPressed(KeyboardKey.Space))
        {
            drawLines = !drawLines;
        }

        float dt = GetFrameTime();
        for (int i = 0; i < STAR_COUNT; i++)
        {
            // Update star's timer
            stars[i].Z -= dt * speed;

            // Calculate the screen position
            starsScreenPos[i] = new Vector2(
                screenWidth * 0.5f + stars[i].X / stars[i].Z,
                screenHeight * 0.5f + stars[i].Y / stars[i].Z
            );

            // If the star is too old, or offscreen, it dies and we make a new random one
            if ((stars[i].Z < 0.0f) || (starsScreenPos[i].X < 0) || (starsScreenPos[i].Y < 0.0f) ||
                (starsScreenPos[i].X > screenWidth) || (starsScreenPos[i].Y > screenHeight))
            {
                stars[i].X = (float)GetRandomValue(-screenWidth / 2, screenWidth / 2);
                stars[i].Y = (float)GetRandomValue(-screenHeight / 2, screenHeight / 2);
                stars[i].Z = 1.0f;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(bgColor);

        for (int i = 0; i < STAR_COUNT; i++)
        {
            if (drawLines)
            {
                // Get the time a little while ago for this star, but clamp it
                float t = Clamp(stars[i].Z + 1.0f / 32.0f, 0.0f, 1.0f);

                // If it's different enough from the current time, we proceed
                if ((t - stars[i].Z) > 1e-3)
                {
                    // Calculate the screen position of the old point
                    Vector2 startPos = new Vector2(
                        screenWidth * 0.5f + stars[i].X / t,
                        screenHeight * 0.5f + stars[i].Y / t
                    );

                    // Draw a line connecting the old point to the current point
                    DrawLineV(startPos, starsScreenPos[i], Color.RayWhite);
                }
            }
            else
            {
                // Make the radius grow as the star ages
                float radius = Lerp(stars[i].Z, 1.0f, 5.0f);

                // Draw the circle
                DrawCircleV(starsScreenPos[i], radius, Color.RayWhite);
            }
        }

        DrawText($"[MOUSE WHEEL] Current Speed: {9.0f * speed / 2.0f:F0}", 10, 40, 20, Color.RayWhite);
        DrawText($"[SPACE] Current draw mode: {(drawLines ? "Lines" : "Circles")}", 10, 70, 20, Color.RayWhite);

        DrawFPS(10, 10);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - starfield effect");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new StarfieldEffect();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
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
