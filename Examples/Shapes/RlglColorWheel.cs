/*******************************************************************************************
*
*   raylib [shapes] example - rlgl color wheel
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by Robin (@RobinsAviary) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Robin (@RobinsAviary)
*
********************************************************************************************/

using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Rlgl;

namespace Examples.Shapes;

public partial class RlglColorWheel : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    public string Name => "Shapes / RLGL Color Wheel";

    public string Title => "raylib [shapes] example - rlgl color wheel";

    public ConfigFlags ConfigFlags => ConfigFlags.Msaa4xHint;

    // The minimum/maximum points the circle can have
    private const int pointsMin = 3;
    private const int pointsMax = 256;

    // The current number of points and the radius of the circle
    private int triangleCount;
    private float pointScale;

    // Slider value, literally maps to value in HSV
    private float value;

    // The center of the screen
    private Vector2 center;
    // The location of the color wheel
    private Vector2 circlePosition;

    // The currently selected color
    private Color color;

    // Indicates if the slider is being clicked
    private bool sliderClicked;

    // Indicates if the current color going to be updated, as well as the handle position
    private bool settingColor;

    // How the color wheel will be rendered
    private DrawMode renderType;

    public void Init()
    {
        triangleCount = 64;
        pointScale = 150.0f;
        value = 1.0f;

        center = new Vector2((float)screenWidth / 2.0f, (float)screenHeight / 2.0f);
        circlePosition = center;

        color = new Color(255, 255, 255, 255);

        sliderClicked = false;
        settingColor = false;

        renderType = DrawMode.Triangles;
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        triangleCount += (int)GetMouseWheelMove();
        triangleCount = (int)Clamp((float)triangleCount, (float)pointsMin, (float)pointsMax);

        Rectangle sliderRectangle = new Rectangle(42.0f, 16.0f + 64.0f + 45.0f, 64.0f, 16.0f);
        Vector2 mousePosition = GetMousePosition();

        // Checks if the user is hovering over the value slider
        bool sliderHover = (mousePosition.X >= sliderRectangle.X && mousePosition.Y >= sliderRectangle.Y && mousePosition.X < sliderRectangle.X + sliderRectangle.Width && mousePosition.Y < sliderRectangle.Y + sliderRectangle.Height);

        // Copy color as hex
        if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyDown(KeyboardKey.C))
        {
            if (IsKeyPressed(KeyboardKey.C))
            {
                SetClipboardText($"#{color.R:X2}{color.G:X2}{color.B:X2}");
            }
        }

        // Scale up the color wheel, adjusting the handle visually
        if (IsKeyDown(KeyboardKey.Up))
        {
            pointScale *= 1.025f;

            if (pointScale > (float)screenHeight / 2.0f)
            {
                pointScale = (float)screenHeight / 2.0f;
            }
            else
            {
                circlePosition = Vector2Add(Vector2Multiply(Vector2Subtract(circlePosition, center), new Vector2(1.025f, 1.025f)), center);
            }
        }

        // Scale down the wheel, adjusting the handle visually
        if (IsKeyDown(KeyboardKey.Down))
        {
            pointScale *= 0.975f;

            if (pointScale < 32.0f)
            {
                pointScale = 32.0f;
            }
            else
            {
                circlePosition = Vector2Add(Vector2Multiply(Vector2Subtract(circlePosition, center), new Vector2(0.975f, 0.975f)), center);
            }

            float distanceDown = Vector2Distance(center, circlePosition) / pointScale;
            float angleDown = ((Vector2Angle(new Vector2(0.0f, -pointScale), Vector2Subtract(center, circlePosition)) / MathF.PI + 1.0f) / 2.0f);

            if (distanceDown > 1.0f)
            {
                circlePosition = Vector2Add(new Vector2(MathF.Sin(angleDown * (MathF.PI * 2.0f)) * pointScale, -MathF.Cos(angleDown * (MathF.PI * 2.0f)) * pointScale), center);
            }
        }

        // Checks if the user clicked on the color wheel
        if (IsMouseButtonPressed(MouseButton.Left) && Vector2Distance(GetMousePosition(), center) <= pointScale + 10.0f)
        {
            settingColor = true;
        }

        // Update flag when mouse button is released
        if (IsMouseButtonReleased(MouseButton.Left)) settingColor = false;

        // Check if the user clicked/released the slider for the color's value
        if (sliderHover && IsMouseButtonPressed(MouseButton.Left)) sliderClicked = true;

        if (sliderClicked && IsMouseButtonReleased(MouseButton.Left)) sliderClicked = false;

        // Update render mode accordingly
        if (IsKeyPressed(KeyboardKey.Space)) renderType = DrawMode.Lines;

        if (IsKeyReleased(KeyboardKey.Space)) renderType = DrawMode.Triangles;

        // If the slider or the wheel was clicked, update the current color
        if (settingColor || sliderClicked)
        {
            if (settingColor) circlePosition = GetMousePosition();

            float distance = Vector2Distance(center, circlePosition) / pointScale;

            float angle = ((Vector2Angle(new Vector2(0.0f, -pointScale), Vector2Subtract(center, circlePosition)) / MathF.PI + 1.0f) / 2.0f);
            if (settingColor && distance > 1.0f) circlePosition = Vector2Add(new Vector2(MathF.Sin(angle * (MathF.PI * 2.0f)) * pointScale, -MathF.Cos(angle * (MathF.PI * 2.0f)) * pointScale), center);

            float angle360 = angle * 360.0f;
            float valueActual = Clamp(distance, 0.0f, 1.0f);
            color = ColorLerp(new Color((int)(value * 255.0f), (int)(value * 255.0f), (int)(value * 255.0f), 255), ColorFromHSV(angle360, Clamp(distance, 0.0f, 1.0f), 1.0f), valueActual);
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        // Begin rendering color wheel
        Begin(renderType);
        for (int i = 0; i < triangleCount; i++)
        {
            float angleOffset = ((MathF.PI * 2.0f) / (float)triangleCount);
            float angle = angleOffset * (float)i;
            float angleOffsetCalculated = ((float)i + 1) * angleOffset;
            Vector2 scale = new Vector2(pointScale, pointScale);

            Vector2 offset = Vector2Multiply(new Vector2(MathF.Sin(angle), -MathF.Cos(angle)), scale);
            Vector2 offset2 = Vector2Multiply(new Vector2(MathF.Sin(angleOffsetCalculated), -MathF.Cos(angleOffsetCalculated)), scale);

            Vector2 position = Vector2Add(center, offset);
            Vector2 position2 = Vector2Add(center, offset2);

            float angleNonRadian = (angle / (2.0f * MathF.PI)) * 360.0f;
            float angleNonRadianOffset = (angleOffset / (2.0f * MathF.PI)) * 360.0f;

            Color currentColor = ColorFromHSV(angleNonRadian, 1.0f, 1.0f);
            Color offsetColor = ColorFromHSV(angleNonRadian + angleNonRadianOffset, 1.0f, 1.0f);

            // Input vertices differently depending on mode
            if (renderType == DrawMode.Triangles)
            {
                // RL_TRIANGLES expects three vertices per triangle
                Color4ub(currentColor.R, currentColor.G, currentColor.B, currentColor.A);
                Vertex2f(position.X, position.Y);
                Color4f(value, value, value, 1.0f);
                Vertex2f(center.X, center.Y);
                Color4ub(offsetColor.R, offsetColor.G, offsetColor.B, offsetColor.A);
                Vertex2f(position2.X, position2.Y);
            }
            else if (renderType == DrawMode.Lines)
            {
                // RL_LINES expects two vertices per line
                Color4ub(currentColor.R, currentColor.G, currentColor.B, currentColor.A);
                Vertex2f(position.X, position.Y);
                Color4ub(Color.White.R, Color.White.G, Color.White.B, Color.White.A);
                Vertex2f(center.X, center.Y);

                Vertex2f(center.X, center.Y);
                Color4ub(offsetColor.R, offsetColor.G, offsetColor.B, offsetColor.A);
                Vertex2f(position2.X, position2.Y);

                Vertex2f(position2.X, position2.Y);
                Color4ub(currentColor.R, currentColor.G, currentColor.B, currentColor.A);
                Vertex2f(position.X, position.Y);
            }
        }
        End();

        // Make the handle slightly more visible overtop darker colors
        Color handleColor = Color.Black;

        if (Vector2Distance(center, circlePosition) / pointScale <= 0.5f && value <= 0.5f)
        {
            handleColor = Color.DarkGray;
        }

        // Draw the color handle
        DrawCircleLinesV(circlePosition, 4.0f, handleColor);

        // Draw the color in a preview, with a darkened outline.
        DrawRectangleV(new Vector2(8.0f, 8.0f), new Vector2(64.0f, 64.0f), color);
        DrawRectangleLinesEx(new Rectangle(8.0f, 8.0f, 64.0f, 64.0f), 2.0f, ColorLerp(color, Color.Black, 0.5f));

        // Draw current color as hex and decimal
        DrawText($"#{color.R:X2}{color.G:X2}{color.B:X2}\n({color.R}, {color.G}, {color.B})", 8, 8 + 64 + 8, 20, Color.DarkGray);

        // Update the visuals for the copying text
        Color copyColor = Color.DarkGray;
        int textOffset = 0;
        if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyDown(KeyboardKey.C))
        {
            copyColor = Color.DarkGreen;
            textOffset = 4;
        }

        // Draw the copying text
        DrawText("press ctrl+c to copy!", 8, 425 - textOffset, 20, copyColor);

        // Display the number of rendered triangles
        DrawText($"triangle count: {triangleCount}", 8, 395, 20, Color.DarkGray);

        // Slider to change color's value (minimal raygui-like control, raygui is not bound in raylib-cs)
        GuiSliderBar(sliderRectangle, "value: ", "", ref value, 0.0f, 1.0f);

        // Draw FPS next to outlined color preview
        DrawFPS(64 + 16, 8);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiSliderBar(Rectangle bounds, string textLeft, string textRight, ref float value, float minValue, float maxValue)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (hover && IsMouseButtonDown(MouseButton.Left))
        {
            value = minValue + ((mouse.X - bounds.X) / bounds.Width) * (maxValue - minValue);
            if (value < minValue) value = minValue;
            if (value > maxValue) value = maxValue;
        }

        DrawRectangleRec(bounds, Color.LightGray);
        float pct = (value - minValue) / (maxValue - minValue);
        DrawRectangleRec(new Rectangle(bounds.X, bounds.Y, bounds.Width * pct, bounds.Height), Color.SkyBlue);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (!string.IsNullOrEmpty(textLeft)) DrawText(textLeft, (int)bounds.X - MeasureText(textLeft, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        if (!string.IsNullOrEmpty(textRight)) DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - rlgl color wheel");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new RlglColorWheel();
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
