/*******************************************************************************************
*
*   raylib [shaders] example - shadowmap rendering
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.0, last time updated with raylib 5.0
*
*   Example contributed by TheManTheMythTheGameDev (@TheManTheMythTheGameDev) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2023-2025 TheManTheMythTheGameDev (@TheManTheMythTheGameDev)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Rlgl;

namespace Examples.Shaders;

public partial class ShadowmapRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    private const int ShadowmapResolution = 1024;

    public string Name => "Shaders / Shadowmap Rendering";

    public string Title => "raylib [shaders] example - shadowmap rendering";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Shader shadowShader;
    private Vector3 lightDir;
    private int lightDirLoc;
    private int lightVPLoc;
    private int shadowMapLoc;
    private Model cube;
    private Model robot;
    private unsafe ModelAnimation* anims;
    private int animCount;
    private RenderTexture2D shadowMap;
    private Camera3D lightCamera;
    private int frameCounter;
    private int textureActiveSlot;

    public unsafe void Init()
    {
        // Shadows are a HUGE topic, and this example shows an extremely simple implementation of the shadowmapping algorithm,
        // which is the industry standard for shadows. This algorithm can be extended in a ridiculous number of ways to improve
        // realism and also adapt it for different scenes. This is pretty much the simplest possible implementation

        camera = new();
        camera.Position = new Vector3(10.0f, 10.0f, 10.0f);
        camera.Target = Vector3.Zero;
        camera.Projection = CameraProjection.Perspective;
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;

        shadowShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/shadowmap.vs",
            $"resources/shaders/glsl{GlslVersion}/shadowmap.fs"
        );
        shadowShader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shadowShader, "viewPos");

        lightDir = Vector3Normalize(new Vector3(0.35f, -1.0f, -0.35f));
        var lightColor = Color.White;
        var lightColorNormalized = ColorNormalize(lightColor);
        lightDirLoc = GetShaderLocation(shadowShader, "lightDir");
        var lightColLoc = GetShaderLocation(shadowShader, "lightColor");
        Raylib.SetShaderValue(shadowShader, lightDirLoc, lightDir, ShaderUniformDataType.Vec3);
        Raylib.SetShaderValue(shadowShader, lightColLoc, lightColorNormalized, ShaderUniformDataType.Vec4);
        var ambientLoc = GetShaderLocation(shadowShader, "ambient");
        var ambient = new[] { 0.1f, 0.1f, 0.1f, 1.0f };
        Raylib.SetShaderValue(shadowShader, ambientLoc, ambient, ShaderUniformDataType.Vec4);
        lightVPLoc = GetShaderLocation(shadowShader, "lightVP");
        shadowMapLoc = GetShaderLocation(shadowShader, "shadowMap");
        var shadowMapResolution = ShadowmapResolution;
        Raylib.SetShaderValue(shadowShader, GetShaderLocation(shadowShader, "shadowMapResolution"), shadowMapResolution, ShaderUniformDataType.Int);

        cube = LoadModelFromMesh(GenMeshCube(1.0f, 1.0f, 1.0f));
        cube.Materials[0].Shader = shadowShader;
        robot = LoadModel("resources/models/robot.glb");
        for (var i = 0; i < robot.MaterialCount; i++)
        {
            robot.Materials[i].Shader = shadowShader;
        }

        animCount = 0;
        anims = LoadModelAnimations("resources/models/robot.glb", ref animCount);

        shadowMap = LoadShadowmapRenderTexture(ShadowmapResolution, ShadowmapResolution);

        // For the shadowmapping algorithm, we will be rendering everything from the light's point of view
        lightCamera = new();
        lightCamera.Position = Vector3Scale(lightDir, -15.0f);
        lightCamera.Target = Vector3.Zero;
        lightCamera.Projection = CameraProjection.Orthographic; // Use an orthographic projection for directional lights
        lightCamera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        lightCamera.FovY = 20.0f;

        frameCounter = 0;
        textureActiveSlot = 10; // Can be anything 0 to 15, but 0 will probably be taken up
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var deltaTime = GetFrameTime();

        var cameraPos = camera.Position;
        Raylib.SetShaderValue(shadowShader, shadowShader.Locs[(int)ShaderLocationIndex.VectorView], cameraPos, ShaderUniformDataType.Vec3);
        UpdateCamera(ref camera, CameraMode.Orbital);

        frameCounter++;
        frameCounter %= anims[0].KeyFrameCount;
        UpdateModelAnimation(robot, anims[0], (float)frameCounter);

        // Move light with arrow keys
        const float cameraSpeed = 0.05f;
        if (IsKeyDown(KeyboardKey.Left))
        {
            if (lightDir.X < 0.6f)
            {
                lightDir.X += cameraSpeed * 60.0f * deltaTime;
            }
        }
        if (IsKeyDown(KeyboardKey.Right))
        {
            if (lightDir.X > -0.6f)
            {
                lightDir.X -= cameraSpeed * 60.0f * deltaTime;
            }
        }
        if (IsKeyDown(KeyboardKey.Up))
        {
            if (lightDir.Z < 0.6f)
            {
                lightDir.Z += cameraSpeed * 60.0f * deltaTime;
            }
        }
        if (IsKeyDown(KeyboardKey.Down))
        {
            if (lightDir.Z > -0.6f)
            {
                lightDir.Z -= cameraSpeed * 60.0f * deltaTime;
            }
        }

        lightDir = Vector3Normalize(lightDir);
        lightCamera.Position = Vector3Scale(lightDir, -15.0f);
        Raylib.SetShaderValue(shadowShader, lightDirLoc, lightDir, ShaderUniformDataType.Vec3);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // PASS 01: Render all objects into the shadowmap render texture
        // We record all the objects' depths (as rendered from the light source's point of view) in a buffer
        // Anything that is "visible" to the light is in light, anything that isn't is in shadow
        // We can later use the depth buffer when rendering everything from the player's point of view
        // to determine whether a given point is "visible" to the light
        Matrix4x4 lightView;
        Matrix4x4 lightProj;
        BeginTextureMode(shadowMap);
        ClearBackground(Color.White);

        BeginMode3D(lightCamera);
        lightView = GetMatrixModelview();
        lightProj = GetMatrixProjection();
        DrawScene(cube, robot);
        EndMode3D();

        EndTextureMode();
        var lightViewProj = MatrixMultiply(lightView, lightProj);

        // PASS 02: Draw the scene into main framebuffer, using the generated shadowmap
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        SetShaderValueMatrix(shadowShader, lightVPLoc, lightViewProj);
        EnableShader(shadowShader.Id);

        ActiveTextureSlot(textureActiveSlot);
        EnableTexture(shadowMap.Depth.Id);
        var slot = textureActiveSlot;
        SetUniform(shadowMapLoc, &slot, (int)ShaderUniformDataType.Int, 1);

        BeginMode3D(camera);
        DrawScene(cube, robot); // Draw the same exact things as we drew in the shadowmap!
        EndMode3D();

        DrawText("Use the arrow keys to rotate the light!", 10, 10, 30, Color.Red);
        DrawText("Shadows in raylib using the shadowmapping algorithm!", screenWidth - 280, screenHeight - 20, 10, Color.Gray);

        EndDrawing();

        if (IsKeyPressed(KeyboardKey.F))
        {
            TakeScreenshot("shaders_shadowmap.png");
        }
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadShader(shadowShader);
        UnloadModel(cube);
        UnloadModel(robot);
        UnloadModelAnimations(anims, animCount);
        UnloadShadowmapRenderTexture(shadowMap);
    }

    // Load render texture for shadowmap projection
    // NOTE: Load framebuffer with only a texture depth attachment,
    // no color attachment required for shadowmap
    private static unsafe RenderTexture2D LoadShadowmapRenderTexture(int width, int height)
    {
        RenderTexture2D target = new();

        target.Id = LoadFramebuffer(); // Load an empty framebuffer
        target.Texture.Width = width;
        target.Texture.Height = height;

        if (target.Id > 0)
        {
            EnableFramebuffer(target.Id);

            // Create depth texture
            // NOTE: No need a color texture attachment for the shadowmap
            target.Depth.Id = LoadTextureDepth(width, height, false);
            target.Depth.Width = width;
            target.Depth.Height = height;
            target.Depth.Format = (PixelFormat)19; // DEPTH_COMPONENT_24BIT?
            target.Depth.Mipmaps = 1;

            // Attach depth texture to FBO
            FramebufferAttach(target.Id, target.Depth.Id, FramebufferAttachType.Depth, FramebufferAttachTextureType.Texture2D, 0);

            // Check if fbo is complete with attachments (valid)
            if (FramebufferComplete(target.Id) != 0)
            {
                TraceLog(TraceLogLevel.Info, $"FBO: [ID {target.Id}] Framebuffer object created successfully");
            }

            DisableFramebuffer();
        }
        else
        {
            TraceLog(TraceLogLevel.Warning, "FBO: Framebuffer object can not be created");
        }

        return target;
    }

    // Unload shadowmap render texture from GPU memory (VRAM)
    private static void UnloadShadowmapRenderTexture(RenderTexture2D target)
    {
        if (target.Id > 0)
        {
            // NOTE: Depth texture/renderbuffer is automatically
            // queried and deleted before deleting framebuffer
            UnloadFramebuffer(target.Id);
        }
    }

    // Draw full scene projecting shadows
    // NOTE: Required  to be called several time to generate shadowmap
    private static void DrawScene(Model cube, Model robot)
    {
        DrawModelEx(cube, Vector3.Zero, new Vector3(0.0f, 1.0f, 0.0f), 0.0f, new Vector3(10.0f, 1.0f, 10.0f), Color.Blue);
        DrawModelEx(cube, new Vector3(1.5f, 1.0f, -1.5f), new Vector3(0.0f, 1.0f, 0.0f), 0.0f, Vector3.One, Color.White);
        DrawModelEx(robot, new Vector3(0.0f, 0.5f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.0f, new Vector3(1.0f, 1.0f, 1.0f), Color.Red);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - shadowmap rendering");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ShadowmapRendering();
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
