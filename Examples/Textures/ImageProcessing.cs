/*******************************************************************************************
*
*   raylib [textures] example - image processing
*
*   Example complexity rating: [★★★☆] 3/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example originally created with raylib 1.4, last time updated with raylib 3.5
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2016-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class ImageProcessing : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public const int NumProcesses = 9;

    public string Name => "Textures / Image Processing";

    public string Title => "raylib [textures] example - image processing";

    private enum ImageProcess
    {
        None = 0,
        ColorGrayScale,
        ColorTint,
        ColorInvert,
        ColorContrast,
        ColorBrightness,
        GaussianBlur,
        FlipVertical,
        FlipHorizontal
    }

    private string[] processText = {
        "NO PROCESSING",
        "COLOR GRAYSCALE",
        "COLOR TINT",
        "COLOR INVERT",
        "COLOR CONTRAST",
        "COLOR BRIGHTNESS",
        "GAUSSIAN BLUR",
        "FLIP VERTICAL",
        "FLIP HORIZONTAL"
    };

    private Image imageOrigin;
    private Image imageCopy;
    private Texture2D texture;
    private ImageProcess currentProcess;
    private bool textureReload;
    private Rectangle[] toggleRecs;
    private int mouseHoverRec;

    public void Init()
    {
        // NOTE: Textures MUST be loaded after Window initialization (OpenGL context is required)

        imageOrigin = LoadImage("resources/parrots.png");   // Loaded in CPU memory (RAM)
        ImageFormat(ref imageOrigin, PixelFormat.UncompressedR8G8B8A8);         // Format image to RGBA 32bit (required for texture update) <-- ISSUE
        texture = LoadTextureFromImage(imageOrigin);    // Image converted to texture, GPU memory (VRAM)

        imageCopy = ImageCopy(imageOrigin);

        currentProcess = ImageProcess.None;
        textureReload = false;

        toggleRecs = new Rectangle[NumProcesses];
        mouseHoverRec = -1;

        for (var i = 0; i < NumProcesses; i++)
        {
            toggleRecs[i] = new Rectangle(40.0f, (float)(50 + 32 * i), 150.0f, 30.0f);
        }
    }

    public unsafe void Update()
    {
        // Update
        //----------------------------------------------------------------------------------

        // Mouse toggle group logic
        for (var i = 0; i < NumProcesses; i++)
        {
            if (CheckCollisionPointRec(GetMousePosition(), toggleRecs[i]))
            {
                mouseHoverRec = i;

                if (IsMouseButtonReleased(MouseButton.Left))
                {
                    currentProcess = (ImageProcess)i;
                    textureReload = true;
                }
                break;
            }
            else
            {
                mouseHoverRec = -1;
            }
        }

        // Keyboard toggle group logic
        if (IsKeyPressed(KeyboardKey.Down))
        {
            currentProcess++;
            if ((int)currentProcess > (NumProcesses - 1))
            {
                currentProcess = 0;
            }

            textureReload = true;
        }
        else if (IsKeyPressed(KeyboardKey.Up))
        {
            currentProcess--;
            if (currentProcess < 0)
            {
                currentProcess = ImageProcess.FlipHorizontal;
            }

            textureReload = true;
        }

        // Reload texture when required
        if (textureReload)
        {
            UnloadImage(imageCopy);                // Unload image-copy data
            imageCopy = ImageCopy(imageOrigin);    // Restore image-copy from image-origin

            // NOTE: Image processing is a costly CPU process to be done every frame,
            // If image processing is required in a frame-basis, it should be done
            // with a texture and by shaders
            switch (currentProcess)
            {
                case ImageProcess.ColorGrayScale:
                    ImageColorGrayscale(ref imageCopy);
                    break;
                case ImageProcess.ColorTint:
                    ImageColorTint(ref imageCopy, Color.Green);
                    break;
                case ImageProcess.ColorInvert:
                    ImageColorInvert(ref imageCopy);
                    break;
                case ImageProcess.ColorContrast:
                    ImageColorContrast(ref imageCopy, -40);
                    break;
                case ImageProcess.ColorBrightness:
                    ImageColorBrightness(ref imageCopy, -80);
                    break;
                case ImageProcess.GaussianBlur:
                    ImageBlurGaussian(ref imageCopy, 10);
                    break;
                case ImageProcess.FlipVertical:
                    ImageFlipVertical(ref imageCopy);
                    break;
                case ImageProcess.FlipHorizontal:
                    ImageFlipHorizontal(ref imageCopy);
                    break;
                default:
                    break;
            }

            var pixels = LoadImageColors(imageCopy);    // Load pixel data from image (RGBA 32bit)
            UpdateTexture(texture, pixels);                // Update texture with new image data
            UnloadImageColors(pixels);                     // Unload pixels data from RAM

            textureReload = false;
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        DrawText("IMAGE PROCESSING:", 40, 30, 10, Color.DarkGray);

        // Draw rectangles
        for (var i = 0; i < NumProcesses; i++)
        {
            DrawRectangleRec(toggleRecs[i], ((i == (int)currentProcess) || (i == mouseHoverRec)) ? Color.SkyBlue : Color.LightGray);
            DrawRectangleLines(
                (int)toggleRecs[i].X,
                (int)toggleRecs[i].Y,
                (int)toggleRecs[i].Width,
                (int)toggleRecs[i].Height,
                ((i == (int)currentProcess) || (i == mouseHoverRec)) ? Color.Blue : Color.Gray
            );

            var labelX = (int)(toggleRecs[i].X + toggleRecs[i].Width / 2);
            DrawText(
                processText[i],
                (int)(labelX - MeasureText(processText[i], 10) / 2),
                (int)toggleRecs[i].Y + 11,
                10,
                ((i == (int)currentProcess) || (i == mouseHoverRec)) ? Color.DarkBlue : Color.DarkGray
            );
        }

        var x = screenWidth - texture.Width - 60;
        var y = screenHeight / 2 - texture.Height / 2;
        DrawTexture(texture, x, y, Color.White);
        DrawRectangleLines(x, y, texture.Width, texture.Height, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);       // Unload texture from VRAM
        UnloadImage(imageOrigin);     // Unload image-origin from RAM
        UnloadImage(imageCopy);       // Unload image-copy from RAM
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image processing");

        SetTargetFPS(60);
        //---------------------------------------------------------------------------------------

        var game = new ImageProcessing();
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
