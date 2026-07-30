/*******************************************************************************************
*
*   raylib [models] example - animation timing
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

// NOTE: The upstream example uses raygui for its UI controls (dropdown, slider, progress bar).
// raygui is not part of raylib-cs, so the required controls are reimplemented here with
// basic raylib drawing primitives, preserving the original behaviour.

namespace Examples.Models;

public partial class AnimationTiming : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Models / Animation Timing";

    public string Title => "raylib [models] example - animation timing";

    private Camera3D camera;
    private Model model;
    private Vector3 position;
    private unsafe ModelAnimation* anims;
    private int animCount;
    private int animIndex;
    private float animCurrentFrame;
    private float animFrameSpeed;
    private bool animPause;
    private string[] animNames;
    private bool dropdownEditMode;
    private float animFrameProgress;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(6.0f, 6.0f, 6.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 2.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load model
        model = LoadModel("resources/models/gltf/robot.glb");
        position = new Vector3(0.0f, 0.0f, 0.0f); // Set model world position

        // Load model animations
        animCount = 0;
        anims = LoadModelAnimations("resources/models/gltf/robot.glb", ref animCount);

        // Animation playing variables
        animIndex = 10;                  // Current animation playing
        animCurrentFrame = 0.0f;        // Current animation frame (supporting interpolated frames)
        animFrameSpeed = 0.5f;          // Animation play speed
        animPause = false;              // Pause animation

        // UI required variables
        animNames = new string[animCount];
        for (var i = 0; i < animCount; i++)
        {
            animNames[i] = anims[i].NameToString();
        }

        dropdownEditMode = false;
        animFrameProgress = 0.0f;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        if (IsKeyPressed(KeyboardKey.P))
        {
            animPause = !animPause;
        }

        if (!animPause && (animIndex < animCount))
        {
            // Update model animation
            animCurrentFrame += animFrameSpeed;
            if (animCurrentFrame >= anims[animIndex].KeyFrameCount)
            {
                animCurrentFrame = 0.0f;
            }
            UpdateModelAnimation(model, anims[animIndex], animCurrentFrame);
        }

        // NOTE: Animation and playing speed selected through UI

        // Update progressbar value with current frame
        animFrameProgress = animCurrentFrame;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, position, 1.0f, Color.White);

        DrawGrid(10, 1.0f);

        EndMode3D();

        // Draw UI, select anim and playing speed
        GuiSlider(new Rectangle(260, 10, 500, 24), "FRAME SPEED: ", $"x{animFrameSpeed:0.0}", ref animFrameSpeed, 0.1f, 2.0f);

        // Draw playing timeline with keyframes
        DrawText($"CURRENT FRAME: {animFrameProgress:0.00} / {anims[animIndex].KeyFrameCount}",
            10, (int)(GetScreenHeight() - 64.0f), 10, Color.DarkGray);
        GuiProgressBar(new Rectangle(10, GetScreenHeight() - 40.0f, GetScreenWidth() - 20.0f, 24), null, null,
            animFrameProgress, 0.0f, anims[animIndex].KeyFrameCount);
        for (var i = 0; i < anims[animIndex].KeyFrameCount; i++)
        {
            DrawRectangle(10 + (int)(((float)(GetScreenWidth() - 20) / anims[animIndex].KeyFrameCount) * i),
                GetScreenHeight() - 40, 1, 24, Color.Blue);
        }

        // NOTE: Dropdown drawn last so its open item list renders on top
        if (GuiDropdownBox(new Rectangle(10, 10, 140, 24), animNames, ref animIndex, dropdownEditMode))
        {
            dropdownEditMode = !dropdownEditMode;
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(anims, animCount); // Unload model animation
        UnloadModel(model);         // Unload model and meshes/material
    }

    // Minimal immediate-mode slider (raygui replacement)
    private static bool GuiSlider(Rectangle bounds, string textLeft, string textRight, ref float value, float minValue, float maxValue)
    {
        var mouse = GetMousePosition();
        var dragging = false;
        if (CheckCollisionPointRec(mouse, bounds) && IsMouseButtonDown(MouseButton.Left))
        {
            value = minValue + ((mouse.X - bounds.X) / bounds.Width) * (maxValue - minValue);
            if (value < minValue)
            {
                value = minValue;
            }
            if (value > maxValue)
            {
                value = maxValue;
            }
            dragging = true;
        }

        DrawRectangleRec(bounds, Color.LightGray);
        var pct = (value - minValue) / (maxValue - minValue);
        DrawRectangleRec(new Rectangle(bounds.X, bounds.Y, bounds.Width * pct, bounds.Height), Color.SkyBlue);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (!string.IsNullOrEmpty(textLeft))
        {
            DrawText(textLeft, (int)(bounds.X - MeasureText(textLeft, 10) - 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
        if (!string.IsNullOrEmpty(textRight))
        {
            DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
        return dragging;
    }

    // Minimal immediate-mode progress bar (raygui replacement)
    private static void GuiProgressBar(Rectangle bounds, string textLeft, string textRight, float value, float minValue, float maxValue)
    {
        DrawRectangleRec(bounds, Color.LightGray);
        var pct = maxValue > minValue ? (value - minValue) / (maxValue - minValue) : 0.0f;
        if (pct < 0)
        {
            pct = 0;
        }
        if (pct > 1)
        {
            pct = 1;
        }
        DrawRectangleRec(new Rectangle(bounds.X, bounds.Y, bounds.Width * pct, bounds.Height), Color.SkyBlue);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (!string.IsNullOrEmpty(textLeft))
        {
            DrawText(textLeft, (int)(bounds.X - MeasureText(textLeft, 10) - 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
        if (!string.IsNullOrEmpty(textRight))
        {
            DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
    }

    // Minimal immediate-mode dropdown box (raygui replacement)
    private static bool GuiDropdownBox(Rectangle bounds, string[] items, ref int active, bool editMode)
    {
        var result = false;
        var mouse = GetMousePosition();

        // Draw main box
        DrawRectangleRec(bounds, Color.LightGray);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (active >= 0 && active < items.Length)
        {
            DrawText(items[active], (int)bounds.X + 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
        DrawText(editMode ? "^" : "v", (int)(bounds.X + bounds.Width - 12), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);

        // Draw items when open
        if (editMode)
        {
            for (var i = 0; i < items.Length; i++)
            {
                Rectangle item = new(bounds.X, bounds.Y + bounds.Height * (i + 1), bounds.Width, bounds.Height);
                var hover = CheckCollisionPointRec(mouse, item);
                DrawRectangleRec(item, hover ? Color.SkyBlue : Color.LightGray);
                DrawRectangleLinesEx(item, 1, Color.Gray);
                DrawText(items[i], (int)item.X + 5, (int)(item.Y + item.Height / 2 - 5), 10, Color.DarkGray);
            }
        }

        if (IsMouseButtonPressed(MouseButton.Left))
        {
            if (editMode)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    Rectangle item = new(bounds.X, bounds.Y + bounds.Height * (i + 1), bounds.Width, bounds.Height);
                    if (CheckCollisionPointRec(mouse, item))
                    {
                        active = i;
                        break;
                    }
                }
                result = true; // any click closes the dropdown
            }
            else if (CheckCollisionPointRec(mouse, bounds))
            {
                result = true; // open the dropdown
            }
        }

        return result;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - animation timing");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new AnimationTiming();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();              // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
