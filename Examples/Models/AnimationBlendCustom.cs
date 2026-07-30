/*******************************************************************************************
*
*   raylib [models] example - animation blend custom
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 6.0
*
*   Example contributed by dmitrii-brand (@dmitrii-brand) and reviewed by Ramon Santamaria (@raysan5)
*
*   DETAILS: Example demonstrates per-bone animation blending, allowing smooth transitions
*   between two animations by interpolating bone transforms. This is useful for:
*    - Blending movement animations (walk/run) with action animations (jump/attack)
*    - Creating smooth animation transitions
*    - Layering animations (e.g., upper body attack while lower body walks)
*
*   WARNING: GPU skinning must be enabled in raylib with a compilation flag,
*   if not enabled, CPU skinning will be used instead
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 dmitrii-brand (@dmitrii-brand)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Models;

public partial class AnimationBlendCustom : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    private const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Models / Animation Blend Custom";

    public string Title => "raylib [models] example - animation blend custom";

    private Camera3D camera;
    private Model model;
    private Vector3 position;
    private Shader skinningShader;
    private unsafe ModelAnimation* anims;
    private int animCount;
    private int animIndex0;
    private int animIndex1;
    private int animCurrentFrame0;
    private int animCurrentFrame1;
    private bool upperBodyBlend;

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(4.0f, 4.0f, 4.0f); // Camera position
        camera.Target = new Vector3(0.0f, 1.0f, 0.0f);  // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);      // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                            // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective; // Camera projection type

        // Load gltf model
        model = LoadModel("resources/models/gltf/greenman.glb");
        position = new Vector3(0.0f, 0.0f, 0.0f); // Set model position

        // Load skinning shader
        // WARNING: GPU skinning must be enabled in raylib with a compilation flag,
        // if not enabled, CPU skinning will be used instead
        skinningShader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/skinning.vs",
            $"resources/shaders/glsl{GlslVersion}/skinning.fs"
        );
        model.Materials[1].Shader = skinningShader;

        // Load gltf model animations
        animCount = 0;
        anims = LoadModelAnimations("resources/models/gltf/greenman.glb", ref animCount);

        // Use specific animation indices: 2-walk/move, 3-attack
        animIndex0 = 2; // Walk/Move animation (index 2)
        animIndex1 = 3; // Attack animation (index 3)
        animCurrentFrame0 = 0;
        animCurrentFrame1 = 0;

        // Validate indices
        if (animIndex0 >= animCount)
        {
            animIndex0 = 0;
        }
        if (animIndex1 >= animCount)
        {
            animIndex1 = (animCount > 1) ? 1 : 0;
        }

        upperBodyBlend = true;     // Toggle: true = upper/lower body blending, false = uniform blending (50/50)
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, CameraMode.Orbital);

        // Toggle upper/lower body blending mode (SPACE key)
        if (IsKeyPressed(KeyboardKey.Space))
        {
            upperBodyBlend = !upperBodyBlend;
        }

        // Update animation frames
        var anim0 = anims[animIndex0];
        var anim1 = anims[animIndex1];

        animCurrentFrame0 = (animCurrentFrame0 + 1) % anim0.KeyFrameCount;
        animCurrentFrame1 = (animCurrentFrame1 + 1) % anim1.KeyFrameCount;

        // Blend the two animations
        // When upperBodyBlend is ON: upper body = attack (1.0), lower body = walk (0.0)
        // When upperBodyBlend is OFF: uniform blend at 0.5 (50% walk, 50% attack)
        var blendFactor = upperBodyBlend ? 1.0f : 0.5f;
        UpdateModelAnimationBones(anim0, animCurrentFrame0, anim1, animCurrentFrame1, blendFactor, upperBodyBlend);

        // raylib provided animation blending function
        //UpdateModelAnimationEx(model, anim0, (float)animCurrentFrame0,
        //    anim1, (float)animCurrentFrame1, blendFactor);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        DrawModel(model, position, 1.0f, Color.White);

        DrawGrid(10, 1.0f);

        EndMode3D();

        // Draw UI
        DrawText($"ANIM 0: {anim0.NameToString()}", 10, 10, 20, Color.Gray);
        DrawText($"ANIM 1: {anim1.NameToString()}", 10, 40, 20, Color.Gray);
        DrawText($"[SPACE] Toggle blending mode: {(upperBodyBlend ? "Upper/Lower Body Blending" : "Uniform Blending")}",
            10, GetScreenHeight() - 30, 20, Color.DarkGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public unsafe void Unload()
    {
        UnloadModelAnimations(anims, animCount); // Unload model animation
        UnloadModel(model);    // Unload model and meshes/material
        UnloadShader(skinningShader);   // Unload GPU skinning shader
    }

    // Check if a bone is part of upper body (for selective blending)
    private static bool IsUpperBodyBone(string boneName)
    {
        // Common upper body bone names (adjust based on your model)
        if (boneName is "spine" or "spine1" or "spine2" or
            "chest" or "upperChest" or
            "neck" or "head" or
            "shoulder" or "shoulder_L" or "shoulder_R" or
            "upperArm" or "upperArm_L" or "upperArm_R" or
            "lowerArm" or "lowerArm_L" or "lowerArm_R" or
            "hand" or "hand_L" or "hand_R" or
            "clavicle" or "clavicle_L" or "clavicle_R")
        {
            return true;
        }

        // Check if bone name contains upper body keywords
        if (boneName.Contains("spine") || boneName.Contains("chest") ||
            boneName.Contains("neck") || boneName.Contains("head") ||
            boneName.Contains("shoulder") || boneName.Contains("arm") ||
            boneName.Contains("hand") || boneName.Contains("clavicle"))
        {
            return true;
        }

        return false;
    }

    // Blend two animations per-bone with selective upper/lower body blending
    private unsafe void UpdateModelAnimationBones(ModelAnimation anim0, int frame0,
        ModelAnimation anim1, int frame1, float blend, bool upperBodyBlend)
    {
        // Validate inputs
        if ((anim0.BoneCount != 0) && (anim0.KeyframePoses != null) &&
            (anim1.BoneCount != 0) && (anim1.KeyframePoses != null) &&
            (model.Skeleton.BoneCount != 0) && (model.Skeleton.BindPose != null))
        {
            // Clamp blend factor to [0, 1]
            blend = MathF.Min(1.0f, MathF.Max(0.0f, blend));

            // Ensure frame indices are valid
            if (frame0 >= anim0.KeyFrameCount)
            {
                frame0 = anim0.KeyFrameCount - 1;
            }
            if (frame1 >= anim1.KeyFrameCount)
            {
                frame1 = anim1.KeyFrameCount - 1;
            }
            if (frame0 < 0)
            {
                frame0 = 0;
            }
            if (frame1 < 0)
            {
                frame1 = 0;
            }

            // Get bone count (use minimum of all to be safe)
            var boneCount = model.Skeleton.BoneCount;
            if (anim0.BoneCount < boneCount)
            {
                boneCount = anim0.BoneCount;
            }
            if (anim1.BoneCount < boneCount)
            {
                boneCount = anim1.BoneCount;
            }

            // Blend each bone
            for (var boneIndex = 0; boneIndex < boneCount; boneIndex++)
            {
                // Determine blend factor for this bone
                var boneBlendFactor = blend;

                // If upper body blending is enabled, use different blend factors for upper vs lower body
                if (upperBodyBlend)
                {
                    var boneName = model.Skeleton.Bones[boneIndex].NameToString();
                    var isUpperBody = IsUpperBodyBone(boneName);

                    // Upper body: use anim1 (attack), Lower body: use anim0 (walk)
                    // blend = 0.0 means full anim0 (walk), 1.0 means full anim1 (attack)
                    if (isUpperBody)
                    {
                        boneBlendFactor = blend; // Upper body: blend towards anim1 (attack)
                    }
                    else
                    {
                        boneBlendFactor = 1.0f - blend; // Lower body: blend towards anim0 (walk) - invert the blend
                    }
                }

                // Get transforms from both animations
                var bindTransform = model.Skeleton.BindPose[boneIndex];
                var animTransform0 = anim0.KeyframePoses[frame0][boneIndex];
                var animTransform1 = anim1.KeyframePoses[frame1][boneIndex];

                // Blend the transforms
                Transform blended = new();
                blended.Translation = Vector3Lerp(animTransform0.Translation, animTransform1.Translation, boneBlendFactor);
                blended.Rotation = QuaternionSlerp(animTransform0.Rotation, animTransform1.Rotation, boneBlendFactor);
                blended.Scale = Vector3Lerp(animTransform0.Scale, animTransform1.Scale, boneBlendFactor);

                // Convert bind pose to matrix
                var bindMatrix = MatrixMultiply(MatrixMultiply(
                    MatrixScale(bindTransform.Scale.X, bindTransform.Scale.Y, bindTransform.Scale.Z),
                    QuaternionToMatrix(bindTransform.Rotation)),
                    MatrixTranslate(bindTransform.Translation.X, bindTransform.Translation.Y, bindTransform.Translation.Z));

                // Convert blended transform to matrix
                var blendedMatrix = MatrixMultiply(MatrixMultiply(
                    MatrixScale(blended.Scale.X, blended.Scale.Y, blended.Scale.Z),
                    QuaternionToMatrix(blended.Rotation)),
                    MatrixTranslate(blended.Translation.X, blended.Translation.Y, blended.Translation.Z));

                // Calculate final bone matrix (similar to UpdateModelAnimationBones)
                model.BoneMatrices[boneIndex] = MatrixMultiply(MatrixInvert(bindMatrix), blendedMatrix);
            }

            // CPU skinning, updates CPU buffers and uploads them to GPU (if available)
            // NOTE: Fallback in case GPU skinning is not supported or enabled
            for (var m = 0; m < model.MeshCount; m++)
            {
                var mesh = model.Meshes[m];
                Vector3 animVertex;
                Vector3 animNormal;
                var vertexValuesCount = mesh.VertexCount * 3;

                var boneCounter = 0;
                var bufferUpdateRequired = false; // Flag to check when anim vertex information is updated

                // Skip if missing bone data or missing anim buffers initialization
                if ((mesh.BoneWeights == null) || (mesh.BoneIndices == null) ||
                    (mesh.AnimVertices == null) || (mesh.AnimNormals == null))
                {
                    continue;
                }

                for (var vCounter = 0; vCounter < vertexValuesCount; vCounter += 3)
                {
                    mesh.AnimVertices[vCounter] = 0;
                    mesh.AnimVertices[vCounter + 1] = 0;
                    mesh.AnimVertices[vCounter + 2] = 0;
                    if (mesh.AnimNormals != null)
                    {
                        mesh.AnimNormals[vCounter] = 0;
                        mesh.AnimNormals[vCounter + 1] = 0;
                        mesh.AnimNormals[vCounter + 2] = 0;
                    }

                    // Iterates over 4 bones per vertex
                    for (var j = 0; j < 4; j++, boneCounter++)
                    {
                        var boneWeight = mesh.BoneWeights[boneCounter];
                        var boneIndex = mesh.BoneIndices[boneCounter];

                        // Early stop when no transformation will be applied
                        if (boneWeight == 0.0f)
                        {
                            continue;
                        }
                        animVertex = new Vector3(mesh.Vertices[vCounter], mesh.Vertices[vCounter + 1], mesh.Vertices[vCounter + 2]);
                        animVertex = Vector3Transform(animVertex, model.BoneMatrices[boneIndex]);
                        mesh.AnimVertices[vCounter] += animVertex.X * boneWeight;
                        mesh.AnimVertices[vCounter + 1] += animVertex.Y * boneWeight;
                        mesh.AnimVertices[vCounter + 2] += animVertex.Z * boneWeight;
                        bufferUpdateRequired = true;

                        // Normals processing
                        // NOTE: We use meshes.baseNormals (default normal) to calculate meshes.normals (animated normals)
                        if ((mesh.Normals != null) && (mesh.AnimNormals != null))
                        {
                            animNormal = new Vector3(mesh.Normals[vCounter], mesh.Normals[vCounter + 1], mesh.Normals[vCounter + 2]);
                            animNormal = Vector3Transform(animNormal, MatrixTranspose(MatrixInvert(model.BoneMatrices[boneIndex])));
                            mesh.AnimNormals[vCounter] += animNormal.X * boneWeight;
                            mesh.AnimNormals[vCounter + 1] += animNormal.Y * boneWeight;
                            mesh.AnimNormals[vCounter + 2] += animNormal.Z * boneWeight;
                        }
                    }
                }

                if (bufferUpdateRequired)
                {
                    // Update GPU vertex buffers with updated data (position + normals)
                    Rlgl.UpdateVertexBuffer(mesh.VboId[(int)ShaderLocationIndex.VertexPosition], mesh.AnimVertices, mesh.VertexCount * 3 * sizeof(float), 0);
                    if (mesh.Normals != null)
                    {
                        Rlgl.UpdateVertexBuffer(mesh.VboId[(int)ShaderLocationIndex.VertexNormal], mesh.AnimNormals, mesh.VertexCount * 3 * sizeof(float), 0);
                    }
                }
            }
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [models] example - animation blend custom");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new AnimationBlendCustom();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                  // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
