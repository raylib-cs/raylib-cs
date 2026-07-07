/*******************************************************************************************
*
*   raylib [shaders] example - rlgl compute
*
*   WARNING: This example requires raylib compiled with OpenGL 4.3 version for
*         compute shaders support, shaders used in this example are #version 430
*
*   Example complexity rating: [★★★★] 4/4
*
*   Example originally created with raylib 4.0, last time updated with raylib 4.0
*
*   Example contributed by Teddy Astie (@tsnake41) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2021-2025 Teddy Astie (@tsnake41)
*
********************************************************************************************/

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static Raylib_cs.Raylib;

namespace Examples.Shaders;

[ExcludeFromBrowser("compute shaders are not available on WebGL")]
public partial class RlglCompute : IExample
{
    // IMPORTANT: This must match gol*.glsl GOL_WIDTH constant
    // This must be a multiple of 16 (check golLogic compute dispatch)
    private const int GolWidth = 768;

    // Maximum amount of queued draw commands (squares draw from mouse down events)
    private const int MaxBufferedTransferts = 48;

    private const int screenWidth = GolWidth;
    private const int screenHeight = GolWidth;

    public string Name => "Shaders / Rlgl Compute";

    public string Title => "raylib [shaders] example - rlgl compute";

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    // Game Of Life Update Command
    [StructLayout(LayoutKind.Sequential)]
    private struct GolUpdateCmd
    {
        public uint X;         // x coordinate of the gol command
        public uint Y;         // y coordinate of the gol command
        public uint W;         // width of the filled zone
        public uint Enabled;   // whether to enable or disable zone
    }

    // Inline fixed-size array of GolUpdateCmd (MAX_BUFFERED_TRANSFERTS entries)
    [InlineArray(MaxBufferedTransferts)]
    private struct GolUpdateCmdBuffer
    {
        private GolUpdateCmd _element0;
    }

    // Game Of Life Update Commands SSBO
    [StructLayout(LayoutKind.Sequential)]
    private struct GolUpdateSSBO
    {
        public uint Count;
        public GolUpdateCmdBuffer Commands;
    }

    private Vector2 resolution;
    private uint brushSize;
    private uint golLogicShader;
    private uint golLogicProgram;
    private uint golTransfertShader;
    private uint golTransfertProgram;
    private Shader golRenderShader;
    private int resUniformLoc;
    private uint ssboA;
    private uint ssboB;
    private uint ssboTransfert;
    private GolUpdateSSBO transfertBuffer;
    private Texture2D whiteTex;

