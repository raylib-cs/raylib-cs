/*******************************************************************************************
*
*   raylib [textures] example - Bunnymark
*
*   This example has been created using raylib 1.6 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2014-2019 Ramon Santamaria (@raysan5), 2024 Moritz Voss (@thygrrr)
*
*
********************************************************************************************/

using System.Diagnostics;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public static class Bunnymark
{
    // limits
    private const int MaxBunnies = 500_000;
    private const int BunnyIncrement = 500;
    private const int BunnyDecrement = 2_500;

    private const int TARGET_FPS = 60;

    // This is the maximum amount of elements (quads) per batch
    private const int MAX_BATCH_ELEMENTS = Rlgl.DEFAULT_BATCH_BUFFER_ELEMENTS;

    private record struct Bunny()
    {
        public Vector2 Position { get; set; } = GetMousePosition();
        public Vector2 Speed
        {
            get; set;
        } = new(
            GetRandomValue(-250, 250) / (float)TARGET_FPS,
            GetRandomValue(-250, 250) / (float)TARGET_FPS);
        public Color Color
        {
            get;
        } = new(
            GetRandomValue(50, 240),
            GetRandomValue(80, 240),
            GetRandomValue(100, 240), 255);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [textures] example - bunnymark");

        // Load bunny texture
        Texture2D texBunny = LoadTexture("resources/wabbit_alpha.png");
        Vector2 halfSize = new Vector2(texBunny.Width, texBunny.Height) / 2;

        // Initialize bunnies storage
        Span<Bunny> bunnies = new Bunny[MaxBunnies];
        int bunniesCount = 0;

        SetTargetFPS(TARGET_FPS);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            if (IsMouseButtonDown(MouseButton.Left) && bunniesCount < MaxBunnies)
            {
                // Add a range of new bunnies
                foreach (ref var bunny in bunnies[bunniesCount..(bunniesCount + BunnyIncrement)])
                {
                    bunny = new();
                }
                bunniesCount += BunnyIncrement;
            }
            else if (IsMouseButtonDown(MouseButton.Right))
            {
                // Remove the oldest bunnies, shifting them back in the span
                if (bunniesCount > BunnyDecrement)
                {
                    bunnies[BunnyDecrement..bunniesCount].CopyTo(bunnies);
                }
                bunniesCount = Math.Max(0, bunniesCount - BunnyDecrement);
            }

            // Update bunnies
            foreach (ref var bunny in bunnies[..bunniesCount])
            {
                // Integrate position
                bunny.Position += bunny.Speed;

                // Bounce bunnies off the screen borders
                bunny.Speed *= (bunny.Position + halfSize) switch
                {
                    { X: < 0 or > screenWidth, Y: < 40 or > screenHeight } => new(-1, -1),
                    { X: < 0 or > screenWidth } => new(-1, 1),
                    { Y: < 40 or > screenHeight } => new(1, -1),
                    _ => Vector2.One,
                };
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            foreach (var bunny in bunnies[..bunniesCount])
            {
                // NOTE: When internal batch buffer limit is reached (MAX_BATCH_ELEMENTS),
                // a draw call is launched and buffer starts being filled again;
                // before issuing a draw call, updated vertex data from internal CPU buffer is send to GPU...
                // Process of sending data is costly, and it could happen that GPU data has not been completely
                // processed for drawing while new data is tried to be sent (updating current in-use buffers)
                // it could generate a stall and consequently a frame drop, limiting the number of drawn bunnies
                DrawTexture(texBunny, (int)bunny.Position.X, (int)bunny.Position.Y, bunny.Color);
            }

            DrawRectangle(0, 0, screenWidth, 40, Color.Black);
            DrawText($"bunnies: {bunniesCount}", 120, 10, 20, Color.Green);
            DrawText($"batched draw calls: {1 + bunniesCount / MAX_BATCH_ELEMENTS}", 320, 10, 20, Color.Maroon);
            DrawText("Left Mouse: Add Bunnies!!! :D", 10, 400, 20, Color.LightGray);
            DrawText("Right Mouse: Remove Bunnies", 10, 420, 20, Color.LightGray);

            DrawFPS(10, 10);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadTexture(texBunny);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }

    static Bunnymark()
    {
        Debug.Assert(MaxBunnies % BunnyIncrement == 0 && MaxBunnies % BunnyDecrement == 0,
            "MaxBunnies must be a common multiple of BunnyIncrement and BunnyDecrement.");
    }
}
