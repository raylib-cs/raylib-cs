/*******************************************************************************************
*
*   raylib [shaders] example - hybrid rendering
*
*   Example complexity rating: [★★★★] 4/4
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

public class HybridRender : IExample
{
    private struct RayLocs
    {
        public int CamPos;
        public int CamDir;
        public int ScreenCenter;
    }

#if BROWSER
    const int GlslVersion = 100;   // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shaders / Hybrid Render";

    public string Title => "raylib [shaders] example - hybrid rendering";

    private Shader shdrRaymarch;
    private Shader shdrRaster;
    private RayLocs marchLocs;
    private RenderTexture2D target;
    private Camera3D camera;
    private float camDist;

    public void Init()
    {
        // This Shader calculates pixel depth and color using raymarch
        shdrRaymarch = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/hybrid_raymarch.fs");

        // This Shader is a standard rasterization fragment shader with the addition of depth writing
        // You are required to write depth for all shaders if one shader does it
        shdrRaster = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/hybrid_raster.fs");

        // Declare Struct used to store camera locs
        marchLocs = new();

        // Fill the struct with shader locs
        marchLocs.CamPos = GetShaderLocation(shdrRaymarch, "camPos");
        marchLocs.CamDir = GetShaderLocation(shdrRaymarch, "camDir");
        marchLocs.ScreenCenter = GetShaderLocation(shdrRaymarch, "screenCenter");

        // Transfer screenCenter position to shader. Which is used to calculate ray direction
        Vector2 screenCenter = new(screenWidth / 2.0f, screenHeight / 2.0f);
        SetShaderValue(
            shdrRaymarch,
            marchLocs.ScreenCenter,
            screenCenter,
            ShaderUniformDataType.Vec2
        );

        // Use Customized function to create writable depth texture buffer
        target = LoadRenderTextureDepthTex(screenWidth, screenHeight);

        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(0.5f, 1.0f, 1.5f);    // Camera position
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Camera FOV is pre-calculated in the camera distance
        camDist = 1.0f / (MathF.Tan(camera.FovY * 0.5f * Raylib.DEG2RAD));
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Update Camera Postion in the ray march shader
        SetShaderValue(
            shdrRaymarch,
            marchLocs.CamPos,
            camera.Position,
            ShaderUniformDataType.Vec3
        );

        // Update Camera Looking Vector. Vector length determines FOV
        var camDir = Vector3.Normalize(camera.Target - camera.Position) * camDist;
        SetShaderValue(shdrRaymarch, marchLocs.CamDir, camDir, ShaderUniformDataType.Vec3);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Draw into our custom render texture (framebuffer)
        BeginTextureMode(target);
        ClearBackground(Color.White);

        // Raymarch Scene
        // Manually enable Depth Test to handle multiple rendering methods
        Rlgl.EnableDepthTest();
        BeginShaderMode(shdrRaymarch);
        DrawRectangleRec(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
        EndShaderMode();

        // Rasterize Scene
        BeginMode3D(camera);
        BeginShaderMode(shdrRaster);
        DrawCubeWiresV(new Vector3(0.0f, 0.5f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Red);
        DrawCubeV(new Vector3(0.0f, 0.5f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Purple);
        DrawCubeWiresV(new Vector3(0.0f, 0.5f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.DarkGreen);
        DrawCubeV(new Vector3(0.0f, 0.5f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), Color.Yellow);
        DrawGrid(10, 1.0f);
        EndShaderMode();
        EndMode3D();

        EndTextureMode();

        // Draw custom render texture
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
        UnloadShader(shdrRaymarch);
        UnloadShader(shdrRaster);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - hybrid rendering");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new HybridRender();
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
