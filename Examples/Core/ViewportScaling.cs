/*******************************************************************************************
*
*   raylib [core] example - viewport scaling
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.5
*
*   Example contributed by Agnis Aldiņš (@nezvers) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Agnis Aldiņš (@nezvers)
*
********************************************************************************************/

namespace Examples.Core;

public partial class ViewportScaling : IExample
{
    private const int ResolutionCount = 4;  // For iteration purposes and teaching example

    private enum ViewportType
    {
        // Only upscale, useful for pixel art
        KeepAspectInteger,
        KeepHeightInteger,
        KeepWidthInteger,
        // Can also downscale
        KeepAspect,
        KeepHeight,
        KeepWidth,
        // For itteration purposes and as a teaching example
        ViewportTypeCount,
    }

    // For displaying on GUI
    private static readonly string[] ViewportTypeNames = new string[]
    {
        "KEEP_ASPECT_INTEGER",
        "KEEP_HEIGHT_INTEGER",
        "KEEP_WIDTH_INTEGER",
        "KEEP_ASPECT",
        "KEEP_HEIGHT",
        "KEEP_WIDTH",
    };

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Viewport Scaling";

    public string Title => "raylib [core] example - viewport scaling";

    public ConfigFlags ConfigFlags => ConfigFlags.ResizableWindow;

    // Mutable window size (tracked from GetScreenWidth/GetScreenHeight)
    private int curScreenWidth;
    private int curScreenHeight;

    private Vector2[] resolutionList;
    private int resolutionIndex;
    private int gameWidth;
    private int gameHeight;

    private RenderTexture2D target;
    private Rectangle sourceRect;
    private Rectangle destRect;

    private ViewportType viewportType;

    // Button rectangles
    private Rectangle decreaseResolutionButton;
    private Rectangle increaseResolutionButton;
    private Rectangle decreaseTypeButton;
    private Rectangle increaseTypeButton;

    public void Init()
    {
        curScreenWidth = screenWidth;
        curScreenHeight = screenHeight;

        // Preset resolutions that could be created by subdividing screen resolution
        resolutionList = new Vector2[]
        {
            new Vector2(64, 64),
            new Vector2(256, 240),
            new Vector2(320, 180),
            // 4K doesn't work with integer scaling but included for example purposes with non-integer scaling
            new Vector2(3840, 2160),
        };

        resolutionIndex = 0;
        gameWidth = 64;
        gameHeight = 64;

        target = default;
        sourceRect = default;
        destRect = default;

        viewportType = ViewportType.KeepAspectInteger;
        ResizeRenderSize(viewportType, ref curScreenWidth, ref curScreenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect, ref target);

        // Button rectangles
        decreaseResolutionButton = new Rectangle(200, 30, 10, 10);
        increaseResolutionButton = new Rectangle(215, 30, 10, 10);
        decreaseTypeButton = new Rectangle(200, 45, 10, 10);
        increaseTypeButton = new Rectangle(215, 45, 10, 10);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsWindowResized())
        {
            ResizeRenderSize(viewportType, ref curScreenWidth, ref curScreenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect, ref target);
        }

        Vector2 mousePosition = GetMousePosition();
        bool mousePressed = IsMouseButtonPressed(MouseButton.Left);

        // Check buttons and rescale
        if (CheckCollisionPointRec(mousePosition, decreaseResolutionButton) && mousePressed)
        {
            resolutionIndex = (resolutionIndex + ResolutionCount - 1) % ResolutionCount;
            gameWidth = (int)resolutionList[resolutionIndex].X;
            gameHeight = (int)resolutionList[resolutionIndex].Y;
            ResizeRenderSize(viewportType, ref curScreenWidth, ref curScreenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect, ref target);
        }

        if (CheckCollisionPointRec(mousePosition, increaseResolutionButton) && mousePressed)
        {
            resolutionIndex = (resolutionIndex + 1) % ResolutionCount;
            gameWidth = (int)resolutionList[resolutionIndex].X;
            gameHeight = (int)resolutionList[resolutionIndex].Y;
            ResizeRenderSize(viewportType, ref curScreenWidth, ref curScreenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect, ref target);
        }

        if (CheckCollisionPointRec(mousePosition, decreaseTypeButton) && mousePressed)
        {
            viewportType = (ViewportType)(((int)viewportType + (int)ViewportType.ViewportTypeCount - 1) % (int)ViewportType.ViewportTypeCount);
            ResizeRenderSize(viewportType, ref curScreenWidth, ref curScreenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect, ref target);
        }

        if (CheckCollisionPointRec(mousePosition, increaseTypeButton) && mousePressed)
        {
            viewportType = (ViewportType)(((int)viewportType + 1) % (int)ViewportType.ViewportTypeCount);
            ResizeRenderSize(viewportType, ref curScreenWidth, ref curScreenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect, ref target);
        }

