/*******************************************************************************************
*
* raylib [shapes] example - Draw a mouse trail (position history)
*
* Example complexity rating: [★☆☆☆] 1/4
*
* Example originally created with raylib 5.6
*
* Example contributed by Balamurugan R (@Bala050814]) and reviewed by Ramon Santamaria (@raysan5)
*
* Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
* BSD-like license that allows static linking with closed source software
*
* Copyright (c) 2025 Balamurugan R (@Bala050814)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class MouseTrail : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Define the maximum number of positions to store in the trail
    private const int MAX_TRAIL_LENGTH = 30;

    public string Name => "Shapes / Mouse Trail";

    public string Title => "raylib [shapes] example - mouse trail";

    // Array to store the history of mouse positions (our fixed-size queue)
    private Vector2[] trailPositions;

    public void Init()
    {
        // Array to store the history of mouse positions (our fixed-size queue)
        trailPositions = new Vector2[MAX_TRAIL_LENGTH];
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        Vector2 mousePosition = GetMousePosition();

        // Shift all existing positions backward by one slot in the array
        // The last element (the oldest position) is dropped
        for (int i = MAX_TRAIL_LENGTH - 1; i > 0; i--)
        {
            trailPositions[i] = trailPositions[i - 1];
        }

        // Store the new, current mouse position at the start of the array (Index 0)
        trailPositions[0] = mousePosition;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.Black);

        // Draw the trail by looping through the history array
        for (int i = 0; i < MAX_TRAIL_LENGTH; i++)
        {
            // Ensure we skip drawing if the array hasn't been fully filled on startup
            if ((trailPositions[i].X != 0.0f) || (trailPositions[i].Y != 0.0f))
            {
                // Calculate relative trail strength (ratio is near 1.0 for new, near 0.0 for old)
                float ratio = (float)(MAX_TRAIL_LENGTH - i) / MAX_TRAIL_LENGTH;

                // Fade effect: oldest positions are more transparent
                // Fade (color, alpha) - alpha is 0.5 to 1.0 based on ratio
                Color trailColor = Fade(Color.SkyBlue, ratio * 0.5f + 0.5f);

                // Size effect: oldest positions are smaller
                float trailRadius = 15.0f * ratio;

                DrawCircleV(trailPositions[i], trailRadius, trailColor);
            }
        }

        // Draw a distinct white circle for the current mouse position (Index 0)
        DrawCircleV(mousePosition, 15.0f, Color.White);

        DrawText("Move the mouse to see the trail effect!", 10, screenHeight - 30, 20, Color.LightGray);

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - mouse trail");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new MouseTrail();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();         // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
