/*******************************************************************************************
*
*   raylib [textures] example - clipboard image
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Maicon Santana (@maiconpintoabreu) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2026 Maicon Santana (@maiconpintoabreu)
*
********************************************************************************************/

namespace Examples.Textures;

[ExcludeFromBrowser("GetClipboardImage() is a desktop-only OS clipboard feature")]
public partial class ClipboardImage : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxTextureCollection = 20;

    public string Name => "Textures / Clipboard Image";

    public string Title => "raylib [textures] example - clipboard image";

    private struct TextureCollection
    {
        public Texture2D Texture;
        public Vector2 Position;
    }

    private TextureCollection[] collection;
    private int currentCollectionIndex;

    public void Init()
    {
        collection = new TextureCollection[MaxTextureCollection];
        currentCollectionIndex = 0;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KeyboardKey.R))    // Reset image collection
        {
            // Unload textures to avoid memory leaks
            for (var i = 0; i < MaxTextureCollection; i++)
            {
                UnloadTexture(collection[i].Texture);
            }

            currentCollectionIndex = 0;
        }

        if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.V) &&
            (currentCollectionIndex < MaxTextureCollection))
        {
            var image = GetClipboardImage();

            if (IsImageValid(image))
            {
                collection[currentCollectionIndex].Texture = LoadTextureFromImage(image);
                collection[currentCollectionIndex].Position = GetMousePosition();
                currentCollectionIndex++;
                UnloadImage(image);
            }
            else
            {
                TraceLog(TraceLogLevel.Info, "IMAGE: Could not retrieve image from clipboard");
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (var i = 0; i < currentCollectionIndex; i++)
        {
            if (IsTextureValid(collection[i].Texture))
            {
                DrawTexturePro(collection[i].Texture,
                    new Rectangle(0, 0, collection[i].Texture.Width, collection[i].Texture.Height),
                    new Rectangle(collection[i].Position.X, collection[i].Position.Y, collection[i].Texture.Width, collection[i].Texture.Height),
                    new Vector2(collection[i].Texture.Width * 0.5f, collection[i].Texture.Height * 0.5f),
                    0.0f, Color.White);
            }
        }

        DrawRectangle(0, 0, screenWidth, 40, Color.Black);
        DrawText("Clipboard Image - Ctrl+V to Paste and R to Reset ", 120, 10, 20, Color.LightGray);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        for (var i = 0; i < MaxTextureCollection; i++)
        {
            UnloadTexture(collection[i].Texture);
        }
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - clipboard image");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new ClipboardImage();
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
