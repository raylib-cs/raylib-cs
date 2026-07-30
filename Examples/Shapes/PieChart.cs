/*******************************************************************************************
*
*   raylib [shapes] example - pie chart
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 5.5, last time updated with raylib 5.6
*
*   Example contributed by Gideon Serfontein (@GideonSerf) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 Gideon Serfontein (@GideonSerf)
*
********************************************************************************************/

using System.Text;

namespace Examples.Shapes;

public partial class PieChart : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MAX_PIE_SLICES = 10;       // Max pie slices

    public string Name => "Shapes / Pie Chart";

    public string Title => "raylib [shapes] example - pie chart";

    private int sliceCount;
    private float donutInnerRadius;
    private float[] values;
    private StringBuilder[] labels;
    private bool[] editingLabel;

    private bool showValues;
    private bool showPercentages;
    private bool showDonut;
    private int hoveredSlice;
    private Vector2 scrollContentOffset;

    // UI layout parameters
    private const int panelWidth = 270;
    private const int panelMargin = 5;

    private Vector2 panelPos;
    private Rectangle panelRect;
    private Rectangle canvas;
    private Vector2 center;
    private const float radius = 205.0f;

    // Total value for percentage calculations
    private float totalValue;

    private int framesCounter;

    // Minimal raygui-like global state
    private static bool guiDisabled;

    public void Init()
    {
        sliceCount = 7;
        donutInnerRadius = 25.0f;
        values = new float[MAX_PIE_SLICES] { 300.0f, 100.0f, 450.0f, 350.0f, 600.0f, 380.0f, 750.0f, 0.0f, 0.0f, 0.0f };
        labels = new StringBuilder[MAX_PIE_SLICES];
        editingLabel = new bool[MAX_PIE_SLICES];

        for (int i = 0; i < MAX_PIE_SLICES; i++)
        {
            labels[i] = new StringBuilder($"Slice {i + 1:D2}");
        }

        showValues = true;
        showPercentages = false;
        showDonut = false;
        hoveredSlice = -1;
        scrollContentOffset = new Vector2(0, 0);

        // UI Panel top-left anchor
        panelPos = new Vector2(
            (float)screenWidth - panelMargin - panelWidth,
            (float)panelMargin
        );

        // UI Panel rectangle
        panelRect = new Rectangle(
            panelPos.X, panelPos.Y,
            (float)panelWidth,
            (float)screenHeight - 2.0f * panelMargin
        );

        // Pie chart geometry
        canvas = new Rectangle(0, 0, panelPos.X, (float)screenHeight);
        center = new Vector2(canvas.Width / 2.0f, canvas.Height / 2.0f);

        totalValue = 0.0f;

        framesCounter = 0;
        guiDisabled = false;
    }

    public void Update()
    {
        framesCounter++;

        // Update
        //----------------------------------------------------------------------------------
        // Calculate total value for percentage calculations
        totalValue = 0.0f;
        for (int i = 0; i < sliceCount; i++)
        {
            totalValue += values[i];
        }

        // Check for mouse hover over slices
        hoveredSlice = -1; // Reset hovered slice
        Vector2 mousePos = GetMousePosition();
        if (CheckCollisionPointRec(mousePos, canvas)) // Only check if mouse is inside the canvas
        {
            float dx = mousePos.X - center.X;
            float dy = mousePos.Y - center.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            if (distance <= radius) // Inside the pie radius
            {
                float angle = MathF.Atan2(dy, dx) * RAD2DEG;
                if (angle < 0)
                {
                    angle += 360;
                }

                float currentAngle = 0.0f;
                for (int i = 0; i < sliceCount; i++)
                {
                    float sweep = (totalValue > 0) ? (values[i] / totalValue) * 360.0f : 0.0f;

                    if ((angle >= currentAngle) && (angle < (currentAngle + sweep)))
                    {
                        hoveredSlice = i;
                        break;
                    }

                    currentAngle += sweep;
                }
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground(Color.RayWhite);

        // Draw the pie chart on the canvas
        float startAngle = 0.0f;
        for (int i = 0; i < sliceCount; i++)
        {
            float sweepAngle = (totalValue > 0) ? (values[i] / totalValue) * 360.0f : 0.0f;
            float midAngle = startAngle + sweepAngle / 2.0f; // Middle angle for label positioning

            Color color = ColorFromHSV((float)i / sliceCount * 360.0f, 0.75f, 0.9f);
            float currentRadius = radius;

            // Make the hovered slice pop out by adding pixels to its radius
            if (i == hoveredSlice)
            {
                currentRadius += 20.0f;
            }

            // Draw the pie slice using raylib's DrawCircleSector function
            DrawCircleSector(center, currentRadius, startAngle, startAngle + sweepAngle, 120, color);

            // Draw the label for the current slice
            if (values[i] > 0)
            {
                string labelText;
                if (showValues && showPercentages)
                {
                    labelText = $"{values[i]:F1} ({(values[i] / totalValue) * 100.0f:F0}%)";
                }
                else if (showValues)
                {
                    labelText = $"{values[i]:F1}";
                }
                else if (showPercentages)
                {
                    labelText = $"{(values[i] / totalValue) * 100.0f:F0}%";
                }
                else
                {
                    labelText = "";
                }

                Vector2 textSize = MeasureTextEx(GetFontDefault(), labelText, 20, 1);
                float labelRadius = radius * 0.7f;
                Vector2 labelPos = new Vector2(center.X + MathF.Cos(midAngle * DEG2RAD) * labelRadius - textSize.X / 2.0f,
                    center.Y + MathF.Sin(midAngle * DEG2RAD) * labelRadius - textSize.Y / 2.0f);
                DrawText(labelText, (int)labelPos.X, (int)labelPos.Y, 20, Color.White);
            }

            // Draw inner circle to create donut effect
            // TODO: This is a hacky solution, better use DrawRing()
            if (showDonut)
            {
                DrawCircleV(center, donutInnerRadius, Color.RayWhite);
            }

            startAngle += sweepAngle;
        }

        // UI control panel (minimal raygui-like controls, raygui is not bound in raylib-cs)
        DrawRectangleRec(panelRect, Fade(Color.LightGray, 0.5f));
        DrawRectangleLinesEx(panelRect, 1.0f, Color.Gray);

        GuiSpinner(new Rectangle(panelPos.X + 95, (float)panelPos.Y + 12, 125, 25), "Slices ", ref sliceCount, 1, MAX_PIE_SLICES);
        GuiCheckBox(new Rectangle(panelPos.X + 20, (float)panelPos.Y + 12 + 40, 20, 20), "Show Values", ref showValues);
        GuiCheckBox(new Rectangle(panelPos.X + 20, (float)panelPos.Y + 12 + 70, 20, 20), "Show Percentages", ref showPercentages);
        GuiCheckBox(new Rectangle(panelPos.X + 20, (float)panelPos.Y + 12 + 100, 20, 20), "Make Donut", ref showDonut);

        if (showDonut)
        {
            GuiDisable();
        }

        GuiSliderBar(new Rectangle(panelPos.X + 80, (float)panelPos.Y + 12 + 130, panelRect.Width - 100, 30),
                        "Inner Radius", null, ref donutInnerRadius, 5.0f, radius - 10.0f);
        GuiEnable();

        GuiLine(new Rectangle(panelPos.X + 10, (float)panelPos.Y + 12 + 170, panelRect.Width - 20, 1));

        // Scrollable area for slice editors
        float scrollTop = (float)panelPos.Y + 12 + 190;
        Rectangle scrollPanelBounds = new Rectangle(
            panelPos.X + panelMargin,
            scrollTop,
            panelRect.Width - panelMargin * 2,
            (panelRect.Y + panelRect.Height) - scrollTop - panelMargin);
        int contentHeight = sliceCount * 35;

        // Simple vertical scroll via mouse wheel while hovering the panel
        if (CheckCollisionPointRec(GetMousePosition(), scrollPanelBounds))
        {
            scrollContentOffset.Y += GetMouseWheelMove() * 20.0f;
            float minOffset = MathF.Min(0.0f, scrollPanelBounds.Height - contentHeight);
            if (scrollContentOffset.Y < minOffset)
            {
                scrollContentOffset.Y = minOffset;
            }

            if (scrollContentOffset.Y > 0.0f)
            {
                scrollContentOffset.Y = 0.0f;
            }
        }

        Rectangle view = scrollPanelBounds;
        float contentX = view.X + scrollContentOffset.X; // Left of content
        float contentY = view.Y + scrollContentOffset.Y; // Top of content

        BeginScissorMode((int)view.X, (int)view.Y, (int)view.Width, (int)view.Height);

        for (int i = 0; i < sliceCount; i++)
        {
            int rowY = (int)(contentY + 5 + i * 35);

            // Color indicator
            Color color = ColorFromHSV((float)i / sliceCount * 360.0f, 0.75f, 0.9f);
            DrawRectangle((int)(contentX + 15), rowY + 5, 20, 20, color);

            // Label textbox
            if (GuiTextBox(new Rectangle(contentX + 45, (float)rowY, 75, 30), labels[i], 32, editingLabel[i]))
            {
                editingLabel[i] = !editingLabel[i];
            }

            GuiSliderBar(new Rectangle(contentX + 130, (float)rowY, 110, 30), null, null, ref values[i], 0.0f, 1000.0f);
        }

        EndScissorMode();

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Minimal raygui-like widgets (plain raylib re-implementation)
    //----------------------------------------------------------------------------------
    private static void GuiDisable() => guiDisabled = true;
    private static void GuiEnable() => guiDisabled = false;

    private static void GuiLine(Rectangle bounds)
    {
        int y = (int)(bounds.Y + bounds.Height / 2);
        DrawLine((int)bounds.X, y, (int)(bounds.X + bounds.Width), y, Color.Gray);
    }

    private static void GuiCheckBox(Rectangle bounds, string text, ref bool active)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (!guiDisabled && hover && IsMouseButtonPressed(MouseButton.Left))
        {
            active = !active;
        }

        DrawRectangleLinesEx(bounds, 1, hover ? Color.DarkBlue : Color.Gray);
        if (active)
        {
            DrawRectangle((int)bounds.X + 4, (int)bounds.Y + 4, (int)bounds.Width - 8, (int)bounds.Height - 8, Color.DarkGray);
        }

        if (text != null)
        {
            DrawText(text, (int)(bounds.X + bounds.Width + 8), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
    }

    private static void GuiSpinner(Rectangle bounds, string text, ref int value, int minValue, int maxValue)
    {
        Vector2 mouse = GetMousePosition();
        Rectangle left = new Rectangle(bounds.X, bounds.Y, bounds.Height, bounds.Height);
        Rectangle right = new Rectangle(bounds.X + bounds.Width - bounds.Height, bounds.Y, bounds.Height, bounds.Height);
        Rectangle mid = new Rectangle(bounds.X + bounds.Height, bounds.Y, bounds.Width - 2 * bounds.Height, bounds.Height);

        if (!guiDisabled && CheckCollisionPointRec(mouse, left) && IsMouseButtonPressed(MouseButton.Left) && value > minValue)
        {
            value--;
        }

        if (!guiDisabled && CheckCollisionPointRec(mouse, right) && IsMouseButtonPressed(MouseButton.Left) && value < maxValue)
        {
            value++;
        }

        DrawRectangleRec(mid, Color.RayWhite);
        DrawRectangleLinesEx(mid, 1, Color.Gray);
        DrawRectangleRec(left, Color.LightGray);
        DrawRectangleLinesEx(left, 1, Color.Gray);
        DrawRectangleRec(right, Color.LightGray);
        DrawRectangleLinesEx(right, 1, Color.Gray);
        DrawText("-", (int)(left.X + left.Width / 2 - 2), (int)(left.Y + left.Height / 2 - 5), 10, Color.DarkGray);
        DrawText("+", (int)(right.X + right.Width / 2 - 3), (int)(right.Y + right.Height / 2 - 5), 10, Color.DarkGray);
        string vs = value.ToString();
        DrawText(vs, (int)(mid.X + mid.Width / 2 - MeasureText(vs, 10) / 2), (int)(mid.Y + mid.Height / 2 - 5), 10, Color.DarkGray);
        if (text != null)
        {
            DrawText(text, (int)bounds.X - MeasureText(text, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
    }

    private static void GuiSliderBar(Rectangle bounds, string textLeft, string textRight, ref float value, float minValue, float maxValue)
    {
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);
        if (!guiDisabled && hover && IsMouseButtonDown(MouseButton.Left))
        {
            value = minValue + ((mouse.X - bounds.X) / bounds.Width) * (maxValue - minValue);
            if (value < minValue)
            {
                value = minValue;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }
        }

        DrawRectangleRec(bounds, guiDisabled ? Color.Gray : Color.LightGray);
        float pct = (value - minValue) / (maxValue - minValue);
        DrawRectangleRec(new Rectangle(bounds.X, bounds.Y, bounds.Width * pct, bounds.Height), guiDisabled ? Color.DarkGray : Color.SkyBlue);
        DrawRectangleLinesEx(bounds, 1, Color.Gray);
        if (!string.IsNullOrEmpty(textLeft))
        {
            DrawText(textLeft, (int)bounds.X - MeasureText(textLeft, 10) - 5, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }

        if (!string.IsNullOrEmpty(textRight))
        {
            DrawText(textRight, (int)(bounds.X + bounds.Width + 5), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }
    }

    private bool GuiTextBox(Rectangle bounds, StringBuilder text, int maxChars, bool editMode)
    {
        bool pressed = false;
        Vector2 mouse = GetMousePosition();
        bool hover = CheckCollisionPointRec(mouse, bounds);

        if (editMode)
        {
            int key = GetCharPressed();
            while (key > 0)
            {
                if ((key >= 32) && (key <= 125) && (text.Length < maxChars - 1))
                {
                    text.Append((char)key);
                }

                key = GetCharPressed();
            }

            if (IsKeyPressed(KeyboardKey.Backspace) && (text.Length > 0))
            {
                text.Remove(text.Length - 1, 1);
            }
        }

        DrawRectangleRec(bounds, Color.RayWhite);
        DrawRectangleLinesEx(bounds, editMode ? 2 : 1, editMode ? Color.Red : (hover ? Color.Blue : Color.Gray));

        string content = text.ToString();
        DrawText(content, (int)bounds.X + 4, (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);

        if (editMode && ((framesCounter / 20) % 2 == 0))
        {
            DrawText("_", (int)bounds.X + 4 + MeasureText(content, 10), (int)(bounds.Y + bounds.Height / 2 - 5), 10, Color.DarkGray);
        }

        if (hover && IsMouseButtonPressed(MouseButton.Left))
        {
            pressed = true;
        }

        return pressed;
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [shapes] example - pie chart");

        SetTargetFPS(60);
        //--------------------------------------------------------------------------------------

        var game = new PieChart();
        game.Init();

        // Main game loop
        while (!WindowShouldClose())
        {
            game.Update();
        }

        game.Unload();

        // De-Initialization
        //--------------------------------------------------------------------------------------
        CloseWindow(); // Close window and OpenGL context
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