        Vector2 textureMousePosition = Screen2RenderTexturePosition(mousePosition, sourceRect, destRect);
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Draw our scene to the render texture
        BeginTextureMode(target);
        ClearBackground(Color.White);
        DrawCircleV(textureMousePosition, 20.0f, Color.Lime);
        EndTextureMode();

        // Draw render texture to main framebuffer
        BeginDrawing();
        ClearBackground(Color.Black);

        // Draw our render texture with rotation applied
        DrawTexturePro(target.Texture, sourceRect, destRect, new Vector2(0.0f, 0.0f), 0.0f, Color.White);

        // Draw Native resolution (GUI or anything)
        // Draw info box
        Rectangle infoRect = new Rectangle(5, 5, 330, 105);
        DrawRectangleRec(infoRect, Fade(Color.LightGray, 0.7f));
        DrawRectangleLinesEx(infoRect, 1, Color.Blue);

        DrawText($"Window Resolution: {curScreenWidth} x {curScreenHeight}", 15, 15, 10, Color.Black);
        DrawText($"Game Resolution: {gameWidth} x {gameHeight}", 15, 30, 10, Color.Black);

        DrawText($"Type: {ViewportTypeNames[(int)viewportType]}", 15, 45, 10, Color.Black);
        Vector2 scaleRatio = new Vector2(destRect.Width / sourceRect.Width, -destRect.Height / sourceRect.Height);
        if (scaleRatio.X < 0.001f || scaleRatio.Y < 0.001f)
        {
            DrawText("Scale ratio: INVALID", 15, 60, 10, Color.Black);
        }
        else
        {
            DrawText($"Scale ratio: {scaleRatio.X:F2} x {scaleRatio.Y:F2}", 15, 60, 10, Color.Black);
        }

        DrawText($"Source size: {sourceRect.Width:F2} x {-sourceRect.Height:F2}", 15, 75, 10, Color.Black);
        DrawText($"Destination size: {destRect.Width:F2} x {destRect.Height:F2}", 15, 90, 10, Color.Black);

        // Draw buttons
        DrawRectangleRec(decreaseTypeButton, Color.SkyBlue);
        DrawRectangleRec(increaseTypeButton, Color.SkyBlue);
        DrawRectangleRec(decreaseResolutionButton, Color.SkyBlue);
        DrawRectangleRec(increaseResolutionButton, Color.SkyBlue);
        DrawText("<", (int)decreaseTypeButton.X + 3, (int)decreaseTypeButton.Y + 1, 10, Color.Black);
        DrawText(">", (int)increaseTypeButton.X + 3, (int)increaseTypeButton.Y + 1, 10, Color.Black);
        DrawText("<", (int)decreaseResolutionButton.X + 3, (int)decreaseResolutionButton.Y + 1, 10, Color.Black);
        DrawText(">", (int)increaseResolutionButton.X + 3, (int)increaseResolutionButton.Y + 1, 10, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(target);
    }

    //--------------------------------------------------------------------------------------
    // Module Functions Definition
    //--------------------------------------------------------------------------------------
    private static void KeepAspectCenteredInteger(int screenWidth, int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect)
    {
        sourceRect.X = 0.0f;
        sourceRect.Y = (float)gameHeight;
        sourceRect.Width = (float)gameWidth;
        sourceRect.Height = (float)-gameHeight;

        int ratioX = (screenWidth / gameWidth);
        int ratioY = (screenHeight / gameHeight);
        float resizeRatio = (float)((ratioX < ratioY) ? ratioX : ratioY);

        destRect.X = (float)(int)((screenWidth - (gameWidth * resizeRatio)) * 0.5f);
        destRect.Y = (float)(int)((screenHeight - (gameHeight * resizeRatio)) * 0.5f);
        destRect.Width = (float)(int)(gameWidth * resizeRatio);
        destRect.Height = (float)(int)(gameHeight * resizeRatio);
    }

    private static void KeepHeightCenteredInteger(int screenWidth, int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect)
    {
        float resizeRatio = (float)screenHeight / gameHeight;
        sourceRect.X = 0.0f;
        sourceRect.Y = 0.0f;
        sourceRect.Width = (float)(int)(screenWidth / resizeRatio);
        sourceRect.Height = (float)-gameHeight;

        destRect.X = (float)(int)((screenWidth - (sourceRect.Width * resizeRatio)) * 0.5f);
        destRect.Y = (float)(int)((screenHeight - (gameHeight * resizeRatio)) * 0.5f);
        destRect.Width = (float)(int)(sourceRect.Width * resizeRatio);
        destRect.Height = (float)(int)(gameHeight * resizeRatio);
    }

    private static void KeepWidthCenteredInteger(int screenWidth, int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect)
    {
        float resizeRatio = (float)screenWidth / gameWidth;
        sourceRect.X = 0.0f;
        sourceRect.Y = 0.0f;
        sourceRect.Width = (float)gameWidth;
        sourceRect.Height = (float)(int)(screenHeight / resizeRatio);

        destRect.X = (float)(int)((screenWidth - (gameWidth * resizeRatio)) * 0.5f);
        destRect.Y = (float)(int)((screenHeight - (sourceRect.Height * resizeRatio)) * 0.5f);
        destRect.Width = (float)(int)(gameWidth * resizeRatio);
        destRect.Height = (float)(int)(sourceRect.Height * resizeRatio);

        sourceRect.Height *= -1.0f;
    }

