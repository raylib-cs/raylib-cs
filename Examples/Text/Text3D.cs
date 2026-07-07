/*******************************************************************************************
*
*   raylib [text] example - 3d drawing
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: Draw a 2D text in 3D space, each letter is drawn in a quad (or 2 quads if backface is set)
*   where the texture coodinates of each quad map to the texture coordinates of the glyphs
*   inside the font texture
*
*   A more efficient approach, i believe, would be to render the text in a render texture and
*   map that texture to a plane and render that, or maybe a shader but my method allows more
*   flexibility...for example to change position of each letter individually to make somethink
*   like a wavy text effect
*
*   Special thanks to:
*        @Nighten for the DrawTextStyle() code https://github.com/NightenDushi/Raylib_DrawTextStyle
*        Chris Camacho (codifies - http://bedroomcoders.co.uk/) for the alpha discard shader
*
*   Example originally created with raylib 3.5, last time updated with raylib 4.0
*
*   Example contributed by Vlad Adrian (@demizdor) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Vlad Adrian (@demizdor)
*
********************************************************************************************/

using System;
using System.Numerics;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Examples.Text;

public partial class Text3D : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

#if BROWSER
    const int GlslVersion = 100;    // WebGL1 needs GLSL ES 100
#else
    private const int GlslVersion = 330;
