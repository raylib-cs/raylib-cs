/*******************************************************************************************
*
*   raylib [shaders] example - mandelbrot set
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3)
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Jordi Santonja (@JordSant)
*   Based on previous work by Josh Colclough (@joshcol9232)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jordi Santonja (@JordSant)
*
********************************************************************************************/

namespace Examples.Shaders;

public partial class MandelbrotSet : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Mandelbrot Set";

    public string Title => "raylib [shaders] example - mandelbrot set";

    // A few good interesting places
    private static readonly float[][] pointsOfInterest = new[]
    {
        new[] { -1.76826775f, -0.00422996283f, 28435.9238f },
        new[] { 0.322004497f, -0.0357099883f, 56499.7266f },
        new[] { -0.748880744f, -0.0562955774f, 9237.59082f },
        new[] { -1.78385007f, -0.0156200649f, 14599.5283f },
        new[] { -0.0985441282f, -0.924688697f, 26259.8535f },
        new[] { 0.317785531f, -0.0322612226f, 29297.9258f },
    };

    private const float zoomSpeed = 1.01f;
    private const float offsetSpeedMul = 2.0f;

    private const float startingZoom = 0.6f;
    private static readonly float[] startingOffset = { -0.5f, 0.0f };

    private Shader shader;
    private RenderTexture2D target;
    private float[] offset;
    private float zoom;
    private int maxIterations;
    private float maxIterationsMultiplier;
    private int zoomLoc;
    private int offsetLoc;
    private int maxIterationsLoc;
    private bool showControls;

    public void Init()
    {
        // Load mandelbrot set shader
        // NOTE: Defining null (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/mandelbrot_set.fs");

        // Create a RenderTexture2D to be used for render to texture
        target = LoadRenderTexture(GetScreenWidth(), GetScreenHeight());

        // Offset and zoom to draw the mandelbrot set at. (centered on screen and default size)
        offset = new[] { startingOffset[0], startingOffset[1] };
        zoom = startingZoom;

        // Depending on the zoom the maximum number of iterations must be adapted to get more detail as we zoom in
        // The solution is not perfect, so a control has been added to increase/decrease the number of iterations with UP/DOWN keys
#if BROWSER
        maxIterations = 43;
        maxIterationsMultiplier = 22.0f;
#else
        maxIterations = 333;
        maxIterationsMultiplier = 166.5f;
#endif

        // Get variable (uniform) locations on the shader to connect with the program
        // NOTE: If uniform variable could not be found in the shader, function returns -1
        zoomLoc = GetShaderLocation(shader, "zoom");
        offsetLoc = GetShaderLocation(shader, "offset");
        maxIterationsLoc = GetShaderLocation(shader, "maxIterations");

        // Upload the shader uniform values!
        Raylib.SetShaderValue(shader, zoomLoc, zoom, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, offsetLoc, offset, ShaderUniformDataType.Vec2);
        Raylib.SetShaderValue(shader, maxIterationsLoc, maxIterations, ShaderUniformDataType.Int);

        showControls = true;           // Show controls
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var updateShader = false;

        // Press [1 - 6] to reset c to a point of interest
        if (IsKeyPressed(KeyboardKey.One) ||
            IsKeyPressed(KeyboardKey.Two) ||
            IsKeyPressed(KeyboardKey.Three) ||
            IsKeyPressed(KeyboardKey.Four) ||
            IsKeyPressed(KeyboardKey.Five) ||
            IsKeyPressed(KeyboardKey.Six))
        {
            var interestIndex = 0;
            if (IsKeyPressed(KeyboardKey.One))
            {
                interestIndex = 0;
            }
            else if (IsKeyPressed(KeyboardKey.Two))
            {
                interestIndex = 1;
            }
            else if (IsKeyPressed(KeyboardKey.Three))
            {
                interestIndex = 2;
            }
            else if (IsKeyPressed(KeyboardKey.Four))
            {
                interestIndex = 3;
            }
            else if (IsKeyPressed(KeyboardKey.Five))
            {
                interestIndex = 4;
            }
            else if (IsKeyPressed(KeyboardKey.Six))
            {
                interestIndex = 5;
            }

            offset[0] = pointsOfInterest[interestIndex][0];
            offset[1] = pointsOfInterest[interestIndex][1];
            zoom = pointsOfInterest[interestIndex][2];
            updateShader = true;
        }

        // If "R" is pressed, reset zoom and offset
        if (IsKeyPressed(KeyboardKey.R))
        {
            offset[0] = startingOffset[0];
            offset[1] = startingOffset[1];
            zoom = startingZoom;
            updateShader = true;
        }

        if (IsKeyPressed(KeyboardKey.F1))
        {
            showControls = !showControls;  // Toggle whether or not to show controls
        }

        // Change number of max iterations with UP and DOWN keys
        // WARNING: Increasing the number of max iterations greatly impacts performance
        if (IsKeyPressed(KeyboardKey.Up))
        {
            maxIterationsMultiplier *= 1.4f;
            updateShader = true;
        }
        else if (IsKeyPressed(KeyboardKey.Down))
        {
            maxIterationsMultiplier /= 1.4f;
            updateShader = true;
        }

        // If either left or right button is pressed, zoom in/out
        if (IsMouseButtonDown(MouseButton.Left) || IsMouseButtonDown(MouseButton.Right))
        {
            // Change zoom. If Mouse left -> zoom in. Mouse right -> zoom out
            zoom *= IsMouseButtonDown(MouseButton.Left) ? zoomSpeed : (1.0f / zoomSpeed);

            var mousePos = GetMousePosition();
            Vector2 offsetVelocity;
            // Find the velocity at which to change the camera. Take the distance of the mouse
            // From the center of the screen as the direction, and adjust magnitude based on the current zoom
            offsetVelocity.X = (mousePos.X / (float)screenWidth - 0.5f) * offsetSpeedMul / zoom;
            offsetVelocity.Y = (mousePos.Y / (float)screenHeight - 0.5f) * offsetSpeedMul / zoom;

            // Apply move velocity to camera
            offset[0] += GetFrameTime() * offsetVelocity.X;
            offset[1] += GetFrameTime() * offsetVelocity.Y;

            updateShader = true;
        }

        // In case a parameter has been changed, update the shader values
        if (updateShader)
        {
            // As we zoom in, increase the number of max iterations to get more detail
            // Aproximate formula, but it works-ish
            maxIterations = (int)(MathF.Sqrt(2.0f * MathF.Sqrt(MathF.Abs(1.0f - MathF.Sqrt(37.5f * zoom)))) * maxIterationsMultiplier);

            // Update the shader uniform values!
            Raylib.SetShaderValue(shader, zoomLoc, zoom, ShaderUniformDataType.Float);
            Raylib.SetShaderValue(shader, offsetLoc, offset, ShaderUniformDataType.Vec2);
            Raylib.SetShaderValue(shader, maxIterationsLoc, maxIterations, ShaderUniformDataType.Int);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Using a render texture to draw Mandelbrot set
        BeginTextureMode(target);       // Enable drawing to texture
        ClearBackground(Color.Black);   // Clear the render texture

        // Draw a rectangle in shader mode to be used as shader canvas
        // NOTE: Rectangle uses font white character texture coordinates,
        // So shader can not be applied here directly because input vertexTexCoord
        // Do not represent full screen coordinates (space where want to apply shader)
        DrawRectangle(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Black);
        EndTextureMode();

        BeginDrawing();
        ClearBackground(Color.Black);   // Clear screen background

        // Draw the saved texture and rendered mandelbrot set with shader
        // NOTE: We do not invert texture on Y, already considered inside shader
        BeginShaderMode(shader);
        // WARNING: If FLAG_WINDOW_HIGHDPI is enabled, HighDPI monitor scaling should be considered
        // When rendering the RenderTexture2D to fit in the HighDPI scaled Window
        DrawTextureEx(target.Texture, new Vector2(0.0f, 0.0f), 0.0f, 1.0f, Color.White);
        EndShaderMode();

        if (showControls)
        {
            DrawText("Press Mouse buttons right/left to zoom in/out and move", 10, 15, 10, Color.RayWhite);
            DrawText("Press F1 to toggle these controls", 10, 30, 10, Color.RayWhite);
            DrawText("Press [1 - 6] to change point of interest", 10, 45, 10, Color.RayWhite);
            DrawText("Press UP | DOWN to change number of iterations", 10, 60, 10, Color.RayWhite);
            DrawText("Press R to recenter the camera", 10, 75, 10, Color.RayWhite);
        }
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);               // Unload shader
        UnloadRenderTexture(target);        // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - mandelbrot set");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new MandelbrotSet();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                      // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
