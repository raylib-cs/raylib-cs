/*******************************************************************************************
*
*   raylib [core] example - 3d camera first person
*
*   This example has been created using raylib 1.3 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2015 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public class Camera3dFirstPerson
{
    public const int MaxColumns = 20;

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d camera first person");

        // Define the camera to look into our 3d world (position, target, up vector)
        Camera3D camera = new();
        camera.Position = new Vector3(4.0f, 2.0f, 4.0f);
        camera.Target = new Vector3(0.0f, 1.8f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 60.0f;
        camera.Projection = CameraProjection.Perspective;

        // Generates some random columns
        float[] heights = new float[MaxColumns];
        Vector3[] positions = new Vector3[MaxColumns];
        Color[] colors = new Color[MaxColumns];

        for (int i = 0; i < MaxColumns; i++)
        {
            heights[i] = (float)GetRandomValue(1, 12);
            positions[i] = new Vector3(GetRandomValue(-15, 15), heights[i] / 2, GetRandomValue(-15, 15));
            colors[i] = new Color(GetRandomValue(20, 255), GetRandomValue(10, 55), 30, 255);
        }

        CameraMode cameraMode = CameraMode.FirstPerson;

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            // Switch camera mode
            if (IsKeyPressed(KeyboardKey.One))
            {
                cameraMode = CameraMode.Free;
                camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
            }

            if (IsKeyPressed(KeyboardKey.Two))
            {
                cameraMode = CameraMode.FirstPerson;
                camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
            }

            if (IsKeyPressed(KeyboardKey.Three))
            {
                cameraMode = CameraMode.ThirdPerson;
                camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
            }

            if (IsKeyPressed(KeyboardKey.Four))
            {
                cameraMode = CameraMode.Orbital;
                camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
            }

            // Switch camera projection
            if (IsKeyPressed(KeyboardKey.P))
            {
                if (camera.Projection == CameraProjection.Perspective)
                {
                    // Create isometric view
                    cameraMode = CameraMode.ThirdPerson;
                    // Note: The target distance is related to the render distance in the orthographic projection
                    camera.Position = new Vector3(0.0f, 2.0f, -100.0f);
                    camera.Target = new Vector3(0.0f, 2.0f, 0.0f);
                    camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                    camera.Projection = CameraProjection.Orthographic;
                    camera.FovY = 20.0f; // near plane width in CAMERA_ORTHOGRAPHIC
                    // CameraYaw(&camera, -135 * DEG2RAD, true);
                    // CameraPitch(&camera, -45 * DEG2RAD, true, true, false);
                }
                else if (camera.Projection == CameraProjection.Orthographic)
                {
                    // Reset to default view
                    cameraMode = CameraMode.ThirdPerson;
                    camera.Position = new Vector3(0.0f, 2.0f, 10.0f);
                    camera.Target = new Vector3(0.0f, 2.0f, 0.0f);
                    camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                    camera.Projection = CameraProjection.Perspective;
                    camera.FovY = 60.0f;
                }
            }

            // Update camera computes movement internally depending on the camera mode
            // Some default standard keyboard/mouse inputs are hardcoded to simplify use
            // For advance camera controls, it's reecommended to compute camera movement manually
            UpdateCamera(ref camera, cameraMode);
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            BeginMode3D(camera);

            // Draw ground
            DrawPlane(new Vector3(0.0f, 0.0f, 0.0f), new Vector2(32.0f, 32.0f), Color.LightGray);

            // Draw a blue wall
            DrawCube(new Vector3(-16.0f, 2.5f, 0.0f), 1.0f, 5.0f, 32.0f, Color.Blue);

            // Draw a green wall
            DrawCube(new Vector3(16.0f, 2.5f, 0.0f), 1.0f, 5.0f, 32.0f, Color.Lime);

            // Draw a yellow wall
            DrawCube(new Vector3(0.0f, 2.5f, 16.0f), 32.0f, 5.0f, 1.0f, Color.Gold);

            // Draw some cubes around
            for (int i = 0; i < MaxColumns; i++)
            {
                DrawCube(positions[i], 2.0f, heights[i], 2.0f, colors[i]);
                DrawCubeWires(positions[i], 2.0f, heights[i], 2.0f, Color.Maroon);
            }

            // Draw player cube
            if (cameraMode == CameraMode.ThirdPerson)
            {
                DrawCube(camera.Target, 0.5f, 0.5f, 0.5f, Color.Purple);
                DrawCubeWires(camera.Target, 0.5f, 0.5f, 0.5f, Color.DarkPurple);
            }

            EndMode3D();

            // Draw info boxes
            DrawRectangle(5, 5, 330, 100, ColorAlpha(Color.SkyBlue, 0.5f));
            DrawRectangleLines(10, 10, 330, 100, Color.Blue);

            DrawText("Camera controls:", 15, 15, 10, Color.Black);
            DrawText("- Move keys: W, A, S, D, Space, Left-Ctrl", 15, 30, 10, Color.Black);
            DrawText("- Look around: arrow keys or mouse", 15, 45, 10, Color.Black);
            DrawText("- Camera mode keys: 1, 2, 3, 4", 15, 60, 10, Color.Black);
            DrawText("- Zoom keys: num-plus, num-minus or mouse scroll", 15, 75, 10, Color.Black);
            DrawText("- Camera projection key: P", 15, 90, 10, Color.Black);

            DrawRectangle(600, 5, 195, 100, Fade(Color.SkyBlue, 0.5f));
            DrawRectangleLines(600, 5, 195, 100, Color.Blue);

            DrawText("Camera status:", 610, 15, 10, Color.Black);
            DrawText($"- Mode: {cameraMode}", 610, 30, 10, Color.Black);
            DrawText($"- Projection: {camera.Projection}", 610, 45, 10, Color.Black);
            DrawText($"- Position: {camera.Position}", 610, 60, 10, Color.Black);
            DrawText($"- Target: {camera.Target}", 610, 75, 10, Color.Black);
            DrawText($"- Up: {camera.Up}", 610, 90, 10, Color.Black);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
