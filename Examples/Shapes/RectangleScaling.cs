/*******************************************************************************************
*
*   raylib [shapes] example - rectangle scaling
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 2.5
*
*   Example contributed by Vlad Adrian (@demizdor) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Vlad Adrian (@demizdor) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class RectangleScaling : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int MOUSE_SCALE_MARK_SIZE = 12;

    public string Name => "Shapes / Rectangle Scaling";

    public string Title => "raylib [shapes] example - rectangle scaling";

    private Rectangle rec;

    private Vector2 mousePosition;

    private bool mouseScaleReady;
    private bool mouseScaleMode;

    public void Init()
    {
        rec = new(100, 100, 200, 80);

        mousePosition = new(0, 0);

        mouseScaleReady = false;
        mouseScaleMode = false;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        mousePosition = GetMousePosition();

        Rectangle area = new(
            rec.X + rec.Width - MOUSE_SCALE_MARK_SIZE,
            rec.Y + rec.Height - MOUSE_SCALE_MARK_SIZE,
            MOUSE_SCALE_MARK_SIZE,
            MOUSE_SCALE_MARK_SIZE
        );

        if (CheckCollisionPointRec(mousePosition, area))
        {
            mouseScaleReady = true;
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                mouseScaleMode = true;
            }
        }
        else
        {
            mouseScaleReady = false;
        }

        if (mouseScaleMode)
        {
            mouseScaleReady = true;

            rec.Width = (mousePosition.X - rec.X);
            rec.Height = (mousePosition.Y - rec.Y);

            // Check minimum rec size
            if (rec.Width < MOUSE_SCALE_MARK_SIZE)
            {
                rec.Width = MOUSE_SCALE_MARK_SIZE;
            }
            if (rec.Height < MOUSE_SCALE_MARK_SIZE)
            {
                rec.Height = MOUSE_SCALE_MARK_SIZE;
            }

            // Check maximum rec size
            if (rec.Width > (GetScreenWidth() - rec.X))
            {
                rec.Width = GetScreenWidth() - rec.X;
            }
            if (rec.Height > (GetScreenHeight() - rec.Y))
            {
                rec.Height = GetScreenHeight() - rec.Y;
            }

            if (IsMouseButtonReleased(MouseButton.Left))
            {
                mouseScaleMode = false;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("Scale rectangle dragging from bottom-right corner!", 10, 10, 20, Color.Gray);

        DrawRectangleRec(rec, Fade(Color.Green, 0.5f));

        if (mouseScaleReady)
        {
            DrawRectangleLinesEx(rec, 1, Color.Red);
            DrawTriangle(
                new Vector2(rec.X + rec.Width - MOUSE_SCALE_MARK_SIZE, rec.Y + rec.Height),
                new Vector2(rec.X + rec.Width, rec.Y + rec.Height),
                new Vector2(rec.X + rec.Width, rec.Y + rec.Height - MOUSE_SCALE_MARK_SIZE),
                Color.Red
            );
        }

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
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - rectangle scaling");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new RectangleScaling();
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
