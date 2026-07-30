/*******************************************************************************************
*
*   raylib [core] example - 2d camera platformer
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.0
*
*   Example contributed by arvyy (@arvyy) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 arvyy (@arvyy)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Core;

public partial class Camera2dPlatformer : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int G = 400;
    private const float PlayerJumpSpeed = 350.0f;
    private const float PlayerHorSpeed = 200.0f;

    public string Name => "Core / 2D Camera Platformer";

    public string Title => "raylib [core] example - 2d camera platformer";

    private struct Player
    {
        public Vector2 Position;
        public float Speed;
        public bool CanJump;
    }

    private struct EnvItem
    {
        public Rectangle Rect;
        public int Blocking;
        public Color Color;

        public EnvItem(Rectangle rect, int blocking, Color color)
        {
            this.Rect = rect;
            this.Blocking = blocking;
            this.Color = color;
        }
    }

    private delegate void CameraUpdaterCallback(
        ref Camera2D camera,
        ref Player player,
        EnvItem[] envItems,
        float delta,
        int width,
        int height
    );

    private Player player;
    private EnvItem[] envItems;
    private Camera2D camera;
    private CameraUpdaterCallback[] cameraUpdaters;
    private int cameraOption;
    private int cameraUpdatersLength;
    private string[] cameraDescriptions;

    public void Init()
    {
        player = new();
        player.Position = new Vector2(400, 280);
        player.Speed = 0;
        player.CanJump = false;

        envItems = new EnvItem[]
        {
            new EnvItem(new Rectangle(0, 0, 1000, 400), 0, Color.LightGray),
            new EnvItem(new Rectangle(0, 400, 1000, 200), 1, Color.Gray),
            new EnvItem(new Rectangle(300, 200, 400, 10), 1, Color.Gray),
            new EnvItem(new Rectangle(250, 300, 100, 10), 1, Color.Gray),
            new EnvItem(new Rectangle(650, 300, 100, 10), 1, Color.Gray)
        };

        camera = new();
        camera.Target = player.Position;
        camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;

        // Store pointers to the multiple update camera functions
        cameraUpdaters = new CameraUpdaterCallback[]
        {
            UpdateCameraCenter,
            UpdateCameraCenterInsideMap,
            UpdateCameraCenterSmoothFollow,
            UpdateCameraEvenOutOnLanding,
            UpdateCameraPlayerBoundsPush
        };

        cameraOption = 0;
        cameraUpdatersLength = cameraUpdaters.Length;

        cameraDescriptions = new string[]{
            "Follow player center",
            "Follow player center, but clamp to map edges",
            "Follow player center; smoothed",
            "Follow player center horizontally; update player center vertically after landing",
            "Player push camera on getting too close to screen edge"
        };
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var deltaTime = GetFrameTime();

        UpdatePlayer(ref player, envItems, deltaTime);

        camera.Zoom += ((float)GetMouseWheelMove() * 0.05f);

        if (camera.Zoom > 3.0f)
        {
            camera.Zoom = 3.0f;
        }
        else if (camera.Zoom < 0.25f)
        {
            camera.Zoom = 0.25f;
        }

        if (IsKeyPressed(KeyboardKey.R))
        {
            camera.Zoom = 1.0f;
            player.Position = new Vector2(400, 280);
        }

        if (IsKeyPressed(KeyboardKey.C))
        {
            cameraOption = (cameraOption + 1) % cameraUpdatersLength;
        }

        // Call update camera function by its pointer
        cameraUpdaters[cameraOption](ref camera, ref player, envItems, deltaTime, screenWidth, screenHeight);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.LightGray);

        BeginMode2D(camera);

        for (var i = 0; i < envItems.Length; i++)
        {
            DrawRectangleRec(envItems[i].Rect, envItems[i].Color);
        }

        Rectangle playerRect = new(player.Position.X - 20, player.Position.Y - 40, 40.0f, 40.0f);
        DrawRectangleRec(playerRect, Color.Red);

        DrawCircleV(player.Position, 5.0f, Color.Gold);

        EndMode2D();

        DrawText("Controls:", 20, 20, 10, Color.Black);
        DrawText("- Right/Left to move", 40, 40, 10, Color.DarkGray);
        DrawText("- Space to jump", 40, 60, 10, Color.DarkGray);
        DrawText("- Mouse Wheel to Zoom in-out", 40, 80, 10, Color.DarkGray);
        DrawText("- R to reset position + zoom", 40, 100, 10, Color.DarkGray);
        DrawText("- C to change camera mode", 40, 120, 10, Color.DarkGray);
        DrawText("Current camera mode:", 20, 140, 10, Color.Black);
        DrawText(cameraDescriptions[cameraOption], 40, 160, 10, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    private static void UpdatePlayer(ref Player player, EnvItem[] envItems, float delta)
    {
        if (IsKeyDown(KeyboardKey.Left))
        {
            player.Position.X -= PlayerHorSpeed * delta;
        }

        if (IsKeyDown(KeyboardKey.Right))
        {
            player.Position.X += PlayerHorSpeed * delta;
        }

        if (IsKeyDown(KeyboardKey.Space) && player.CanJump)
        {
            player.Speed = -PlayerJumpSpeed;
            player.CanJump = false;
        }

        var hitObstacle = 0;
        for (var i = 0; i < envItems.Length; i++)
        {
            var ei = envItems[i];
            var p = player.Position;
            if (ei.Blocking != 0 &&
                ei.Rect.X <= p.X &&
                ei.Rect.X + ei.Rect.Width >= p.X &&
                ei.Rect.Y >= p.Y &&
                ei.Rect.Y <= p.Y + player.Speed * delta)
            {
                hitObstacle = 1;
                player.Speed = 0.0f;
                player.Position.Y = ei.Rect.Y;
                break;
            }
        }

        if (hitObstacle == 0)
        {
            player.Position.Y += player.Speed * delta;
            player.Speed += G * delta;
            player.CanJump = false;
        }
        else
        {
            player.CanJump = true;
        }
    }

    private static void UpdateCameraCenter(
        ref Camera2D camera,
        ref Player player,
        EnvItem[] envItems,
        float delta,
        int width,
        int height
    )
    {
        camera.Offset = new Vector2(width / 2.0f, height / 2.0f);
        camera.Target = player.Position;
    }

    private static void UpdateCameraCenterInsideMap(
        ref Camera2D camera,
        ref Player player,
        EnvItem[] envItems,
        float delta,
        int width,
        int height)
    {
        camera.Target = player.Position;
        camera.Offset = new Vector2(width / 2.0f, height / 2.0f);
        float minX = 1000, minY = 1000, maxX = -1000, maxY = -1000;

        for (var i = 0; i < envItems.Length; i++)
        {
            var ei = envItems[i];
            minX = Math.Min(ei.Rect.X, minX);
            maxX = Math.Max(ei.Rect.X + ei.Rect.Width, maxX);
            minY = Math.Min(ei.Rect.Y, minY);
            maxY = Math.Max(ei.Rect.Y + ei.Rect.Height, maxY);
        }

        var max = GetWorldToScreen2D(new Vector2(maxX, maxY), camera);
        var min = GetWorldToScreen2D(new Vector2(minX, minY), camera);

        if (max.X < width)
        {
            camera.Offset.X = width - (max.X - width / 2.0f);
        }

        if (max.Y < height)
        {
            camera.Offset.Y = height - (max.Y - height / 2.0f);
        }

        if (min.X > 0)
        {
            camera.Offset.X = width / 2.0f - min.X;
        }

        if (min.Y > 0)
        {
            camera.Offset.Y = height / 2.0f - min.Y;
        }
    }

    private static void UpdateCameraCenterSmoothFollow(
        ref Camera2D camera,
        ref Player player,
        EnvItem[] envItems,
        float delta,
        int width,
        int height
    )
    {
        const float minSpeed = 30;
        const float minEffectLength = 10;
        const float fractionSpeed = 0.8f;

        camera.Offset = new Vector2(width / 2.0f, height / 2.0f);
        var diff = Vector2Subtract(player.Position, camera.Target);
        var length = Vector2Length(diff);

        if (length > minEffectLength)
        {
            var speed = Math.Max(fractionSpeed * length, minSpeed);
            camera.Target = Vector2Add(camera.Target, Vector2Scale(diff, speed * delta / length));
        }
    }

    private static void UpdateCameraEvenOutOnLanding(
        ref Camera2D camera,
        ref Player player,
        EnvItem[] envItems,
        float delta,
        int width,
        int height
    )
    {
        float evenOutSpeed = 700;
        var eveningOut = 0;
        var evenOutTarget = 0.0f;

        camera.Offset = new Vector2(width / 2.0f, height / 2.0f);
        camera.Target.X = player.Position.X;

        if (eveningOut != 0)
        {
            if (evenOutTarget > camera.Target.Y)
            {
                camera.Target.Y += evenOutSpeed * delta;

                if (camera.Target.Y > evenOutTarget)
                {
                    camera.Target.Y = evenOutTarget;
                    eveningOut = 0;
                }
            }
            else
            {
                camera.Target.Y -= evenOutSpeed * delta;

                if (camera.Target.Y < evenOutTarget)
                {
                    camera.Target.Y = evenOutTarget;
                    eveningOut = 0;
                }
            }
        }
        else
        {
            if (player.CanJump && (player.Speed == 0) && (player.Position.Y != camera.Target.Y))
            {
                eveningOut = 1;
                evenOutTarget = player.Position.Y;
            }
        }
    }

    private static void UpdateCameraPlayerBoundsPush(
        ref Camera2D camera,
        ref Player player,
        EnvItem[] envItems,
        float delta,
        int width,
        int height
    )
    {
        Vector2 bbox = new(0.2f, 0.2f);

        var bboxWorldMin = GetScreenToWorld2D(
            new Vector2((1 - bbox.X) * 0.5f * width, (1 - bbox.Y) * 0.5f * height),
            camera
        );
        var bboxWorldMax = GetScreenToWorld2D(
            new Vector2((1 + bbox.X) * 0.5f * width,
            (1 + bbox.Y) * 0.5f * height),
            camera
        );
        camera.Offset = new Vector2((1 - bbox.X) * 0.5f * width, (1 - bbox.Y) * 0.5f * height);

        if (player.Position.X < bboxWorldMin.X)
        {
            camera.Target.X = player.Position.X;
        }

        if (player.Position.Y < bboxWorldMin.Y)
        {
            camera.Target.Y = player.Position.Y;
        }

        if (player.Position.X > bboxWorldMax.X)
        {
            camera.Target.X = bboxWorldMin.X + (player.Position.X - bboxWorldMax.X);
        }

        if (player.Position.Y > bboxWorldMax.Y)
        {
            camera.Target.Y = bboxWorldMin.Y + (player.Position.Y - bboxWorldMax.Y);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 2d camera platformer");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new Camera2dPlatformer();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
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
