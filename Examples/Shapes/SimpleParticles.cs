/*******************************************************************************************
*
*   raylib [shapes] example - simple particles
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example contributed by Jordi Santonja (@JordSant)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jordi Santonja (@JordSant)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shapes;

public partial class SimpleParticles : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_PARTICLES = 3000;  // Max number of particles

    public string Name => "Shapes / Simple Particles";

    public string Title => "raylib [shapes] example - simple particles";

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    private enum ParticleType
    {
        Water = 0,
        Smoke,
        Fire
    }

    private static readonly string[] particleTypeNames = ["WATER", "SMOKE", "FIRE"];

    private struct Particle
    {
        public ParticleType type;   // Particle type (WATER, SMOKE, FIRE)
        public Vector2 position;    // Particle position on screen
        public Vector2 velocity;    // Particle current speed and direction
        public float radius;        // Particle radius
        public Color color;         // Particle color

        public float lifeTime;      // Particle life time
        public bool alive;          // Particle alive: inside screen and life time
    }

    // Circular buffer state
    private int head;               // Index for the next write
    private int tail;               // Index for the next read
    private Particle[] buffer;      // Particle buffer array

    // Particle emitter parameters
    private int emissionRate;       // Negative: on average every -X frames. Positive: particles per frame
    private ParticleType currentType;
    private Vector2 emitterPosition;

    private Random random;

    public void Init()
    {
        // Definition of particles
        buffer = new Particle[MAX_PARTICLES]; // Particle array
        head = 0;
        tail = 0;

        // Particle emitter parameters
        emissionRate = -2;          // Negative: on average every -X frames. Positive: particles per frame
        currentType = ParticleType.Water;
        emitterPosition = new(screenWidth / 2.0f, screenHeight / 2.0f);

        random = new Random();
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Emit new particles: when emissionRate is 1, emit every frame
        if (emissionRate < 0)
        {
            if (random.Next(-emissionRate) == 0) EmitParticle(emitterPosition, currentType);
        }
        else
        {
            for (int i = 0; i <= emissionRate; i++) EmitParticle(emitterPosition, currentType);
        }

        // Update the parameters of each particle
        UpdateParticles(screenWidth, screenHeight);

        // Remove dead particles from the circular buffer
        UpdateCircularBuffer();

        // Change Particle Emission Rate (UP/DOWN arrows)
        if (IsKeyPressed(KeyboardKey.Up)) emissionRate++;
        if (IsKeyPressed(KeyboardKey.Down)) emissionRate--;

        // Change Particle Type (LEFT/RIGHT arrows)
        if (IsKeyPressed(KeyboardKey.Right)) currentType = (currentType == ParticleType.Fire) ? ParticleType.Water : (ParticleType)((int)currentType + 1);
        if (IsKeyPressed(KeyboardKey.Left)) currentType = (currentType == ParticleType.Water) ? ParticleType.Fire : (ParticleType)((int)currentType - 1);

        if (IsMouseButtonDown(MouseButton.Left)) emitterPosition = GetMousePosition();
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Call the function with a loop to draw all particles
        DrawParticles();

        // Draw UI and Instructions
        DrawRectangle(5, 5, 315, 75, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(5, 5, 315, 75, Color.Blue);

        DrawText("CONTROLS:", 15, 15, 10, Color.Black);
        DrawText("UP/DOWN: Change Particle Emission Rate", 15, 35, 10, Color.Black);
        DrawText("LEFT/RIGHT: Change Particle Type (Water, Smoke, Fire)", 15, 55, 10, Color.Black);

        if (emissionRate < 0) DrawText($"Particles every {-emissionRate} frames | Type: {particleTypeNames[(int)currentType]}", 15, 95, 10, Color.DarkGray);
        else DrawText($"{emissionRate + 1} Particles per frame | Type: {particleTypeNames[(int)currentType]}", 15, 95, 10, Color.DarkGray);

        DrawFPS(screenWidth - 80, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    private void EmitParticle(Vector2 emitterPosition, ParticleType type)
    {
        int index = AddToCircularBuffer();

        // If buffer is full, index is -1
        if (index != -1)
        {
            ref Particle newParticle = ref buffer[index];

            // Fill particle properties
            newParticle.position = emitterPosition;
            newParticle.alive = true;
            newParticle.lifeTime = 0.0f;
            newParticle.type = type;
            float speed = (float)(random.Next(10)) / 5.0f;
            switch (type)
            {
                case ParticleType.Water:
                {
                    newParticle.radius = 5.0f;
                    newParticle.color = Color.Blue;
                } break;
                case ParticleType.Smoke:
                {
                    newParticle.radius = 7.0f;
                    newParticle.color = Color.Gray;
                } break;
                case ParticleType.Fire:
                {
                    newParticle.radius = 10.0f;
                    newParticle.color = Color.Yellow;
                    speed /= 10.0f;
                } break;
                default: break;
            }

            float direction = (float)(random.Next(360));
            newParticle.velocity = new(speed * MathF.Cos(direction * DEG2RAD), speed * MathF.Sin(direction * DEG2RAD));
        }
    }

    private int AddToCircularBuffer()
    {
        int index = -1;

        // Check if buffer full
        if (((head + 1) % MAX_PARTICLES) != tail)
        {
            // Add new particle to the head position and advance head
            index = head;
            head = (head + 1) % MAX_PARTICLES;
        }

        return index;
    }

    private void UpdateParticles(int screenWidth, int screenHeight)
    {
        for (int i = tail; i != head; i = (i + 1) % MAX_PARTICLES)
        {
            // Update particle life and positions
            buffer[i].lifeTime += 1.0f / 60.0f; // 60 FPS -> 1/60 seconds per frame

            switch (buffer[i].type)
            {
                case ParticleType.Water:
                {
                    buffer[i].position.X += buffer[i].velocity.X;
                    buffer[i].velocity.Y += 0.2f;   // Gravity
                    buffer[i].position.Y += buffer[i].velocity.Y;
                } break;
                case ParticleType.Smoke:
                {
                    buffer[i].position.X += buffer[i].velocity.X;
                    buffer[i].velocity.Y -= 0.05f;  // Upwards
                    buffer[i].position.Y += buffer[i].velocity.Y;
                    buffer[i].radius += 0.5f;       // Increment radius: smoke expands
                    buffer[i].color.A -= 4;         // Decrement alpha: smoke fades

                    // If alpha transparent, particle dies
                    if (buffer[i].color.A < 4) buffer[i].alive = false;
                } break;
                case ParticleType.Fire:
                {
                    // Add a little horizontal oscillation to fire particles
                    buffer[i].position.X += buffer[i].velocity.X + MathF.Cos(buffer[i].lifeTime * 215.0f);
                    buffer[i].velocity.Y -= 0.05f;  // Upwards
                    buffer[i].position.Y += buffer[i].velocity.Y;
                    buffer[i].radius -= 0.15f;      // Decrement radius: fire shrinks
                    buffer[i].color.G -= 3;         // Decrement green: fire turns reddish starting from yellow

                    // If radius too small, particle dies
                    if (buffer[i].radius <= 0.02f) buffer[i].alive = false;
                } break;
                default: break;
            }

            // Disable particle when out of screen
            Vector2 center = buffer[i].position;
            float radius = buffer[i].radius;

            if ((center.X < -radius) || (center.X > (screenWidth + radius)) ||
                (center.Y < -radius) || (center.Y > (screenHeight + radius)))
            {
                buffer[i].alive = false;
            }
        }
    }

    private void UpdateCircularBuffer()
    {
        // Update circular buffer: advance tail over dead particles
        while ((tail != head) && !buffer[tail].alive)
        {
            tail = (tail + 1) % MAX_PARTICLES;
        }
    }

    private void DrawParticles()
    {
        for (int i = tail; i != head; i = (i + 1) % MAX_PARTICLES)
        {
            if (buffer[i].alive)
            {
                DrawCircleV(buffer[i].position,
                            buffer[i].radius,
                            buffer[i].color);
            }
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - simple particles");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new SimpleParticles();
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
