/*******************************************************************************************
*
*   raylib [core] example - input actions
*
*   Example complexity rating: [★★☆☆] 2/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Jett (@JettMonstersGoBoom) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Jett (@JettMonstersGoBoom)
*
********************************************************************************************/

// Simple example for decoding input as actions, allowing remapping of input to different keys or gamepad buttons
// For example instead of using `IsKeyDown(KEY_LEFT)`, you can use `IsActionDown(ACTION_LEFT)`
// which can be reassigned to e.g. KEY_A and also assigned to a gamepad button. the action will trigger with either gamepad or keys

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Core;

public partial class InputActions : IExample
{
    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    private enum ActionType
    {
        NoAction = 0,
        ActionUp,
        ActionDown,
        ActionLeft,
        ActionRight,
        ActionFire,
        MaxAction
    }

    // Key and button inputs
    private struct ActionInput
    {
        public KeyboardKey Key;
        public GamepadButton Button;
    }

    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Core / Input Actions";

    public string Title => "raylib [core] example - input actions";

    private int gamepadIndex; // Gamepad default index
    private ActionInput[] actionInputs;

    private int actionSet;
    private bool releaseAction;
    private Vector2 position;
    private Vector2 size;

    public void Init()
    {
        gamepadIndex = 0;
        actionInputs = new ActionInput[(int)ActionType.MaxAction];

        // Set default actions
        actionSet = 0;
        SetActionsDefault();
        releaseAction = false;

        position = new Vector2(400.0f, 200.0f);
        size = new Vector2(40.0f, 40.0f);
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        gamepadIndex = 0; //  Set gamepad being checked

        if (IsActionDown(ActionType.ActionUp)) position.Y -= 2;
        if (IsActionDown(ActionType.ActionDown)) position.Y += 2;
        if (IsActionDown(ActionType.ActionLeft)) position.X -= 2;
        if (IsActionDown(ActionType.ActionRight)) position.X += 2;
        if (IsActionPressed(ActionType.ActionFire))
        {
            position.X = (screenWidth - size.X) / 2;
            position.Y = (screenHeight - size.Y) / 2;
        }

        // Register release action for one frame
        releaseAction = false;
        if (IsActionReleased(ActionType.ActionFire)) releaseAction = true;

        // Switch control scheme by pressing TAB
        if (IsKeyPressed(KeyboardKey.Tab))
        {
            actionSet = (actionSet == 0) ? 1 : 0;
            if (actionSet == 0) SetActionsDefault();
            else SetActionsCursor();
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.Gray);

        DrawRectangleV(position, size, releaseAction ? Color.Blue : Color.Red);

        DrawText((actionSet == 0) ? "Current input set: WASD (default)" : "Current input set: Arrow keys", 10, 10, 20, Color.White);
        DrawText("Use TAB key to toggles Actions keyset", 10, 50, 20, Color.Green);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    // Check action key/button pressed
    // NOTE: Combines key pressed and gamepad button pressed in one action
    private bool IsActionPressed(ActionType action)
    {
        bool result = false;

        if (action < ActionType.MaxAction) result = (IsKeyPressed(actionInputs[(int)action].Key) || IsGamepadButtonPressed(gamepadIndex, actionInputs[(int)action].Button));

        return result;
    }

    // Check action key/button released
    // NOTE: Combines key released and gamepad button released in one action
    private bool IsActionReleased(ActionType action)
    {
        bool result = false;

        if (action < ActionType.MaxAction) result = (IsKeyReleased(actionInputs[(int)action].Key) || IsGamepadButtonReleased(gamepadIndex, actionInputs[(int)action].Button));

        return result;
    }

    // Check action key/button down
    // NOTE: Combines key down and gamepad button down in one action
    private bool IsActionDown(ActionType action)
    {
        bool result = false;

        if (action < ActionType.MaxAction) result = (IsKeyDown(actionInputs[(int)action].Key) || IsGamepadButtonDown(gamepadIndex, actionInputs[(int)action].Button));

        return result;
    }

    // Set the "default" keyset
    // NOTE: Here WASD and gamepad buttons on the left side for movement
    private void SetActionsDefault()
    {
        actionInputs[(int)ActionType.ActionUp].Key = KeyboardKey.W;
        actionInputs[(int)ActionType.ActionDown].Key = KeyboardKey.S;
        actionInputs[(int)ActionType.ActionLeft].Key = KeyboardKey.A;
        actionInputs[(int)ActionType.ActionRight].Key = KeyboardKey.D;
        actionInputs[(int)ActionType.ActionFire].Key = KeyboardKey.Space;

        actionInputs[(int)ActionType.ActionUp].Button = GamepadButton.LeftFaceUp;
        actionInputs[(int)ActionType.ActionDown].Button = GamepadButton.LeftFaceDown;
        actionInputs[(int)ActionType.ActionLeft].Button = GamepadButton.LeftFaceLeft;
        actionInputs[(int)ActionType.ActionRight].Button = GamepadButton.LeftFaceRight;
        actionInputs[(int)ActionType.ActionFire].Button = GamepadButton.RightFaceDown;
    }

    // Set the "alternate" keyset
    // NOTE: Here cursor keys and gamepad buttons on the right side for movement
    private void SetActionsCursor()
    {
        actionInputs[(int)ActionType.ActionUp].Key = KeyboardKey.Up;
        actionInputs[(int)ActionType.ActionDown].Key = KeyboardKey.Down;
        actionInputs[(int)ActionType.ActionLeft].Key = KeyboardKey.Left;
        actionInputs[(int)ActionType.ActionRight].Key = KeyboardKey.Right;
        actionInputs[(int)ActionType.ActionFire].Key = KeyboardKey.Space;

        actionInputs[(int)ActionType.ActionUp].Button = GamepadButton.RightFaceUp;
        actionInputs[(int)ActionType.ActionDown].Button = GamepadButton.RightFaceDown;
        actionInputs[(int)ActionType.ActionLeft].Button = GamepadButton.RightFaceLeft;
        actionInputs[(int)ActionType.ActionRight].Button = GamepadButton.RightFaceRight;
        actionInputs[(int)ActionType.ActionFire].Button = GamepadButton.LeftFaceDown;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [core] example - input actions");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new InputActions();
        game.Init();

        // Main game loop
        while (!WindowShouldClose()) // Detect window close button or ESC key
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
