/*******************************************************************************************
*
*   raylib [models] example - Skybox loading and drawing
*
*   This example has been created using raylib 1.8 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2017 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Models;

public class SkyboxDemo
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [models] example - skybox loading and drawing");

        // Define the camera to look into our 3d world
        Camera3D camera = new();
        camera.Position = new Vector3(1.0f, 1.0f, 1.0f);
        camera.Target = new Vector3(4.0f, 1.0f, 4.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load skybox model
        Mesh cube = GenMeshCube(1.0f, 1.0f, 1.0f);
        Model skybox = LoadModelFromMesh(cube);

        bool useHdr = false;

        // Load skybox shader and set required locations
        // NOTE: Some locations are automatically set at shader loading
        Shader shdrSkybox = LoadShader("resources/shaders/glsl330/skybox.vs", "resources/shaders/glsl330/skybox.fs");

        Raylib.SetShaderValue(
            shdrSkybox,
            GetShaderLocation(shdrSkybox, "environmentMap"),
            (int)MaterialMapIndex.Cubemap,
            ShaderUniformDataType.Int
        );

        Raylib.SetShaderValue(
            shdrSkybox,
            GetShaderLocation(shdrSkybox, "doGamma"),
            useHdr ? 1 : 0,
            ShaderUniformDataType.Int
        );

        Raylib.SetShaderValue(
            shdrSkybox,
            GetShaderLocation(shdrSkybox, "vflipped"),
            useHdr ? 1 : 0,
            ShaderUniformDataType.Int
        );

        Raylib.SetMaterialShader(ref skybox, 0, ref shdrSkybox);

        // Load cubemap shader and setup required shader locations
        Shader shdrCubemap = LoadShader(
            "resources/shaders/glsl330/cubemap.vs",
            "resources/shaders/glsl330/cubemap.fs"
        );
        Raylib.SetShaderValue(
            shdrCubemap,
            GetShaderLocation(shdrCubemap, "equirectangularMap"),
            0,
            ShaderUniformDataType.Int
        );

        // Load skybox
        string skyboxFileName = "resources/dresden_square_2k.hdr";

        Texture2D panorama;

        if (useHdr)
        {
            panorama = LoadTexture(skyboxFileName);
            Texture2D cubemap = GenTextureCubemap(
                shdrCubemap,
                panorama,
                1024,
                PixelFormat.UncompressedR8G8B8A8
            );
            SetMaterialTexture(ref skybox, 0, MaterialMapIndex.Cubemap, ref cubemap);
            UnloadTexture(panorama);
        }
        else
        {
            Image img = LoadImage("resources/skybox.png");
            Texture2D cubemap = LoadTextureCubemap(img, CubemapLayout.AutoDetect);
            SetMaterialTexture(ref skybox, 0, MaterialMapIndex.Cubemap, ref cubemap);
            UnloadImage(img);
        }

        DisableCursor();

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            UpdateCamera(ref camera, CameraMode.FirstPerson);

            // Load new cubemap texture on drag & drop
            if (IsFileDropped())
            {
                string[] files = Raylib.GetDroppedFiles();

                if (files.Length == 1)
                {
                    if (IsFileExtension(files[0], ".png;.jpg;.hdr;.bmp;.tga"))
                    {
                        // Unload cubemap texture and load new one
                        UnloadTexture(Raylib.GetMaterialTexture(ref skybox, 0, MaterialMapIndex.Cubemap));

                        if (useHdr)
                        {
                            panorama = LoadTexture(files[0]);
                            Texture2D cubemap = GenTextureCubemap(
                                shdrCubemap,
                                panorama,
                                1024,
                                PixelFormat.UncompressedR8G8B8A8
                            );
                            SetMaterialTexture(ref skybox, 0, MaterialMapIndex.Cubemap, ref cubemap);
                            UnloadTexture(panorama);
                        }
                        else
                        {
                            Image img = LoadImage(files[0]);
                            Texture2D cubemap = LoadTextureCubemap(img, CubemapLayout.AutoDetect);
                            SetMaterialTexture(ref skybox, 0, MaterialMapIndex.Cubemap, ref cubemap);
                            UnloadImage(img);
                        }

                        skyboxFileName = files[0];
                    }
                }
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            BeginMode3D(camera);

            // We are inside the cube, we need to disable backface culling!
            Rlgl.DisableBackfaceCulling();
            Rlgl.DisableDepthMask();
            DrawModel(skybox, Vector3.Zero, 1.0f, Color.White);
            Rlgl.EnableBackfaceCulling();
            Rlgl.EnableDepthMask();

            DrawGrid(10, 1.0f);

            EndMode3D();

            if (useHdr)
            {
                DrawText(
                    $"Panorama image from hdrihaven.com: {skyboxFileName}",
                    10,
                    GetScreenHeight() - 20,
                    10,
                    Color.Black
                );
            }
            else
            {
                DrawText($": {skyboxFileName}", 10, GetScreenHeight() - 20, 10, Color.Black);
            }

            DrawFPS(10, 10);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadShader(Raylib.GetMaterial(ref skybox, 0).Shader);
        UnloadTexture(Raylib.GetMaterialTexture(ref skybox, 0, MaterialMapIndex.Cubemap));

        UnloadModel(skybox);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }

    // Generate cubemap texture from HDR texture
    private static unsafe Texture2D GenTextureCubemap(Shader shader, Texture2D panorama, int size, PixelFormat format)
    {
        Texture2D cubemap;

        // Disable backface culling to render inside the cube
        Rlgl.DisableBackfaceCulling();

        // STEP 1: Setup framebuffer
        //------------------------------------------------------------------------------------------
        uint rbo = Rlgl.LoadTextureDepth(size, size, true);
        cubemap.Id = Rlgl.LoadTextureCubemap(null, size, format, 1);

        uint fbo = Rlgl.LoadFramebuffer();
        Rlgl.FramebufferAttach(
            fbo,
            rbo,
            FramebufferAttachType.Depth,
            FramebufferAttachTextureType.Renderbuffer,
            0
        );
        Rlgl.FramebufferAttach(
            fbo,
            cubemap.Id,
            FramebufferAttachType.ColorChannel0,
            FramebufferAttachTextureType.CubemapPositiveX,
            0
        );

        // Check if framebuffer is complete with attachments (valid)
        if (Rlgl.FramebufferComplete(fbo))
        {
            Console.WriteLine($"FBO: [ID {fbo}] Framebuffer object created successfully");
        }
        //------------------------------------------------------------------------------------------

        // STEP 2: Draw to framebuffer
        //------------------------------------------------------------------------------------------
        // NOTE: Shader is used to convert HDR equirectangular environment map to cubemap equivalent (6 faces)
        Rlgl.EnableShader(shader.Id);

        // Define projection matrix and send it to shader
        Matrix4x4 matFboProjection = Raymath.MatrixPerspective(
            90.0f * DEG2RAD,
            1.0f,
            Rlgl.CULL_DISTANCE_NEAR,
            Rlgl.CULL_DISTANCE_FAR
        );
        Rlgl.SetUniformMatrix(shader.Locs[(int)ShaderLocationIndex.MatrixProjection], matFboProjection);

        // Define view matrix for every side of the cubemap
        Matrix4x4[] fboViews = new[]
        {
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3(-1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  1.0f,  0.0f), new Vector3( 0.0f,  0.0f,  1.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f, -1.0f,  0.0f), new Vector3( 0.0f,  0.0f, -1.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f, -1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f,  1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
        };

        // Set viewport to current fbo dimensions
        Rlgl.Viewport(0, 0, size, size);

        // Activate and enable texture for drawing to cubemap faces
        Rlgl.ActiveTextureSlot(0);
        Rlgl.EnableTexture(panorama.Id);

        for (int i = 0; i < 6; i++)
        {
            // Set the view matrix for the current cube face
            Rlgl.SetUniformMatrix(shader.Locs[(int)ShaderLocationIndex.MatrixView], fboViews[i]);

            // Select the current cubemap face attachment for the fbo
            // WARNING: This function by default enables->attach->disables fbo!!!
            Rlgl.FramebufferAttach(
                fbo,
                cubemap.Id,
                FramebufferAttachType.ColorChannel0,
                FramebufferAttachTextureType.CubemapPositiveX + i,
                0
            );
            Rlgl.EnableFramebuffer(fbo);

            // Load and draw a cube, it uses the current enabled texture
            Rlgl.ClearScreenBuffers();
            Rlgl.LoadDrawCube();
        }
        //------------------------------------------------------------------------------------------

        // STEP 3: Unload framebuffer and reset state
        //------------------------------------------------------------------------------------------
        Rlgl.DisableShader();
        Rlgl.DisableTexture();
        Rlgl.DisableFramebuffer();

        // Unload framebuffer (and automatically attached depth texture/renderbuffer)
        Rlgl.UnloadFramebuffer(fbo);

        // Reset viewport dimensions to default
        Rlgl.Viewport(0, 0, Rlgl.GetFramebufferWidth(), Rlgl.GetFramebufferHeight());
        Rlgl.EnableBackfaceCulling();
        //------------------------------------------------------------------------------------------

        cubemap.Width = size;
        cubemap.Height = size;
        cubemap.Mipmaps = 1;
        cubemap.Format = format;

        return cubemap;
    }
}
