/*******************************************************************************************
*
*   raylib [core] example - directory files
*
*   Example complexity rating: [★☆☆☆] 1/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Hugo ARNAL (@hugoarnal) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Hugo ARNAL (@hugoarnal)
*
********************************************************************************************/

// NOTE: The original example relies on raygui (GuiButton, GuiLabel, GuiListViewEx) for its UI.
// raygui is not part of the raylib-cs bindings, so the back button, directory label and file
// list view are reimplemented here with plain raylib primitives. The directory navigation
// behaviour (enter directories, go back) is preserved.

namespace Examples.Core;

public partial class DirectoryFiles : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const string FileFilter = "DIRS*;.png;.c";

    public string Name => "Core / Directory Files";

    public string Title => "raylib [core] example - directory files";

    private string directory;
    private FilePathList files;

    private bool btnBackPressed;

    private int listScrollIndex;
    private int listItemActive;
    private int listItemFocused;

    public void Init()
    {
        directory = GetWorkingDirectoryAsString();

        // Load file-paths on current working directory
        // NOTE: LoadDirectoryFiles() loads files and directories by default,
        // use LoadDirectoryFilesEx() for custom filters and recursive directories loading
        //files = LoadDirectoryFiles(directory);
        files = LoadDirectoryFilesEx(directory, FileFilter, false);

        btnBackPressed = false;

        listScrollIndex = 0;
        listItemActive = -1;
        listItemFocused = -1;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (btnBackPressed)
        {
            directory = GetPrevDirectoryPath(directory);
            UnloadDirectoryFiles(files);
            files = LoadDirectoryFilesEx(directory, FileFilter, false);

            listScrollIndex = 0;
            listItemActive = -1;
            listItemFocused = -1;
        }

        if ((listItemActive >= 0) && (listItemActive < (int)files.Count))
        {
            string selected = files[(uint)listItemActive];
            bool isDirectory = DirectoryExists(selected);
            if (isDirectory)
            {
                directory = selected;
                UnloadDirectoryFiles(files);
                files = LoadDirectoryFilesEx(directory, FileFilter, false);

                listScrollIndex = 0;
                listItemActive = -1;
                listItemFocused = -1;
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        Vector2 mouse = GetMousePosition();

        // Back button "<"
        Rectangle backBounds = new Rectangle(40.0f, 10.0f, 48, 28);
        bool backHover = CheckCollisionPointRec(mouse, backBounds);
        DrawRectangleRec(backBounds, backHover ? Color.SkyBlue : Color.LightGray);
        DrawRectangleLinesEx(backBounds, 1, Color.Gray);
        int backTextWidth = MeasureText("<", 20);
        DrawText("<", (int)(backBounds.X + (backBounds.Width - backTextWidth) / 2), (int)(backBounds.Y + 4), 20, Color.DarkGray);
        btnBackPressed = backHover && IsMouseButtonReleased(MouseButton.Left);

        // Current directory label
        DrawText(directory, 40 + 48 + 10, 16, 20, Color.DarkGray);

        // File list view
        Rectangle listBounds = new Rectangle(0, 50, GetScreenWidth(), GetScreenHeight() - 50);
        DrawRectangleRec(listBounds, Color.RayWhite);
        DrawRectangleLinesEx(listBounds, 1, Color.Gray);

        int count = (int)files.Count;
        float rowHeight = 28;
        int visibleRows = (int)(listBounds.Height / rowHeight);

        bool mouseInList = CheckCollisionPointRec(mouse, listBounds);
        if (mouseInList)
        {
            listScrollIndex -= (int)GetMouseWheelMove();
        }
        int maxScroll = Math.Max(0, count - visibleRows);
        listScrollIndex = Math.Clamp(listScrollIndex, 0, maxScroll);

        listItemFocused = -1;
        for (int i = 0; i < visibleRows; i++)
        {
            int itemIndex = listScrollIndex + i;
            if (itemIndex >= count)
            {
                break;
            }

            Rectangle rowRec = new Rectangle(listBounds.X + 1, listBounds.Y + 1 + i * rowHeight, listBounds.Width - 2, rowHeight);
            bool rowHover = mouseInList && CheckCollisionPointRec(mouse, rowRec);

            if (itemIndex == listItemActive)
            {
                DrawRectangleRec(rowRec, Fade(Color.SkyBlue, 0.7f));
            }
            else if (rowHover)
            {
                DrawRectangleRec(rowRec, Fade(Color.SkyBlue, 0.3f));
            }

            if (rowHover)
            {
                listItemFocused = itemIndex;
                if (IsMouseButtonReleased(MouseButton.Left))
                {
                    listItemActive = itemIndex;
                }
            }

            DrawText(files[(uint)itemIndex], (int)rowRec.X + 40, (int)rowRec.Y + 8, 10, Color.DarkGray);
        }

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadDirectoryFiles(files);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - directory files");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new DirectoryFiles();
        game.Init();

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
}
