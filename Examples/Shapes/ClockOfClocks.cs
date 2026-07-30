/*******************************************************************************************
*
*   raylib [shapes] example - clock of clocks
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

using static Raylib_cs.Raymath;    // Required for: Lerp(), Clamp()

namespace Examples.Shapes;

public partial class ClockOfClocks : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / Clock of Clocks";

    public string Title => "raylib [shapes] example - clock of clocks";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Color bgColor;
    private Color handsColor;

    private const float clockFaceSize = 24;
    private const float clockFaceSpacing = 8.0f;
    private const float sectionSpacing = 16.0f;

    private static readonly Vector2 TL = new(0.0f, 90.0f);    // Top-left corner
    private static readonly Vector2 TR = new(90.0f, 180.0f);  // Top-right corner
    private static readonly Vector2 BR = new(180.0f, 270.0f); // Bottom-right corner
    private static readonly Vector2 BL = new(0.0f, 270.0f);   // Bottom-left corner
    private static readonly Vector2 HH = new(0.0f, 180.0f);   // Horizontal line
    private static readonly Vector2 VV = new(90.0f, 270.0f);  // Vertical line
    private static readonly Vector2 ZZ = new(135.0f, 135.0f); // Not relevant

    private Vector2[,] digitAngles;

    // Time for the hands to move to the new position (in seconds); this must be <1s
    private const float handsMoveDuration = 0.5f;

    private int prevSeconds;
    private Vector2[,] currentAngles;
    private Vector2[,] srcAngles;
    private Vector2[,] dstAngles;

    private float handsMoveTimer;
    private int hourMode;

    public void Init()
    {
        bgColor = ColorLerp(Color.DarkBlue, Color.Black, 0.75f);
        handsColor = ColorLerp(Color.Yellow, Color.RayWhite, .25f);

        digitAngles = new Vector2[10, 24]
        {
            /* 0 */ { TL, HH, HH, TR, /* */ VV, TL, TR, VV, /* */ VV, VV, VV, VV, /* */ VV, VV, VV, VV, /* */ VV, BL, BR, VV, /* */ BL, HH, HH, BR },
            /* 1 */ { TL, HH, TR, ZZ, /* */ BL, TR, VV, ZZ, /* */ ZZ, VV, VV, ZZ, /* */ ZZ, VV, VV, ZZ, /* */ TL, BR, BL, TR, /* */ BL, HH, HH, BR },
            /* 2 */ { TL, HH, HH, TR, /* */ BL, HH, TR, VV, /* */ TL, HH, BR, VV, /* */ VV, TL, HH, BR, /* */ VV, BL, HH, TR, /* */ BL, HH, HH, BR },
            /* 3 */ { TL, HH, HH, TR, /* */ BL, HH, TR, VV, /* */ TL, HH, BR, VV, /* */ BL, HH, TR, VV, /* */ TL, HH, BR, VV, /* */ BL, HH, HH, BR },
            /* 4 */ { TL, TR, TL, TR, /* */ VV, VV, VV, VV, /* */ VV, BL, BR, VV, /* */ BL, HH, TR, VV, /* */ ZZ, ZZ, VV, VV, /* */ ZZ, ZZ, BL, BR },
            /* 5 */ { TL, HH, HH, TR, /* */ VV, TL, HH, BR, /* */ VV, BL, HH, TR, /* */ BL, HH, TR, VV, /* */ TL, HH, BR, VV, /* */ BL, HH, HH, BR },
            /* 6 */ { TL, HH, HH, TR, /* */ VV, TL, HH, BR, /* */ VV, BL, HH, TR, /* */ VV, TL, TR, VV, /* */ VV, BL, BR, VV, /* */ BL, HH, HH, BR },
            /* 7 */ { TL, HH, HH, TR, /* */ BL, HH, TR, VV, /* */ ZZ, ZZ, VV, VV, /* */ ZZ, ZZ, VV, VV, /* */ ZZ, ZZ, VV, VV, /* */ ZZ, ZZ, BL, BR },
            /* 8 */ { TL, HH, HH, TR, /* */ VV, TL, TR, VV, /* */ VV, BL, BR, VV, /* */ VV, TL, TR, VV, /* */ VV, BL, BR, VV, /* */ BL, HH, HH, BR },
            /* 9 */ { TL, HH, HH, TR, /* */ VV, TL, TR, VV, /* */ VV, BL, BR, VV, /* */ BL, HH, TR, VV, /* */ TL, HH, BR, VV, /* */ BL, HH, HH, BR },
        };

        prevSeconds = -1;
        currentAngles = new Vector2[6, 24];
        srcAngles = new Vector2[6, 24];
        dstAngles = new Vector2[6, 24];

        handsMoveTimer = 0.0f;
        hourMode = 24;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Get the current time
        DateTime timeinfo = DateTime.Now;

        if (timeinfo.Second != prevSeconds)
        {
            // The time has changed, so we need to move the hands to the new positions
            prevSeconds = timeinfo.Second;

            // Format the current time so we can access the individual digits
            string clockDigits = $"{timeinfo.Hour % hourMode:D2}{timeinfo.Minute:D2}{timeinfo.Second:D2}";

            // Fetch where we want all the hands to be
            for (int digit = 0; digit < 6; digit++)
            {
                for (int cell = 0; cell < 24; cell++)
                {
                    srcAngles[digit, cell] = currentAngles[digit, cell];
                    dstAngles[digit, cell] = digitAngles[clockDigits[digit] - '0', cell];

                    // Quick exception for 12h mode
                    if ((digit == 0) && (hourMode == 12) && (clockDigits[0] == '0'))
                    {
                        dstAngles[digit, cell] = ZZ;
                    }

                    if (srcAngles[digit, cell].X > dstAngles[digit, cell].X)
                    {
                        srcAngles[digit, cell].X -= 360.0f;
                    }

                    if (srcAngles[digit, cell].Y > dstAngles[digit, cell].Y)
                    {
                        srcAngles[digit, cell].Y -= 360.0f;
                    }
                }
            }

            // Reset the timer
            handsMoveTimer = -GetFrameTime();
        }

        // Now let's animate all the hands if we need to
        if (handsMoveTimer < handsMoveDuration)
        {
            // Increase the timer but don't go above the maximum
            handsMoveTimer = Clamp(handsMoveTimer + GetFrameTime(), 0, handsMoveDuration);

            // Calculate the % completion of the animation
            float t = handsMoveTimer / handsMoveDuration;

            // A little cheeky smoothstep
            t = t * t * (3.0f - 2.0f * t);

            for (int digit = 0; digit < 6; digit++)
            {
                for (int cell = 0; cell < 24; cell++)
                {
                    currentAngles[digit, cell].X = Lerp(srcAngles[digit, cell].X, dstAngles[digit, cell].X, t);
                    currentAngles[digit, cell].Y = Lerp(srcAngles[digit, cell].Y, dstAngles[digit, cell].Y, t);
                }
            }
        }

        // Handle input
        if (IsKeyPressed(KeyboardKey.Space))
        {
            hourMode = 36 - hourMode; // Toggle between 12 and 24 hour mode with space
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(bgColor);

        DrawText($"{hourMode}-h mode, space to change", 10, 30, 20, Color.RayWhite);

        float xOffset = 4.0f;

        for (int digit = 0; digit < 6; digit++)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Vector2 centre = new(
                        xOffset + col * (clockFaceSize + clockFaceSpacing) + clockFaceSize * 0.5f,
                        100 + row * (clockFaceSize + clockFaceSpacing) + clockFaceSize * 0.5f
                    );

                    DrawRing(centre, clockFaceSize * 0.5f - 2.0f, clockFaceSize * 0.5f, 0, 360, 24, Color.DarkGray);

                    // Big hand
                    DrawRectanglePro(
                        new Rectangle(centre.X, centre.Y, clockFaceSize * 0.5f + 4.0f, 4.0f),
                        new Vector2(2.0f, 2.0f),
                        currentAngles[digit, row * 4 + col].X,
                        handsColor
                    );

                    // Little hand
                    DrawRectanglePro(
                        new Rectangle(centre.X, centre.Y, clockFaceSize * 0.5f + 2.0f, 4.0f),
                        new Vector2(2.0f, 2.0f),
                        currentAngles[digit, row * 4 + col].Y,
                        handsColor
                    );
                }
            }

            xOffset += (clockFaceSize + clockFaceSpacing) * 4;
            if (digit % 2 == 1)
            {
                DrawRing(new Vector2(xOffset + 4.0f, 160.0f), 6.0f, 8.0f, 0.0f, 360.0f, 24, handsColor);
                DrawRing(new Vector2(xOffset + 4.0f, 225.0f), 6.0f, 8.0f, 0.0f, 360.0f, 24, handsColor);
                xOffset += sectionSpacing;
            }
        }

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
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - clock of clocks");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new ClockOfClocks();
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
