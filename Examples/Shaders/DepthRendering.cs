/*******************************************************************************************
*
*   raylib [shaders] example - depth rendering
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Luís Almeida (@luis605) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Luís Almeida (@luis605)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class DepthRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Depth Rendering";

    public string Title => "raylib [shaders] example - depth rendering";

    public bool CursorDisabled => true;

    private Camera3D camera;
    private RenderTexture2D target;
    private Shader depthShader;
    private int depthLoc;
    private Model cube;
    private Model floor;

    public void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(4.0f, 1.0f, 5.0f);
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load render texture with a depth texture attached
        target = LoadRenderTextureDepthTex(screenWidth, screenHeight);

        // Load depth shader and get depth texture shader location
        depthShader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/depth_render.fs");
        depthLoc = GetShaderLocation(depthShader, "depthTexture");
        var flipTextureLoc = GetShaderLocation(depthShader, "flipY");
        Raylib.SetShaderValue(depthShader, flipTextureLoc, 1, ShaderUniformDataType.Int); // Flip Y texture

        // Load scene models
        cube = LoadModelFromMesh(GenMeshCube(1.0f, 1.0f, 1.0f));
        floor = LoadModelFromMesh(GenMeshPlane(20.0f, 20.0f, 1, 1));
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Free);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginTextureMode(target);
        ClearBackground(Color.White);

        BeginMode3D(camera);
        DrawModel(cube, new Vector3(0.0f, 0.0f, 0.0f), 3.0f, Color.Yellow);
        DrawModel(floor, new Vector3(10.0f, 0.0f, 2.0f), 2.0f, Color.Red);
        EndMode3D();
        EndTextureMode();

        // Draw into screen (main framebuffer)
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginShaderMode(depthShader);
        SetShaderValueTexture(depthShader, depthLoc, target.Depth);
        DrawTexture(target.Depth, 0, 0, Color.White);
        EndShaderMode();

        DrawRectangle(10, 10, 320, 93, Fade(Color.SkyBlue, 0.5f));
        DrawRectangleLines(10, 10, 320, 93, Color.Blue);

        DrawText("Camera Controls:", 20, 20, 10, Color.Black);
        DrawText("- WASD to move", 40, 40, 10, Color.DarkGray);
        DrawText("- Mouse Wheel Pressed to Pan", 40, 60, 10, Color.DarkGray);
        DrawText("- Z to zoom to (0, 0, 0)", 40, 80, 10, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(cube);              // Unload model
        UnloadModel(floor);             // Unload model
        UnloadRenderTextureDepthTex(target);
        UnloadShader(depthShader);      // Unload shader
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
            target.Depth.Format = PixelFormat.CompressedPvrtRgba;    // DEPTH_COMPONENT_24BIT: Not defined in raylib
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

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - depth rendering");

        DisableCursor();  // Limit cursor to relative movement inside the window

        SetTargetFPS(60); // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DepthRendering();
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
}
