/*******************************************************************************************
*
*   raylib [shaders] example - ascii rendering
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 6.0
*
*   Example contributed by Maicon Santana (@maiconpintoabreu) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Maicon Santana (@maiconpintoabreu)
*
********************************************************************************************/

namespace Examples.Shaders;

public partial class AsciiRendering : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    public string Name => "Shaders / Ascii Rendering";

    public string Title => "raylib [shaders] example - ascii rendering";

    private Texture2D fudesumi;
    private Texture2D raysan;
    private Shader shader;
    private int resolutionLoc;
    private int fontSizeLoc;
    private float fontSize;
    private Vector2 circlePos;
    private float circleSpeed;
    private RenderTexture2D target;

    public void Init()
    {
        // Texture to test static drawing
        fudesumi = LoadTexture("resources/fudesumi.png");
        // Texture to test moving drawing
        raysan = LoadTexture("resources/raysan.png");

        // Load shader to be used on postprocessing
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/ascii.fs");

        // These locations are used to send data to the GPU
        resolutionLoc = GetShaderLocation(shader, "resolution");
        fontSizeLoc = GetShaderLocation(shader, "fontSize");

        // Set the character size for the ASCII effect
        // Fontsize should be 9 or more
        fontSize = 9.0f;

        // Send the updated values to the shader
        var resolution = new[] { (float)screenWidth, (float)screenHeight };
        Raylib.SetShaderValue(shader, resolutionLoc, resolution, ShaderUniformDataType.Vec2);

        circlePos = new Vector2(40.0f, screenHeight * 0.5f);
        circleSpeed = 1.0f;

        // RenderTexture to apply the postprocessing later
        target = LoadRenderTexture(screenWidth, screenHeight);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        circlePos.X += circleSpeed;
        if ((circlePos.X > 200.0f) || (circlePos.X < 40.0f))
        {
            circleSpeed *= -1; // Revert speed
        }

        if (IsKeyPressed(KeyboardKey.Left) && (fontSize > 9.0))
        {
            fontSize -= 1;  // Reduce fontSize
        }
        if (IsKeyPressed(KeyboardKey.Right) && (fontSize < 15.0))
        {
            fontSize += 1;  // Increase fontSize
        }

        // Set fontsize for the shader
        Raylib.SetShaderValue(shader, fontSizeLoc, fontSize, ShaderUniformDataType.Float);

        // Draw
        //----------------------------------------------------------------------------------
        BeginTextureMode(target);
        ClearBackground(Color.White);

        // Draw scene in our render texture
        DrawTexture(fudesumi, 500, -30, Color.White);
        DrawTextureV(raysan, circlePos, Color.White);
        EndTextureMode();

        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginShaderMode(shader);
        // Draw the scene texture (that we rendered earlier) to the screen
        // The shader will process every pixel of this texture
        DrawTextureRec(
            target.Texture,
            new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
            new Vector2(0, 0),
            Color.White
        );
        EndShaderMode();

        DrawRectangle(0, 0, screenWidth, 40, Color.Black);
        DrawText($"Ascii effect - FontSize:{fontSize,2:F0} - [Left] -1 [Right] +1 ", 120, 10, 20, Color.LightGray);
        DrawFPS(10, 10);
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(target);    // Unload render texture

        UnloadShader(shader);           // Unload shader
        UnloadTexture(fudesumi);        // Unload texture
        UnloadTexture(raysan);          // Unload texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - ascii rendering");

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new AsciiRendering();
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