#endif

    //--------------------------------------------------------------------------------------
    // Global variables
    //--------------------------------------------------------------------------------------
    private const float LetterBoundrySize = 0.25f;
    private const int TextMaxLayers = 32;
    private static readonly Color LetterBoundryColor = Color.Violet;

    private bool showLetterBoundry;
    private bool showTextBoundry;

    //--------------------------------------------------------------------------------------
    // Types and Structures Definition
    //--------------------------------------------------------------------------------------
    // Configuration structure for waving the text
    private struct WaveTextConfig
    {
        public Vector3 WaveRange;
        public Vector3 WaveSpeed;
        public Vector3 WaveOffset;
    }

    public string Name => "Text / Text 3D";

    public string Title => "raylib [text] example - 3d drawing";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint;

    public bool CursorDisabled => true;

    private bool spin;        // Spin the camera?
    private bool multicolor;  // Multicolor mode

    private Camera3D camera;
    private CameraMode cameraMode;

    private Vector3 cubePosition;
    private Vector3 cubeSize;

    private Font font;
    private float fontSize;
    private float fontSpacing;
    private float lineSpacing;

    private string text;
    private Vector3 tbox;
    private int layers;
    private int quads;
    private float layerDistance;

    private WaveTextConfig wcfg;
    private float time;

    private Color light;
    private Color dark;

    private Shader alphaDiscard;
    private Color[] multi;

    public void Init()
    {
        spin = true;        // Spin the camera?
        multicolor = false; // Multicolor mode
        showLetterBoundry = false;
        showTextBoundry = false;

        // Define the camera to look into our 3d world
        camera = new();
        camera.Position = new Vector3(-10.0f, 15.0f, -10.0f);   // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);          // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);              // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                    // Camera field-of-view Y
        camera.Projection = CameraProjection.Perspective;       // Camera projection type

        cameraMode = CameraMode.Orbital;

        cubePosition = new Vector3(0.0f, 1.0f, 0.0f);
        cubeSize = new Vector3(2.0f, 2.0f, 2.0f);

        // Use the default font
        font = GetFontDefault();
        fontSize = 0.8f;
        fontSpacing = 0.05f;
        lineSpacing = -0.1f;

        // Set the text (using markdown!)
        text = "Hello ~~World~~ in 3D!";
        tbox = new Vector3(0, 0, 0);
        layers = 1;
        quads = 0;
        layerDistance = 0.01f;

        wcfg = new WaveTextConfig();
        wcfg.WaveSpeed.X = wcfg.WaveSpeed.Y = 3.0f; wcfg.WaveSpeed.Z = 0.5f;
        wcfg.WaveOffset.X = wcfg.WaveOffset.Y = wcfg.WaveOffset.Z = 0.35f;
        wcfg.WaveRange.X = wcfg.WaveRange.Y = wcfg.WaveRange.Z = 0.45f;

        time = 0.0f;

        // Setup a light and dark color
        light = Color.Maroon;
        dark = Color.Red;

        // Load the alpha discard shader
        alphaDiscard = LoadShader(null, $"resources/shaders/glsl{GlslVersion}/alpha_discard.fs");

        // Array filled with multiple random colors (when multicolor mode is set)
        multi = new Color[TextMaxLayers];
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        UpdateCamera(ref camera, cameraMode);

        // Handle font files dropped
        if (IsFileDropped())
        {
            string[] droppedFiles = GetDroppedFiles();

            // NOTE: We only support first ttf file dropped
            if (IsFileExtension(droppedFiles[0], ".ttf"))
            {
                UnloadFont(font);
                font = LoadFontEx(droppedFiles[0], (int)fontSize, null, 0);
            }
            else if (IsFileExtension(droppedFiles[0], ".fnt"))
            {
                UnloadFont(font);
                font = LoadFont(droppedFiles[0]);
                fontSize = (float)font.BaseSize;
            }
        }

        // Handle Events
        if (IsKeyPressed(KeyboardKey.F1)) showLetterBoundry = !showLetterBoundry;
        if (IsKeyPressed(KeyboardKey.F2)) showTextBoundry = !showTextBoundry;
        if (IsKeyPressed(KeyboardKey.F3))
        {
            // Handle camera change
            spin = !spin;
            // we need to reset the camera when changing modes
            camera = new();
            camera.Target = new Vector3(0.0f, 0.0f, 0.0f);          // Camera looking at point
            camera.Up = new Vector3(0.0f, 1.0f, 0.0f);              // Camera up vector (rotation towards target)
            camera.FovY = 45.0f;                                    // Camera field-of-view Y
            camera.Projection = CameraProjection.Perspective;       // Camera mode type

            if (spin)
            {
                camera.Position = new Vector3(-10.0f, 15.0f, -10.0f);   // Camera position
                cameraMode = CameraMode.Orbital;
            }
            else
            {
                camera.Position = new Vector3(10.0f, 10.0f, -10.0f);   // Camera position
                cameraMode = CameraMode.Free;
            }
        }

        // Handle clicking the cube
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            Ray ray = GetScreenToWorldRay(GetMousePosition(), camera);

            // Check collision between ray and box
            RayCollision collision = GetRayCollisionBox(ray,
                new BoundingBox(
                    new Vector3(cubePosition.X - cubeSize.X / 2, cubePosition.Y - cubeSize.Y / 2, cubePosition.Z - cubeSize.Z / 2),
                    new Vector3(cubePosition.X + cubeSize.X / 2, cubePosition.Y + cubeSize.Y / 2, cubePosition.Z + cubeSize.Z / 2)));
            if (collision.Hit)
            {
                // Generate new random colors
                light = GenerateRandomColor(0.5f, 0.78f);
                dark = GenerateRandomColor(0.4f, 0.58f);
            }
        }

        // Handle text layers changes
        if (IsKeyPressed(KeyboardKey.Home)) { if (layers > 1) --layers; }
        else if (IsKeyPressed(KeyboardKey.End)) { if (layers < TextMaxLayers) ++layers; }

        // Handle text changes
        if (IsKeyPressed(KeyboardKey.Left)) fontSize -= 0.5f;
        else if (IsKeyPressed(KeyboardKey.Right)) fontSize += 0.5f;
        else if (IsKeyPressed(KeyboardKey.Up)) fontSpacing -= 0.1f;
        else if (IsKeyPressed(KeyboardKey.Down)) fontSpacing += 0.1f;
        else if (IsKeyPressed(KeyboardKey.PageUp)) lineSpacing -= 0.1f;
        else if (IsKeyPressed(KeyboardKey.PageDown)) lineSpacing += 0.1f;
        else if (IsKeyDown(KeyboardKey.Insert)) layerDistance -= 0.001f;
        else if (IsKeyDown(KeyboardKey.Delete)) layerDistance += 0.001f;
        else if (IsKeyPressed(KeyboardKey.Tab))
        {
            multicolor = !multicolor;   // Enable /disable multicolor mode

            if (multicolor)
            {
                // Fill color array with random colors
                for (int i = 0; i < TextMaxLayers; i++)
                {
                    multi[i] = GenerateRandomColor(0.5f, 0.8f);
                    multi[i].A = (byte)GetRandomValue(0, 255);
                }
            }
        }

        // Handle text input
        int ch = GetCharPressed();
        if (IsKeyPressed(KeyboardKey.Backspace))
        {
            // Remove last char
            if (text.Length > 0) text = text.Substring(0, text.Length - 1);
        }
        else if (IsKeyPressed(KeyboardKey.Enter))
        {
            // handle newline
            if (text.Length < 63) text += '\n';
        }
        else
        {
            // append only printable chars
            if ((ch != 0) && (text.Length < 63)) text += char.ConvertFromUtf32(ch);
        }

        // Measure 3D text so we can center it
        tbox = MeasureTextWave3D(font, text, fontSize, fontSpacing, lineSpacing);

        quads = 0;                      // Reset quad counter
        time += GetFrameTime();         // Update timer needed by `DrawTextWave3D()`
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        BeginMode3D(camera);
        DrawCubeV(cubePosition, cubeSize, dark);
        DrawCubeWires(cubePosition, 2.1f, 2.1f, 2.1f, light);

        DrawGrid(10, 2.0f);

        // Use a shader to handle the depth buffer issue with transparent textures
        // NOTE: more info at https://bedroomcoders.co.uk/posts/198
        BeginShaderMode(alphaDiscard);

        // Draw the 3D text above the red cube
        Rlgl.PushMatrix();
        Rlgl.Rotatef(90.0f, 1.0f, 0.0f, 0.0f);
        Rlgl.Rotatef(90.0f, 0.0f, 0.0f, -1.0f);

        for (int i = 0; i < layers; i++)
        {
            Color clr = light;
            if (multicolor) clr = multi[i];
            DrawTextWave3D(font, text, new Vector3(-tbox.X / 2.0f, layerDistance * i, -4.5f), fontSize, fontSpacing, lineSpacing, true, wcfg, time, clr);
        }

        // Draw the text boundry if set
        if (showTextBoundry) DrawCubeWiresV(new Vector3(0.0f, 0.0f, -4.5f + tbox.Z / 2), tbox, dark);
        Rlgl.PopMatrix();

        // Don't draw the letter boundries for the 3D text below
        bool slb = showLetterBoundry;
        showLetterBoundry = false;

        // Draw 3D options (use default font)
        //-------------------------------------------------------------------------
        Rlgl.PushMatrix();
        Rlgl.Rotatef(180.0f, 0.0f, 1.0f, 0.0f);
        string opt = $"< SIZE: {fontSize:0.0} >";
        quads += opt.Length;
        Vector2 m = MeasureTextEx(GetFontDefault(), opt, 0.8f, 0.1f);
        Vector3 pos = new(-m.X / 2.0f, 0.01f, 2.0f);
        DrawText3D(GetFontDefault(), opt, pos, 0.8f, 0.1f, 0.0f, false, Color.Blue);
        pos.Z += 0.5f + m.Y;

        opt = $"< SPACING: {fontSpacing:0.0} >";
        quads += opt.Length;
        m = MeasureTextEx(GetFontDefault(), opt, 0.8f, 0.1f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.8f, 0.1f, 0.0f, false, Color.Blue);
        pos.Z += 0.5f + m.Y;

        opt = $"< LINE: {lineSpacing:0.0} >";
        quads += opt.Length;
        m = MeasureTextEx(GetFontDefault(), opt, 0.8f, 0.1f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.8f, 0.1f, 0.0f, false, Color.Blue);
        pos.Z += 0.5f + m.Y;

        opt = $"< LBOX: {(slb ? "ON" : "OFF"),3} >";
        quads += opt.Length;
        m = MeasureTextEx(GetFontDefault(), opt, 0.8f, 0.1f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.8f, 0.1f, 0.0f, false, Color.Red);
        pos.Z += 0.5f + m.Y;

        opt = $"< TBOX: {(showTextBoundry ? "ON" : "OFF"),3} >";
        quads += opt.Length;
        m = MeasureTextEx(GetFontDefault(), opt, 0.8f, 0.1f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.8f, 0.1f, 0.0f, false, Color.Red);
        pos.Z += 0.5f + m.Y;

        opt = $"< LAYER DISTANCE: {layerDistance:0.000} >";
        quads += opt.Length;
        m = MeasureTextEx(GetFontDefault(), opt, 0.8f, 0.1f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.8f, 0.1f, 0.0f, false, Color.DarkPurple);
        Rlgl.PopMatrix();
        //-------------------------------------------------------------------------

        // Draw 3D info text (use default font)
        //-------------------------------------------------------------------------
        opt = "All the text displayed here is in 3D";
        quads += 36;
        m = MeasureTextEx(GetFontDefault(), opt, 1.0f, 0.05f);
        pos = new Vector3(-m.X / 2.0f, 0.01f, 2.0f);
        DrawText3D(GetFontDefault(), opt, pos, 1.0f, 0.05f, 0.0f, false, Color.DarkBlue);
        pos.Z += 1.5f + m.Y;

        opt = "press [Left]/[Right] to change the font size";
        quads += 44;
        m = MeasureTextEx(GetFontDefault(), opt, 0.6f, 0.05f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.6f, 0.05f, 0.0f, false, Color.DarkBlue);
        pos.Z += 0.5f + m.Y;

        opt = "press [Up]/[Down] to change the font spacing";
        quads += 44;
        m = MeasureTextEx(GetFontDefault(), opt, 0.6f, 0.05f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.6f, 0.05f, 0.0f, false, Color.DarkBlue);
        pos.Z += 0.5f + m.Y;

        opt = "press [PgUp]/[PgDown] to change the line spacing";
        quads += 48;
        m = MeasureTextEx(GetFontDefault(), opt, 0.6f, 0.05f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.6f, 0.05f, 0.0f, false, Color.DarkBlue);
        pos.Z += 0.5f + m.Y;

        opt = "press [F1] to toggle the letter boundry";
        quads += 39;
        m = MeasureTextEx(GetFontDefault(), opt, 0.6f, 0.05f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.6f, 0.05f, 0.0f, false, Color.DarkBlue);
        pos.Z += 0.5f + m.Y;

        opt = "press [F2] to toggle the text boundry";
        quads += 37;
        m = MeasureTextEx(GetFontDefault(), opt, 0.6f, 0.05f);
        pos.X = -m.X / 2.0f;
        DrawText3D(GetFontDefault(), opt, pos, 0.6f, 0.05f, 0.0f, false, Color.DarkBlue);
        //-------------------------------------------------------------------------

        showLetterBoundry = slb;
        EndShaderMode();

        EndMode3D();

        // Draw 2D info text & stats
        //-------------------------------------------------------------------------
        DrawText("Drag & drop a font file to change the font!\nType something, see what happens!\n\n" +
        "Press [F3] to toggle the camera", 10, 35, 10, Color.Black);

        quads += TextLengthUtf8(text) * 2 * layers;
        string tmp = $"{layers,2} layer(s) | {(spin ? "ORBITAL" : "FREE")} camera | {quads,4} quads ({quads * 4,4} verts)";
        int width = MeasureText(tmp, 10);
        DrawText(tmp, screenWidth - 20 - width, 10, 10, Color.DarkGreen);

        tmp = "[Home]/[End] to add/remove 3D text layers";
        width = MeasureText(tmp, 10);
        DrawText(tmp, screenWidth - 20 - width, 25, 10, Color.DarkGray);

        tmp = "[Insert]/[Delete] to increase/decrease distance between layers";
        width = MeasureText(tmp, 10);
        DrawText(tmp, screenWidth - 20 - width, 40, 10, Color.DarkGray);

        tmp = "click the [CUBE] for a random color";
        width = MeasureText(tmp, 10);
        DrawText(tmp, screenWidth - 20 - width, 55, 10, Color.DarkGray);

        tmp = "[Tab] to toggle multicolor mode";
        width = MeasureText(tmp, 10);
        DrawText(tmp, screenWidth - 20 - width, 70, 10, Color.DarkGray);
        //-------------------------------------------------------------------------

        DrawFPS(10, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadFont(font);
    }

    //--------------------------------------------------------------------------------------
    // Module Functions Definitions
    //--------------------------------------------------------------------------------------
    // Get the total length in bytes of a UTF-8 string (raylib TextLength equivalent)
    private static int TextLengthUtf8(string text)
    {
        return Encoding.UTF8.GetByteCount(text);
    }

    // Draw codepoint at specified position in 3D space
    private unsafe void DrawTextCodepoint3D(Font font, int codepoint, Vector3 position, float fontSize, bool backface, Color tint)
    {
        // Character index position in sprite font
        // NOTE: In case a codepoint is not available in the font, index returned points to '?'
        int index = GetGlyphIndex(font, codepoint);
        float scale = fontSize / (float)font.BaseSize;

        // Character destination rectangle on screen
        // NOTE: We consider charsPadding on drawing
        position.X += (font.Glyphs[index].OffsetX - font.GlyphPadding) * scale;
        position.Z += (font.Glyphs[index].OffsetY - font.GlyphPadding) * scale;

        // Character source rectangle from font texture atlas
        // NOTE: We consider chars padding when drawing, it could be required for outline/glow shader effects
        Rectangle srcRec = new(font.Recs[index].X - font.GlyphPadding, font.Recs[index].Y - font.GlyphPadding,
                             font.Recs[index].Width + 2.0f * font.GlyphPadding, font.Recs[index].Height + 2.0f * font.GlyphPadding);

        float width = (font.Recs[index].Width + 2.0f * font.GlyphPadding) * scale;
        float height = (font.Recs[index].Height + 2.0f * font.GlyphPadding) * scale;

        if (font.Texture.Id > 0)
        {
            const float x = 0.0f;
            const float y = 0.0f;
            const float z = 0.0f;

            // normalized texture coordinates of the glyph inside the font texture (0.0f -> 1.0f)
            float tx = srcRec.X / font.Texture.Width;
            float ty = srcRec.Y / font.Texture.Height;
            float tw = (srcRec.X + srcRec.Width) / font.Texture.Width;
            float th = (srcRec.Y + srcRec.Height) / font.Texture.Height;

            if (showLetterBoundry) DrawCubeWiresV(new Vector3(position.X + width / 2, position.Y, position.Z + height / 2), new Vector3(width, LetterBoundrySize, height), LetterBoundryColor);

            Rlgl.CheckRenderBatchLimit(4 + 4 * (backface ? 1 : 0));
            Rlgl.SetTexture(font.Texture.Id);

            Rlgl.PushMatrix();
            Rlgl.Translatef(position.X, position.Y, position.Z);

            Rlgl.Begin(DrawMode.Quads);
            Rlgl.Color4ub(tint.R, tint.G, tint.B, tint.A);

            // Front Face
            Rlgl.Normal3f(0.0f, 1.0f, 0.0f);                                   // Normal Pointing Up
            Rlgl.TexCoord2f(tx, ty); Rlgl.Vertex3f(x, y, z);                   // Top Left Of The Texture and Quad
            Rlgl.TexCoord2f(tx, th); Rlgl.Vertex3f(x, y, z + height);          // Bottom Left Of The Texture and Quad
            Rlgl.TexCoord2f(tw, th); Rlgl.Vertex3f(x + width, y, z + height);  // Bottom Right Of The Texture and Quad
            Rlgl.TexCoord2f(tw, ty); Rlgl.Vertex3f(x + width, y, z);           // Top Right Of The Texture and Quad

            if (backface)
            {
                // Back Face
                Rlgl.Normal3f(0.0f, -1.0f, 0.0f);                              // Normal Pointing Down
                Rlgl.TexCoord2f(tx, ty); Rlgl.Vertex3f(x, y, z);              // Top Right Of The Texture and Quad
                Rlgl.TexCoord2f(tw, ty); Rlgl.Vertex3f(x + width, y, z);      // Top Left Of The Texture and Quad
                Rlgl.TexCoord2f(tw, th); Rlgl.Vertex3f(x + width, y, z + height); // Bottom Left Of The Texture and Quad
                Rlgl.TexCoord2f(tx, th); Rlgl.Vertex3f(x, y, z + height);     // Bottom Right Of The Texture and Quad
            }
            Rlgl.End();
            Rlgl.PopMatrix();

            Rlgl.SetTexture(0);
        }
    }

    // Draw a 2D text in 3D space
    private unsafe void DrawText3D(Font font, string text, Vector3 position, float fontSize, float fontSpacing, float lineSpacing, bool backface, Color tint)
    {
        using var textNative = new Utf8Buffer(text);
        sbyte* t = textNative.AsPointer();
        int length = Encoding.UTF8.GetByteCount(text);  // Total length in bytes of the text, scanned by codepoints in loop

        float textOffsetY = 0.0f;               // Offset between lines (on line break '\n')
        float textOffsetX = 0.0f;               // Offset X to next character to draw

        float scale = fontSize / (float)font.BaseSize;

        for (int i = 0; i < length;)
        {
            // Get next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = GetCodepoint(&t[i], &codepointByteCount);
            int index = GetGlyphIndex(font, codepoint);

            // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol moving one byte
            if (codepoint == 0x3f) codepointByteCount = 1;

            if (codepoint == '\n')
            {
                // NOTE: Fixed line spacing of 1.5 line-height
                // TODO: Support custom line spacing defined by user
                textOffsetY += fontSize + lineSpacing;
                textOffsetX = 0.0f;
            }
            else
            {
                if ((codepoint != ' ') && (codepoint != '\t'))
                {
                    DrawTextCodepoint3D(font, codepoint, new Vector3(position.X + textOffsetX, position.Y, position.Z + textOffsetY), fontSize, backface, tint);
                }

                if (font.Glyphs[index].AdvanceX == 0) textOffsetX += font.Recs[index].Width * scale + fontSpacing;
                else textOffsetX += font.Glyphs[index].AdvanceX * scale + fontSpacing;
            }

            i += codepointByteCount;   // Move text bytes counter to next codepoint
        }
    }

    // Draw a 2D text in 3D space and wave the parts that start with `~~` and end with `~~`
    // This is a modified version of the original code by @Nighten found here https://github.com/NightenDushi/Raylib_DrawTextStyle
    private unsafe void DrawTextWave3D(Font font, string text, Vector3 position, float fontSize, float fontSpacing, float lineSpacing, bool backface, WaveTextConfig config, float time, Color tint)
    {
        using var textNative = new Utf8Buffer(text);
        sbyte* t = textNative.AsPointer();
        int length = Encoding.UTF8.GetByteCount(text);  // Total length in bytes of the text, scanned by codepoints in loop

        float textOffsetY = 0.0f;               // Offset between lines (on line break '\n')
        float textOffsetX = 0.0f;               // Offset X to next character to draw

        float scale = fontSize / (float)font.BaseSize;

        bool wave = false;

        for (int i = 0, k = 0; i < length; ++k)
        {
            // Get next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = GetCodepoint(&t[i], &codepointByteCount);
            int index = GetGlyphIndex(font, codepoint);

            // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol moving one byte
            if (codepoint == 0x3f) codepointByteCount = 1;

            if (codepoint == '\n')
            {
                // NOTE: Fixed line spacing of 1.5 line-height
                // TODO: Support custom line spacing defined by user
                textOffsetY += fontSize + lineSpacing;
                textOffsetX = 0.0f;
                k = 0;
            }
            else if (codepoint == '~')
            {
                if (GetCodepoint(&t[i + 1], &codepointByteCount) == '~')
                {
                    codepointByteCount += 1;
                    wave = !wave;
                }
            }
            else
            {
                if ((codepoint != ' ') && (codepoint != '\t'))
                {
                    Vector3 pos = position;
                    if (wave) // Apply the wave effect
                    {
                        pos.X += MathF.Sin(time * config.WaveSpeed.X - k * config.WaveOffset.X) * config.WaveRange.X;
                        pos.Y += MathF.Sin(time * config.WaveSpeed.Y - k * config.WaveOffset.Y) * config.WaveRange.Y;
                        pos.Z += MathF.Sin(time * config.WaveSpeed.Z - k * config.WaveOffset.Z) * config.WaveRange.Z;
                    }

                    DrawTextCodepoint3D(font, codepoint, new Vector3(pos.X + textOffsetX, pos.Y, pos.Z + textOffsetY), fontSize, backface, tint);
                }

                if (font.Glyphs[index].AdvanceX == 0) textOffsetX += font.Recs[index].Width * scale + fontSpacing;
                else textOffsetX += font.Glyphs[index].AdvanceX * scale + fontSpacing;
            }

            i += codepointByteCount;   // Move text bytes counter to next codepoint
        }
    }

    // Measure a text in 3D ignoring the `~~` chars
    private unsafe Vector3 MeasureTextWave3D(Font font, string text, float fontSize, float fontSpacing, float lineSpacing)
    {
        using var textNative = new Utf8Buffer(text);
        sbyte* t = textNative.AsPointer();
        int len = Encoding.UTF8.GetByteCount(text);
        int tempLen = 0;                // Used to count longer text line num chars
        int lenCounter = 0;

        float tempTextWidth = 0.0f;     // Used to count longer text line width

        float scale = fontSize / (float)font.BaseSize;
        float textHeight = scale;
        float textWidth = 0.0f;

        int letter = 0;                 // Current character
        int index = 0;                  // Index position in sprite font

        for (int i = 0; i < len; i++)
        {
            int next = 0;
            letter = GetCodepoint(&t[i], &next);
            index = GetGlyphIndex(font, letter);

            // NOTE: normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol so to not skip any we set next = 1
            if (letter == 0x3f) next = 1;
            i += next - 1;

            if (letter != '\n')
            {
                if (letter == '~' && GetCodepoint(&t[i + 1], &next) == '~')
                {
                    i++;
                }
                else
                {
                    lenCounter++;
                    if (font.Glyphs[index].AdvanceX != 0) textWidth += font.Glyphs[index].AdvanceX * scale;
                    else textWidth += (font.Recs[index].Width + font.Glyphs[index].OffsetX) * scale;
                }
            }
            else
            {
                if (tempTextWidth < textWidth) tempTextWidth = textWidth;
                lenCounter = 0;
                textWidth = 0.0f;
                textHeight += fontSize + lineSpacing;
            }

            if (tempLen < lenCounter) tempLen = lenCounter;
        }

        if (tempTextWidth < textWidth) tempTextWidth = textWidth;

        Vector3 vec = new(0, 0, 0);
        vec.X = tempTextWidth + ((tempLen - 1) * fontSpacing); // Adds chars spacing to measure
        vec.Y = 0.25f;
        vec.Z = textHeight;

        return vec;
    }

    // Generates a nice color with a random hue
    private static Color GenerateRandomColor(float s, float v)
    {
        const float Phi = 0.618033988749895f; // Golden ratio conjugate
        float h = (float)GetRandomValue(0, 360);
        h = (h + h * Phi) % 360.0f;
        return ColorFromHSV(h, s, v);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint);
        InitWindow(screenWidth, screenHeight, "raylib [text] example - 3d drawing");

        DisableCursor();                    // Limit cursor to relative movement inside the window

        SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new Text3D();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())        // Detect window close button or ESC key
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
