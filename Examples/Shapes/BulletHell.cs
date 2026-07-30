/*******************************************************************************************
*
*   raylib [shapes] example - bullet hell
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example contributed by Zero (@zerohorsepower) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Zero (@zerohorsepower)
*
********************************************************************************************/

namespace Examples.Shapes;

public partial class BulletHell : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_BULLETS = 500000;      // Max bullets to be processed

    public string Name => "Shapes / Bullet Hell";

    public string Title => "raylib [shapes] example - bullet hell";

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    private struct Bullet
    {
        public Vector2 position;       // Bullet position on screen
        public Vector2 acceleration;   // Amount of pixels to be incremented to position every frame
        public bool disabled;          // Skip processing and draw case out of screen
        public Color color;            // Bullet color
    }

    // Bullets definition
    private Bullet[] bullets;
    private int bulletCount;
    private int bulletDisabledCount; // Used to calculate how many bullets are on screen
    private int bulletRadius;
    private float bulletSpeed;
    private int bulletRows;
    private Color[] bulletColor;

    // Spawner variables
    private float baseDirection;
    private int angleIncrement; // After spawn all bullet rows, increment this value on the baseDirection for next the frame
    private float spawnCooldown;
    private float spawnCooldownTimer;

    // Magic circle
    private float magicCircleRotation;

    // Used on performance drawing
    private RenderTexture2D bulletTexture;

    private bool drawInPerformanceMode; // Switch between DrawCircle() and DrawTexture()

    public void Init()
    {
        // Bullets definition
        bullets = new Bullet[MAX_BULLETS]; // Bullets array
        bulletCount = 0;
        bulletDisabledCount = 0;
        bulletRadius = 10;
        bulletSpeed = 3.0f;
        bulletRows = 6;
        bulletColor = new[] { Color.Red, Color.Blue };

        // Spawner variables
        baseDirection = 0;
        angleIncrement = 5;
        spawnCooldown = 2;
        spawnCooldownTimer = spawnCooldown;

        // Magic circle
        magicCircleRotation = 0;

        // Used on performance drawing
        bulletTexture = LoadRenderTexture(24, 24);

        // Draw circle to bullet texture, then draw bullet using DrawTexture()
        // NOTE: This is done to improve the performance, since DrawCircle() is very slow
        BeginTextureMode(bulletTexture);
        DrawCircle(12, 12, (float)bulletRadius, Color.White);
        DrawCircleLines(12, 12, (float)bulletRadius, Color.Black);
        EndTextureMode();

        drawInPerformanceMode = true;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Reset the bullet index
        // New bullets will replace the old ones that are already disabled due to out-of-screen
        if (bulletCount >= MAX_BULLETS)
        {
            bulletCount = 0;
            bulletDisabledCount = 0;
        }

        spawnCooldownTimer--;
        if (spawnCooldownTimer < 0)
        {
            spawnCooldownTimer = spawnCooldown;

            // Spawn bullets
            float degreesPerRow = 360.0f / bulletRows;
            for (int row = 0; row < bulletRows; row++)
            {
                if (bulletCount < MAX_BULLETS)
                {
                    bullets[bulletCount].position = new Vector2((float)screenWidth / 2, (float)screenHeight / 2);
                    bullets[bulletCount].disabled = false;
                    bullets[bulletCount].color = bulletColor[row % 2];

                    float bulletDirection = baseDirection + (degreesPerRow * row);

                    // Bullet speed*bullet direction, this will determine how much pixels will be incremented/decremented
                    // from the bullet position every frame. Since the bullets doesn't change its direction and speed,
                    // only need to calculate it at the spawning time
                    // 0 degrees = right, 90 degrees = down, 180 degrees = left and 270 degrees = up, basically clockwise
                    // Case you want it to be anti-clockwise, add "* -1" at the y acceleration
                    bullets[bulletCount].acceleration = new Vector2(
                        bulletSpeed * MathF.Cos(bulletDirection * DEG2RAD),
                        bulletSpeed * MathF.Sin(bulletDirection * DEG2RAD)
                    );

                    bulletCount++;
                }
            }

            baseDirection += angleIncrement;
        }

        // Update bullets position based on its acceleration
        for (int i = 0; i < bulletCount; i++)
        {
            // Only update bullet if inside the screen
            if (!bullets[i].disabled)
            {
                bullets[i].position.X += bullets[i].acceleration.X;
                bullets[i].position.Y += bullets[i].acceleration.Y;

                // Disable bullet if out of screen
                if ((bullets[i].position.X < -bulletRadius * 2) ||
                    (bullets[i].position.X > screenWidth + bulletRadius * 2) ||
                    (bullets[i].position.Y < -bulletRadius * 2) ||
                    (bullets[i].position.Y > screenHeight + bulletRadius * 2))
                {
                    bullets[i].disabled = true;
                    bulletDisabledCount++;
                }
            }
        }

        // Input logic
        if ((IsKeyPressed(KeyboardKey.Right) || IsKeyPressed(KeyboardKey.D)) && (bulletRows < 359))
        {
            bulletRows++;
        }

        if ((IsKeyPressed(KeyboardKey.Left) || IsKeyPressed(KeyboardKey.A)) && (bulletRows > 1))
        {
            bulletRows--;
        }

        if (IsKeyPressed(KeyboardKey.Up) || IsKeyPressed(KeyboardKey.W))
        {
            bulletSpeed += 0.25f;
        }

        if ((IsKeyPressed(KeyboardKey.Down) || IsKeyPressed(KeyboardKey.S)) && (bulletSpeed > 0.50f))
        {
            bulletSpeed -= 0.25f;
        }

        if (IsKeyPressed(KeyboardKey.Z) && (spawnCooldown > 1))
        {
            spawnCooldown--;
        }

        if (IsKeyPressed(KeyboardKey.X))
        {
            spawnCooldown++;
        }

        if (IsKeyPressed(KeyboardKey.Enter))
        {
            drawInPerformanceMode = !drawInPerformanceMode;
        }

        if (IsKeyDown(KeyboardKey.Space))
        {
            angleIncrement += 1;
            angleIncrement %= 360;
        }

        if (IsKeyPressed(KeyboardKey.C))
        {
            bulletCount = 0;
            bulletDisabledCount = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw magic circle
        magicCircleRotation++;
        DrawRectanglePro(new Rectangle((float)screenWidth / 2, (float)screenHeight / 2, 120, 120),
            new Vector2(60.0f, 60.0f), magicCircleRotation, Color.Purple);
        DrawRectanglePro(new Rectangle((float)screenWidth / 2, (float)screenHeight / 2, 120, 120),
            new Vector2(60.0f, 60.0f), magicCircleRotation + 45, Color.Purple);
        DrawCircleLines(screenWidth / 2, screenHeight / 2, 70, Color.Black);
        DrawCircleLines(screenWidth / 2, screenHeight / 2, 50, Color.Black);
        DrawCircleLines(screenWidth / 2, screenHeight / 2, 30, Color.Black);

        // Draw bullets
        if (drawInPerformanceMode)
        {
            // Draw bullets using pre-rendered texture containing circle
            for (int i = 0; i < bulletCount; i++)
            {
                // Do not draw disabled bullets (out of screen)
                if (!bullets[i].disabled)
                {
                    DrawTexture(bulletTexture.Texture,
                        (int)(bullets[i].position.X - bulletTexture.Texture.Width * 0.5f),
                        (int)(bullets[i].position.Y - bulletTexture.Texture.Height * 0.5f),
                        bullets[i].color);
                }
            }
        }
        else
        {
            // Draw bullets using DrawCircle(), less performant
            for (int i = 0; i < bulletCount; i++)
            {
                // Do not draw disabled bullets (out of screen)
                if (!bullets[i].disabled)
                {
                    DrawCircleV(bullets[i].position, (float)bulletRadius, bullets[i].color);
                    DrawCircleLinesV(bullets[i].position, (float)bulletRadius, Color.Black);
                }
            }
        }

        // Draw UI
        DrawRectangle(10, 10, 280, 150, new Color(0, 0, 0, 200));
        DrawText("Controls:", 20, 20, 10, Color.LightGray);
        DrawText("- Right/Left or A/D: Change rows number", 40, 40, 10, Color.LightGray);
        DrawText("- Up/Down or W/S: Change bullet speed", 40, 60, 10, Color.LightGray);
        DrawText("- Z or X: Change spawn cooldown", 40, 80, 10, Color.LightGray);
        DrawText("- Space (Hold): Change the angle increment", 40, 100, 10, Color.LightGray);
        DrawText("- Enter: Switch draw method (Performance)", 40, 120, 10, Color.LightGray);
        DrawText("- C: Clear bullets", 40, 140, 10, Color.LightGray);

        DrawRectangle(610, 10, 170, 30, new Color(0, 0, 0, 200));
        if (drawInPerformanceMode)
        {
            DrawText("Draw method: DrawTexture(*)", 620, 20, 10, Color.Green);
        }
        else
        {
            DrawText("Draw method: DrawCircle(*)", 620, 20, 10, Color.Red);
        }

        DrawRectangle(135, 410, 530, 30, new Color(0, 0, 0, 200));
        DrawText($"[ FPS: {GetFPS()}, Bullets: {bulletCount - bulletDisabledCount}, Rows: {bulletRows}, Bullet speed: {bulletSpeed:F2}, Angle increment per frame: {angleIncrement}, Cooldown: {spawnCooldown:F0} ]",
            155, 420, 10, Color.Green);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(bulletTexture); // Unload bullet texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - bullet hell");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new BulletHell();
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
