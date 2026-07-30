/*******************************************************************************************
*
*   raylib [shaders] example - rounded rectangle
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Anstro Pleuton (@anstropleuton) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Anstro Pleuton (@anstropleuton)
*
********************************************************************************************/

namespace Examples.Shaders;

public class RoundedRectangle : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Rounded Rectangle";

    public string Title => "raylib [shaders] example - rounded rectangle";

    // Rounded rectangle data
    private struct RoundedRect
    {
        public Vector4 CornerRadius; // Individual corner radius (top-left, top-right, bottom-left, bottom-right)

        // Shadow variables
        public float ShadowRadius;
        public Vector2 ShadowOffset;
        public float ShadowScale;

        // Border variables
        public float BorderThickness; // Inner-border thickness

        // Shader locations
        public int RectangleLoc;
        public int RadiusLoc;
        public int ColorLoc;
        public int ShadowRadiusLoc;
        public int ShadowOffsetLoc;
        public int ShadowScaleLoc;
        public int ShadowColorLoc;
        public int BorderThicknessLoc;
        public int BorderColorLoc;
    }

    private Shader shader;
    private RoundedRect roundedRectangle;

    private readonly Color rectangleColor = Color.Blue;
    private readonly Color shadowColor = Color.DarkBlue;
    private readonly Color borderColor = Color.SkyBlue;

    public void Init()
    {
        // Load the shader
        shader = LoadShader(
            $"resources/shaders/glsl{GlslVersion}/base.vs",
            $"resources/shaders/glsl{GlslVersion}/rounded_rectangle.fs"
        );

        // Create a rounded rectangle
        roundedRectangle = CreateRoundedRectangle(
            new Vector4(5.0f, 10.0f, 15.0f, 20.0f),     // Corner radius
            20.0f,                                      // Shadow radius
            new Vector2(0.0f, -5.0f),                   // Shadow offset
            0.95f,                                      // Shadow scale
            5.0f,                                       // Border thickness
            shader                                      // Shader
        );

        // Update shader uniforms
        UpdateRoundedRectangle(roundedRectangle, shader);
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw rectangle box with rounded corners using shader
        Rectangle rec = new(50, 70, 110, 60);
        DrawRectangleLines((int)rec.X - 20, (int)rec.Y - 20, (int)rec.Width + 40, (int)rec.Height + 40, Color.DarkGray);
        DrawText("Rounded rectangle", (int)rec.X - 20, (int)rec.Y - 35, 10, Color.DarkGray);

        // Flip Y axis to match shader coordinate system
        rec.Y = screenHeight - rec.Y - rec.Height;
        Raylib.SetShaderValue(shader, roundedRectangle.RectangleLoc, new[] { rec.X, rec.Y, rec.Width, rec.Height }, ShaderUniformDataType.Vec4);

        // Only rectangle color
        Raylib.SetShaderValue(shader, roundedRectangle.ColorLoc, new[] { rectangleColor.R / 255.0f, rectangleColor.G / 255.0f, rectangleColor.B / 255.0f, rectangleColor.A / 255.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.ShadowColorLoc, new[] { 0.0f, 0.0f, 0.0f, 0.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.BorderColorLoc, new[] { 0.0f, 0.0f, 0.0f, 0.0f }, ShaderUniformDataType.Vec4);

        BeginShaderMode(shader);
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

        // Draw rectangle shadow using shader
        rec = new Rectangle(50, 200, 110, 60);
        DrawRectangleLines((int)rec.X - 20, (int)rec.Y - 20, (int)rec.Width + 40, (int)rec.Height + 40, Color.DarkGray);
        DrawText("Rounded rectangle shadow", (int)rec.X - 20, (int)rec.Y - 35, 10, Color.DarkGray);

        rec.Y = screenHeight - rec.Y - rec.Height;
        Raylib.SetShaderValue(shader, roundedRectangle.RectangleLoc, new[] { rec.X, rec.Y, rec.Width, rec.Height }, ShaderUniformDataType.Vec4);

        // Only shadow color
        Raylib.SetShaderValue(shader, roundedRectangle.ColorLoc, new[] { 0.0f, 0.0f, 0.0f, 0.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.ShadowColorLoc, new[] { shadowColor.R / 255.0f, shadowColor.G / 255.0f, shadowColor.B / 255.0f, shadowColor.A / 255.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.BorderColorLoc, new[] { 0.0f, 0.0f, 0.0f, 0.0f }, ShaderUniformDataType.Vec4);

        BeginShaderMode(shader);
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

        // Draw rectangle's border using shader
        rec = new Rectangle(50, 330, 110, 60);
        DrawRectangleLines((int)rec.X - 20, (int)rec.Y - 20, (int)rec.Width + 40, (int)rec.Height + 40, Color.DarkGray);
        DrawText("Rounded rectangle border", (int)rec.X - 20, (int)rec.Y - 35, 10, Color.DarkGray);

        rec.Y = screenHeight - rec.Y - rec.Height;
        Raylib.SetShaderValue(shader, roundedRectangle.RectangleLoc, new[] { rec.X, rec.Y, rec.Width, rec.Height }, ShaderUniformDataType.Vec4);

        // Only border color
        Raylib.SetShaderValue(shader, roundedRectangle.ColorLoc, new[] { 0.0f, 0.0f, 0.0f, 0.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.ShadowColorLoc, new[] { 0.0f, 0.0f, 0.0f, 0.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.BorderColorLoc, new[] { borderColor.R / 255.0f, borderColor.G / 255.0f, borderColor.B / 255.0f, borderColor.A / 255.0f }, ShaderUniformDataType.Vec4);

        BeginShaderMode(shader);
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

        // Draw one more rectangle with all three colors
        rec = new Rectangle(240, 80, 500, 300);
        DrawRectangleLines((int)rec.X - 30, (int)rec.Y - 30, (int)rec.Width + 60, (int)rec.Height + 60, Color.DarkGray);
        DrawText("Rectangle with all three combined", (int)rec.X - 30, (int)rec.Y - 45, 10, Color.DarkGray);

        rec.Y = screenHeight - rec.Y - rec.Height;
        Raylib.SetShaderValue(shader, roundedRectangle.RectangleLoc, new[] { rec.X, rec.Y, rec.Width, rec.Height }, ShaderUniformDataType.Vec4);

        // All three colors
        Raylib.SetShaderValue(shader, roundedRectangle.ColorLoc, new[] { rectangleColor.R / 255.0f, rectangleColor.G / 255.0f, rectangleColor.B / 255.0f, rectangleColor.A / 255.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.ShadowColorLoc, new[] { shadowColor.R / 255.0f, shadowColor.G / 255.0f, shadowColor.B / 255.0f, shadowColor.A / 255.0f }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, roundedRectangle.BorderColorLoc, new[] { borderColor.R / 255.0f, borderColor.G / 255.0f, borderColor.B / 255.0f, borderColor.A / 255.0f }, ShaderUniformDataType.Vec4);

        BeginShaderMode(shader);
        DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
        EndShaderMode();

        DrawText("(c) Rounded rectangle SDF by Iñigo Quilez. MIT License.", screenWidth - 300, screenHeight - 20, 10, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader); // Unload shader
    }

    // Create a rounded rectangle and set uniform locations
    private static RoundedRect CreateRoundedRectangle(Vector4 cornerRadius, float shadowRadius, Vector2 shadowOffset, float shadowScale, float borderThickness, Shader shader)
    {
        RoundedRect rec;
        rec.CornerRadius = cornerRadius;
        rec.ShadowRadius = shadowRadius;
        rec.ShadowOffset = shadowOffset;
        rec.ShadowScale = shadowScale;
        rec.BorderThickness = borderThickness;

        // Get shader uniform locations
        rec.RectangleLoc = GetShaderLocation(shader, "rectangle");
        rec.RadiusLoc = GetShaderLocation(shader, "radius");
        rec.ColorLoc = GetShaderLocation(shader, "color");
        rec.ShadowRadiusLoc = GetShaderLocation(shader, "shadowRadius");
        rec.ShadowOffsetLoc = GetShaderLocation(shader, "shadowOffset");
        rec.ShadowScaleLoc = GetShaderLocation(shader, "shadowScale");
        rec.ShadowColorLoc = GetShaderLocation(shader, "shadowColor");
        rec.BorderThicknessLoc = GetShaderLocation(shader, "borderThickness");
        rec.BorderColorLoc = GetShaderLocation(shader, "borderColor");

        UpdateRoundedRectangle(rec, shader);

        return rec;
    }

    // Update rounded rectangle uniforms
    private static void UpdateRoundedRectangle(RoundedRect rec, Shader shader)
    {
        Raylib.SetShaderValue(shader, rec.RadiusLoc, new[] { rec.CornerRadius.X, rec.CornerRadius.Y, rec.CornerRadius.Z, rec.CornerRadius.W }, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(shader, rec.ShadowRadiusLoc, rec.ShadowRadius, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, rec.ShadowOffsetLoc, new[] { rec.ShadowOffset.X, rec.ShadowOffset.Y }, ShaderUniformDataType.Vec2);
        Raylib.SetShaderValue(shader, rec.ShadowScaleLoc, rec.ShadowScale, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, rec.BorderThicknessLoc, rec.BorderThickness, ShaderUniformDataType.Float);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - rounded rectangle");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new RoundedRectangle();
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
