/*******************************************************************************************
*
*   raylib [shaders] example - palette switch
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: This example requires raylib OpenGL 3.3 or ES2 versions for shaders support,
*         OpenGL 1.1 does not support shaders, recompile raylib to OpenGL 3.3 version
*
*   NOTE: Shaders used in this example are #version 330 (OpenGL 3.3), to test this example
*         on OpenGL ES 2.0 platforms (Android, Raspberry Pi, HTML5), use #version 100 shaders
*         raylib comes with shaders ready for both versions, check raylib/shaders install folder
*
*   Example originally created with raylib 2.5, last time updated with raylib 3.7
*
*   Example contributed by Marco Lizza (@MarcoLizza) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2019-2025 Marco Lizza (@MarcoLizza) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Shaders;

public class PaletteSwitch : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif
    private const int ColorsPerPalette = 8;
    private const int VALUES_PER_COLOR = 3;

    public string Name => "Shaders / Palette Switch";

    public string Title => "raylib [shaders] example - palette switch";

    private int[][] Palettes = new int[][] {
            // 3-BIT RGB
            new int[] {
                0, 0, 0,
                255, 0, 0,
                0, 255, 0,
                0, 0, 255,
                0, 255, 255,
                255, 0, 255,
                255, 255, 0,
                255, 255, 255,
            },
            // AMMO-8 (GameBoy-like)
            new int[] {
                4, 12, 6,
                17, 35, 24,
                30, 58, 41,
                48, 93, 66,
                77, 128, 97,
                137, 162, 87,
                190, 220, 127,
                238, 255, 204,
            },
            // RKBV (2-strip film)
            new int[] {
                21, 25, 26,
                138, 76, 88,
                217, 98, 117,
                230, 184, 193,
                69, 107, 115,
                75, 151, 166,
                165, 189, 194,
                255, 245, 247,
            }
        };

    private string[] PaletteText = new string[] {
            "3-BIT RGB",
            "AMMO-8 (GameBoy-like)",
            "RKBV (2-strip film)"
        };

    private Shader shader;
    private int paletteLoc;
    private int currentPalette;
    private int lineHeight;

    public void Init()
    {
        // Load shader to be used on some parts drawing
        // NOTE 1: Using GLSL 330 shader version, on OpenGL ES 2.0 use GLSL 100 shader version
        // NOTE 2: Defining 0 (NULL) for vertex shader forces usage of internal default vertex shader
        shader = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/palette_switch.fs");

        // Get variable (uniform) location on the shader to connect with the program
        // NOTE: If uniform variable could not be found in the shader, function returns -1
        paletteLoc = GetShaderLocation(shader, "palette");

        currentPalette = 0;
        lineHeight = screenHeight / ColorsPerPalette;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.Right))
        {
            currentPalette++;
        }
        else if (IsKeyPressed(KeyboardKey.Left))
        {
            currentPalette--;
        }

        if (currentPalette >= Palettes.Length)
        {
            currentPalette = 0;
        }
        else if (currentPalette < 0)
        {
            currentPalette = Palettes.Length - 1;
        }

        // Send palette data to the shader to be used on drawing
        // NOTE: We are sending RGB triplets w/o the alpha channel
        Raylib.SetShaderValueV(
            shader,
            paletteLoc,
            Palettes[currentPalette],
            ShaderUniformDataType.IVec3,
            ColorsPerPalette
        );
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        BeginShaderMode(shader);

        for (var i = 0; i < ColorsPerPalette; i++)
        {
            // Draw horizontal screen-wide rectangles with increasing "palette index"
            // The used palette index is encoded in the RGB components of the pixel
            DrawRectangle(0, lineHeight * i, GetScreenWidth(), lineHeight, new Color(i, i, i, 255));
        }

        EndShaderMode();

        DrawText("< >", 10, 10, 30, Color.DarkBlue);
        DrawText("CURRENT PALETTE:", 60, 15, 20, Color.RayWhite);
        DrawText(PaletteText[currentPalette], 300, 15, 20, Color.Red);

        DrawFPS(700, 15);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadShader(shader);       // Unload shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - palette switch");

        SetTargetFPS(60);                       // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new PaletteSwitch();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())            // Detect window close button or ESC key
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
