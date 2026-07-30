/*******************************************************************************************
*
*   raylib [textures] example - image kernel
*
*   Example complexity rating: [★★★★] 4/4
*
*   NOTE: Images are loaded in CPU memory (RAM); textures are loaded in GPU memory (VRAM)
*
*   Example contributed by Karim Salem (@kimo-s) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example originally created with raylib 1.3, last time updated with raylib 1.3
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2015-2025 Karim Salem (@kimo-s)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class ImageKernel : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Textures / Image Kernel";

    public string Title => "raylib [textures] example - image kernel";

    private Texture2D texture;
    private Texture2D catSharpendTexture;
    private Texture2D catSobelTexture;
    private Texture2D catGaussianTexture;

    private static void NormalizeKernel(float[] kernel, int size)
    {
        var sum = 0.0f;
        for (var i = 0; i < size; i++)
        {
            sum += kernel[i];
        }

        if (sum != 0.0f)
        {
            for (var i = 0; i < size; i++)
            {
                kernel[i] /= sum;
            }
        }
    }

    public void Init()
    {
        var image = LoadImage("resources/cat.png"); // Loaded in CPU memory (RAM)

        float[] gaussiankernel = {
            1.0f, 2.0f, 1.0f,
            2.0f, 4.0f, 2.0f,
            1.0f, 2.0f, 1.0f
        };

        float[] sobelkernel = {
            1.0f, 0.0f, -1.0f,
            2.0f, 0.0f, -2.0f,
            1.0f, 0.0f, -1.0f
        };

        float[] sharpenkernel = {
            0.0f, -1.0f, 0.0f,
           -1.0f, 5.0f, -1.0f,
            0.0f, -1.0f, 0.0f
        };

        NormalizeKernel(gaussiankernel, 9);
        NormalizeKernel(sharpenkernel, 9);
        NormalizeKernel(sobelkernel, 9);

        var catSharpend = ImageCopy(image);
        ImageKernelConvolution(ref catSharpend, sharpenkernel);

        var catSobel = ImageCopy(image);
        ImageKernelConvolution(ref catSobel, sobelkernel);

        var catGaussian = ImageCopy(image);

        for (var i = 0; i < 6; i++)
        {
            ImageKernelConvolution(ref catGaussian, gaussiankernel);
        }

        ImageCrop(ref image, new Rectangle(0, 0, 200, 450));
        ImageCrop(ref catGaussian, new Rectangle(0, 0, 200, 450));
        ImageCrop(ref catSobel, new Rectangle(0, 0, 200, 450));
        ImageCrop(ref catSharpend, new Rectangle(0, 0, 200, 450));

        // Images converted to texture, GPU memory (VRAM)
        texture = LoadTextureFromImage(image);
        catSharpendTexture = LoadTextureFromImage(catSharpend);
        catSobelTexture = LoadTextureFromImage(catSobel);
        catGaussianTexture = LoadTextureFromImage(catGaussian);

        // Once images have been converted to texture and uploaded to VRAM,
        // they can be unloaded from RAM
        UnloadImage(image);
        UnloadImage(catGaussian);
        UnloadImage(catSobel);
        UnloadImage(catSharpend);
    }

    public void Update()
    {
        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        DrawTexture(catSharpendTexture, 0, 0, Color.White);
        DrawTexture(catSobelTexture, 200, 0, Color.White);
        DrawTexture(catGaussianTexture, 400, 0, Color.White);
        DrawTexture(texture, 600, 0, Color.White);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadTexture(texture);
        UnloadTexture(catGaussianTexture);
        UnloadTexture(catSobelTexture);
        UnloadTexture(catSharpendTexture);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - image kernel");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ImageKernel();
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
