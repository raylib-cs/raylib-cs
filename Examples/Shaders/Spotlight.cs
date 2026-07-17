/*******************************************************************************************
*
*   raylib [shaders] example - spotlight rendering
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.7
*
*   Example contributed by Chris Camacho (@chriscamacho) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Chris Camacho (@chriscamacho) and Ramon Santamaria (@raysan5)
*
********************************************************************************************
*
*   The shader makes alpha holes in the forground to give the appearance of a top
*   down look at a spotlight casting a pool of light...
*
*   The right hand side of the screen there is just enough light to see whats
*   going on without the spot light, great for a stealth type game where you
*   have to avoid the spotlights
*
*   The left hand side of the screen is in pitch dark except for where the spotlights are
*
*   Although this example doesn't scale like the letterbox example, you could integrate
*   the two techniques, but by scaling the actual colour of the render texture rather
*   than using alpha as a mask
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class Spotlight : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    // NOTE: It must be the same as define in shader
    private const int MaxSpots = 3;
    private const int MaxStars = 400;

    public string Name => "Shaders / Spotlight";

    public string Title => "raylib [shaders] example - spotlight rendering";

    public bool CursorHidden => true;

    // Spot data
    private struct Spot
    {
        public Vector2 pos;
        public Vector2 vel;
        public float inner;
        public float radius;

        // Shader locations
        public int posLoc;
        public int innerLoc;
        public int radiusLoc;
    }

    // Stars in the star field have a position and velocity
    private struct Star
    {
        public Vector2 pos;
        public Vector2 vel;
    }

    private Texture2D texRay;
    private Star[] stars;
    private int frameCounter;
    private Shader shdrSpot;
    private Spot[] spots;

    public void Init()
    {
        texRay = LoadTexture("resources/raysan.png");

        stars = new Star[MaxStars];

        for (var n = 0; n < MaxStars; n++)
        {
            ResetStar(ref stars[n]);
        }

        // Progress all the stars on, so they don't all start in the centre
        for (var m = 0; m < screenWidth / 2.0; m++)
        {
            for (var n = 0; n < MaxStars; n++)
            {
                UpdateStar(ref stars[n]);
            }
        }

        frameCounter = 0;

        // Use default vert shader
        shdrSpot = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/spotlight.fs");

        // Get the locations of spots in the shader
        spots = new Spot[MaxSpots];

        for (var i = 0; i < MaxSpots; i++)
        {
            var posName = $"spots[{i}].pos";
            var innerName = $"spots[{i}].inner";
            var radiusName = $"spots[{i}].radius";

            spots[i].posLoc = GetShaderLocation(shdrSpot, posName);
            spots[i].innerLoc = GetShaderLocation(shdrSpot, innerName);
            spots[i].radiusLoc = GetShaderLocation(shdrSpot, radiusName);
        }

        // Tell the shader how wide the screen is so we can have
        // a pitch black half and a dimly lit half
        var wLoc = GetShaderLocation(shdrSpot, "screenWidth");
        var sw = (float)GetScreenWidth();
        Raylib.SetShaderValue(shdrSpot, wLoc, sw, ShaderUniformDataType.Float);

        // Randomize the locations and velocities of the spotlights
        // and initialize the shader locations
        for (var i = 0; i < MaxSpots; i++)
        {
            spots[i].pos.X = GetRandomValue(64, screenWidth - 64);
            spots[i].pos.Y = GetRandomValue(64, screenHeight - 64);
            spots[i].vel = new Vector2(0, 0);

            while ((MathF.Abs(spots[i].vel.X) + MathF.Abs(spots[i].vel.Y)) < 2)
            {
                spots[i].vel.X = GetRandomValue(-400, 40) / 25.0f;
                spots[i].vel.Y = GetRandomValue(-400, 40) / 25.0f;
            }

            spots[i].inner = 28.0f * (i + 1);
            spots[i].radius = 48.0f * (i + 1);

            Raylib.SetShaderValue(
                shdrSpot,
                spots[i].posLoc,
                spots[i].pos,
                ShaderUniformDataType.Vec2
            );
            Raylib.SetShaderValue(
                shdrSpot,
                spots[i].innerLoc,
                spots[i].inner,
                ShaderUniformDataType.Float
            );
            Raylib.SetShaderValue(
                shdrSpot,
                spots[i].radiusLoc,
                spots[i].radius,
                ShaderUniformDataType.Float
            );
        }
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        frameCounter++;

        // Move the stars, resetting them if the go offscreen
        for (var n = 0; n < MaxStars; n++)
        {
            UpdateStar(ref stars[n]);
        }

        // Update the spots, send them to the shader
        for (var i = 0; i < MaxSpots; i++)
        {
            if (i == 0)
            {
                var mp = GetMousePosition();
                spots[i].pos.X = mp.X;
                spots[i].pos.Y = screenHeight - mp.Y;
            }
            else
            {
                spots[i].pos.X += spots[i].vel.X;
                spots[i].pos.Y += spots[i].vel.Y;

                if (spots[i].pos.X < 64)
                {
                    spots[i].vel.X = -spots[i].vel.X;
                }

                if (spots[i].pos.X > (screenWidth - 64))
                {
                    spots[i].vel.X = -spots[i].vel.X;
                }

                if (spots[i].pos.Y < 64)
                {
                    spots[i].vel.Y = -spots[i].vel.Y;
                }

                if (spots[i].pos.Y > (screenHeight - 64))
                {
                    spots[i].vel.Y = -spots[i].vel.Y;
                }
            }

            Raylib.SetShaderValue(
                shdrSpot,
                spots[i].posLoc,
                spots[i].pos,
                ShaderUniformDataType.Vec2
            );
        }

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.DarkBlue);

        // Draw stars and bobs
        for (var n = 0; n < MaxStars; n++)
        {
            // Single pixel is just too small these days!
            DrawRectangle((int)stars[n].pos.X, (int)stars[n].pos.Y, 2, 2, Color.White);
        }

        for (var i = 0; i < 16; i++)
        {
            DrawTexture(
                texRay,
                (int)((screenWidth / 2.0) + MathF.Cos((frameCounter + i * 8) / 51.45f) * (screenWidth / 2.2) - 32),
                (int)((screenHeight / 2.0) + MathF.Sin((frameCounter + i * 8) / 17.87f) * (screenHeight / 4.2)),
                Color.White
            );
        }

        // Draw spot lights
        BeginShaderMode(shdrSpot);

        // Instead of a blank rectangle you could render a render texture of the full screen used to do screen
        // scaling (slight adjustment to shader would be required to actually pay attention to the colour!)
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

        DrawFPS(10, 10);

        DrawText("Move the mouse!", 10, 30, 20, Color.Green);
        DrawText("Pitch Black", (int)(screenWidth * 0.2f), screenHeight / 2, 20, Color.Green);
        DrawText("Dark", (int)(screenWidth * 0.66f), screenHeight / 2, 20, Color.Green);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texRay);
        UnloadShader(shdrSpot);
    }

    private static void ResetStar(ref Star s)
    {
        s.pos = new Vector2(GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f);

        do
        {
            s.vel.X = (float)GetRandomValue(-1000, 1000) / 100.0f;
            s.vel.Y = (float)GetRandomValue(-1000, 1000) / 100.0f;
        } while (!((MathF.Abs(s.vel.X) + (MathF.Abs(s.vel.Y)) > 1)));

        s.pos += s.vel * new Vector2(8.0f, 8.0f);
    }

    private static void UpdateStar(ref Star s)
    {
        s.pos += s.vel;

        if ((s.pos.X < 0) || (s.pos.X > GetScreenWidth()) ||
            (s.pos.Y < 0) || (s.pos.Y > GetScreenHeight()))
        {
            ResetStar(ref s);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - spotlight rendering");
        HideCursor();

        SetTargetFPS(60);               // Set  to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Spotlight();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
