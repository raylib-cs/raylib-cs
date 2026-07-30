/*******************************************************************************************
*
*   raylib [shaders] example - deferred rendering
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or OpenGL ES 3.0
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by Justin Andreas Lacoste (@27justin) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 Justin Andreas Lacoste (@27justin)
*
********************************************************************************************/

using Examples.Shared;

namespace Examples.Shaders;

[ExcludeFromBrowser("multiple-render-target G-buffer, unsupported on WebGL1/GLSL100")]
public partial class DeferredRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int MaxCubes = 30;
    private const int MaxLights = 4;
    private const float CubeScale = 0.25f;

    // GL_READ_FRAMEBUFFER / GL_DRAW_FRAMEBUFFER / GL_DEPTH_BUFFER_BIT
    private const uint RlReadFramebuffer = 0x8CA8;
    private const uint RlDrawFramebuffer = 0x8CA9;
    private const int GlDepthBufferBit = 0x00000100;

    public string Name => "Shaders / Deferred Rendering";

    public string Title => "raylib [shaders] example - deferred rendering";

    // GBuffer data
    private struct GBuffer
    {
        public uint FramebufferId;

        public uint PositionTextureId;
        public uint NormalTextureId;
        public uint AlbedoSpecTextureId;

        public uint DepthRenderbufferId;
    }

    // Deferred mode passes
    private enum DeferredMode
    {
        Position,
        Normal,
        Albedo,
        Shading
    }

    private Camera3D camera;
    private Model model;
    private Model cube;
    private Shader gbufferShader;
    private Shader deferredShader;
    private GBuffer gBuffer;
    private Light[] lights;
    private Vector3[] cubePositions;
    private float[] cubeRotations;
    private DeferredMode mode;

    // Texture units our g-buffer textures are bound to
    private const int TexUnitPosition = 0;
    private const int TexUnitNormal = 1;
    private const int TexUnitAlbedoSpec = 2;

    public unsafe void Init()
    {
        camera = new();
        camera.Position = new Vector3(5.0f, 4.0f, 5.0f);    // Camera position
        camera.Target = new Vector3(0.0f, 1.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 60.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load plane model from a generated mesh
        model = LoadModelFromMesh(GenMeshPlane(10.0f, 10.0f, 3, 3));
        cube = LoadModelFromMesh(GenMeshCube(2.0f, 2.0f, 2.0f));

        // Load geometry buffer (G-buffer) shader and deferred shader
        gbufferShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/gbuffer.vs",
            $"resources/shaders/glsl{GlslVersion}/gbuffer.fs"
        );

        deferredShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/deferred_shading.vs",
            $"resources/shaders/glsl{GlslVersion}/deferred_shading.fs"
        );
        deferredShader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(deferredShader, "viewPosition");

        // Initialize the G-buffer
        gBuffer = new();
        gBuffer.FramebufferId = Rlgl.LoadFramebuffer();
        if (gBuffer.FramebufferId == 0)
        {
            TraceLog(TraceLogLevel.Warning, "Failed to create framebufferId");
        }

        Rlgl.EnableFramebuffer(gBuffer.FramebufferId);

        // NOTE: Vertex positions are stored in a texture for simplicity. A better approach would use a depth texture
        // (instead of a detph renderbuffer) to reconstruct world positions in the final render shader via clip-space position,
        // depth, and the inverse view/projection matrices

        // 16-bit precision ensures OpenGL ES 3 compatibility, though it may lack precision for real scenarios
        gBuffer.PositionTextureId = Rlgl.LoadTexture(null, screenWidth, screenHeight, PixelFormat.UncompressedR16G16B16, 1);

        // Similarly, 16-bit precision is used for normals ensures OpenGL ES 3 compatibility
        gBuffer.NormalTextureId = Rlgl.LoadTexture(null, screenWidth, screenHeight, PixelFormat.UncompressedR16G16B16, 1);

        // Albedo (diffuse color) and specular strength can be combined into one texture
        // The color in RGB, and the specular strength in the alpha channel
        gBuffer.AlbedoSpecTextureId = Rlgl.LoadTexture(null, screenWidth, screenHeight, PixelFormat.UncompressedR8G8B8A8, 1);

        // Activate the draw buffers for our framebufferId
        Rlgl.ActiveDrawBuffers(3);

        // Now we attach our textures to the framebufferId
        Rlgl.FramebufferAttach(gBuffer.FramebufferId, gBuffer.PositionTextureId, FramebufferAttachType.ColorChannel0, FramebufferAttachTextureType.Texture2D, 0);
        Rlgl.FramebufferAttach(gBuffer.FramebufferId, gBuffer.NormalTextureId, FramebufferAttachType.ColorChannel1, FramebufferAttachTextureType.Texture2D, 0);
        Rlgl.FramebufferAttach(gBuffer.FramebufferId, gBuffer.AlbedoSpecTextureId, FramebufferAttachType.ColorChannel2, FramebufferAttachTextureType.Texture2D, 0);

        // Finally we attach the depth buffer
        gBuffer.DepthRenderbufferId = Rlgl.LoadTextureDepth(screenWidth, screenHeight, true);
        Rlgl.FramebufferAttach(gBuffer.FramebufferId, gBuffer.DepthRenderbufferId, FramebufferAttachType.Depth, FramebufferAttachTextureType.Renderbuffer, 0);

        // Make sure our framebufferId is complete
        // NOTE: rlFramebufferComplete() automatically unbinds the framebufferId, so we don't have to rlDisableFramebuffer() here
        if (Rlgl.FramebufferComplete(gBuffer.FramebufferId) == 0)
        {
            TraceLog(TraceLogLevel.Warning, "Framebuffer is not complete");
        }

        // Now we initialize the sampler2D uniform's in the deferred shader
        // We do this by setting the uniform's values to the texture units that
        // we later bind our g-buffer textures to
        Rlgl.EnableShader(deferredShader.Id);
        int texUnitPosition = TexUnitPosition;
        int texUnitNormal = TexUnitNormal;
        int texUnitAlbedoSpec = TexUnitAlbedoSpec;
        Raylib.SetShaderValue(deferredShader, GetShaderLocation(deferredShader, "gPosition"), texUnitPosition, ShaderUniformDataType.Sampler2D);
        Raylib.SetShaderValue(deferredShader, GetShaderLocation(deferredShader, "gNormal"), texUnitNormal, ShaderUniformDataType.Sampler2D);
        Raylib.SetShaderValue(deferredShader, GetShaderLocation(deferredShader, "gAlbedoSpec"), texUnitAlbedoSpec, ShaderUniformDataType.Sampler2D);
        Rlgl.DisableShader();

        // Assign out lighting shader to model
        model.Materials[0].Shader = gbufferShader;
        cube.Materials[0].Shader = gbufferShader;

        // Create lights
        lights = new Light[MaxLights];
        lights[0] = Rlights.CreateLight(0, LightType.Point, new Vector3(-2, 1, -2), Vector3.Zero, Color.Yellow, deferredShader);
        lights[1] = Rlights.CreateLight(1, LightType.Point, new Vector3(2, 1, 2), Vector3.Zero, Color.Red, deferredShader);
        lights[2] = Rlights.CreateLight(2, LightType.Point, new Vector3(-2, 1, 2), Vector3.Zero, Color.Green, deferredShader);
        lights[3] = Rlights.CreateLight(3, LightType.Point, new Vector3(2, 1, -2), Vector3.Zero, Color.Blue, deferredShader);

        var rand = new Random();
        cubePositions = new Vector3[MaxCubes];
        cubeRotations = new float[MaxCubes];

        for (var i = 0; i < MaxCubes; i++)
        {
            cubePositions[i] = new Vector3(
                (float)(rand.Next() % 10) - 5,
                (float)(rand.Next() % 5),
                (float)(rand.Next() % 10) - 5
            );

            cubeRotations[i] = (float)(rand.Next() % 360);
        }

        mode = DeferredMode.Shading;

        Rlgl.EnableDepthTest();
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Update the shader with the camera view vector (points towards { 0.0f, 0.0f, 0.0f })
        Raylib.SetShaderValue(
            deferredShader,
            deferredShader.Locs[(int)ShaderLocationIndex.VectorView],
            camera.Position,
            ShaderUniformDataType.Vec3
        );

        // Check key inputs to enable/disable lights
        if (IsKeyPressed(KeyboardKey.Y))
        {
            lights[0].Enabled = !lights[0].Enabled;
        }
        if (IsKeyPressed(KeyboardKey.R))
        {
            lights[1].Enabled = !lights[1].Enabled;
        }
        if (IsKeyPressed(KeyboardKey.G))
        {
            lights[2].Enabled = !lights[2].Enabled;
        }
        if (IsKeyPressed(KeyboardKey.B))
        {
            lights[3].Enabled = !lights[3].Enabled;
        }

        // Check key inputs to switch between G-buffer textures
        if (IsKeyPressed(KeyboardKey.One))
        {
            mode = DeferredMode.Position;
        }

        if (IsKeyPressed(KeyboardKey.Two))
        {
            mode = DeferredMode.Normal;
        }

        if (IsKeyPressed(KeyboardKey.Three))
        {
            mode = DeferredMode.Albedo;
        }

        if (IsKeyPressed(KeyboardKey.Four))
        {
            mode = DeferredMode.Shading;
        }

        // Update light values (actually, only enable/disable them)
        for (var i = 0; i < MaxLights; i++)
        {
            Rlights.UpdateLightValues(deferredShader, lights[i]);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        // Draw to the geometry buffer by first activating it
        Rlgl.EnableFramebuffer(gBuffer.FramebufferId);
        Rlgl.ClearColor(0, 0, 0, 0);
        Rlgl.ClearScreenBuffers();  // Clear color and depth buffer
        Rlgl.DisableColorBlend();

        BeginMode3D(camera);
        // NOTE: We have to use rlEnableShader here. `BeginShaderMode` or thus `rlSetShader`
        // will not work, as they won't immediately load the shader program
        Rlgl.EnableShader(gbufferShader.Id);
        // When drawing a model here, make sure that the material's shaders are set to the gbuffer shader!
        DrawModel(model, Vector3.Zero, 1.0f, Color.White);
        DrawModel(cube, new Vector3(0.0f, 1.0f, 0.0f), 1.0f, Color.White);

        for (var i = 0; i < MaxCubes; i++)
        {
            var position = cubePositions[i];
            DrawModelEx(cube, position, new Vector3(1, 1, 1), cubeRotations[i], new Vector3(CubeScale, CubeScale, CubeScale), Color.White);
        }
        Rlgl.DisableShader();
        EndMode3D();

        Rlgl.EnableColorBlend();

        // Go back to the default framebufferId (0) and draw our deferred shading
        Rlgl.DisableFramebuffer();
        Rlgl.ClearScreenBuffers(); // Clear color & depth buffer

        switch (mode)
        {
            case DeferredMode.Shading:
                {
                    BeginMode3D(camera);
                    Rlgl.DisableColorBlend();
                    Rlgl.EnableShader(deferredShader.Id);
                    // Bind our g-buffer textures
                    // We are binding them to locations that we earlier set in sampler2D uniforms `gPosition`, `gNormal`,
                    // and `gAlbedoSpec`
                    Rlgl.ActiveTextureSlot(TexUnitPosition);
                    Rlgl.EnableTexture(gBuffer.PositionTextureId);
                    Rlgl.ActiveTextureSlot(TexUnitNormal);
                    Rlgl.EnableTexture(gBuffer.NormalTextureId);
                    Rlgl.ActiveTextureSlot(TexUnitAlbedoSpec);
                    Rlgl.EnableTexture(gBuffer.AlbedoSpecTextureId);

                    // Finally, we draw a fullscreen quad to our default framebufferId
                    // This will now be shaded using our deferred shader
                    Rlgl.LoadDrawQuad();
                    Rlgl.DisableShader();
                    Rlgl.EnableColorBlend();
                    EndMode3D();

                    // As a last step, we now copy over the depth buffer from our g-buffer to the default framebufferId
                    Rlgl.BindFramebuffer(RlReadFramebuffer, gBuffer.FramebufferId);
                    Rlgl.BindFramebuffer(RlDrawFramebuffer, 0);
                    Rlgl.BlitFramebuffer(0, 0, screenWidth, screenHeight, 0, 0, screenWidth, screenHeight, GlDepthBufferBit);
                    Rlgl.DisableFramebuffer();

                    // Since our shader is now done and disabled, we can draw spheres
                    // that represent light positions in default forward rendering
                    BeginMode3D(camera);
                    Rlgl.EnableShader(Rlgl.GetShaderIdDefault());
                    for (var i = 0; i < MaxLights; i++)
                    {
                        if (lights[i].Enabled)
                        {
                            DrawSphereEx(lights[i].Position, 0.2f, 8, 8, lights[i].Color);
                        }
                        else
                        {
                            DrawSphereWires(lights[i].Position, 0.2f, 8, 8, ColorAlpha(lights[i].Color, 0.3f));
                        }
                    }
                    Rlgl.DisableShader();
                    EndMode3D();

                    DrawText("FINAL RESULT", 10, screenHeight - 30, 20, Color.DarkGreen);
                }
                break;
            case DeferredMode.Position:
                {
                    DrawTextureRec(
                        new Texture2D { Id = gBuffer.PositionTextureId, Width = screenWidth, Height = screenHeight },
                        new Rectangle(0, 0, screenWidth, -screenHeight),
                        Vector2.Zero,
                        Color.RayWhite
                    );

                    DrawText("POSITION TEXTURE", 10, screenHeight - 30, 20, Color.DarkGreen);
                }
                break;
            case DeferredMode.Normal:
                {
                    DrawTextureRec(
                        new Texture2D { Id = gBuffer.NormalTextureId, Width = screenWidth, Height = screenHeight },
                        new Rectangle(0, 0, screenWidth, -screenHeight),
                        Vector2.Zero,
                        Color.RayWhite
                    );

                    DrawText("NORMAL TEXTURE", 10, screenHeight - 30, 20, Color.DarkGreen);
                }
                break;
            case DeferredMode.Albedo:
                {
                    DrawTextureRec(
                        new Texture2D { Id = gBuffer.AlbedoSpecTextureId, Width = screenWidth, Height = screenHeight },
                        new Rectangle(0, 0, screenWidth, -screenHeight),
                        Vector2.Zero,
                        Color.RayWhite
                    );

                    DrawText("ALBEDO TEXTURE", 10, screenHeight - 30, 20, Color.DarkGreen);
                }
                break;
            default:
                break;
        }

        DrawText("Toggle lights keys: [Y][R][G][B]", 10, 40, 20, Color.DarkGray);
        DrawText("Switch G-buffer textures: [1][2][3][4]", 10, 70, 20, Color.DarkGray);

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // Unload the models
        UnloadModel(model);
        UnloadModel(cube);

        // Unload shaders
        UnloadShader(deferredShader);
        UnloadShader(gbufferShader);

        // Unload geometry buffer and all attached textures
        Rlgl.UnloadFramebuffer(gBuffer.FramebufferId);
        Rlgl.UnloadTexture(gBuffer.PositionTextureId);
        Rlgl.UnloadTexture(gBuffer.NormalTextureId);
        Rlgl.UnloadTexture(gBuffer.AlbedoSpecTextureId);
        Rlgl.UnloadTexture(gBuffer.DepthRenderbufferId);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - deferred rendering");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new DeferredRendering();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();          // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
