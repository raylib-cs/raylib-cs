/*******************************************************************************************
*
*   raylib [textures] example - particles blending
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 1.7, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2017-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class ParticlesBlending : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int MaxParticles = 200;

    public string Name => "Textures / Particles Blending";

    public string Title => "raylib [textures] example - particles blending";

    // Particle structure with basic data
    private struct Particle
    {
        public Vector2 Position;
        public Color Color;
        public float Alpha;
        public float Size;
        public float Rotation;
        // NOTE: Use it to activate/deactive particle
        public bool Active;
    }

    private Particle[] mouseTail;
    private float gravity;
    private Texture2D smoke;
    private BlendMode blending;

    public void Init()
    {
        // Particles pool, reuse them!
        mouseTail = new Particle[MaxParticles];

        // Initialize particles
        for (var i = 0; i < mouseTail.Length; i++)
        {
            mouseTail[i].Position = new Vector2(0, 0);
            mouseTail[i].Color = new Color(
                GetRandomValue(0, 255),
                GetRandomValue(0, 255),
                GetRandomValue(0, 255),
                255
            );
            mouseTail[i].Alpha = 1.0f;
            mouseTail[i].Size = (float)GetRandomValue(1, 30) / 20.0f;
            mouseTail[i].Rotation = GetRandomValue(0, 360);
            mouseTail[i].Active = false;
        }

        gravity = 3.0f;
        smoke = LoadTexture("resources/spark_flame.png");
        blending = BlendMode.Alpha;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------

        // Activate one particle every frame and Update active particles
        // NOTE: Particles initial position should be mouse position when activated
        // NOTE: Particles fall down with gravity and rotation... and disappear after 2 seconds (alpha = 0)
        // NOTE: When a particle disappears, active = false and it can be reused
        for (var i = 0; i < mouseTail.Length; i++)
        {
            if (!mouseTail[i].Active)
            {
                mouseTail[i].Active = true;
                mouseTail[i].Alpha = 1.0f;
                mouseTail[i].Position = GetMousePosition();
                i = mouseTail.Length;
            }
        }

        for (var i = 0; i < mouseTail.Length; i++)
        {
            if (mouseTail[i].Active)
            {
                mouseTail[i].Position.Y += gravity / 2;
                mouseTail[i].Alpha -= 0.005f;

                if (mouseTail[i].Alpha <= 0.0f)
                {
                    mouseTail[i].Active = false;
                }

                mouseTail[i].Rotation += 2.0f;
            }
        }

        if (IsKeyPressed(KeyboardKey.Space))
        {
            if (blending == BlendMode.Alpha)
            {
                blending = BlendMode.Additive;
            }
            else
            {
                blending = BlendMode.Alpha;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.DarkGray);

        BeginBlendMode(blending);

        // Draw active particles
        for (var i = 0; i < mouseTail.Length; i++)
        {
            if (mouseTail[i].Active)
            {
                Rectangle source = new(0, 0, smoke.Width, smoke.Height);
                Rectangle dest = new(
                    mouseTail[i].Position.X,
                    mouseTail[i].Position.Y,
                    smoke.Width * mouseTail[i].Size,
                    smoke.Height * mouseTail[i].Size
                );
                Vector2 position = new(
                    smoke.Width * mouseTail[i].Size / 2,
                    smoke.Height * mouseTail[i].Size / 2
                );
                var color = Fade(mouseTail[i].Color, mouseTail[i].Alpha);
                DrawTexturePro(smoke, source, dest, position, mouseTail[i].Rotation, color);
            }
        }

        EndBlendMode();

        DrawText("PRESS SPACE to CHANGE BLENDING MODE", 180, 20, 20, Color.Black);

        if (blending == BlendMode.Alpha)
        {
            DrawText("ALPHA BLENDING", 290, screenHeight - 40, 20, Color.Black);
        }
        else
        {
            DrawText("ADDITIVE BLENDING", 280, screenHeight - 40, 20, Color.RayWhite);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(smoke);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - particles blending");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ParticlesBlending();
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
