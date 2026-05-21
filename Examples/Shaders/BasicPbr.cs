/*******************************************************************************************
 *
 *   raylib [shaders] example - Basic PBR
 *
 *   Example originally created with raylib 5.0, last time updated with raylib 5.1-dev
 *
 *   Example contributed by Afan OLOVCIC (@_DevDad) and reviewed by Ramon Santamaria (@raysan5)
 *
 *   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
 *   BSD-like license that allows static linking with closed source software
 *
 *   Copyright (c) 2023-2024 Afan OLOVCIC (@_DevDad)
 *
 *   Model: "Old Rusty Car" (https://skfb.ly/LxRy) by Renafox,
 *   licensed under Creative Commons Attribution-NonCommercial
 *   (http://creativecommons.org/licenses/by-nc/4.0/)
 *
 ********************************************************************************************/

using System.Numerics;
using Examples.Shared;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

public class BasicPbr
{
    private const int GLSL_VERSION = 330;

    public static unsafe int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        // Enable Multi Sampling Anti Aliasing 4x (if available)
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - basic pbr");

        // Define the camera to look into our 3d world
        Camera3D camera = new();
        camera.Position = new Vector3(2.0f, 4.0f, 6.0f);
        camera.Target = new Vector3(0.0f, 0.5f, 0.0f);
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.FovY = 45.0f;
        camera.Projection = CameraProjection.Perspective;

        // Load PBR shader and setup all required locations
        var shader = LoadShader("resources/shaders/glsl330/pbr.vs", "resources/shaders/glsl330/pbr.fs");

        shader.Locs[(int)ShaderLocationIndex.MapAlbedo] = GetShaderLocation(shader, "albedoMap");
        // WARNING: Metalness, roughness, and ambient occlusion are all packed into a MRA texture
        // They are passed as to the SHADER_LOC_MAP_METALNESS location for convenience,
        // shader already takes care of it accordingly
        shader.Locs[(int)ShaderLocationIndex.MapMetalness] = GetShaderLocation(shader, "mraMap");
        shader.Locs[(int)ShaderLocationIndex.MapNormal] = GetShaderLocation(shader, "normalMap");
        // WARNING: Similar to the MRA map, the emissive map packs different information
        // into a single texture: it stores height and emission data
        // It is binded to SHADER_LOC_MAP_EMISSION location an properly processed on shader
        shader.Locs[(int)ShaderLocationIndex.MapEmission] = GetShaderLocation(shader, "emissiveMap");
        shader.Locs[(int)ShaderLocationIndex.ColorDiffuse] = GetShaderLocation(shader, "albedoColor");

