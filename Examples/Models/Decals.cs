/*******************************************************************************************
*
*   raylib [models] example - decals
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by JP Mortiboys (@themushroompirates) and reviewed by Ramon Santamaria (@raysan5)
*   Based on previous work by @mrdoob
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 JP Mortiboys (@themushroompirates) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using static Raylib_cs.Raymath;

namespace Examples.Models;

public partial class Decals : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxDecals = 256;

    public string Name => "Models / Decals";

    public string Title => "raylib [models] example - decals";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    private Camera3D camera;
    private Model model;
    private Texture2D modelTexture;
    private BoundingBox modelBBox;
    private float decalSize;
    private float decalOffset;
    private Model placementCube;
    private Material decalMaterial;
    private Texture2D decalTexture;
    private bool showModel;
    private readonly List<Model> decalModels = new();

    public unsafe void Init()
    {
        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(5.0f, 5.0f, 5.0f); // Camera position
        camera.Target = new Vector3(0.0f, 1.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.6f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;   // Camera projection type

        // Load character model
        model = LoadModel("resources/models/obj/character.obj");

        // Apply character skin
        modelTexture = LoadTexture("resources/models/obj/character_diffuse.png");
        SetTextureFilter(modelTexture, TextureFilter.Bilinear);
        model.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = modelTexture;

        modelBBox = GetMeshBoundingBox(model.Meshes[0]);    // Get mesh bounding box

        camera.Target = Vector3Lerp(modelBBox.Min, modelBBox.Max, 0.5f);
        camera.Position = modelBBox.Max * 1.0f;
        camera.Position.X *= 0.1f;

        var modelSize = MathF.Min(
            MathF.Min(MathF.Abs(modelBBox.Max.X - modelBBox.Min.X), MathF.Abs(modelBBox.Max.Y - modelBBox.Min.Y)),
            MathF.Abs(modelBBox.Max.Z - modelBBox.Min.Z));

        camera.Position = new Vector3(0.0f, modelBBox.Max.Y * 1.2f, modelSize * 3.0f);

        decalSize = modelSize * 0.25f;
        decalOffset = 0.01f;

        placementCube = LoadModelFromMesh(GenMeshCube(decalSize, decalSize, decalSize));
        placementCube.Materials[0].Maps[0].Color = Color.Lime;

        decalMaterial = LoadMaterialDefault();
        decalMaterial.Maps[0].Color = Color.Yellow;

        var decalImage = LoadImage("resources/raylib_logo.png");
        ImageResizeNN(ref decalImage, decalImage.Width / 4, decalImage.Height / 4);
        decalTexture = LoadTextureFromImage(decalImage);
        UnloadImage(decalImage);

        SetTextureFilter(decalTexture, TextureFilter.Bilinear);
        decalMaterial.Maps[(int)MaterialMapIndex.Diffuse].Texture = decalTexture;
        decalMaterial.Maps[(int)MaterialMapIndex.Diffuse].Color = Color.RayWhite;

        showModel = true;
        decalModels.Clear();
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsMouseButtonDown(MouseButton.Right))
        {
            UpdateCamera(ref camera, CameraMode.ThirdPerson);
        }

        // Display information about closest hit
        RayCollision collision = new();
        collision.Distance = float.MaxValue;
        collision.Hit = false;

        // Get mouse ray
        var ray = GetScreenToWorldRay(GetMousePosition(), camera);

        // Check ray collision against bounding box first, before trying the full ray-mesh test
        var boxHitInfo = GetRayCollisionBox(ray, modelBBox);

        RayCollision meshHitInfo = new();
        if (boxHitInfo.Hit && (decalModels.Count < MaxDecals))
        {
            // Check ray collision against model meshes
            for (var m = 0; m < model.MeshCount; m++)
            {
                // NOTE: We consider the model.transform for the collision check but
                // it can be checked against any transform Matrix, used when checking against same
                // model drawn multiple times with multiple transforms
                meshHitInfo = GetRayCollisionMesh(ray, model.Meshes[m], model.Transform);
                if (meshHitInfo.Hit)
                {
                    // Save the closest hit mesh
                    if (!collision.Hit || (collision.Distance > meshHitInfo.Distance))
                    {
                        collision = meshHitInfo;
                    }
                }
            }

            if (meshHitInfo.Hit)
            {
                collision = meshHitInfo;
            }
        }

        // Add decal to mesh on hit point
        if (collision.Hit && IsMouseButtonPressed(MouseButton.Left) && (decalModels.Count < MaxDecals))
        {
            // Create the transformation to project the decal
            var origin = collision.Point + (collision.Normal * 1.0f);
            var splat = MatrixLookAt(collision.Point, origin, new Vector3(0.0f, 1.0f, 0.0f));

            // Spin the placement around a bit
            splat = MatrixMultiply(splat, MatrixRotateZ(DEG2RAD * GetRandomValue(-180, 180)));

            var decalMesh = GenMeshDecal(model, splat, decalSize, decalOffset);

            if (decalMesh.VertexCount > 0)
            {
                var decalModel = LoadModelFromMesh(decalMesh);
                decalModel.Materials[0].Maps[0] = decalMaterial.Maps[0];
                decalModels.Add(decalModel);
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);

        // Draw the model at the origin and default scale
        if (showModel)
        {
            DrawModel(model, new Vector3(0.0f, 0.0f, 0.0f), 1.0f, Color.White);
        }

        // Draw the decal models
        for (var i = 0; i < decalModels.Count; i++)
        {
            DrawModel(decalModels[i], Vector3.Zero, 1.0f, Color.White);
        }

        // If we hit the mesh, draw the box for the decal
        if (collision.Hit)
        {
            var origin = collision.Point + (collision.Normal * 1.0f);
            var splat = MatrixLookAt(collision.Point, origin, new Vector3(0, 1, 0));
            placementCube.Transform = MatrixInvert(splat);
            DrawModel(placementCube, Vector3.Zero, 1.0f, Fade(Color.White, 0.5f));
        }

        DrawGrid(10, 10.0f);
        EndMode3D();

        float yPos = 10;
        var x0 = GetScreenWidth() - 300.0f;
        var x1 = x0 + 100;
        var x2 = x1 + 100;

        DrawText("Vertices", (int)x1, (int)yPos, 10, Color.Lime);
        DrawText("Triangles", (int)x2, (int)yPos, 10, Color.Lime);
        yPos += 15;

        var vertexCount = 0;
        var triangleCount = 0;

        for (var i = 0; i < model.MeshCount; i++)
        {
            vertexCount += model.Meshes[i].VertexCount;
            triangleCount += model.Meshes[i].TriangleCount;
        }

        DrawText("Main model", (int)x0, (int)yPos, 10, Color.Lime);
        DrawText($"{vertexCount}", (int)x1, (int)yPos, 10, Color.Lime);
        DrawText($"{triangleCount}", (int)x2, (int)yPos, 10, Color.Lime);
        yPos += 15;

        for (var i = 0; i < decalModels.Count; i++)
        {
            if (i == 20)
            {
                DrawText("...", (int)x0, (int)yPos, 10, Color.Lime);
                yPos += 15;
            }

            if (i < 20)
            {
                DrawText($"Decal #{i + 1}", (int)x0, (int)yPos, 10, Color.Lime);
                DrawText($"{decalModels[i].Meshes[0].VertexCount}", (int)x1, (int)yPos, 10, Color.Lime);
                DrawText($"{decalModels[i].Meshes[0].TriangleCount}", (int)x2, (int)yPos, 10, Color.Lime);
                yPos += 15;
            }

            vertexCount += decalModels[i].Meshes[0].VertexCount;
            triangleCount += decalModels[i].Meshes[0].TriangleCount;
        }

        DrawText("TOTAL", (int)x0, (int)yPos, 10, Color.Lime);
        DrawText($"{vertexCount}", (int)x1, (int)yPos, 10, Color.Lime);
        DrawText($"{triangleCount}", (int)x2, (int)yPos, 10, Color.Lime);
        yPos += 15;

        DrawText("Hold RMB to move camera", 10, 430, 10, Color.Gray);
        DrawText("(c) Character model and texture from kenney.nl", screenWidth - 260, screenHeight - 20, 10, Color.Gray);

        // UI elements
        if (GuiButton(new Rectangle(10, screenHeight - 1000.0f, 100, 60), showModel ? "Hide Model" : "Show Model"))
        {
            showModel = !showModel;
        }

        if (GuiButton(new Rectangle(10 + 110, screenHeight - 100.0f, 100, 60), "Clear Decals"))
        {
            // Clear decals, unload all decal models
            for (var i = 0; i < decalModels.Count; i++)
            {
                UnloadModel(decalModels[i]);
            }
            decalModels.Clear();
        }

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadModel(model);
        UnloadTexture(modelTexture);

        // Unload decal models
        for (var i = 0; i < decalModels.Count; i++)
        {
            UnloadModel(decalModels[i]);
        }
        decalModels.Clear();

        UnloadTexture(decalTexture);
        UnloadModel(placementCube);
    }

    // Clip segment
    private static Vector3 ClipSegment(Vector3 v0, Vector3 v1, Vector3 p, float s)
    {
        var d0 = Vector3.Dot(v0, p) - s;
        var d1 = Vector3.Dot(v1, p) - s;
        var s0 = d0 / (d0 - d1);

        return Vector3.Lerp(v0, v1, s0);
    }

    // Generate mesh decals for provided model
    private static unsafe Mesh GenMeshDecal(Model target, Matrix4x4 projection, float decalSize, float decalOffset)
    {
        // We're going to use these to build up our decal meshes
        var meshBuilders = new List<Vector3>[2] { new(), new() };

        // We're going to need the inverse matrix
        var invProj = MatrixInvert(projection);

        // We'll be flip-flopping between the two mesh builders
        // Reading from one and writing to the other, then swapping
        var mbIndex = 0;

        // First pass, just get any triangle inside the bounding box (for each mesh of the model)
        for (var meshIndex = 0; meshIndex < target.MeshCount; meshIndex++)
        {
            var mesh = target.Meshes[meshIndex];
            for (var tri = 0; tri < mesh.TriangleCount; tri++)
            {
                var vertices = new Vector3[3];

                // The way we calculate the vertices of the mesh triangle
                // depend on whether the mesh vertices are indexed or not
                if (mesh.Indices == null)
                {
                    for (var v = 0; v < 3; v++)
                    {
                        vertices[v] = new Vector3(
                            mesh.Vertices[3 * 3 * tri + 3 * v + 0],
                            mesh.Vertices[3 * 3 * tri + 3 * v + 1],
                            mesh.Vertices[3 * 3 * tri + 3 * v + 2]
                        );
                    }
                }
                else
                {
                    for (var v = 0; v < 3; v++)
                    {
                        vertices[v] = new Vector3(
                            mesh.Vertices[3 * mesh.Indices[3 * tri + 0] + v],
                            mesh.Vertices[3 * mesh.Indices[3 * tri + 1] + v],
                            mesh.Vertices[3 * mesh.Indices[3 * tri + 2] + v]
                        );
                    }
                }

                // Transform all 3 vertices of the triangle
                // and check if they are inside our decal box
                var insideCount = 0;
                for (var i = 0; i < 3; i++)
                {
                    // To projection space
                    var v = Vector3Transform(vertices[i], projection);

                    if ((MathF.Abs(v.X) < decalSize) || (MathF.Abs(v.Y) <= decalSize) || (MathF.Abs(v.Z) <= decalSize))
                    {
                        insideCount++;
                    }

                    // We need to keep the transformed vertex
                    vertices[i] = v;
                }

                // If any of them are inside, we add the triangle - we'll clip it later
                if (insideCount > 0)
                {
                    meshBuilders[mbIndex].Add(vertices[0]);
                    meshBuilders[mbIndex].Add(vertices[1]);
                    meshBuilders[mbIndex].Add(vertices[2]);
                }
            }
        }

        // Clipping time! We need to clip against all 6 directions
        Vector3[] planes =
        {
            new(1, 0, 0),
            new(-1, 0, 0),
            new(0, 1, 0),
            new(0, -1, 0),
            new(0, 0, 1),
            new(0, 0, -1)
        };

        for (var face = 0; face < 6; face++)
        {
            // Swap current model builder (so we read from the one we just wrote to)
            mbIndex = 1 - mbIndex;

            var inMesh = meshBuilders[1 - mbIndex];
            var outMesh = meshBuilders[mbIndex];

            // Reset write builder
            outMesh.Clear();

            var s = 0.5f * decalSize;

            for (var i = 0; i < inMesh.Count; i += 3)
            {
                Vector3 nV1, nV2, nV3, nV4;

                var d1 = Vector3.Dot(inMesh[i + 0], planes[face]) - s;
                var d2 = Vector3.Dot(inMesh[i + 1], planes[face]) - s;
                var d3 = Vector3.Dot(inMesh[i + 2], planes[face]) - s;

                var v1Out = d1 > 0;
                var v2Out = d2 > 0;
                var v3Out = d3 > 0;

                // Calculate, how many vertices of the face lie outside of the clipping plane
                var total = (v1Out ? 1 : 0) + (v2Out ? 1 : 0) + (v3Out ? 1 : 0);

                switch (total)
                {
                    case 0:
                        // The entire face lies inside of the plane, no clipping needed
                        outMesh.Add(inMesh[i]);
                        outMesh.Add(inMesh[i + 1]);
                        outMesh.Add(inMesh[i + 2]);
                        break;
                    case 1:
                        // One vertex lies outside of the plane, perform clipping
                        if (v2Out)
                        {
                            nV1 = inMesh[i];
                            nV2 = inMesh[i + 2];
                            nV3 = ClipSegment(inMesh[i + 1], nV1, planes[face], s);
                            nV4 = ClipSegment(inMesh[i + 1], nV2, planes[face], s);

                            outMesh.Add(nV3);
                            outMesh.Add(nV2);
                            outMesh.Add(nV1);
                            outMesh.Add(nV2);
                            outMesh.Add(nV3);
                            outMesh.Add(nV4);
                        }
                        else
                        {
                            if (v1Out)
                            {
                                nV1 = inMesh[i + 1];
                                nV2 = inMesh[i + 2];
                                nV3 = ClipSegment(inMesh[i], nV1, planes[face], s);
                                nV4 = ClipSegment(inMesh[i], nV2, planes[face], s);
                            }
                            else // v3Out
                            {
                                nV1 = inMesh[i];
                                nV2 = inMesh[i + 1];
                                nV3 = ClipSegment(inMesh[i + 2], nV1, planes[face], s);
                                nV4 = ClipSegment(inMesh[i + 2], nV2, planes[face], s);
                            }

                            outMesh.Add(nV1);
                            outMesh.Add(nV2);
                            outMesh.Add(nV3);
                            outMesh.Add(nV4);
                            outMesh.Add(nV3);
                            outMesh.Add(nV2);
                        }
                        break;
                    case 2:
                        // Two vertices lies outside of the plane, perform clipping
                        if (!v1Out)
                        {
                            nV1 = inMesh[i];
                            nV2 = ClipSegment(nV1, inMesh[i + 1], planes[face], s);
                            nV3 = ClipSegment(nV1, inMesh[i + 2], planes[face], s);
                            outMesh.Add(nV1);
                            outMesh.Add(nV2);
                            outMesh.Add(nV3);
                        }

                        if (!v2Out)
                        {
                            nV1 = inMesh[i + 1];
                            nV2 = ClipSegment(nV1, inMesh[i + 2], planes[face], s);
                            nV3 = ClipSegment(nV1, inMesh[i], planes[face], s);
                            outMesh.Add(nV1);
                            outMesh.Add(nV2);
                            outMesh.Add(nV3);
                        }

                        if (!v3Out)
                        {
                            nV1 = inMesh[i + 2];
                            nV2 = ClipSegment(nV1, inMesh[i], planes[face], s);
                            nV3 = ClipSegment(nV1, inMesh[i + 1], planes[face], s);
                            outMesh.Add(nV1);
                            outMesh.Add(nV2);
                            outMesh.Add(nV3);
                        }
                        break;
                    default: // The entire face lies outside of the plane, so let's discard the corresponding vertices
                        break;
                }
            }
        }

        // Now we just need to re-transform the vertices
        var theMesh = meshBuilders[mbIndex];

        // Allocate room for UVs
        if (theMesh.Count > 0)
        {
            var uvs = new Vector2[theMesh.Count];

            for (var i = 0; i < theMesh.Count; i++)
            {
                var vert = theMesh[i];

                // Calculate the UVs based on the projected coords
                // They are clipped to (-decalSize .. decalSize) and we want them (0..1)
                uvs[i] = new Vector2(vert.X / decalSize + 0.5f, vert.Y / decalSize + 0.5f);

                // Tiny nudge in the normal direction so it renders properly over the mesh
                vert.Z -= decalOffset;

                // From projection space to world space
                theMesh[i] = Vector3Transform(vert, invProj);
            }

            // Decal model data ready, create the mesh and return it
            return BuildMesh(theMesh, uvs);
        }

        // Return a blank mesh as there's nothing to add
        return new Mesh();
    }

    // Build a Mesh from builder data
    private static unsafe Mesh BuildMesh(List<Vector3> builderVertices, Vector2[] uvs)
    {
        Mesh outMesh = new(builderVertices.Count, builderVertices.Count / 3);
        outMesh.AllocVertices();
        outMesh.AllocTexCoords();

        var vertices = outMesh.VerticesAs<Vector3>();
        var texcoords = outMesh.TexCoordsAs<Vector2>();

        for (var i = 0; i < builderVertices.Count; i++)
        {
            vertices[i] = builderVertices[i];
            texcoords[i] = uvs[i];
        }

        UploadMesh(ref outMesh, false);

        return outMesh;
    }

    // Button UI element
    private static bool GuiButton(Rectangle rec, string label)
    {
        var bgColor = Color.Gray;
        var pressed = false;

        if (CheckCollisionPointRec(GetMousePosition(), rec))
        {
            bgColor = Color.LightGray;
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                pressed = true;
            }
        }

        DrawRectangleRec(rec, bgColor);
        DrawRectangleLinesEx(rec, 2.0f, Color.DarkGray);

        var fontSize = 10;
        var textWidth = MeasureText(label, fontSize);

        DrawText(label, (int)(rec.X + rec.Width * 0.5f - textWidth * 0.5f), (int)(rec.Y + rec.Height * 0.5f - fontSize * 0.5f), fontSize, Color.DarkGray);

        return pressed;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [models] example - decals");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Decals();
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
