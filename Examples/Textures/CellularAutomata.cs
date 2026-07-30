/*******************************************************************************************
*
*   raylib [textures] example - cellular automata
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.6, last time updated with raylib 5.6
*
*   Example contributed by Jordi Santonja (@JordSant) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jordi Santonja (@JordSant)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class CellularAutomata : IExample
{
    // Initialization constants
    //--------------------------------------------------------------------------------------
    private const int screenWidth = 800;
    private const int screenHeight = 450;
    private const int imageWidth = 800;
    private const int imageHeight = 800 / 2;

    // Rule button sizes and positions
    private const int drawRuleStartX = 585;
    private const int drawRuleStartY = 10;
    private const int drawRuleSpacing = 15;
    private const int drawRuleGroupSpacing = 50;
    private const int drawRuleSize = 14;
    private const int drawRuleInnerSize = 10;

    // Preset button sizes
    private const int presetsSizeX = 42;
    private const int presetsSizeY = 22;

    private const int linesUpdatedPerFrame = 4;

    public string Name => "Textures / Cellular Automata";

    public string Title => "raylib [textures] example - cellular automata";

    // Some interesting rules
    private static readonly int[] presetValues = { 18, 30, 60, 86, 102, 124, 126, 150, 182, 225 };
    private const int presetsCount = 10;

    private Image image;
    private Texture2D texture;
    private int rule;
    private int line;

    private static void ComputeLine(ref Image image, int line, int rule)
    {
        // Compute next line pixels. Boundaries are not computed, always 0
        for (var i = 1; i < imageWidth - 1; i++)
        {
            // Get, from the previous line, the 3 pixels states as a binary value
            var prevValue = ((GetImageColor(image, i - 1, line - 1).R < 5) ? 4 : 0) +     // Left pixel
                            ((GetImageColor(image, i, line - 1).R < 5) ? 2 : 0) +         // Center pixel
                            ((GetImageColor(image, i + 1, line - 1).R < 5) ? 1 : 0);      // Right pixel
            // Get next value from rule bitmask
            var currValue = (rule & (1 << prevValue)) != 0;
            // Update pixel color
            ImageDrawPixel(ref image, i, line, currValue ? Color.Black : Color.RayWhite);
        }
    }

    public void Init()
    {
        // Image that contains the cellular automaton
        image = GenImageColor(imageWidth, imageHeight, Color.RayWhite);
        // The top central pixel set as black
        ImageDrawPixel(ref image, imageWidth / 2, 0, Color.Black);

        texture = LoadTextureFromImage(image);

        // Variables
        rule = 30;  // Starting rule
        line = 1;   // Line to compute, starting from line 1. One point in line 0 is already set
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Handle mouse
        var mouse = GetMousePosition();
        var mouseInCell = -1;   // -1: outside any button; 0-7: rule cells; 8+: preset cells

        // Check mouse on rule cells
        for (var i = 0; i < 8; i++)
        {
            var cellX = drawRuleStartX - drawRuleGroupSpacing * i + drawRuleSpacing;
            var cellY = drawRuleStartY + drawRuleSpacing;
            if ((mouse.X >= cellX) && (mouse.X <= cellX + drawRuleSize) &&
                (mouse.Y >= cellY) && (mouse.Y <= cellY + drawRuleSize))
            {
                mouseInCell = i;    // 0-7: rule cells
                break;
            }
        }

        // Check mouse on preset cells
        if (mouseInCell < 0)
        {
            for (var i = 0; i < presetsCount; i++)
            {
                var cellX = 4 + (presetsSizeX + 2) * (i / 2);
                var cellY = 2 + (presetsSizeY + 2) * (i % 2);
                if ((mouse.X >= cellX) && (mouse.X <= cellX + presetsSizeX) &&
                    (mouse.Y >= cellY) && (mouse.Y <= cellY + presetsSizeY))
                {
                    mouseInCell = i + 8;    // 8+: preset cells
                    break;
                }
            }
        }

        if (IsMouseButtonPressed(MouseButton.Left) && (mouseInCell >= 0))
        {
            // Rule changed both by selecting a preset or toggling a bit
            if (mouseInCell < 8)
            {
                rule ^= (1 << mouseInCell);
            }
            else
            {
                rule = presetValues[mouseInCell - 8];
            }

            // Reset image
            ImageClearBackground(ref image, Color.RayWhite);
            ImageDrawPixel(ref image, imageWidth / 2, 0, Color.Black);
            line = 1;
        }

        // Compute next lines
        //----------------------------------------------------------------------------------
        if (line < imageHeight)
        {
            for (var i = 0; (i < linesUpdatedPerFrame) && (line + i < imageHeight); i++)
            {
                ComputeLine(ref image, line + i, rule);
            }
            line += linesUpdatedPerFrame;

            UpdateTexture(texture, image.Data);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw cellular automaton texture
        DrawTexture(texture, 0, screenHeight - imageHeight, Color.White);

        // Draw preset values
        for (var i = 0; i < presetsCount; i++)
        {
            DrawText($"{presetValues[i]}", 8 + (presetsSizeX + 2) * (i / 2), 4 + (presetsSizeY + 2) * (i % 2), 20, Color.Gray);
            DrawRectangleLines(4 + (presetsSizeX + 2) * (i / 2), 2 + (presetsSizeY + 2) * (i % 2), presetsSizeX, presetsSizeY, Color.Blue);

            // If the mouse is on this preset, highlight it
            if (mouseInCell == i + 8)
            {
                DrawRectangleLinesEx(new Rectangle(2 + (presetsSizeX + 2.0f) * (i / 2),
                                                   (presetsSizeY + 2.0f) * (i % 2),
                                                   presetsSizeX + 4.0f, presetsSizeY + 4.0f), 3, Color.Red);
            }
        }

        // Draw rule bits
        for (var i = 0; i < 8; i++)
        {
            // The three input bits
            for (var j = 0; j < 3; j++)
            {
                DrawRectangleLines(drawRuleStartX - drawRuleGroupSpacing * i + drawRuleSpacing * j, drawRuleStartY, drawRuleSize, drawRuleSize, Color.Gray);
                if ((i & (4 >> j)) != 0)
                {
                    DrawRectangle(drawRuleStartX + 2 - drawRuleGroupSpacing * i + drawRuleSpacing * j, drawRuleStartY + 2, drawRuleInnerSize, drawRuleInnerSize, Color.Black);
                }
            }

            // The output bit
            DrawRectangleLines(drawRuleStartX - drawRuleGroupSpacing * i + drawRuleSpacing, drawRuleStartY + drawRuleSpacing, drawRuleSize, drawRuleSize, Color.Blue);
            if ((rule & (1 << i)) != 0)
            {
                DrawRectangle(drawRuleStartX + 2 - drawRuleGroupSpacing * i + drawRuleSpacing, drawRuleStartY + 2 + drawRuleSpacing, drawRuleInnerSize, drawRuleInnerSize, Color.Black);
            }

            // If the mouse is on this rule bit, highlight it
            if (mouseInCell == i)
            {
                DrawRectangleLinesEx(new Rectangle(drawRuleStartX - drawRuleGroupSpacing * i + drawRuleSpacing - 2.0f,
                                                   drawRuleStartY + drawRuleSpacing - 2.0f,
                                                   drawRuleSize + 4.0f, drawRuleSize + 4.0f), 3, Color.Red);
            }
        }

        DrawText($"RULE: {rule}", drawRuleStartX + drawRuleSpacing * 4, drawRuleStartY + 1, 30, Color.Gray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadImage(image);
        UnloadTexture(texture);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - cellular automata");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new CellularAutomata();
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
