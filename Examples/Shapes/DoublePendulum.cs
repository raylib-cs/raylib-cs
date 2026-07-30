/*******************************************************************************************
*
*   raylib [shapes] example - double pendulum
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by JoeCheong (@Joecheong2006) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 JoeCheong (@Joecheong2006)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class DoublePendulum : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    // Constant for Simulation
    private const int SIMULATION_STEPS = 30;
    private const float G = 9.81f;

    public string Name => "Shapes / Double Pendulum";

    public string Title => "raylib [shapes] example - double pendulum";

    public ConfigFlags ConfigFlags => ConfigFlags.HighDpiWindow;

    // Simulation Parameters
    private float l1, m1, theta1, w1;
    private float l2, m2, theta2, w2;
    private float lengthScaler;
    private float totalM;

    private Vector2 previousPosition;

    // Scale length
    private float L1;
    private float L2;

    // Draw parameters
    private float lineThick, trailThick;
    private float fateAlpha;

    // Create framebuffer
    private RenderTexture2D target;

    // Calculate pendulum end point
    private static Vector2 CalculatePendulumEndPoint(float l, float theta)
    {
        return new(10 * l * MathF.Sin(theta), 10 * l * MathF.Cos(theta));
    }

    // Calculate double pendulum end point
    private static Vector2 CalculateDoublePendulumEndPoint(float l1, float theta1, float l2, float theta2)
    {
        Vector2 endpoint1 = CalculatePendulumEndPoint(l1, theta1);
        Vector2 endpoint2 = CalculatePendulumEndPoint(l2, theta2);
        return new(endpoint1.X + endpoint2.X, endpoint1.Y + endpoint2.Y);
    }

    public void Init()
    {
        // Simulation Parameters
        l1 = 15.0f;
        m1 = 0.2f;
        theta1 = DEG2RAD * 170;
        w1 = 0;
        l2 = 15.0f;
        m2 = 0.1f;
        theta2 = DEG2RAD * 0;
        w2 = 0;
        lengthScaler = 0.1f;
        totalM = m1 + m2;

        previousPosition = CalculateDoublePendulumEndPoint(l1, theta1, l2, theta2);
        previousPosition.X += ((float)screenWidth / 2);
        previousPosition.Y += ((float)screenHeight / 2 - 100);

        // Scale length
        L1 = l1 * lengthScaler;
        L2 = l2 * lengthScaler;

        // Draw parameters
        lineThick = 20;
        trailThick = 2;
        fateAlpha = 0.01f;

        // Create framebuffer
        target = LoadRenderTexture(screenWidth, screenHeight);
        SetTextureFilter(target.Texture, TextureFilter.Bilinear);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float dt = GetFrameTime();
        float step = dt / SIMULATION_STEPS, step2 = step * step;

        // Update Physics - larger steps = better approximation
        for (int i = 0; i < SIMULATION_STEPS; i++)
        {
            float delta = theta1 - theta2;
            float sinD = MathF.Sin(delta), cosD = MathF.Cos(delta), cos2D = MathF.Cos(2 * delta);
            float ww1 = w1 * w1, ww2 = w2 * w2;

            // Calculate a1
            float a1 = (-G * (2 * m1 + m2) * MathF.Sin(theta1)
                         - m2 * G * MathF.Sin(theta1 - 2 * theta2)
                         - 2 * sinD * m2 * (ww2 * L2 + ww1 * L1 * cosD))
                        / (L1 * (2 * m1 + m2 - m2 * cos2D));

            // Calculate a2
            float a2 = (2 * sinD * (ww1 * L1 * totalM
                         + G * totalM * MathF.Cos(theta1)
                         + ww2 * L2 * m2 * cosD))
                        / (L2 * (2 * m1 + m2 - m2 * cos2D));

            // Update thetas
            theta1 += w1 * step + 0.5f * a1 * step2;
            theta2 += w2 * step + 0.5f * a2 * step2;

            // Update omegas
            w1 += a1 * step;
            w2 += a2 * step;
        }

        // Calculate position
        Vector2 currentPosition = CalculateDoublePendulumEndPoint(l1, theta1, l2, theta2);
        currentPosition.X += (float)screenWidth / 2;
        currentPosition.Y += (float)screenHeight / 2 - 100;

        // Draw to render texture
        BeginTextureMode(target);
        // Draw a transparent rectangle - smaller alpha = longer trails
        DrawRectangle(0, 0, screenWidth, screenHeight, Fade(Color.Black, fateAlpha));

        // Draw trail
        DrawCircleV(previousPosition, trailThick, Color.Red);
        DrawLineEx(previousPosition, currentPosition, trailThick * 2, Color.Red);
        EndTextureMode();

        // Update previous position
        previousPosition = currentPosition;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.Black);

        // Draw trails texture
        DrawTextureRec(target.Texture, new Rectangle(0, 0, (float)target.Texture.Width, (float)-target.Texture.Height), new Vector2(0, 0), Color.White);

        // Draw double pendulum
        DrawRectanglePro(new Rectangle(screenWidth / 2.0f, screenHeight / 2.0f - 100, 10 * l1, lineThick),
            new Vector2(0, lineThick * 0.5f), 90 - RAD2DEG * theta1, Color.RayWhite);

        Vector2 endpoint1 = CalculatePendulumEndPoint(l1, theta1);
        DrawRectanglePro(new Rectangle(screenWidth / 2.0f + endpoint1.X, screenHeight / 2.0f - 100 + endpoint1.Y, 10 * l2, lineThick),
            new Vector2(0, lineThick * 0.5f), 90 - RAD2DEG * theta2, Color.RayWhite);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(target);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.HighDpiWindow);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - double pendulum");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new DoublePendulum();
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
