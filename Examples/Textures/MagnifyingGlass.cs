/*******************************************************************************************
*
*   raylib textures example - magnifying glass
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example contributed by Luke Vaughan (@badram) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Luke Vaughan (@badram)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public partial class MagnifyingGlass : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Magnifying Glass";

    public string Title => "raylib [textures] example - magnifying glass";

    private Texture2D bunny;
    private Texture2D parrots;
    private Texture2D mask;
    private RenderTexture2D magnifiedWorld;
    private Camera2D camera;

    public void Init()
    {
        bunny = LoadTexture("resources/raybunny.png");
        parrots = LoadTexture("resources/parrots.png");

        // Use image draw to generate a mask texture instead of loading it from a file.
        var circle = GenImageColor(256, 256, Color.Blank);
        ImageDrawCircle(ref circle, 128, 128, 128, Color.White);
        mask = LoadTextureFromImage(circle); // Copy the mask image from RAM to VRAM
        UnloadImage(circle); // Unload the image from RAM

        magnifiedWorld = LoadRenderTexture(256, 256);

        camera = new Camera2D();
        // Set magnifying glass zoom
        camera.Zoom = 2;
        // Offset by half the size of the magnifying glass to counteract drawing the texture centered on the mouse position
        camera.Offset = new Vector2(128, 128);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        var mPos = GetMousePosition();
        camera.Target = mPos;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Draw the normal version of the world
        DrawTexture(parrots, 144, 33, Color.White);
        DrawText("Use the magnifying glass to find hidden bunnies!", 154, 6, 20, Color.Black);

        // Render to a the magnifying glass
        BeginTextureMode(magnifiedWorld);
        ClearBackground(Color.RayWhite);

        BeginMode2D(camera);
        // Draw the same things in the magnified world as were in the normal version
        DrawTexture(parrots, 144, 33, Color.White);
        DrawText("Use the magnifying glass to find hidden bunnies!", 154, 6, 20, Color.Black);

        // Draw bunnies only in the magnified world.
        // BLEND_MULTIPLIED lets them take on the color of the image below them.
        BeginBlendMode(BlendMode.Multiplied);
        DrawTexture(bunny, 250, 350, Color.White);
        DrawTexture(bunny, 500, 100, Color.White);
        DrawTexture(bunny, 420, 300, Color.White);
        DrawTexture(bunny, 650, 10, Color.White);
        EndBlendMode();
        EndMode2D();

        // Mask the magnifying glass view texture to a circle
        // To make the mask affect only alpha, a CUSTOM blend mode is used with SEPARATE color/alpha functions
        BeginBlendMode(BlendMode.CustomSeparate);
        // C: Color, A: Alpha, s: source (texture to draw), d: destination (texture drawn to)
        //   glSrcRGB: RL_ZERO      - Cs * 0 = 0  - discard source rgb because we don't want to draw our texture's colors at all
        //   glDstRGB: RL_ONE       - Cd * 1 = Cd - use destination colors unmodified
        //   glSrcAlpha: RL_ONE     - As * 1 = As - use source alpha unmodified
        //   glDstAlpha: RL_ZERO    - Ad * 0 = 0  - discard destination alpha
        //   glEqRGB: RL_FUNC_ADD   - Cs(0) + Cd = Cd - destination color is unmodified
        //   glEqAlpha: RL_FUNC_ADD - As + Ad(0) = As - destination alpha is set to source alpha
        Rlgl.SetBlendFactorsSeparate(Rlgl.ZERO, Rlgl.ONE, Rlgl.ONE, Rlgl.ZERO, Rlgl.FUNC_ADD, Rlgl.FUNC_ADD);
        DrawTexture(mask, 0, 0, Color.White);
        EndBlendMode();
        EndTextureMode();

        // Draw magnifiedWorld to screen, centered on cursor
        DrawTextureRec(magnifiedWorld.Texture, new Rectangle(0, 0, 256, -256), new Vector2(mPos.X - 128, mPos.Y - 128), Color.White);

        // Draw the outer ring of the magnifying glass
        DrawRing(mPos, 126, 130, 0, 360, 64, Color.Black);

        // Draw floating specular highlight on the glass
        var rx = mPos.X / 800;
        var ry = mPos.Y / 800;
        DrawCircle((int)(mPos.X - 64 * rx) - 32, (int)(mPos.Y - 64 * ry) - 32, 4, ColorAlpha(Color.White, 0.5f));

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(parrots);
        UnloadTexture(bunny);
        UnloadTexture(mask);
        UnloadRenderTexture(magnifiedWorld);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - magnifying glass");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new MagnifyingGlass();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();                // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