    public unsafe void Init()
    {
        resolution = new Vector2(screenWidth, screenHeight);
        brushSize = 8;

        // Game of Life logic compute shader
        var golLogicCode = LoadFileText("resources/shaders/glsl430/gol.glsl");
        var golLogicBytes = Encoding.UTF8.GetBytes(golLogicCode + "\0");
        fixed (byte* p = golLogicBytes)
        {
            golLogicShader = Rlgl.LoadShader((sbyte*)p, (int)ShaderType.Compute);
        }
        golLogicProgram = Rlgl.LoadShaderProgramCompute(golLogicShader);

        // Game of Life logic render shader
        golRenderShader = LoadShader(null, "resources/shaders/glsl430/gol_render.glsl");
        resUniformLoc = GetShaderLocation(golRenderShader, "resolution");

        // Game of Life transfert shader (CPU<->GPU download and upload)
        var golTransfertCode = LoadFileText("resources/shaders/glsl430/gol_transfert.glsl");
        var golTransfertBytes = Encoding.UTF8.GetBytes(golTransfertCode + "\0");
        fixed (byte* p = golTransfertBytes)
        {
            golTransfertShader = Rlgl.LoadShader((sbyte*)p, (int)ShaderType.Compute);
        }
        golTransfertProgram = Rlgl.LoadShaderProgramCompute(golTransfertShader);

        // Load shader storage buffer object (SSBO), id returned
        ssboA = Rlgl.LoadShaderBuffer((uint)(GolWidth * GolWidth * sizeof(uint)), null, Rlgl.DYNAMIC_COPY);
        ssboB = Rlgl.LoadShaderBuffer((uint)(GolWidth * GolWidth * sizeof(uint)), null, Rlgl.DYNAMIC_COPY);
        ssboTransfert = Rlgl.LoadShaderBuffer((uint)sizeof(GolUpdateSSBO), null, Rlgl.DYNAMIC_COPY);

        transfertBuffer = new();

        // Create a white texture of the size of the window to update
        // each pixel of the window using the fragment shader: golRenderShader
        var whiteImage = GenImageColor(GolWidth, GolWidth, Color.White);
        whiteTex = LoadTextureFromImage(whiteImage);
        UnloadImage(whiteImage);
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        brushSize += (uint)(int)GetMouseWheelMove();

        if ((IsMouseButtonDown(MouseButton.Left) || IsMouseButtonDown(MouseButton.Right))
            && (transfertBuffer.Count < MaxBufferedTransferts))
        {
            // Buffer a new command
            transfertBuffer.Commands[(int)transfertBuffer.Count].X = (uint)GetMouseX() - brushSize / 2;
            transfertBuffer.Commands[(int)transfertBuffer.Count].Y = (uint)GetMouseY() - brushSize / 2;
            transfertBuffer.Commands[(int)transfertBuffer.Count].W = brushSize;
            transfertBuffer.Commands[(int)transfertBuffer.Count].Enabled = IsMouseButtonDown(MouseButton.Left) ? 1u : 0u;
            transfertBuffer.Count++;
        }
        else if (transfertBuffer.Count > 0)  // Process transfert buffer
        {
            // Send SSBO buffer to GPU
            fixed (GolUpdateSSBO* ptr = &transfertBuffer)
            {
                Rlgl.UpdateShaderBuffer(ssboTransfert, ptr, (uint)sizeof(GolUpdateSSBO), 0);
            }

            // Process SSBO commands on GPU
            Rlgl.EnableShader(golTransfertProgram);
            Rlgl.BindShaderBuffer(ssboA, 1);
            Rlgl.BindShaderBuffer(ssboTransfert, 3);
            Rlgl.ComputeShaderDispatch(transfertBuffer.Count, 1, 1); // Each GPU unit will process a command!
            Rlgl.DisableShader();

            transfertBuffer.Count = 0;
        }
        else
        {
            // Process game of life logic
            Rlgl.EnableShader(golLogicProgram);
            Rlgl.BindShaderBuffer(ssboA, 1);
            Rlgl.BindShaderBuffer(ssboB, 2);
            Rlgl.ComputeShaderDispatch(GolWidth / 16, GolWidth / 16, 1);
            Rlgl.DisableShader();

            // ssboA <-> ssboB
            var temp = ssboA;
            ssboA = ssboB;
            ssboB = temp;
        }

        Rlgl.BindShaderBuffer(ssboA, 1);
        Raylib.SetShaderValue(golRenderShader, resUniformLoc, resolution, ShaderUniformDataType.Vec2);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.Blank);

        BeginShaderMode(golRenderShader);
        DrawTexture(whiteTex, 0, 0, Color.White);
        EndShaderMode();

        DrawRectangleLines(GetMouseX() - (int)(brushSize / 2), GetMouseY() - (int)(brushSize / 2), (int)brushSize, (int)brushSize, Color.Red);

        DrawText("Use Mouse wheel to increase/decrease brush size", 10, 10, 20, Color.White);
        DrawFPS(GetScreenWidth() - 100, 10);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // Unload shader buffers objects
        Rlgl.UnloadShaderBuffer(ssboA);
        Rlgl.UnloadShaderBuffer(ssboB);
        Rlgl.UnloadShaderBuffer(ssboTransfert);

        // Unload compute shader
        Rlgl.UnloadShader(golLogicShader);
        Rlgl.UnloadShader(golTransfertShader);
        Rlgl.UnloadShaderProgram(golTransfertProgram);
        Rlgl.UnloadShaderProgram(golLogicProgram);

        UnloadTexture(whiteTex);            // Unload white texture
        UnloadShader(golRenderShader);      // Unload rendering fragment shader
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shaders] example - rlgl compute");
        //--------------------------------------------------------------------------------------

        var game = new RlglCompute();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
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
