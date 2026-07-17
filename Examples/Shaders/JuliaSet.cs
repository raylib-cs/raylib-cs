/*******************************************************************************************
*
*   raylib [shaders] example - julia set
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example originally created with raylib 2.5, last time updated with raylib 4.0
*
*   Example contributed by Josh Colclough (@joshcol9232) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Josh Colclough (@joshcol9232) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class JuliaSet : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;
    private const float zoomSpeed = 1.01f;
    private const float offsetSpeedMul = 2.0f;

    private const float startingZoom = 0.75f;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Julia Set";

    public string Title => "raylib [shaders] example - julia set";

    // A few good julia sets
    private float[][] PointsOfInterest = new float[][] {
            new float[] { -0.348827f, 0.607167f },
            new float[] { -0.786268f, 0.169728f },
            new float[] { -0.8f, 0.156f },
            new float[] { 0.285f, 0.0f },
            new float[] { -0.835f, -0.2321f },
            new float[] { -0.70176f, -0.3842f },
        };

    private Shader shader;
    private RenderTexture2D target;
    private float[] c;
    private float[] offset;
    private float zoom;
    private int cLoc;
    private int zoomLoc;
    private int offsetLoc;
    private int incrementSpeed;
    private bool showControls;

    public void Init()
    {
        // Load julia set shader
        // NOTE: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/julia_set.fs");

        // Create a RenderTexture2D to be used for render to texture
        target = LoadRenderTexture(screenWidth, screenHeight);

        // c constant to use in z^2 + c
        c = new float[] { PointsOfInterest[0][0], PointsOfInterest[0][1] };

        // Offset and zoom to draw the julia set at. (centered on screen and default size)
        offset = new float[] { 0, 0 };
        zoom = startingZoom;

        // Get variable (uniform) locations on the shader to connect with the program
        // NOTE: If uniform variable could not be found in the shader, function returns -1
        cLoc = GetShaderLocation(shader, "c");
        zoomLoc = GetShaderLocation(shader, "zoom");
        offsetLoc = GetShaderLocation(shader, "offset");

        // Upload the shader uniform values!
        Raylib.SetShaderValue(shader, cLoc, c, ShaderUniformDataType.Vec2);
        Raylib.SetShaderValue(shader, zoomLoc, zoom, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, offsetLoc, offset, ShaderUniformDataType.Vec2);

        // Multiplier of speed to change c value
        incrementSpeed = 0;
        // Show controls
        showControls = true;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Press [1 - 6] to reset c to a point of interest
        if (IsKeyPressed(KeyboardKey.One) ||
            IsKeyPressed(KeyboardKey.Two) ||
            IsKeyPressed(KeyboardKey.Three) ||
            IsKeyPressed(KeyboardKey.Four) ||
            IsKeyPressed(KeyboardKey.Five) ||
            IsKeyPressed(KeyboardKey.Six))
        {

            if (IsKeyPressed(KeyboardKey.One))
            {
                c[0] = PointsOfInterest[0][0];
                c[1] = PointsOfInterest[0][1];
            }
            else if (IsKeyPressed(KeyboardKey.Two))
            {
                c[0] = PointsOfInterest[1][0];
                c[1] = PointsOfInterest[1][1];
            }
            else if (IsKeyPressed(KeyboardKey.Three))
            {
                c[0] = PointsOfInterest[2][0];
                c[1] = PointsOfInterest[2][1];
            }
            else if (IsKeyPressed(KeyboardKey.Four))
            {
                c[0] = PointsOfInterest[3][0];
                c[1] = PointsOfInterest[3][1];
            }
            else if (IsKeyPressed(KeyboardKey.Five))
            {
                c[0] = PointsOfInterest[4][0];
                c[1] = PointsOfInterest[4][1];
            }
            else if (IsKeyPressed(KeyboardKey.Six))
            {
                c[0] = PointsOfInterest[5][0];
                c[1] = PointsOfInterest[5][1];
            }
            Raylib.SetShaderValue(shader, cLoc, c, ShaderUniformDataType.Vec2);
        }

        // If "R" is pressed, reset zoom and offset
        if (IsKeyPressed(KeyboardKey.R))
        {
            zoom = startingZoom;
            offset[0] = 0.0f;
            offset[1] = 0.0f;
            Raylib.SetShaderValue(shader, zoomLoc, zoom, ShaderUniformDataType.Float);
            Raylib.SetShaderValue(shader, offsetLoc, offset, ShaderUniformDataType.Vec2);
        }

        // Pause animation (c change)
        if (IsKeyPressed(KeyboardKey.Space))
        {
            incrementSpeed = 0;
        }

        // Toggle whether or not to show controls
        if (IsKeyPressed(KeyboardKey.F1))
        {
            showControls = !showControls;
        }

        if (IsKeyPressed(KeyboardKey.Right))
        {
            incrementSpeed++;
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            incrementSpeed--;
        }

        // If either left or right button is pressed, zoom in/out
        if (IsMouseButtonDown(MouseButton.Left) || IsMouseButtonDown(MouseButton.Right))
        {
            if (IsMouseButtonDown(MouseButton.Left))
            {
                zoom *= zoomSpeed;
            }

            if (IsMouseButtonDown(MouseButton.Right))
            {
                zoom *= 1.0f / zoomSpeed;
            }

            var mousePos = GetMousePosition();
            var offsetVelocity = Vector2.Zero;

            offsetVelocity.X = (mousePos.X / screenWidth - 0.5f) * offsetSpeedMul / zoom;
            offsetVelocity.Y = (mousePos.Y / screenHeight - 0.5f) * offsetSpeedMul / zoom;

            // Apply move velocity to camera
            offset[0] += GetFrameTime() * offsetVelocity.X;
            offset[1] += GetFrameTime() * offsetVelocity.Y;

            Raylib.SetShaderValue(shader, zoomLoc, zoom, ShaderUniformDataType.Float);
            Raylib.SetShaderValue(shader, offsetLoc, offset, ShaderUniformDataType.Vec2);
        }

        // Increment c value with time
        var dc = GetFrameTime() * incrementSpeed * 0.0005f;
        c[0] += dc;
        c[1] += dc;

        Raylib.SetShaderValue(shader, cLoc, c, ShaderUniformDataType.Vec2);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Using a render texture to draw Julia set
        BeginTextureMode(target);
        ClearBackground(Color.Black);

        // Draw a rectangle in shader mode to be used as shader canvas
        // NOTE: Rectangle uses font white character texture coordinates,
        // so shader can not be applied here directly because input vertexTexCoord
        // do not represent full screen coordinates (space where want to apply shader)
        DrawRectangle(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Black);
        EndTextureMode();

        BeginDrawing();
        ClearBackground(Color.Black);

        // Draw the saved texture and rendered julia set with shader
        // NOTE: We do not invert texture on Y, already considered inside shader
        BeginShaderMode(shader);
        // WARNING: If FLAG_WINDOW_HIGHDPI is enabled, HighDPI monitor scaling should be considered
        // when rendering the RenderTexture2D to fit in the HighDPI scaled Window
        DrawTextureEx(target.Texture, new Vector2(0.0f, 0.0f), 0.0f, 1.0f, Color.White);
        EndShaderMode();

        if (showControls)
        {
            DrawText("Press Mouse buttons right/left to zoom in/out and move", 10, 15, 10, Color.RayWhite);
            DrawText("Press KEY_F1 to toggle these controls", 10, 30, 10, Color.RayWhite);
            DrawText("Press KEYS [1 - 6] to change point of interest", 10, 45, 10, Color.RayWhite);
            DrawText("Press KEY_LEFT | KEY_RIGHT to change speed", 10, 60, 10, Color.RayWhite);
            DrawText("Press KEY_SPACE to stop movement animation", 10, 75, 10, Color.RayWhite);
            DrawText("Press KEY_R to recenter the camera", 10, 90, 10, Color.RayWhite);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);
        UnloadRenderTexture(target);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - julia set");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new JuliaSet();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
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
