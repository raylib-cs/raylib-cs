/*******************************************************************************************
*
*   raylib [core] example - 3d camera fps
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Agnis Aldiņš (@nezvers) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Agnis Aldiņš (@nezvers)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Core;

public partial class Camera3dFps : IExample
{
    // Movement constants
    private const float GRAVITY = 32.0f;
    private const float MAX_SPEED = 20.0f;
    private const float CROUCH_SPEED = 5.0f;
    private const float JUMP_FORCE = 12.0f;
    private const float MAX_ACCEL = 150.0f;
    // Grounded drag
    private const float FRICTION = 0.86f;
    // Increasing air drag, increases strafing speed
    private const float AIR_DRAG = 0.98f;
    // Responsiveness for turning movement direction to looked direction
    private const float CONTROL = 15.0f;
    private const float CROUCH_HEIGHT = 0.0f;
    private const float STAND_HEIGHT = 1.0f;
    private const float BOTTOM_HEIGHT = 0.5f;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / 3D Camera FPS";

    public string Title => "raylib [core] example - 3d camera fps";

    public bool CursorDisabled => true;

    // Body structure
    private struct Body
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Dir;
        public bool IsGrounded;
    }

    // State that was global in the C example
    private readonly Vector2 sensitivity = new Vector2(0.001f, 0.001f);

    private Body player;
    private Vector2 lookRotation;
    private float headTimer;
    private float walkLerp;
    private float headLerp;
    private Vector2 lean;

    private Camera3D camera;

    public void Init()
    {
        player = new Body();
        lookRotation = new Vector2(0, 0);
        headTimer = 0.0f;
        walkLerp = 0.0f;
        headLerp = STAND_HEIGHT;
        lean = new Vector2(0, 0);

        // Initialize camera variables
        // NOTE: UpdateCameraFPS() takes care of the rest
        camera = new Camera3D();
        camera.FovY = 60.0f;
        camera.Projection = CameraProjection.Perspective;
        camera.Position = new Vector3(
            player.Position.X,
            player.Position.Y + (BOTTOM_HEIGHT + headLerp),
            player.Position.Z);

        UpdateCameraFPS(ref camera); // Update camera parameters
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        Vector2 mouseDelta = GetMouseDelta();
        lookRotation.X -= mouseDelta.X * sensitivity.X;
        lookRotation.Y += mouseDelta.Y * sensitivity.Y;

        int sideway = (IsKeyDown(KeyboardKey.D) ? 1 : 0) - (IsKeyDown(KeyboardKey.A) ? 1 : 0);
        int forward = (IsKeyDown(KeyboardKey.W) ? 1 : 0) - (IsKeyDown(KeyboardKey.S) ? 1 : 0);
        bool crouching = IsKeyDown(KeyboardKey.LeftControl);
        UpdateBody(ref player, lookRotation.X, sideway, forward, IsKeyPressed(KeyboardKey.Space), crouching);

        float delta = GetFrameTime();
        headLerp = Lerp(headLerp, (crouching ? CROUCH_HEIGHT : STAND_HEIGHT), 20.0f * delta);
        camera.Position = new Vector3(
            player.Position.X,
            player.Position.Y + (BOTTOM_HEIGHT + headLerp),
            player.Position.Z);

        if (player.IsGrounded && ((forward != 0) || (sideway != 0)))
        {
            headTimer += delta * 3.0f;
            walkLerp = Lerp(walkLerp, 1.0f, 10.0f * delta);
            camera.FovY = Lerp(camera.FovY, 55.0f, 5.0f * delta);
        }
        else
        {
            walkLerp = Lerp(walkLerp, 0.0f, 10.0f * delta);
            camera.FovY = Lerp(camera.FovY, 60.0f, 5.0f * delta);
        }

        lean.X = Lerp(lean.X, sideway * 0.02f, 10.0f * delta);
        lean.Y = Lerp(lean.Y, forward * 0.015f, 10.0f * delta);

        UpdateCameraFPS(ref camera);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        DrawLevel();
        EndMode3D();

        // Draw info box
        DrawRectangle(5, 5, 330, 75, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(5, 5, 330, 75, Color.Blue);

        DrawText("Camera controls:", 15, 15, 10, Color.Black);
        DrawText("- Move keys: W, A, S, D, Space, Left-Ctrl", 15, 30, 10, Color.Black);
        DrawText("- Look around: arrow keys or mouse", 15, 45, 10, Color.Black);
        float velLen = Vector2Length(new Vector2(player.Velocity.X, player.Velocity.Z));
        DrawText($"- Velocity Len: ({velLen:00.000})", 15, 60, 10, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    // Update body considering current world state
    private void UpdateBody(ref Body body, float rot, int side, int forward, bool jumpPressed, bool crouchHold)
    {
        Vector2 input = new Vector2((float)side, (float)-forward);

        // Upstream guards this with `#if defined(NORMALIZE_INPUT)`, which is always true given the
        // `#define NORMALIZE_INPUT 0` above it (defined() tests definedness, not the value), so the
        // diagonal-movement normalization is active.
        // Slow down diagonal movement
        if ((side != 0) && (forward != 0)) input = Vector2Normalize(input);

        float delta = GetFrameTime();

        if (!body.IsGrounded) body.Velocity.Y -= GRAVITY * delta;

        if (body.IsGrounded && jumpPressed)
        {
            body.Velocity.Y = JUMP_FORCE;
            body.IsGrounded = false;

            // Sound can be played at this moment
            //SetSoundPitch(fxJump, 1.0f + (GetRandomValue(-100, 100)*0.001));
            //PlaySound(fxJump);
        }

        Vector3 front = new Vector3(MathF.Sin(rot), 0.0f, MathF.Cos(rot));
        Vector3 right = new Vector3(MathF.Cos(-rot), 0.0f, MathF.Sin(-rot));

        Vector3 desiredDir = new Vector3(
            input.X * right.X + input.Y * front.X,
            0.0f,
            input.X * right.Z + input.Y * front.Z);
        body.Dir = Vector3Lerp(body.Dir, desiredDir, CONTROL * delta);

        float decel = (body.IsGrounded ? FRICTION : AIR_DRAG);
        Vector3 hvel = new Vector3(body.Velocity.X * decel, 0.0f, body.Velocity.Z * decel);

        float hvelLength = Vector3Length(hvel); // Magnitude
        if (hvelLength < (MAX_SPEED * 0.01f)) hvel = new Vector3(0, 0, 0);

        // This is what creates strafing
        float speed = Vector3DotProduct(hvel, body.Dir);

        // Whenever the amount of acceleration to add is clamped by the maximum acceleration constant,
        // a Player can make the speed faster by bringing the direction closer to horizontal velocity angle
        // More info here: https://youtu.be/v3zT3Z5apaM?t=165
        float maxSpeed = (crouchHold ? CROUCH_SPEED : MAX_SPEED);
        float accel = Clamp(maxSpeed - speed, 0.0f, MAX_ACCEL * delta);
        hvel.X += body.Dir.X * accel;
        hvel.Z += body.Dir.Z * accel;

        body.Velocity.X = hvel.X;
        body.Velocity.Z = hvel.Z;

        body.Position.X += body.Velocity.X * delta;
        body.Position.Y += body.Velocity.Y * delta;
        body.Position.Z += body.Velocity.Z * delta;

        // Fancy collision system against the floor
        if (body.Position.Y <= 0.0f)
        {
            body.Position.Y = 0.0f;
            body.Velocity.Y = 0.0f;
            body.IsGrounded = true; // Enable jumping
        }
    }

    // Update camera for FPS behaviour
    private void UpdateCameraFPS(ref Camera3D camera)
    {
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 targetOffset = new Vector3(0.0f, 0.0f, -1.0f);

        // Left and right
        Vector3 yaw = Vector3RotateByAxisAngle(targetOffset, up, lookRotation.X);

        // Clamp view up
        float maxAngleUp = Vector3Angle(up, yaw);
        maxAngleUp -= 0.001f; // Avoid numerical errors
        if (-(lookRotation.Y) > maxAngleUp) { lookRotation.Y = -maxAngleUp; }

        // Clamp view down
        float maxAngleDown = Vector3Angle(Vector3Negate(up), yaw);
        maxAngleDown *= -1.0f; // Downwards angle is negative
        maxAngleDown += 0.001f; // Avoid numerical errors
        if (-(lookRotation.Y) < maxAngleDown) { lookRotation.Y = -maxAngleDown; }

        // Up and down
        Vector3 right = Vector3Normalize(Vector3CrossProduct(yaw, up));

        // Rotate view vector around right axis
        float pitchAngle = -lookRotation.Y - lean.Y;
        pitchAngle = Clamp(pitchAngle, -MathF.PI / 2 + 0.0001f, MathF.PI / 2 - 0.0001f); // Clamp angle so it doesn't go past straight up or straight down
        Vector3 pitch = Vector3RotateByAxisAngle(yaw, right, pitchAngle);

        // Head animation
        // Rotate up direction around forward axis
        float headSin = MathF.Sin(headTimer * MathF.PI);
        float headCos = MathF.Cos(headTimer * MathF.PI);
        const float stepRotation = 0.01f;
        camera.Up = Vector3RotateByAxisAngle(up, pitch, headSin * stepRotation + lean.X);

        // Camera BOB
        const float bobSide = 0.1f;
        const float bobUp = 0.15f;
        Vector3 bobbing = Vector3Scale(right, headSin * bobSide);
        bobbing.Y = MathF.Abs(headCos * bobUp);

        camera.Position = Vector3Add(camera.Position, Vector3Scale(bobbing, walkLerp));
        camera.Target = Vector3Add(camera.Position, pitch);
    }

    // Draw game level
    private void DrawLevel()
    {
        const int floorExtent = 25;
        const float tileSize = 5.0f;
        Color tileColor1 = new Color(150, 200, 200, 255);

        // Floor tiles
        for (int y = -floorExtent; y < floorExtent; y++)
        {
            for (int x = -floorExtent; x < floorExtent; x++)
            {
                if ((y & 1) != 0 && (x & 1) != 0)
                {
                    DrawPlane(new Vector3(x * tileSize, 0.0f, y * tileSize), new Vector2(tileSize, tileSize), tileColor1);
                }
                else if ((y & 1) == 0 && (x & 1) == 0)
                {
                    DrawPlane(new Vector3(x * tileSize, 0.0f, y * tileSize), new Vector2(tileSize, tileSize), Color.LightGray);
                }
            }
        }

        Vector3 towerSize = new Vector3(16.0f, 32.0f, 16.0f);
        Color towerColor = new Color(150, 200, 200, 255);

        Vector3 towerPos = new Vector3(16.0f, 16.0f, 16.0f);
        DrawCubeV(towerPos, towerSize, towerColor);
        DrawCubeWiresV(towerPos, towerSize, Color.DarkBlue);

        towerPos.X *= -1;
        DrawCubeV(towerPos, towerSize, towerColor);
        DrawCubeWiresV(towerPos, towerSize, Color.DarkBlue);

        towerPos.Z *= -1;
        DrawCubeV(towerPos, towerSize, towerColor);
        DrawCubeWiresV(towerPos, towerSize, Color.DarkBlue);

        towerPos.X *= -1;
        DrawCubeV(towerPos, towerSize, towerColor);
        DrawCubeWiresV(towerPos, towerSize, Color.DarkBlue);

        // Red sun
        DrawSphere(new Vector3(300.0f, 300.0f, 0.0f), 100.0f, new Color(255, 0, 0, 255));
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d camera fps");

        DisableCursor();        // Limit cursor to relative movement inside the window

        SetTargetFPS(60);       // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Camera3dFps();
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
