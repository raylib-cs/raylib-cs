/*******************************************************************************************
*
*   raylib [models] example - animation blending
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 6.0
*
*   Example contributed by Kirandeep (@Kirandeep-Singh-Khehra) and reviewed by Ramon Santamaria (@raysan5)
*
*   WARNING: GPU skinning must be enabled in raylib with a compilation flag,
*   if not enabled, CPU skinning will be used instead
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2026 Kirandeep (@Kirandeep-Singh-Khehra) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

// NOTE: The upstream example uses raygui for its UI controls (dropdowns, sliders, progress bars).
// raygui is not part of raylib-cs, so the required controls are reimplemented here with
// basic raylib drawing primitives, preserving the original behaviour.

namespace Examples.Models;

public partial class AnimationBlending : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    private const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Models / Animation Blending";

    public string Title => "raylib [models] example - animation blending";

    private Camera3D camera;
    private Model model;
    private Vector3 position;
    private Shader skinningShader;
    private unsafe ModelAnimation* anims;
    private int animCount;

    private int currentAnimPlaying;
    private int nextAnimToPlay;
    private bool animTransition;

    private int animIndex0;
    private float animCurrentFrame0;
    private float animFrameSpeed0;
    private int animIndex1;
    private float animCurrentFrame1;
    private float animFrameSpeed1;

    private float animBlendFactor;
    private float animBlendTime;
    private float animBlendTimeCounter;

    private bool animPause;

    private string[] animNames;
    private bool dropdownEditMode0;
    private bool dropdownEditMode1;
    private float animFrameProgress0;
    private float animFrameProgress1;
    private float animBlendProgress;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(6.0f, 6.0f, 6.0f); // Camera position
        camera.Target = new Vector3(0.0f, 2.0f, 0.0f);  // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);      // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                            // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective; // Camera projection type

        // Load model
        model = LoadModel("resources/models/gltf/robot.glb"); // Load character model
        position = new Vector3(0.0f, 0.0f, 0.0f); // Set model world position

        // Load skinning shader
        // NOTE: It must be a valid shader, following raylib attribs/uniform conventions for GPU skinning
        skinningShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/skinning.vs",
            $"resources/shaders/glsl{GlslVersion}/skinning.fs"
        );

        // Skinning shader could be required to be assigned to all materials shaders, just to make
        // sure required uniforms are being updated for the mesh using that material (and shader)
        for (var i = 0; i < model.MaterialCount; i++)
        {
            model.Materials[i].Shader = skinningShader;
        }

        // Load model animations
        animCount = 0;
        anims = LoadModelAnimations("resources/models/gltf/robot.glb", ref animCount);

        // Animation playing variables
        // NOTE: Two animations are played with a smooth transition between them
        currentAnimPlaying = 0;         // Current animation playing (0 o 1)
        nextAnimToPlay = 1;             // Next animation to play (to transition)
        animTransition = false;         // Flag to register anim transition state

        animIndex0 = 10;                // Current animation playing (walking)
        animCurrentFrame0 = 0.0f;       // Current animation frame (supporting interpolated frames)
        animFrameSpeed0 = 0.5f;         // Current animation play speed
        animIndex1 = 6;                 // Next animation to play (running)
        animCurrentFrame1 = 0.0f;       // Next animation frame (supporting interpolated frames)
        animFrameSpeed1 = 0.5f;         // Next animation play speed

        animBlendFactor = 0.0f;         // Blend factor from anim0[frame0] --> anim1[frame1], [0.0f..1.0f]

        animBlendTime = 2.0f;           // Time to blend from one playing animation to another (in seconds)
        animBlendTimeCounter = 0.0f;    // Time counter (delta time)

        animPause = false;              // Pause animation

        // UI required variables
        animNames = new string[animCount];
        for (var i = 0; i < animCount; i++)
        {
            animNames[i] = anims[i].NameToString();
        }

        dropdownEditMode0 = false;
        dropdownEditMode1 = false;
        animFrameProgress0 = 0.0f;
        animFrameProgress1 = 0.0f;
        animBlendProgress = 0.0f;
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

        if (!animPause)
        {
            // Start transition from anim0[] to anim1[]
            if (IsKeyPressed(KeyboardKey.Space) && !animTransition)
            {
                if (currentAnimPlaying == 0)
                {
                    // Transition anim0 --> anim1
                    nextAnimToPlay = 1;
                    animCurrentFrame1 = 0.0f;
                }
                else
                {
                    // Transition anim1 --> anim0
                    nextAnimToPlay = 0;
                    animCurrentFrame0 = 0.0f;
                }

                // Set animation transition
                animTransition = true;
                animBlendTimeCounter = 0.0f;
                animBlendFactor = 0.0f;
            }

            if (animTransition)
            {
                // Playing anim0 and anim1 at the same time
                animCurrentFrame0 += animFrameSpeed0;
                if (animCurrentFrame0 >= anims[animIndex0].KeyFrameCount)
                {
                    animCurrentFrame0 = 0.0f;
                }
                animCurrentFrame1 += animFrameSpeed1;
                if (animCurrentFrame1 >= anims[animIndex1].KeyFrameCount)
                {
                    animCurrentFrame1 = 0.0f;
                }

                // Increment blend factor over time to transition from anim0 --> anim1 over time
                // NOTE: Time blending could be other than linear, using some easing
                animBlendFactor = animBlendTimeCounter / animBlendTime;
                animBlendTimeCounter += GetFrameTime();
                animBlendProgress = animBlendFactor;

                // Update model with animations blending
                if (nextAnimToPlay == 1)
                {
                    // Blend anim0 --> anim1
                    UpdateModelAnimationEx(model, anims[animIndex0], animCurrentFrame0,
                        anims[animIndex1], animCurrentFrame1, animBlendFactor);
                }
                else
                {
                    // Blend anim1 --> anim0
                    UpdateModelAnimationEx(model, anims[animIndex1], animCurrentFrame1,
                        anims[animIndex0], animCurrentFrame0, animBlendFactor);
                }

                // Check if transition completed
                if (animBlendFactor > 1.0f)
                {
                    // Reset frame states
                    if (currentAnimPlaying == 0)
                    {
                        animCurrentFrame0 = 0.0f;
                    }
                    else if (currentAnimPlaying == 1)
                    {
                        animCurrentFrame1 = 0.0f;
                    }
                    currentAnimPlaying = nextAnimToPlay; // Update current animation playing

                    animBlendFactor = 0.0f; // Reset blend factor
                    animTransition = false; // Exit transition mode
                    animBlendTimeCounter = 0.0f;
                }
            }
            else
            {
                // Play only one anim, the current one
                if (currentAnimPlaying == 0)
                {
                    // Playing anim0 at defined speed
                    animCurrentFrame0 += animFrameSpeed0;
                    if (animCurrentFrame0 >= anims[animIndex0].KeyFrameCount)
                    {
                        animCurrentFrame0 = 0.0f;
                    }
                    UpdateModelAnimation(model, anims[animIndex0], animCurrentFrame0);
                }
                else if (currentAnimPlaying == 1)
                {
                    // Playing anim1 at defined speed
                    animCurrentFrame1 += animFrameSpeed1;
                    if (animCurrentFrame1 >= anims[animIndex1].KeyFrameCount)
                    {
                        animCurrentFrame1 = 0.0f;
                    }
                    UpdateModelAnimation(model, anims[animIndex1], animCurrentFrame1);
                }
            }
        }

        // Update progress bars values with current frame for each animation
        animFrameProgress0 = animCurrentFrame0;
        animFrameProgress1 = animCurrentFrame1;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, position, 1.0f, Color.White); // Draw animated model

        DrawGrid(10, 1.0f);

        EndMode3D();

        if (animTransition)
        {
            DrawText("ANIM TRANSITION BLENDING!", 170, 50, 30, Color.Blue);
        }

        // Draw UI elements
        //---------------------------------------------------------------------------------------------
        GuiSlider(new Rectangle(10, 38, 160, 12), null, $"x{animFrameSpeed0:0.0}", ref animFrameSpeed0, 0.1f, 2.0f);
        GuiSlider(new Rectangle(GetScreenWidth() - 170.0f, 38, 160, 12), $"{animFrameSpeed1:0.0}x", null, ref animFrameSpeed1, 0.1f, 2.0f);

        // Blending process progress bar
        GuiProgressBar(new Rectangle(180, 14, 440, 16), null, null, animBlendProgress, 0.0f, 1.0f);

        // Centered "PRESS SPACE" label
        const string spaceLabel = "PRESS SPACE to START BLENDING";
        var spaceLabelWidth = MeasureText(spaceLabel, 20);
        DrawText(spaceLabel, (GetScreenWidth() - spaceLabelWidth) / 2, (int)(GetScreenHeight() - 100.0f + 10), 20, Color.DarkGray);

        // Draw playing timeline with keyframes for anim0[]
        GuiProgressBar(new Rectangle(60, GetScreenHeight() - 60.0f, GetScreenWidth() - 180.0f, 20), "ANIM 0",
            $"FRAME: {animFrameProgress0:0.00} / {anims[animIndex0].KeyFrameCount}",
            animFrameProgress0, 0.0f, anims[animIndex0].KeyFrameCount);
        for (var i = 0; i < anims[animIndex0].KeyFrameCount; i++)
        {
            DrawRectangle(60 + (int)(((float)(GetScreenWidth() - 180) / anims[animIndex0].KeyFrameCount) * i),
                GetScreenHeight() - 60, 1, 20, Color.Blue);
        }

        // Draw playing timeline with keyframes for anim1[]
        GuiProgressBar(new Rectangle(60, GetScreenHeight() - 30.0f, GetScreenWidth() - 180.0f, 20), "ANIM 1",
            $"FRAME: {animFrameProgress1:0.00} / {anims[animIndex1].KeyFrameCount}",
            animFrameProgress1, 0.0f, anims[animIndex1].KeyFrameCount);
        for (var i = 0; i < anims[animIndex1].KeyFrameCount; i++)
        {
            DrawRectangle(60 + (int)(((float)(GetScreenWidth() - 180) / anims[animIndex1].KeyFrameCount) * i),
                GetScreenHeight() - 30, 1, 20, Color.Blue);
        }

        // Draw animation selectors for blending transition (drawn last so open lists render on top)
        // NOTE: Transition does not start until requested
        if (GuiDropdownBox(new Rectangle(10, 10, 160, 24), animNames, ref animIndex0, dropdownEditMode0))
        {
            dropdownEditMode0 = !dropdownEditMode0;
        }
        if (GuiDropdownBox(new Rectangle(GetScreenWidth() - 170.0f, 10, 160, 24), animNames, ref animIndex1, dropdownEditMode1))
        {
            dropdownEditMode1 = !dropdownEditMode1;
        }
        //---------------------------------------------------------------------------------------------

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(anims, animCount); // Unload model animation
        UnloadModel(model);             // Unload model and meshes/material
        UnloadShader(skinningShader);   // Unload GPU skinning shader
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
        InitWindow(screenWidth, screenHeight, "raylib [models] example - animation blending");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new AnimationBlending();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                  // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
