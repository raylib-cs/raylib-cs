/*******************************************************************************************
*
*   raylib [models] example - bone socket
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 4.5, last time updated with raylib 4.5
*
*   Example contributed by iP (@ipzaur) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2024-2025 iP (@ipzaur)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;

namespace Examples.Models;

public partial class BoneSocket : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int BoneSockets = 3;
    private const int BoneSocketHat = 0;
    private const int BoneSocketHandR = 1;
    private const int BoneSocketHandL = 2;

    public string Name => "Models / Bone Socket";

    public string Title => "raylib [models] example - bone socket";

    public bool CursorDisabled => true;

    private Camera3D camera;
    private Model characterModel;
    private Model[] equipModel;
    private bool[] showEquip;
    private int animsCount;
    private int animIndex;
    private int animCurrentFrame;
    private unsafe ModelAnimation* modelAnimations;
    private int[] boneSocketIndex;
    private Vector3 position;
    private int angle;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(5.0f, 5.0f, 5.0f); // Camera position
        camera.Target = new Vector3(0.0f, 2.0f, 0.0f);  // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);      // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                            // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective; // Camera projection type

        // Load gltf model
        characterModel = LoadModel("resources/models/gltf/greenman.glb"); // Load character model
        equipModel = new Model[BoneSockets]
        {
            LoadModel("resources/models/gltf/greenman_hat.glb"),    // Index for the hat model is the same as BONE_SOCKET_HAT
            LoadModel("resources/models/gltf/greenman_sword.glb"),  // Index for the sword model is the same as BONE_SOCKET_HAND_R
            LoadModel("resources/models/gltf/greenman_shield.glb")  // Index for the shield model is the same as BONE_SOCKET_HAND_L
        };

        showEquip = new bool[3] { true, true, true };   // Toggle on/off equip

        // Load gltf model animations
        animsCount = 0;
        animIndex = 0;
        animCurrentFrame = 0;
        modelAnimations = LoadModelAnimations("resources/models/gltf/greenman.glb", ref animsCount);

        // Indices of bones for sockets
        boneSocketIndex = new int[BoneSockets] { -1, -1, -1 };

        // Search bones for sockets
        for (var i = 0; i < characterModel.Skeleton.BoneCount; i++)
        {
            var boneName = characterModel.Skeleton.Bones[i].NameToString();

            if (boneName == "socket_hat")
            {
                boneSocketIndex[BoneSocketHat] = i;
                continue;
            }

            if (boneName == "socket_hand_R")
            {
                boneSocketIndex[BoneSocketHandR] = i;
                continue;
            }

            if (boneName == "socket_hand_L")
            {
                boneSocketIndex[BoneSocketHandL] = i;
                continue;
            }
        }

        position = new Vector3(0.0f, 0.0f, 0.0f); // Set model position
        angle = 0;                                // Set angle for rotate character
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.ThirdPerson);

        // Rotate character
        if (IsKeyDown(KeyboardKey.F))
        {
            angle = (angle + 1) % 360;
        }
        else if (IsKeyDown(KeyboardKey.H))
        {
            angle = (360 + angle - 1) % 360;
        }

        // Select current animation
        if (IsKeyPressed(KeyboardKey.T))
        {
            animIndex = (animIndex + 1) % animsCount;
        }
        else if (IsKeyPressed(KeyboardKey.G))
        {
            animIndex = (animIndex + animsCount - 1) % animsCount;
        }

        // Toggle shown of equip
        if (IsKeyPressed(KeyboardKey.One))
        {
            showEquip[BoneSocketHat] = !showEquip[BoneSocketHat];
        }
        if (IsKeyPressed(KeyboardKey.Two))
        {
            showEquip[BoneSocketHandR] = !showEquip[BoneSocketHandR];
        }
        if (IsKeyPressed(KeyboardKey.Three))
        {
            showEquip[BoneSocketHandL] = !showEquip[BoneSocketHandL];
        }

        // Update model animation
        var anim = modelAnimations[animIndex];
        animCurrentFrame = (animCurrentFrame + 1) % anim.KeyFrameCount;
        UpdateModelAnimation(characterModel, anim, animCurrentFrame);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw character
        var characterRotate = QuaternionFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), angle * DEG2RAD);
        characterModel.Transform = MatrixMultiply(QuaternionToMatrix(characterRotate), MatrixTranslate(position.X, position.Y, position.Z));
        UpdateModelAnimation(characterModel, anim, animCurrentFrame);
        DrawMesh(characterModel.Meshes[0], characterModel.Materials[1], characterModel.Transform);

        // Draw equipments (hat, sword, shield)
        for (var i = 0; i < BoneSockets; i++)
        {
            if (!showEquip[i])
            {
                continue;
            }

            var transform = &anim.KeyframePoses[animCurrentFrame][boneSocketIndex[i]];
            var inRotation = characterModel.Skeleton.BindPose[boneSocketIndex[i]].Rotation;
            var outRotation = transform->Rotation;

            // Calculate socket rotation (angle between bone in initial pose and same bone in current animation frame)
            var rotate = QuaternionMultiply(outRotation, QuaternionInvert(inRotation));
            var matrixTransform = QuaternionToMatrix(rotate);
            // Translate socket to its position in the current animation
            matrixTransform = MatrixMultiply(matrixTransform, MatrixTranslate(transform->Translation.X, transform->Translation.Y, transform->Translation.Z));
            // Transform the socket using the transform of the character (angle and translate)
            matrixTransform = MatrixMultiply(matrixTransform, characterModel.Transform);

            // Draw mesh at socket position with socket angle rotation
            DrawMesh(equipModel[i].Meshes[0], equipModel[i].Materials[1], matrixTransform);
        }

        DrawGrid(10, 1.0f);
        EndMode3D();

        DrawText("Use the T/G to switch animation", 10, 10, 20, Color.Gray);
        DrawText("Use the F/H to rotate character left/right", 10, 35, 20, Color.Gray);
        DrawText("Use the 1,2,3 to toggle shown of hat, sword and shield", 10, 60, 20, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(modelAnimations, animsCount);
        UnloadModel(characterModel);         // Unload character model and meshes/material

        // Unload equipment model and meshes/material
        for (var i = 0; i < BoneSockets; i++)
        {
            UnloadModel(equipModel[i]);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - bone socket");

        DisableCursor();                    // Limit cursor to relative movement inside the window

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new BoneSocket();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();              // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