    private static void KeepAspectCentered(int screenWidth, int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect)
    {
        sourceRect.X = 0.0f;
        sourceRect.Y = (float)gameHeight;
        sourceRect.Width = (float)gameWidth;
        sourceRect.Height = (float)-gameHeight;

        float ratioX = ((float)screenWidth / (float)gameWidth);
        float ratioY = ((float)screenHeight / (float)gameHeight);
        float resizeRatio = (ratioX < ratioY ? ratioX : ratioY);

        destRect.X = (float)(int)((screenWidth - (gameWidth * resizeRatio)) * 0.5f);
        destRect.Y = (float)(int)((screenHeight - (gameHeight * resizeRatio)) * 0.5f);
        destRect.Width = (float)(int)(gameWidth * resizeRatio);
        destRect.Height = (float)(int)(gameHeight * resizeRatio);
    }

    private static void KeepHeightCentered(int screenWidth, int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect)
    {
        float resizeRatio = ((float)screenHeight / (float)gameHeight);
        sourceRect.X = 0.0f;
        sourceRect.Y = 0.0f;
        sourceRect.Width = (float)(int)((float)screenWidth / resizeRatio);
        sourceRect.Height = (float)-gameHeight;

        destRect.X = (float)(int)((screenWidth - (sourceRect.Width * resizeRatio)) * 0.5f);
        destRect.Y = (float)(int)((screenHeight - (gameHeight * resizeRatio)) * 0.5f);
        destRect.Width = (float)(int)(sourceRect.Width * resizeRatio);
        destRect.Height = (float)(int)(gameHeight * resizeRatio);
    }

    private static void KeepWidthCentered(int screenWidth, int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect)
    {
        float resizeRatio = ((float)screenWidth / (float)gameWidth);
        sourceRect.X = 0.0f;
        sourceRect.Y = 0.0f;
        sourceRect.Width = (float)gameWidth;
        sourceRect.Height = (float)(int)((float)screenHeight / resizeRatio);

        destRect.X = (float)(int)((screenWidth - (gameWidth * resizeRatio)) * 0.5f);
        destRect.Y = (float)(int)((screenHeight - (sourceRect.Height * resizeRatio)) * 0.5f);
        destRect.Width = (float)(int)(gameWidth * resizeRatio);
        destRect.Height = (float)(int)(sourceRect.Height * resizeRatio);

        sourceRect.Height *= -1.0f;
    }

    private static void ResizeRenderSize(ViewportType viewportType, ref int screenWidth, ref int screenHeight, int gameWidth, int gameHeight, ref Rectangle sourceRect, ref Rectangle destRect, ref RenderTexture2D target)
    {
        screenWidth = GetScreenWidth();
        screenHeight = GetScreenHeight();

        switch (viewportType)
        {
            case ViewportType.KeepAspectInteger:
                KeepAspectCenteredInteger(screenWidth, screenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect);
                break;
            case ViewportType.KeepHeightInteger:
                KeepHeightCenteredInteger(screenWidth, screenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect);
                break;
            case ViewportType.KeepWidthInteger:
                KeepWidthCenteredInteger(screenWidth, screenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect);
                break;
            case ViewportType.KeepAspect:
                KeepAspectCentered(screenWidth, screenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect);
                break;
            case ViewportType.KeepHeight:
                KeepHeightCentered(screenWidth, screenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect);
                break;
            case ViewportType.KeepWidth:
                KeepWidthCentered(screenWidth, screenHeight, gameWidth, gameHeight, ref sourceRect, ref destRect);
                break;
            default:
                break;
        }

        UnloadRenderTexture(target);
        target = LoadRenderTexture((int)sourceRect.Width, -(int)sourceRect.Height);
    }

    // Example how to calculate position on RenderTexture
    private static Vector2 Screen2RenderTexturePosition(Vector2 point, Rectangle textureRect, Rectangle scaledRect)
    {
        Vector2 relativePosition = new Vector2(point.X - scaledRect.X, point.Y - scaledRect.Y);
        Vector2 ratio = new Vector2(textureRect.Width / scaledRect.Width, -textureRect.Height / scaledRect.Height);

        return new Vector2(relativePosition.X * ratio.X, relativePosition.Y * ratio.X);
    }

    public static int Main()
    {
        // Initialization
        //---------------------------------------------------------
        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(screenWidth, screenHeight, "raylib [core] example - viewport scaling");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //----------------------------------------------------------

        var game = new ViewportScaling();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //----------------------------------------------------------------------------------
        CloseWindow();        // Close window and OpenGL context
        //----------------------------------------------------------------------------------

        return 0;
    }
}
