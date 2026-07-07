/*******************************************************************************************
*
*   raylib [core] example - screen recording
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

// NOTE: The upstream C example records frames into an animated GIF using the bundled msf_gif.h
// single-header library. raylib-cs does not bind msf_gif, so this port replaces it with a small,
// self-contained GIF89a encoder (GifRecorder, below) that uses a fixed 3-3-2 RGB palette. The
// rest of the example (rendering, CTRL+R toggle, saving to <appdir>/screenrecording.gif) mirrors
// upstream. Frame capture via LoadImageFromScreen() is slow and can cause stuttering, as noted
// upstream.
[ExcludeFromBrowser("desktop screen capture + gif file export, no web equivalent")]
public partial class ScreenRecording : IExample
{
    private const int GIF_RECORD_FRAMERATE = 5;     // Record framerate, we get a frame every N frames

    private const int MAX_SINEWAVE_POINTS = 256;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Screen Recording";

    public string Title => "raylib [core] example - screen recording";

    private bool gifRecording;              // GIF recording state
    private uint gifFrameCounter;           // GIF frames counter
    private GifRecorder gifState;           // GIF context state

    private Vector2 circlePosition;
    private float timeCounter;

    private Vector2[] sinePoints;

    public void Init()
    {
        gifRecording = false;
        gifFrameCounter = 0;
        gifState = new GifRecorder();

        circlePosition = new Vector2(0.0f, screenHeight / 2.0f);
        timeCounter = 0.0f;

        // Get sine wave points for line drawing
        sinePoints = new Vector2[MAX_SINEWAVE_POINTS];
        for (int i = 0; i < MAX_SINEWAVE_POINTS; i++)
        {
            sinePoints[i].X = i * GetScreenWidth() / 180.0f;
            sinePoints[i].Y = screenHeight / 2.0f + 150 * MathF.Sin((2 * MathF.PI / 1.5f) * (1.0f / 60.0f) * (float)i); // Calculate for 60 fps
        }
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Update circle sinusoidal movement
        timeCounter += GetFrameTime();
        circlePosition.X += GetScreenWidth() / 180.0f;
        circlePosition.Y = screenHeight / 2.0f + 150 * MathF.Sin((2 * MathF.PI / 1.5f) * timeCounter);
        if (circlePosition.X > screenWidth)
        {
            circlePosition.X = 0.0f;
            circlePosition.Y = screenHeight / 2.0f;
            timeCounter = 0.0f;
        }

        // Start-Stop GIF recording on CTRL+R
        if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.R))
        {
            if (gifRecording)
            {
                // Stop current recording and save file
                gifRecording = false;
                byte[] result = gifState.End();
                SaveFileData(result, $"{GetApplicationDirectoryString()}/screenrecording.gif");

                TraceLog(TraceLogLevel.Info, "Finish animated GIF recording");
            }
            else
            {
                // Start a new recording
                gifRecording = true;
                gifFrameCounter = 0;
                gifState.Begin(GetRenderWidth(), GetRenderHeight());

                TraceLog(TraceLogLevel.Info, "Start animated GIF recording");
            }
        }

        if (gifRecording)
        {
            gifFrameCounter++;

            // NOTE: We record one gif frame depending on the desired gif framerate
            if (gifFrameCounter > GIF_RECORD_FRAMERATE)
            {
                // Get image data for the current frame (from backbuffer)
                // WARNING: This process is quite slow, it can generate stuttering
                Image imScreen = LoadImageFromScreen();

                // Add the frame to the gif recording, providing and "estimated" time for display in centiseconds
                int delayCs = (int)((1.0f / 60.0f) * GIF_RECORD_FRAMERATE) / 10;
                gifState.AddFrame((byte*)imScreen.Data, imScreen.Width, imScreen.Height, imScreen.Width * 4, delayCs);
                gifFrameCounter = 0;

                UnloadImage(imScreen);    // Free image data
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (int i = 0; i < (MAX_SINEWAVE_POINTS - 1); i++)
        {
            DrawLineV(sinePoints[i], sinePoints[i + 1], Color.Maroon);
            DrawCircleV(sinePoints[i], 3, Color.Maroon);
        }

        DrawCircleV(circlePosition, 30, Color.Red);

        DrawFPS(10, 10);

        /*
        // Draw record indicator
        // WARNING: If drawn here, it will appear in the recorded image,
        // use a render texture instead for the recording and LoadImageFromTexture(rt.texture)
        if (gifRecording)
        {
            // Display the recording indicator every half-second
            if ((int)(GetTime()/0.5)%2 == 1)
            {
                DrawCircle(30, GetScreenHeight() - 20, 10, Color.Maroon);
                DrawText("GIF RECORDING", 50, GetScreenHeight() - 25, 10, Color.Red);
            }
        }
        */
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // If still recording a GIF on close window, just finish
        if (gifRecording)
        {
            gifState.End();
            gifRecording = false;
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - screen recording");

        var game = new ScreenRecording();
        game.Init();

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

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

    // Minimal self-contained animated GIF89a encoder (replacement for msf_gif.h)
    // Uses a fixed 3-3-2 RGB global palette and standard GIF-variant LZW compression.
    private class GifRecorder
    {
        private List<byte> output;
        private int width;
        private int height;

        // LZW bit-packing state (per-frame)
        private int bitBuffer;
        private int bitCount;
        private List<byte> subBlock;

        public void Begin(int w, int h)
        {
            width = w;
            height = h;
            output = new List<byte>();

            // Header
            output.AddRange(new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a' });

            // Logical Screen Descriptor
            WriteU16(width);
            WriteU16(height);
            output.Add(0xF7);   // Global color table present, 8-bit color res, 256-entry table
            output.Add(0x00);   // Background color index
            output.Add(0x00);   // Pixel aspect ratio

            // Global Color Table: 256 entries, 3-3-2 RGB
            for (int k = 0; k < 256; k++)
            {
                int r3 = (k >> 5) & 0x7;
                int g3 = (k >> 2) & 0x7;
                int b2 = k & 0x3;
                output.Add((byte)((r3 << 5) | (r3 << 2) | (r3 >> 1)));
                output.Add((byte)((g3 << 5) | (g3 << 2) | (g3 >> 1)));
                output.Add((byte)((b2 << 6) | (b2 << 4) | (b2 << 2) | b2));
            }

            // NETSCAPE2.0 application extension (loop forever)
            output.Add(0x21);
            output.Add(0xFF);
            output.Add(0x0B);
            output.AddRange(new byte[] { (byte)'N', (byte)'E', (byte)'T', (byte)'S', (byte)'C', (byte)'A', (byte)'P', (byte)'E', (byte)'2', (byte)'.', (byte)'0' });
            output.Add(0x03);
            output.Add(0x01);
            WriteU16(0);        // Loop count (0 = forever)
            output.Add(0x00);
        }

        public unsafe void AddFrame(byte* data, int w, int h, int stride, int delayCs)
        {
            if (output == null) return;

            // Graphic Control Extension
            output.Add(0x21);
            output.Add(0xF9);
            output.Add(0x04);
            output.Add(0x00);   // No transparency, disposal method 0
            WriteU16(delayCs);
            output.Add(0x00);   // Transparent color index
            output.Add(0x00);   // Block terminator

            // Image Descriptor
            output.Add(0x2C);
            WriteU16(0);        // Left
            WriteU16(0);        // Top
            WriteU16(w);
            WriteU16(h);
            output.Add(0x00);   // No local color table, not interlaced

            // Map pixels to palette indices (3-3-2)
            byte[] indices = new byte[w * h];
            for (int y = 0; y < h; y++)
            {
                int row = y * stride;
                int dst = y * w;
                for (int x = 0; x < w; x++)
                {
                    byte r = data[row + x * 4 + 0];
                    byte g = data[row + x * 4 + 1];
                    byte b = data[row + x * 4 + 2];
                    indices[dst + x] = (byte)((r & 0xE0) | ((g & 0xE0) >> 3) | (b >> 6));
                }
            }

            // LZW image data
            const int minCodeSize = 8;
            output.Add((byte)minCodeSize);

            bitBuffer = 0;
            bitCount = 0;
            subBlock = new List<byte>();

            int clearCode = 1 << minCodeSize;   // 256
            int stopCode = clearCode + 1;       // 257
            int keySize = minCodeSize + 1;      // 9
            int nkeys = clearCode + 2;          // 258
            var dict = new Dictionary<int, int>();

            WriteBits(clearCode, keySize);

            int key = indices[0];
            for (int i = 1; i < indices.Length; i++)
            {
                int p = indices[i];
                int combined = (key << 8) | p;
                if (dict.TryGetValue(combined, out int existing))
                {
                    key = existing;
                }
                else
                {
                    WriteBits(key, keySize);
                    dict[combined] = nkeys;
                    nkeys++;
                    if (nkeys == (1 << keySize))
                    {
                        if (keySize < 12) keySize++;
                    }
                    if (nkeys == 0x1000)
                    {
                        WriteBits(clearCode, keySize);
                        dict.Clear();
                        keySize = minCodeSize + 1;
                        nkeys = clearCode + 2;
                    }
                    key = p;
                }
            }

            WriteBits(key, keySize);
            WriteBits(stopCode, keySize);

            // Flush remaining bits
            if (bitCount > 0)
            {
                subBlock.Add((byte)(bitBuffer & 0xFF));
                bitBuffer = 0;
                bitCount = 0;
            }
            if (subBlock.Count > 0) FlushSubBlock();
            output.Add(0x00);   // Image data block terminator
        }

        public byte[] End()
        {
            if (output == null) return Array.Empty<byte>();
            output.Add(0x3B);   // Trailer
            byte[] result = output.ToArray();
            output = null;
            return result;
        }

        private void WriteBits(int code, int len)
        {
            bitBuffer |= code << bitCount;
            bitCount += len;
            while (bitCount >= 8)
            {
                subBlock.Add((byte)(bitBuffer & 0xFF));
                bitBuffer >>= 8;
                bitCount -= 8;
                if (subBlock.Count == 255) FlushSubBlock();
            }
        }

        private void FlushSubBlock()
        {
            output.Add((byte)subBlock.Count);
            output.AddRange(subBlock);
            subBlock.Clear();
        }

        private void WriteU16(int value)
        {
            output.Add((byte)(value & 0xFF));
            output.Add((byte)((value >> 8) & 0xFF));
        }
    }
}
