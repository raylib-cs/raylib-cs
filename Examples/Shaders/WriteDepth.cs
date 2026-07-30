/*******************************************************************************************
*
*   raylib [shaders] example - depth writing
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example contributed by Buğra Alptekin Sarı (@BugraAlptekinSari) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2022-2025 Buğra Alptekin Sarı (@BugraAlptekinSari)
*
********************************************************************************************/

namespace Examples.Shaders;

public class WriteDepth : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Write Depth";

    public string Title => "raylib [shaders] example - depth writing";

    private Camera3D camera;
    private RenderTexture2D target;
    private Shader shader;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(2.0f, 2.0f, 3.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load custom render texture with writable depth texture buffer
        target = LoadRenderTextureDepthTex(screenWidth, screenHeight);

        // Load depth writing shader
        // NOTE: The shader inverts the depth buffer by writing into it by `gl_FragDepth = 1 - gl_FragCoord.z;`
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/depth_write.fs");
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------

        // Draw into our custom render texture
        BeginTextureMode(target);
        ClearBackground(Color.White);

        BeginMode3D(camera);
        BeginShaderMode(shader);

        DrawCubeWiresV(new Vector3(0.0f, 0.5f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Red);
        DrawCubeV(new Vector3(0.0f, 0.5f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Purple);
        DrawCubeWiresV(new Vector3(0.0f, 0.5f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.DarkGreen);
        DrawCubeV(new Vector3(0.0f, 0.5f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Yellow);
        DrawGrid(10, 1.0f);

        EndShaderMode();
        EndMode3D();
        EndTextureMode();

        // Draw into screen our custom render texture
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawTextureRec(
            target.Texture,
            new Rectangle(0, 0, screenWidth, -screenHeight),
            Vector2.Zero,
            Color.White
        );
        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTextureDepthTex(target);
        UnloadShader(shader);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - depth writing");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new WriteDepth();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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

    // Load custom render texture, create a writable depth texture buffer
    private static unsafe RenderTexture2D LoadRenderTextureDepthTex(int width, int height)
    {
        RenderTexture2D target = new();

        // Load an empty framebuffer
        target.Id = Rlgl.LoadFramebuffer();

        if (target.Id > 0)
        {
            Rlgl.EnableFramebuffer(target.Id);

            // Create color texture (default to RGBA)
            target.Texture.Id = Rlgl.LoadTexture(
                null,
                width,
                height,
                PixelFormat.UncompressedR8G8B8A8,
                1
            );
            target.Texture.Width = width;
            target.Texture.Height = height;
            target.Texture.Format = PixelFormat.UncompressedR8G8B8A8;
            target.Texture.Mipmaps = 1;

            // Create depth texture buffer (instead of raylib default renderbuffer)
            target.Depth.Id = Rlgl.LoadTextureDepth(width, height, false);
            target.Depth.Width = width;
            target.Depth.Height = height;
            target.Depth.Format = PixelFormat.CompressedPvrtRgba;
            target.Depth.Mipmaps = 1;

            // Attach color texture and depth texture to FBO
            Rlgl.FramebufferAttach(
                target.Id,
                target.Texture.Id,
                FramebufferAttachType.ColorChannel0,
                FramebufferAttachTextureType.Texture2D,
                0
            );
            Rlgl.FramebufferAttach(
                target.Id,
                target.Depth.Id,
                FramebufferAttachType.Depth,
                FramebufferAttachTextureType.Texture2D,
                0
            );

            // Check if fbo is complete with attachments (valid)
            if (Rlgl.FramebufferComplete(target.Id) != 0)
            {
                TraceLog(TraceLogLevel.Info, $"FBO: [ID {target.Id}] Framebuffer object created successfully");
            }

            Rlgl.DisableFramebuffer();
        }
        else
        {
            TraceLog(TraceLogLevel.Warning, "FBO: Framebuffer object can not be created");
        }

        return target;
    }

    // Unload render texture from GPU memory (VRAM)
    private static void UnloadRenderTextureDepthTex(RenderTexture2D target)
    {
        if (target.Id > 0)
        {
            // Color texture attached to FBO is deleted
            Rlgl.UnloadTexture(target.Texture.Id);
            Rlgl.UnloadTexture(target.Depth.Id);

            // NOTE: Depth texture is automatically
            // queried and deleted before deleting framebuffer
            Rlgl.UnloadFramebuffer(target.Id);
        }
    }
}
