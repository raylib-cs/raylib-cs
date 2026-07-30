/*******************************************************************************************
*
*   raylib [textures] example - fog of war
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 4.2, last time updated with raylib 4.2
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2018-2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

namespace Examples.Textures;

public partial class FogOfWar : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MapTileSize = 32;         // Tiles size 32x32 pixels
    private const int PlayerSize = 16;          // Player size
    private const int PlayerTileVisibility = 2; // Player can see 2 tiles around its position

    public string Name => "Textures / Fog of War";

    public string Title => "raylib [textures] example - fog of war";

    // Map data type
    private struct Map
    {
        public uint TilesX;         // Number of tiles in X axis
        public uint TilesY;         // Number of tiles in Y axis
        public byte[] TileIds;      // Tile ids (tilesX*tilesY), defines type of tile to draw
        public byte[] TileFog;      // Tile fog state (tilesX*tilesY), defines if a tile has fog or half-fog
    }

    private Map map;
    private Vector2 playerPosition;
    private int playerTileX;
    private int playerTileY;
    private RenderTexture2D fogOfWar;

    public void Init()
    {
        map = new Map();
        map.TilesX = 25;
        map.TilesY = 15;

        // NOTE: We can have up to 256 values for tile ids and for tile fog state,
        // probably we don't need that many values for fog state, it can be optimized
        // to use only 2 bits per fog state (reducing size by 4) but logic will be a bit more complex
        map.TileIds = new byte[map.TilesX * map.TilesY];
        map.TileFog = new byte[map.TilesX * map.TilesY];

        // Load map tiles (generating 2 random tile ids for testing)
        // NOTE: Map tile ids should be probably loaded from an external map file
        for (uint i = 0; i < map.TilesY * map.TilesX; i++)
        {
            map.TileIds[i] = (byte)GetRandomValue(0, 1);
        }

        // Player position on the screen (pixel coordinates, not tile coordinates)
        playerPosition = new Vector2(180, 130);
        playerTileX = 0;
        playerTileY = 0;

        // Render texture to render fog of war
        // NOTE: To get an automatic smooth-fog effect we use a render texture to render fog
        // at a smaller size (one pixel per tile) and scale it on drawing with bilinear filtering
        fogOfWar = LoadRenderTexture((int)map.TilesX, (int)map.TilesY);
        SetTextureFilter(fogOfWar.Texture, TextureFilter.Bilinear);
        SetTextureWrap(fogOfWar.Texture, TextureWrap.Clamp);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Move player around
        if (IsKeyDown(KeyboardKey.Right))
        {
            playerPosition.X += 5;
        }
        if (IsKeyDown(KeyboardKey.Left))
        {
            playerPosition.X -= 5;
        }
        if (IsKeyDown(KeyboardKey.Down))
        {
            playerPosition.Y += 5;
        }
        if (IsKeyDown(KeyboardKey.Up))
        {
            playerPosition.Y -= 5;
        }

        // Check player position to avoid moving outside tilemap limits
        if (playerPosition.X < 0)
        {
            playerPosition.X = 0;
        }
        else if ((playerPosition.X + PlayerSize) > (map.TilesX * MapTileSize))
        {
            playerPosition.X = (float)map.TilesX * MapTileSize - PlayerSize;
        }
        if (playerPosition.Y < 0)
        {
            playerPosition.Y = 0;
        }
        else if ((playerPosition.Y + PlayerSize) > (map.TilesY * MapTileSize))
        {
            playerPosition.Y = (float)map.TilesY * MapTileSize - PlayerSize;
        }

        // Previous visited tiles are set to partial fog
        for (uint i = 0; i < map.TilesX * map.TilesY; i++)
        {
            if (map.TileFog[i] == 1)
            {
                map.TileFog[i] = 2;
            }
        }

        // Get current tile position from player pixel position
        playerTileX = (int)((playerPosition.X + (float)MapTileSize / 2) / MapTileSize);
        playerTileY = (int)((playerPosition.Y + (float)MapTileSize / 2) / MapTileSize);

        // Check visibility and update fog
        // NOTE: We check tilemap limits to avoid processing tiles out-of-array-bounds (it could crash program)
        for (var y = (playerTileY - PlayerTileVisibility); y < (playerTileY + PlayerTileVisibility); y++)
        {
            for (var x = (playerTileX - PlayerTileVisibility); x < (playerTileX + PlayerTileVisibility); x++)
            {
                if ((x >= 0) && (x < (int)map.TilesX) && (y >= 0) && (y < (int)map.TilesY))
                {
                    map.TileFog[y * map.TilesX + x] = 1;
                }
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        // Draw fog of war to a small render texture for automatic smoothing on scaling
        BeginTextureMode(fogOfWar);
        ClearBackground(Color.Blank);
        for (uint y = 0; y < map.TilesY; y++)
        {
            for (uint x = 0; x < map.TilesX; x++)
            {
                if (map.TileFog[y * map.TilesX + x] == 0)
                {
                    DrawRectangle((int)x, (int)y, 1, 1, Color.Black);
                }
                else if (map.TileFog[y * map.TilesX + x] == 2)
                {
                    DrawRectangle((int)x, (int)y, 1, 1, Fade(Color.Black, 0.8f));
                }
            }
        }
        EndTextureMode();

        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (uint y = 0; y < map.TilesY; y++)
        {
            for (uint x = 0; x < map.TilesX; x++)
            {
                // Draw tiles from id (and tile borders)
                DrawRectangle((int)x * MapTileSize, (int)y * MapTileSize, MapTileSize, MapTileSize,
                    (map.TileIds[y * map.TilesX + x] == 0) ? Color.Blue : Fade(Color.Blue, 0.9f));
                DrawRectangleLines((int)x * MapTileSize, (int)y * MapTileSize, MapTileSize, MapTileSize, Fade(Color.DarkBlue, 0.5f));
            }
        }

        // Draw player
        DrawRectangleV(playerPosition, new Vector2(PlayerSize, PlayerSize), Color.Red);

        // Draw fog of war (scaled to full map, bilinear filtering)
        DrawTexturePro(fogOfWar.Texture,
            new Rectangle(0, 0, (float)fogOfWar.Texture.Width, (float)-fogOfWar.Texture.Height),
            new Rectangle(0, 0, (float)map.TilesX * MapTileSize, (float)map.TilesY * MapTileSize),
            new Vector2(0, 0), 0.0f, Color.White);

        // Draw player current tile
        DrawText($"Current tile: [{playerTileX},{playerTileY}]", 10, 10, 20, Color.RayWhite);
        DrawText("ARROW KEYS to move", 10, screenHeight - 25, 20, Color.RayWhite);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        UnloadRenderTexture(fogOfWar);  // Unload render texture
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [textures] example - fog of war");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new FogOfWar();
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