        // Setup additional required shader locations, including lights data
        shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shader, "viewPos");
        var lightCountLoc = GetShaderLocation(shader, "numOfLights");
        var maxLightCount = 4;
        SetShaderValue(shader, lightCountLoc, &maxLightCount, ShaderUniformDataType.Int);

        // Setup ambient color and intensity parameters
        var ambientIntensity = 0.02f;
        var ambientColor = new Color(26, 32, 135, 255);
        var ambientColorNormalized = new Vector3(ambientColor.R / 255.0F, ambientColor.G / 255.0F, ambientColor.B / 255.0F);
        SetShaderValue(shader, GetShaderLocation(shader, "ambientColor"), &ambientColorNormalized, ShaderUniformDataType.Vec3);
        SetShaderValue(shader, GetShaderLocation(shader, "ambient"), &ambientIntensity, ShaderUniformDataType.Float);

        // Get location for shader parameters that can be modified in real time
        var emissiveIntensityLoc = GetShaderLocation(shader, "emissivePower");
        var emissiveColorLoc = GetShaderLocation(shader, "emissiveColor");
        var textureTilingLoc = GetShaderLocation(shader, "tiling");

        // Load old car model using PBR maps and shader
        // WARNING: We know this model consists of a single model.meshes[0] and
        // that model.materials[0] is by default assigned to that mesh
        // There could be more complex models consisting of multiple meshes and
        // multiple materials defined for those meshes... but always 1 mesh = 1 material
        var car = LoadModel("resources/models/gltf/old_car_new.glb");

        // Assign already setup PBR shader to model.materials[0], used by models.meshes[0]
        car.Materials[0].Shader = shader;

        // Setup materials[0].maps default parameters
        car.Materials[0].Maps[(int)MaterialMapIndex.Albedo].Color = Color.White;
        car.Materials[0].Maps[(int)MaterialMapIndex.Metalness].Value = 0.0f;
        car.Materials[0].Maps[(int)MaterialMapIndex.Roughness].Value = 0.0f;
        car.Materials[0].Maps[(int)MaterialMapIndex.Occlusion].Value = 1.0f;
        car.Materials[0].Maps[(int)MaterialMapIndex.Emission].Color = new Color(255, 162, 0, 255);

        // Setup materials[0].maps default textures
        car.Materials[0].Maps[(int)MaterialMapIndex.Albedo].Texture = LoadTexture("resources/old_car_d.png");
        car.Materials[0].Maps[(int)MaterialMapIndex.Metalness].Texture = LoadTexture("resources/old_car_mra.png");
        car.Materials[0].Maps[(int)MaterialMapIndex.Normal].Texture = LoadTexture("resources/old_car_n.png");
        car.Materials[0].Maps[(int)MaterialMapIndex.Emission].Texture = LoadTexture("resources/old_car_e.png");

        // Load floor model mesh and assign material parameters
        // NOTE: A basic plane shape can be generated instead of being loaded from a model file
        var floor = LoadModel("resources/models/gltf/plane.glb");
        //Mesh floorMesh = GenMeshPlane(10, 10, 10, 10);
        //GenMeshTangents(&floorMesh);      // TODO: Review tangents generation
        //Model floor = LoadModelFromMesh(floorMesh);

        // Assign material shader for our floor model, same PBR shader
        floor.Materials[0].Shader = shader;

        floor.Materials[0].Maps[(int)MaterialMapIndex.Albedo].Color = Color.White;
        floor.Materials[0].Maps[(int)MaterialMapIndex.Metalness].Value = 0.0f;
        floor.Materials[0].Maps[(int)MaterialMapIndex.Roughness].Value = 0.0f;
        floor.Materials[0].Maps[(int)MaterialMapIndex.Occlusion].Value = 1.0f;
        floor.Materials[0].Maps[(int)MaterialMapIndex.Emission].Color = Color.Black;

        floor.Materials[0].Maps[(int)MaterialMapIndex.Albedo].Texture = LoadTexture("resources/road_a.png");
        floor.Materials[0].Maps[(int)MaterialMapIndex.Metalness].Texture = LoadTexture("resources/road_mra.png");
        floor.Materials[0].Maps[(int)MaterialMapIndex.Normal].Texture = LoadTexture("resources/road_n.png");

        // Models texture tiling parameter can be stored in the Material struct if required (CURRENTLY NOT USED)
        // NOTE: Material.params[4] are available for generic parameters storage (float)
        var carTextureTiling = new Vector2(0.5f, 0.5f);
        var floorTextureTiling = new Vector2(0.5f, 0.5f);

        // Create some lights
        var lights = new PbrLight[4];
        lights[0] = PbrLights.CreateLight(
            0,
            PbrLightType.Point,
            new Vector3(-1.0f, 1.0f, -2.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            Color.Yellow,
            4.0f,
            shader);
        lights[1] = PbrLights.CreateLight(1,
            PbrLightType.Point,
            new Vector3(2.0f, 1.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            Color.Green,
            3.3f,
            shader);
        lights[2] = PbrLights.CreateLight(
            2, PbrLightType.Point,
            new Vector3(-2.0f, 1.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            Color.Red,
            8.3f,
            shader);
        lights[3] = PbrLights.CreateLight(
            3,
            PbrLightType.Point,
            new Vector3(1.0f, 1.0f, -2.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            Color.Blue,
            2.0f,
            shader);

        // Setup material texture maps usage in shader
        // NOTE: By default, the texture maps are always used
        var usage = 1;
        SetShaderValue(shader, GetShaderLocation(shader, "useTexAlbedo"), &usage, ShaderUniformDataType.Int);
        SetShaderValue(shader, GetShaderLocation(shader, "useTexNormal"), &usage, ShaderUniformDataType.Int);
        SetShaderValue(shader, GetShaderLocation(shader, "useTexMRA"), &usage, ShaderUniformDataType.Int);
        SetShaderValue(shader, GetShaderLocation(shader, "useTexEmissive"), &usage, ShaderUniformDataType.Int);

        SetTargetFPS(60); // Set our game to run at 60 frames-per-second
        //---------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose()) // Detect window close button or ESC key
        {
            // Update
            //----------------------------------------------------------------------------------
            UpdateCamera(&camera, CameraMode.Orbital);

            // Update the shader with the camera view vector (points towards { 0.0f, 0.0f, 0.0f })
            var cameraPos = camera.Position;
            SetShaderValue(shader, shader.Locs[(int)ShaderLocationIndex.VectorView], cameraPos, ShaderUniformDataType.Vec3);

            // Check key inputs to enable/disable lights
            if (IsKeyPressed(KeyboardKey.One))
            {
                lights[2].Enabled = !lights[2].Enabled;
            }

            if (IsKeyPressed(KeyboardKey.Two))
            {
                lights[1].Enabled = !lights[1].Enabled;
            }

            if (IsKeyPressed(KeyboardKey.Three))
            {
                lights[3].Enabled = !lights[3].Enabled;
            }

            if (IsKeyPressed(KeyboardKey.Four))
            {
                lights[0].Enabled = !lights[0].Enabled;
            }

            // Update light values on shader (actually, only enable/disable them)
            for (var i = 0; i < 4; i++)
            {
                UpdateLight(shader, lights[i]);
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();

            ClearBackground(Color.Black);

            BeginMode3D(camera);

            // Set floor model texture tiling and emissive color parameters on shader
            SetShaderValue(shader, textureTilingLoc, &floorTextureTiling, ShaderUniformDataType.Vec2);
            var floorEmissiveColor = ColorNormalize(floor.Materials[0].Maps[(int)MaterialMapIndex.Emission].Color);
            SetShaderValue(shader, emissiveColorLoc, &floorEmissiveColor, ShaderUniformDataType.Vec4);

            DrawModel(floor, Vector3.Zero, 5.0f, Color.White); // Draw floor model

            // Set old car model texture tiling, emissive color and emissive intensity parameters on shader
            SetShaderValue(shader, textureTilingLoc, &carTextureTiling, ShaderUniformDataType.Vec2);
            var carEmissiveColor = ColorNormalize(car.Materials[0].Maps[(int)MaterialMapIndex.Emission].Color);
            SetShaderValue(shader, emissiveColorLoc, &carEmissiveColor, ShaderUniformDataType.Vec4);
            var emissiveIntensity = 0.01f;
            SetShaderValue(shader, emissiveIntensityLoc, &emissiveIntensity, ShaderUniformDataType.Float);

            DrawModel(car, Vector3.Zero, 0.25f, Color.White); // Draw car model

            // Draw spheres to show the lights positions
            for (var i = 0; i < 4; i++)
            {
                var color = lights[i].Color;
                var lightColor = new Color((byte)(color.X * 255), (byte)(color.Y * 255), (byte)(color.Z * 255),
                    (byte)(color.W * 255));

                if (lights[i].Enabled)
                {
                    DrawSphereEx(lights[i].Position, 0.2f, 8, 8, lightColor);
                }
                else
                {
                    DrawSphereWires(lights[i].Position, 0.2f, 8, 8, ColorAlpha(lightColor, 0.3f));
                }
            }

            EndMode3D();

            DrawText("Toggle lights: [1][2][3][4]", 10, 40, 20, Color.LightGray);

            DrawText("(c) Old Rusty Car model by Renafox (https://skfb.ly/LxRy)", screenWidth - 320, screenHeight - 20, 10, Color.LightGray);

            DrawFPS(10, 10);

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        // Unbind (disconnect) shader from car.material[0]
        // to avoid UnloadMaterial() trying to unload it automatically
        car.Materials[0].Shader = new();
        UnloadMaterial(car.Materials[0]);
        car.Materials[0].Maps = null;
        UnloadModel(car);

        floor.Materials[0].Shader = new();
        UnloadMaterial(floor.Materials[0]);
        floor.Materials[0].Maps = null;
        UnloadModel(floor);

        UnloadShader(shader);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }

    private static void UpdateLight(Shader shader, PbrLight light)
    {
        SetShaderValue(shader, light.EnabledLoc, light.Enabled, ShaderUniformDataType.Int);
        SetShaderValue(shader, light.TypeLoc, light.Type, ShaderUniformDataType.Int);

        // Send to shader light position values
        SetShaderValue(shader, light.PositionLoc, light.Position, ShaderUniformDataType.Vec3);

        // Send to shader light target position values
        SetShaderValue(shader, light.TargetLoc, light.Target, ShaderUniformDataType.Vec3);
        SetShaderValue(shader, light.ColorLoc, light.Color, ShaderUniformDataType.Vec4);
        SetShaderValue(shader, light.IntensityLoc, light.Intensity, ShaderUniformDataType.Float);
    }
}
