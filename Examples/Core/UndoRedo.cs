/*******************************************************************************************
*
*   raylib [core] example - undo redo
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class UndoRedo : IExample
{
    private const int MAX_UNDO_STATES = 26;      // Maximum undo states supported for the ring buffer

    private const int GRID_CELL_SIZE = 24;
    private const int MAX_GRID_CELLS_X = 30;
    private const int MAX_GRID_CELLS_Y = 13;

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Undo Redo";

    public string Title => "raylib [core] example - undo redo";

    // Point struct, like Vector2 but using int
    private struct Point
    {
        public int X;
        public int Y;
    }

    // Player state struct
    // NOTE: Contains all player data that needs to be affected by undo/redo
    private struct PlayerState
    {
        public Point Cell;
        public Color Color;
    }

    // Undo/redo system variables
    private int currentUndoIndex;
    private int firstUndoIndex;
    private int lastUndoIndex;
    private int undoFrameCounter;
    private Vector2 undoInfoPos;

    private PlayerState player;
    private PlayerState[] states;

    // Grid variables
    private Vector2 gridPosition;

    // Compare two player states (replaces memcmp)
    private static bool SameState(in PlayerState a, in PlayerState b)
    {
        return (a.Cell.X == b.Cell.X) && (a.Cell.Y == b.Cell.Y) &&
            (a.Color.R == b.Color.R) && (a.Color.G == b.Color.G) &&
            (a.Color.B == b.Color.B) && (a.Color.A == b.Color.A);
    }

    public void Init()
    {
        currentUndoIndex = 0;
        firstUndoIndex = 0;
        lastUndoIndex = 0;
        undoFrameCounter = 0;
        undoInfoPos = new Vector2(110, 400);

        // Init current player state and undo/redo recorded states array
        player = new PlayerState();
        player.Cell = new Point { X = 10, Y = 10 };
        player.Color = Color.Red;

        // Init undo buffer to store MAX_UNDO_STATES states
        states = new PlayerState[MAX_UNDO_STATES];
        // Init all undo states to current state
        for (int i = 0; i < MAX_UNDO_STATES; i++) states[i] = player;

        // Grid variables
        gridPosition = new Vector2(40, 60);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        // Player movement logic
        if (IsKeyPressed(KeyboardKey.Right)) player.Cell.X++;
        else if (IsKeyPressed(KeyboardKey.Left)) player.Cell.X--;
        else if (IsKeyPressed(KeyboardKey.Up)) player.Cell.Y--;
        else if (IsKeyPressed(KeyboardKey.Down)) player.Cell.Y++;

        // Make sure player does not go out of bounds
        if (player.Cell.X < 0) player.Cell.X = 0;
        else if (player.Cell.X >= MAX_GRID_CELLS_X) player.Cell.X = MAX_GRID_CELLS_X - 1;
        if (player.Cell.Y < 0) player.Cell.Y = 0;
        else if (player.Cell.Y >= MAX_GRID_CELLS_Y) player.Cell.Y = MAX_GRID_CELLS_Y - 1;

        // Player color change logic
        if (IsKeyPressed(KeyboardKey.Space))
        {
            player.Color.R = (byte)GetRandomValue(20, 255);
            player.Color.G = (byte)GetRandomValue(20, 220);
            player.Color.B = (byte)GetRandomValue(20, 240);
        }

        // Undo state change logic
        undoFrameCounter++;

        // Waiting a number of frames before checking if we should store a new state snapshot
        if (undoFrameCounter >= 2) // Checking every 2 frames
        {
            if (!SameState(states[currentUndoIndex], player))
            {
                // Move cursor to next available position of the undo ring buffer to record state
                currentUndoIndex++;
                if (currentUndoIndex >= MAX_UNDO_STATES) currentUndoIndex = 0;
                if (currentUndoIndex == firstUndoIndex) firstUndoIndex++;
                if (firstUndoIndex >= MAX_UNDO_STATES) firstUndoIndex = 0;

                states[currentUndoIndex] = player;
                lastUndoIndex = currentUndoIndex;
            }

            undoFrameCounter = 0;
        }

        // Recover previous state from buffer: CTRL+Z
        if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.Z))
        {
            if (currentUndoIndex != firstUndoIndex)
            {
                currentUndoIndex--;
                if (currentUndoIndex < 0) currentUndoIndex = MAX_UNDO_STATES - 1;

                if (!SameState(states[currentUndoIndex], player))
                {
                    player = states[currentUndoIndex];
                }
            }
        }

        // Recover next state from buffer: CTRL+Y
        if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.Y))
        {
            if (currentUndoIndex != lastUndoIndex)
            {
                int nextUndoIndex = currentUndoIndex + 1;
                if (nextUndoIndex >= MAX_UNDO_STATES) nextUndoIndex = 0;

                if (nextUndoIndex != firstUndoIndex)
                {
                    currentUndoIndex = nextUndoIndex;

                    if (!SameState(states[currentUndoIndex], player))
                    {
                        player = states[currentUndoIndex];
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw controls info
        DrawText("[ARROWS] MOVE PLAYER - [SPACE] CHANGE PLAYER COLOR", 40, 20, 20, Color.DarkGray);

        // Draw player visited cells recorded by undo
        // NOTE: Remember we are using a ring buffer approach so,
        // some cells info could start at the end of the array and end at the beginning
        if (lastUndoIndex > firstUndoIndex)
        {
            for (int i = firstUndoIndex; i < currentUndoIndex; i++)
                DrawRectangleRec(new Rectangle(gridPosition.X + states[i].Cell.X * GRID_CELL_SIZE, gridPosition.Y + states[i].Cell.Y * GRID_CELL_SIZE,
                    GRID_CELL_SIZE, GRID_CELL_SIZE), Color.LightGray);
        }
        else if (firstUndoIndex > lastUndoIndex)
        {
            if ((currentUndoIndex < MAX_UNDO_STATES) && (currentUndoIndex > lastUndoIndex))
            {
                for (int i = firstUndoIndex; i < currentUndoIndex; i++)
                    DrawRectangleRec(new Rectangle(gridPosition.X + states[i].Cell.X * GRID_CELL_SIZE, gridPosition.Y + states[i].Cell.Y * GRID_CELL_SIZE,
                        GRID_CELL_SIZE, GRID_CELL_SIZE), Color.LightGray);
            }
            else
            {
                for (int i = firstUndoIndex; i < MAX_UNDO_STATES; i++)
                    DrawRectangle((int)gridPosition.X + states[i].Cell.X * GRID_CELL_SIZE, (int)gridPosition.Y + states[i].Cell.Y * GRID_CELL_SIZE,
                        GRID_CELL_SIZE, GRID_CELL_SIZE, Color.LightGray);
                for (int i = 0; i < currentUndoIndex; i++)
                    DrawRectangle((int)gridPosition.X + states[i].Cell.X * GRID_CELL_SIZE, (int)gridPosition.Y + states[i].Cell.Y * GRID_CELL_SIZE,
                        GRID_CELL_SIZE, GRID_CELL_SIZE, Color.LightGray);
            }
        }

        // Draw game grid
        for (int y = 0; y <= MAX_GRID_CELLS_Y; y++)
            DrawLine((int)gridPosition.X, (int)gridPosition.Y + y * GRID_CELL_SIZE,
                (int)gridPosition.X + MAX_GRID_CELLS_X * GRID_CELL_SIZE, (int)gridPosition.Y + y * GRID_CELL_SIZE, Color.Gray);
        for (int x = 0; x <= MAX_GRID_CELLS_X; x++)
            DrawLine((int)gridPosition.X + x * GRID_CELL_SIZE, (int)gridPosition.Y,
                (int)gridPosition.X + x * GRID_CELL_SIZE, (int)gridPosition.Y + MAX_GRID_CELLS_Y * GRID_CELL_SIZE, Color.Gray);

        // Draw player
        DrawRectangle((int)gridPosition.X + player.Cell.X * GRID_CELL_SIZE, (int)gridPosition.Y + player.Cell.Y * GRID_CELL_SIZE,
            GRID_CELL_SIZE + 1, GRID_CELL_SIZE + 1, player.Color);

        // Draw undo system buffer info
        DrawText("UNDO STATES:", (int)undoInfoPos.X - 85, (int)undoInfoPos.Y + 9, 10, Color.DarkGray);
        DrawUndoBuffer(undoInfoPos, firstUndoIndex, lastUndoIndex, currentUndoIndex, 24);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    // Draw undo system visualization logic
    // NOTE: Visualizing the ring buffer array, every square can store a player state
    private static void DrawUndoBuffer(Vector2 position, int firstUndoIndex, int lastUndoIndex, int currentUndoIndex, int slotSize)
    {
        // Draw index marks
        DrawRectangle((int)position.X + 8 + slotSize * currentUndoIndex, (int)position.Y - 10, 8, 8, Color.Red);
        DrawRectangleLines((int)position.X + 2 + slotSize * firstUndoIndex, (int)position.Y + 27, 8, 8, Color.Black);
        DrawRectangle((int)position.X + 14 + slotSize * lastUndoIndex, (int)position.Y + 27, 8, 8, Color.Black);

        // Draw background gray slots
        for (int i = 0; i < MAX_UNDO_STATES; i++)
        {
            DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.LightGray);
            DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Gray);
        }

        // Draw occupied slots: firstUndoIndex --> lastUndoIndex
        if (firstUndoIndex <= lastUndoIndex)
        {
            for (int i = firstUndoIndex; i < lastUndoIndex + 1; i++)
            {
                DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.SkyBlue);
                DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Blue);
            }
        }
        else if (lastUndoIndex < firstUndoIndex)
        {
            for (int i = firstUndoIndex; i < MAX_UNDO_STATES; i++)
            {
                DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.SkyBlue);
                DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Blue);
            }

            for (int i = 0; i < lastUndoIndex + 1; i++)
            {
                DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.SkyBlue);
                DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Blue);
            }
        }

        // Draw occupied slots: firstUndoIndex --> currentUndoIndex
        if (firstUndoIndex < currentUndoIndex)
        {
            for (int i = firstUndoIndex; i < currentUndoIndex; i++)
            {
                DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Green);
                DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Lime);
            }
        }
        else if (currentUndoIndex < firstUndoIndex)
        {
            for (int i = firstUndoIndex; i < MAX_UNDO_STATES; i++)
            {
                DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Green);
                DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Lime);
            }

            for (int i = 0; i < currentUndoIndex; i++)
            {
                DrawRectangle((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Green);
                DrawRectangleLines((int)position.X + slotSize * i, (int)position.Y, slotSize, slotSize, Color.Lime);
            }
        }

        // Draw current selected UNDO slot
        DrawRectangle((int)position.X + slotSize * currentUndoIndex, (int)position.Y, slotSize, slotSize, Color.Gold);
        DrawRectangleLines((int)position.X + slotSize * currentUndoIndex, (int)position.Y, slotSize, slotSize, Color.Orange);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - undo redo");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new UndoRedo();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())    // Detect window close button or ESC key
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow();          // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
