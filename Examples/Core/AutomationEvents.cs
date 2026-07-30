/*******************************************************************************************
*
*   raylib [core] example - automation events
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.0
*
*   Example based on 2d_camera_platformer example by arvyy (@arvyy)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Core;

public partial class AutomationEvents : IExample
{
    private const int GRAVITY = 400;
    private const float PLAYER_JUMP_SPD = 350.0f;
    private const float PLAYER_HOR_SPD = 200.0f;

    private const int MAX_ENVIRONMENT_ELEMENTS = 5;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Automation Events";

    public string Title => "raylib [core] example - automation events";

    private struct Player
    {
        public Vector2 Position;
        public float Speed;
        public bool CanJump;
    }

    private struct EnvElement
    {
        public Rectangle Rect;
        public int Blocking;
        public Color Color;
    }

    private Player player;
    private EnvElement[] envElements;
    private Camera2D camera;

    private AutomationEventList aelist;
    private bool eventRecording;
    private bool eventPlaying;

    private uint frameCounter;
    private uint playFrameCounter;
    private uint currentPlayFrame;

    public unsafe void Init()
    {
        // Define player
        player = new Player();
        player.Position = new Vector2(400, 280);
        player.Speed = 0;
        player.CanJump = false;

        // Define environment elements (platforms)
        envElements = new EnvElement[MAX_ENVIRONMENT_ELEMENTS]
        {
            new EnvElement { Rect = new Rectangle(0, 0, 1000, 400), Blocking = 0, Color = Color.LightGray },
            new EnvElement { Rect = new Rectangle(0, 400, 1000, 200), Blocking = 1, Color = Color.Gray },
            new EnvElement { Rect = new Rectangle(300, 200, 400, 10), Blocking = 1, Color = Color.Gray },
            new EnvElement { Rect = new Rectangle(250, 300, 100, 10), Blocking = 1, Color = Color.Gray },
            new EnvElement { Rect = new Rectangle(650, 300, 100, 10), Blocking = 1, Color = Color.Gray }
        };

        // Define camera
        camera = new Camera2D();
        camera.Target = player.Position;
        camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;

        // Automation events
        aelist = LoadAutomationEventList((sbyte*)null);  // Initialize list of automation events to record new events
        SetAutomationEventList(ref aelist);
        eventRecording = false;
        eventPlaying = false;

        frameCounter = 0;
        playFrameCounter = 0;
        currentPlayFrame = 0;
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float deltaTime = 0.015f;//GetFrameTime();

        // Dropped files logic
        //----------------------------------------------------------------------------------
#if BROWSER
        // NOTE: drag-and-drop event-list loading (.txt/.rae) is not supported in the browser host.
#else
        if (IsFileDropped())
        {
            FilePathList droppedFiles = LoadDroppedFiles();

            // Supports loading .rgs style files (text or binary) and .png style palette images
            if (IsFileExtension(droppedFiles[0], ".txt;.rae"))
            {
                UnloadAutomationEventList(aelist);
                aelist = LoadAutomationEventList(droppedFiles[0]);

                eventRecording = false;

                // Reset scene state to play
                eventPlaying = true;
                playFrameCounter = 0;
                currentPlayFrame = 0;

                player.Position = new Vector2(400, 280);
                player.Speed = 0;
                player.CanJump = false;

                camera.Target = player.Position;
                camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
                camera.Rotation = 0.0f;
                camera.Zoom = 1.0f;
            }

            UnloadDroppedFiles(droppedFiles);   // Unload filepaths from memory
        }
#endif
        //----------------------------------------------------------------------------------

        // Update player
        //----------------------------------------------------------------------------------
        if (IsKeyDown(KeyboardKey.Left))
        {
            player.Position.X -= PLAYER_HOR_SPD * deltaTime;
        }

        if (IsKeyDown(KeyboardKey.Right))
        {
            player.Position.X += PLAYER_HOR_SPD * deltaTime;
        }

        if (IsKeyDown(KeyboardKey.Space) && player.CanJump)
        {
            player.Speed = -PLAYER_JUMP_SPD;
            player.CanJump = false;
        }

        int hitObstacle = 0;
        for (int i = 0; i < MAX_ENVIRONMENT_ELEMENTS; i++)
        {
            EnvElement element = envElements[i];
            if (element.Blocking != 0 &&
                element.Rect.X <= player.Position.X &&
                element.Rect.X + element.Rect.Width >= player.Position.X &&
                element.Rect.Y >= player.Position.Y &&
                element.Rect.Y <= player.Position.Y + player.Speed * deltaTime)
            {
                hitObstacle = 1;
                player.Speed = 0.0f;
                player.Position.Y = element.Rect.Y;
            }
        }

        if (hitObstacle == 0)
        {
            player.Position.Y += player.Speed * deltaTime;
            player.Speed += GRAVITY * deltaTime;
            player.CanJump = false;
        }
        else
        {
            player.CanJump = true;
        }

        if (IsKeyPressed(KeyboardKey.R))
        {
            // Reset game state
            player.Position = new Vector2(400, 280);
            player.Speed = 0;
            player.CanJump = false;

            camera.Target = player.Position;
            camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
            camera.Rotation = 0.0f;
            camera.Zoom = 1.0f;
        }
        //----------------------------------------------------------------------------------

        // Events playing
        // NOTE: Logic must be before Camera update because it depends on mouse-wheel value,
        // that can be set by the played event... but some other inputs could be affected
        //----------------------------------------------------------------------------------
        if (eventPlaying)
        {
            // NOTE: Multiple events could be executed in a single frame
            while (playFrameCounter == aelist.Events[currentPlayFrame].Frame)
            {
                PlayAutomationEvent(aelist.Events[currentPlayFrame]);
                currentPlayFrame++;

                if (currentPlayFrame == aelist.Count)
                {
                    eventPlaying = false;
                    currentPlayFrame = 0;
                    playFrameCounter = 0;

                    TraceLog(TraceLogLevel.Info, "FINISH PLAYING!");
                    break;
                }
            }

            playFrameCounter++;
        }
        //----------------------------------------------------------------------------------

        // Update camera
        //----------------------------------------------------------------------------------
        camera.Target = player.Position;
        camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
        float minX = 1000, minY = 1000, maxX = -1000, maxY = -1000;

        // WARNING: On event replay, mouse-wheel internal value is set
        camera.Zoom += ((float)GetMouseWheelMove() * 0.05f);
        if (camera.Zoom > 3.0f)
        {
            camera.Zoom = 3.0f;
        }
        else if (camera.Zoom < 0.25f)
        {
            camera.Zoom = 0.25f;
        }

        for (int i = 0; i < MAX_ENVIRONMENT_ELEMENTS; i++)
        {
            EnvElement element = envElements[i];
            minX = MathF.Min(element.Rect.X, minX);
            maxX = MathF.Max(element.Rect.X + element.Rect.Width, maxX);
            minY = MathF.Min(element.Rect.Y, minY);
            maxY = MathF.Max(element.Rect.Y + element.Rect.Height, maxY);
        }

        Vector2 max = GetWorldToScreen2D(new Vector2(maxX, maxY), camera);
        Vector2 min = GetWorldToScreen2D(new Vector2(minX, minY), camera);

        if (max.X < screenWidth)
        {
            camera.Offset.X = screenWidth - (max.X - (float)screenWidth / 2);
        }

        if (max.Y < screenHeight)
        {
            camera.Offset.Y = screenHeight - (max.Y - (float)screenHeight / 2);
        }

        if (min.X > 0)
        {
            camera.Offset.X = (float)screenWidth / 2 - min.X;
        }

        if (min.Y > 0)
        {
            camera.Offset.Y = (float)screenHeight / 2 - min.Y;
        }
        //----------------------------------------------------------------------------------

        // Events management
        if (IsKeyPressed(KeyboardKey.S))    // Toggle events recording
        {
            if (!eventPlaying)
            {
                if (eventRecording)
                {
                    StopAutomationEventRecording();
                    eventRecording = false;

                    ExportAutomationEventList(aelist, "automation.rae");

                    TraceLog(TraceLogLevel.Info, $"RECORDED FRAMES: {aelist.Count}");
                }
                else
                {
                    SetAutomationEventBaseFrame(180);
                    StartAutomationEventRecording();
                    eventRecording = true;
                }
            }
        }
        else if (IsKeyPressed(KeyboardKey.A)) // Toggle events playing (WARNING: Starts next frame)
        {
            if (!eventRecording && (aelist.Count > 0))
            {
                // Reset scene state to play
                eventPlaying = true;
                playFrameCounter = 0;
                currentPlayFrame = 0;

                player.Position = new Vector2(400, 280);
                player.Speed = 0;
                player.CanJump = false;

                camera.Target = player.Position;
                camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
                camera.Rotation = 0.0f;
                camera.Zoom = 1.0f;
            }
        }

        if (eventRecording || eventPlaying)
        {
            frameCounter++;
        }
        else
        {
            frameCounter = 0;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.LightGray);

        BeginMode2D(camera);

        // Draw environment elements
        for (int i = 0; i < MAX_ENVIRONMENT_ELEMENTS; i++)
        {
            DrawRectangleRec(envElements[i].Rect, envElements[i].Color);
        }

        // Draw player rectangle
        DrawRectangleRec(new Rectangle(player.Position.X - 20, player.Position.Y - 40, 40, 40), Color.Red);

        EndMode2D();

        // Draw game controls
        DrawRectangle(10, 10, 290, 145, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(10, 10, 290, 145, Fade(Color.Blue, 0.8f));

        DrawText("Controls:", 20, 20, 10, Color.Black);
        DrawText("- RIGHT | LEFT: Player movement", 30, 40, 10, Color.DarkGray);
        DrawText("- SPACE: Player jump", 30, 60, 10, Color.DarkGray);
        DrawText("- R: Reset game state", 30, 80, 10, Color.DarkGray);

        DrawText("- S: START/STOP RECORDING INPUT EVENTS", 30, 110, 10, Color.Black);
        DrawText("- A: REPLAY LAST RECORDED INPUT EVENTS", 30, 130, 10, Color.Black);

        // Draw automation events recording indicator
        if (eventRecording)
        {
            DrawRectangle(10, 160, 290, 30, Fade(Color.Red, 0.3f));
            DrawRectangleLines(10, 160, 290, 30, Fade(Color.Maroon, 0.8f));
            DrawCircle(30, 175, 10, Color.Maroon);

            if (((frameCounter / 15) % 2) == 1)
            {
                DrawText($"RECORDING EVENTS... [{aelist.Count}]", 50, 170, 10, Color.Maroon);
            }
        }
        else if (eventPlaying)
        {
            DrawRectangle(10, 160, 290, 30, Fade(Color.Lime, 0.3f));
            DrawRectangleLines(10, 160, 290, 30, Fade(Color.DarkGreen, 0.8f));
            DrawTriangle(new Vector2(20, 155 + 10), new Vector2(20, 155 + 30), new Vector2(40, 155 + 20), Color.DarkGreen);

            if (((frameCounter / 15) % 2) == 1)
            {
                DrawText($"PLAYING RECORDED EVENTS... [{currentPlayFrame}]", 50, 170, 10, Color.DarkGreen);
            }
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadAutomationEventList(aelist);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - automation events");

        var game = new AutomationEvents();
        game.Init();

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

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
