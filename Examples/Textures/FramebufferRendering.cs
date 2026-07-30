/*******************************************************************************************
*
*   raylib [textures] example - framebuffer rendering
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example contributed by Jack Boakes (@jackboakes) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Jack Boakes (@jackboakes)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Textures;

public partial class FramebufferRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;
    private const int splitWidth = screenWidth / 2;

    public string Name => "Textures / Framebuffer Rendering";

    public string Title => "raylib [textures] example - framebuffer rendering";

    public bool CursorDisabled => true;

    private Camera3D subjectCamera;
    private Camera3D observerCamera;

    private RenderTexture2D observerTarget;
    private Rectangle observerSource;
    private Rectangle observerDest;

    private RenderTexture2D subjectTarget;
    private Rectangle subjectSource;
    private Rectangle subjectDest;
    private float textureAspectRatio;

    private const float captureSize = 128.0f;
    private Rectangle cropSource;
    private Rectangle cropDest;

    public void Init()
    {
        // Camera to look at the 3D world
        subjectCamera = new Camera3D();
        subjectCamera.Position = new Vector3(5.0f, 5.0f, 5.0f);
        subjectCamera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        subjectCamera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        subjectCamera.FovY = 45.0f;
        subjectCamera.Projection = CameraProjection.Perspective;

        // Camera to observe the subject camera and 3D world
        observerCamera = new Camera3D();
        observerCamera.Position = new Vector3(10.0f, 10.0f, 10.0f);
        observerCamera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        observerCamera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        observerCamera.FovY = 45.0f;
        observerCamera.Projection = CameraProjection.Perspective;

        // Set up render textures
        observerTarget = LoadRenderTexture(splitWidth, screenHeight);
        observerSource = new Rectangle(0.0f, 0.0f, observerTarget.Texture.Width, -observerTarget.Texture.Height);
        observerDest = new Rectangle(0.0f, 0.0f, splitWidth, screenHeight);

        subjectTarget = LoadRenderTexture(splitWidth, screenHeight);
        subjectSource = new Rectangle(0.0f, 0.0f, subjectTarget.Texture.Width, -subjectTarget.Texture.Height);
        subjectDest = new Rectangle(splitWidth, 0.0f, splitWidth, screenHeight);
        textureAspectRatio = (float)subjectTarget.Texture.Width / subjectTarget.Texture.Height;

        // Rectangles for cropping render texture
        cropSource = new Rectangle((subjectTarget.Texture.Width - captureSize) / 2.0f, (subjectTarget.Texture.Height - captureSize) / 2.0f, captureSize, -captureSize);
        cropDest = new Rectangle(splitWidth + 20.0f, 20.0f, captureSize, captureSize);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref observerCamera, CameraMode.Free);
        UpdateCamera(ref subjectCamera, CameraMode.Orbital);

        if (IsKeyPressed(KeyboardKey.R))
        {
            observerCamera.Target = new Vector3(0.0f, 0.0f, 0.0f);
        }

        // Build LHS observer view texture
        BeginTextureMode(observerTarget);

        ClearBackground(Color.RayWhite);

        BeginMode3D(observerCamera);

        DrawGrid(10, 1.0f);
        DrawCube(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Gold);
        DrawCubeWires(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Pink);
        DrawCameraPrism(subjectCamera, textureAspectRatio, Color.Green);

        EndMode3D();

        DrawText("Observer View", 10, observerTarget.Texture.Height - 30, 20, Color.Black);
        DrawText("WASD + Mouse to Move", 10, 10, 20, Color.DarkGray);
        DrawText("Scroll to Zoom", 10, 30, 20, Color.DarkGray);
        DrawText("R to Reset Observer Target", 10, 50, 20, Color.DarkGray);

        EndTextureMode();

        // Build RHS subject view texture
        BeginTextureMode(subjectTarget);

        ClearBackground(Color.RayWhite);

        BeginMode3D(subjectCamera);

        DrawCube(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Gold);
        DrawCubeWires(new Vector3(0.0f, 0.0f, 0.0f), 2.0f, 2.0f, 2.0f, Color.Pink);
        DrawGrid(10, 1.0f);

        EndMode3D();

        DrawRectangleLines((int)((subjectTarget.Texture.Width - captureSize) / 2.0f), (int)((subjectTarget.Texture.Height - captureSize) / 2.0f), (int)captureSize, (int)captureSize, Color.Green);
        DrawText("Subject View", 10, subjectTarget.Texture.Height - 30, 20, Color.Black);

        EndTextureMode();
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.Black);

        // Draw observer texture LHS
        DrawTexturePro(observerTarget.Texture, observerSource, observerDest, new Vector2(0.0f, 0.0f), 0.0f, Color.White);

        // Draw subject texture RHS
        DrawTexturePro(subjectTarget.Texture, subjectSource, subjectDest, new Vector2(0.0f, 0.0f), 0.0f, Color.White);

        // Draw the small crop overlay on top
        DrawTexturePro(subjectTarget.Texture, cropSource, cropDest, new Vector2(0.0f, 0.0f), 0.0f, Color.White);
        DrawRectangleLinesEx(cropDest, 2, Color.Black);

        // Draw split screen divider line
        DrawLine(splitWidth, 0, splitWidth, screenHeight, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(observerTarget);
        UnloadRenderTexture(subjectTarget);
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    private static void DrawCameraPrism(Camera3D camera, float aspect, Color color)
    {
        float length = Vector3Distance(camera.Position, camera.Target);
        // Define the 4 corners of the camera's prism plane sliced at the target in Normalized Device Coordinates
        Vector3[] planeNDC =
        {
            new(-1.0f, -1.0f, 1.0f), // Bottom Left
            new( 1.0f, -1.0f, 1.0f), // Bottom Right
            new( 1.0f,  1.0f, 1.0f), // Top Right
            new(-1.0f,  1.0f, 1.0f)  // Top Left
        };

        // Build the matrices
        Matrix4x4 view = GetCameraMatrix(camera);
        Matrix4x4 proj = MatrixPerspective(camera.FovY * DEG2RAD, aspect, 0.05f, length);
        // Combine view and projection so we can reverse the full camera transform
        Matrix4x4 viewProj = MatrixMultiply(view, proj);
        // Invert the view-projection matrix to unproject points from NDC space back into world space
        Matrix4x4 inverseViewProj = MatrixInvert(viewProj);

        // Transform the 4 plane corners from NDC into world space
        Vector3[] corners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            float x = planeNDC[i].X;
            float y = planeNDC[i].Y;
            float z = planeNDC[i].Z;

            // Multiply NDC position by the inverse view-projection matrix
            // This produces a homogeneous (x, y, z, w) position in world space
            float vx = inverseViewProj.M11 * x + inverseViewProj.M12 * y + inverseViewProj.M13 * z + inverseViewProj.M14;
            float vy = inverseViewProj.M21 * x + inverseViewProj.M22 * y + inverseViewProj.M23 * z + inverseViewProj.M24;
            float vz = inverseViewProj.M31 * x + inverseViewProj.M32 * y + inverseViewProj.M33 * z + inverseViewProj.M34;
            float vw = inverseViewProj.M41 * x + inverseViewProj.M42 * y + inverseViewProj.M43 * z + inverseViewProj.M44;

            corners[i] = new Vector3(vx / vw, vy / vw, vz / vw);
        }

        // Draw the far plane sliced at the target
        DrawLine3D(corners[0], corners[1], color);
        DrawLine3D(corners[1], corners[2], color);
        DrawLine3D(corners[2], corners[3], color);
        DrawLine3D(corners[3], corners[0], color);

        // Draw the prism lines from the far plane to the camera position
        for (int i = 0; i < 4; i++)
        {
            DrawLine3D(camera.Position, corners[i], color);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - framebuffer rendering");

        SetTargetFPS(60);
        DisableCursor();
        //--------------------------------------------------------------------------------------

        var game = new FramebufferRendering();
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
