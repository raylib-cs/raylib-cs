/*******************************************************************************************
*
*   raylib [text] example - strings management
*
*   Example complexity rating: [★★★☆] 3/4
*
*   Example originally created with raylib 6.0, last time updated with raylib 6.0
*
*   Example contributed by David Buzatto (@davidbuzatto) and reviewed by Ramon Santamaria (@raysan5)
*
*   Example licensed under an unmodified zlib/libpng license, which is an OSI-certified,
*   BSD-like license that allows static linking with closed source software
*
*   Copyright (c) 2025 David Buzatto (@davidbuzatto)
*
********************************************************************************************/

using System.Text;

namespace Examples.Text;

public partial class StringsManagement : IExample
{
    private const int screenWidth = 800;
    private const int screenHeight = 450;

    private const int MaxTextLength = 100;
    private const int MaxTextParticles = 100;
    private const int FontSize = 30;

    public string Name => "Text / Strings Management";

    public string Title => "raylib [text] example - strings management";

    //----------------------------------------------------------------------------------
    // Types and Structures Definition
    //----------------------------------------------------------------------------------
    private class TextParticle
    {
        public string Text;
        public Rectangle Rect;      // Boundary
        public Vector2 Vel;         // Velocity
        public Vector2 Ppos;        // Previous position
        public float Padding;
        public float BorderWidth;
        public float Friction;
        public float Elasticity;
        public Color Color;
        public bool Grabbed;
    }

    private List<TextParticle> textParticles;
    private TextParticle grabbedTextParticle;
    private Vector2 pressOffset;

    public void Init()
    {
        textParticles = new();
        grabbedTextParticle = null;
        pressOffset = new(0, 0);

        PrepareFirstTextParticle("raylib => fun videogames programming!");
    }

    public void Update()
    {
        // Update
        //----------------------------------------------------------------------------------
        float delta = GetFrameTime();
        Vector2 mousePos = GetMousePosition();

        // Checks if a text particle was grabbed
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            for (int i = textParticles.Count - 1; i >= 0; i--)
            {
                TextParticle tp = textParticles[i];
                if (CheckCollisionPointRec(mousePos, tp.Rect))
                {
                    pressOffset.X = mousePos.X - tp.Rect.X;
                    pressOffset.Y = mousePos.Y - tp.Rect.Y;
                    tp.Grabbed = true;
                    grabbedTextParticle = tp;
                    break;
                }
            }
        }

        // Releases any text particle the was grabbed
        if (IsMouseButtonReleased(MouseButton.Left))
        {
            if (grabbedTextParticle != null)
            {
                grabbedTextParticle.Grabbed = false;
                grabbedTextParticle = null;
            }
        }

        // Slice os shatter a text particle
        if (IsMouseButtonPressed(MouseButton.Right))
        {
            for (int i = textParticles.Count - 1; i >= 0; i--)
            {
                TextParticle tp = textParticles[i];
                if (CheckCollisionPointRec(mousePos, tp.Rect))
                {
                    if (IsKeyDown(KeyboardKey.LeftShift))
                    {
                        ShatterTextParticle(tp, i);
                    }
                    else
                    {
                        SliceTextParticle(tp, i, tp.Text.Length / 2);
                    }
                    break;
                }
            }
        }

        // Shake text particles
        if (IsMouseButtonPressed(MouseButton.Middle))
        {
            for (int i = 0; i < textParticles.Count; i++)
            {
                if (!textParticles[i].Grabbed)
                {
                    textParticles[i].Vel = new Vector2(GetRandomValue(-2000, 2000), GetRandomValue(-2000, 2000));
                }
            }
        }

        // Reset using TextTo* functions
        if (IsKeyPressed(KeyboardKey.One))
        {
            PrepareFirstTextParticle("raylib => fun videogames programming!");
        }

        if (IsKeyPressed(KeyboardKey.Two))
        {
            PrepareFirstTextParticle(TextToUpper("raylib => fun videogames programming!"));
        }

        if (IsKeyPressed(KeyboardKey.Three))
        {
            PrepareFirstTextParticle(TextToLower("raylib => fun videogames programming!"));
        }

        if (IsKeyPressed(KeyboardKey.Four))
        {
            PrepareFirstTextParticle(TextToPascal("raylib_fun_videogames_programming"));
        }

        if (IsKeyPressed(KeyboardKey.Five))
        {
            PrepareFirstTextParticle(TextToSnake("RaylibFunVideogamesProgramming"));
        }

        if (IsKeyPressed(KeyboardKey.Six))
        {
            PrepareFirstTextParticle(TextToCamel("raylib_fun_videogames_programming"));
        }

        // Slice by char pressed only when we have one text particle
        int charPressed = GetCharPressed();
        if ((charPressed >= 'A') && (charPressed <= 'z') && (textParticles.Count == 1))
        {
            SliceTextParticleByChar(textParticles[0], (char)charPressed);
        }

        // Updates each text particle state
        for (int i = 0; i < textParticles.Count; i++)
        {
            TextParticle tp = textParticles[i];

            // The text particle is not grabbed
            if (!tp.Grabbed)
            {
                // text particle repositioning using the velocity
                tp.Rect.X += tp.Vel.X * delta;
                tp.Rect.Y += tp.Vel.Y * delta;

                // Does the text particle hit the screen right boundary?
                if ((tp.Rect.X + tp.Rect.Width) >= screenWidth)
                {
                    tp.Rect.X = screenWidth - tp.Rect.Width; // Text particle repositioning
                    tp.Vel.X = -tp.Vel.X * tp.Elasticity;  // Elasticity makes the text particle lose 10% of its velocity on hit
                }
                // Does the text particle hit the screen left boundary?
                else if (tp.Rect.X <= 0)
                {
                    tp.Rect.X = 0.0f;
                    tp.Vel.X = -tp.Vel.X * tp.Elasticity;
                }

                // The same for y axis
                if ((tp.Rect.Y + tp.Rect.Height) >= screenHeight)
                {
                    tp.Rect.Y = screenHeight - tp.Rect.Height;
                    tp.Vel.Y = -tp.Vel.Y * tp.Elasticity;
                }
                else if (tp.Rect.Y <= 0)
                {
                    tp.Rect.Y = 0.0f;
                    tp.Vel.Y = -tp.Vel.Y * tp.Elasticity;
                }

                // Friction makes the text particle lose 1% of its velocity each frame
                tp.Vel.X = tp.Vel.X * tp.Friction;
                tp.Vel.Y = tp.Vel.Y * tp.Friction;
            }
            else
            {
                // Text particle repositioning using the mouse position
                tp.Rect.X = mousePos.X - pressOffset.X;
                tp.Rect.Y = mousePos.Y - pressOffset.Y;

                // While the text particle is grabbed, recalculates its velocity
                tp.Vel.X = (tp.Rect.X - tp.Ppos.X) / delta;
                tp.Vel.Y = (tp.Rect.Y - tp.Ppos.Y) / delta;
                tp.Ppos.X = tp.Rect.X;
                tp.Ppos.Y = tp.Rect.Y;

                // Glue text particles when dragging and pressing left ctrl
                if (IsKeyDown(KeyboardKey.LeftControl))
                {
                    for (int j = 0; j < textParticles.Count; j++)
                    {
                        if (textParticles[j] != grabbedTextParticle && grabbedTextParticle.Grabbed)
                        {
                            if (CheckCollisionRecs(grabbedTextParticle.Rect, textParticles[j].Rect))
                            {
                                GlueTextParticles(grabbedTextParticle, textParticles[j]);
                                grabbedTextParticle = textParticles[textParticles.Count - 1];
                            }
                        }
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

        ClearBackground(Color.RayWhite);

        for (int i = 0; i < textParticles.Count; i++)
        {
            TextParticle tp = textParticles[i];
            DrawRectangleRec(new Rectangle(tp.Rect.X - tp.BorderWidth, tp.Rect.Y - tp.BorderWidth, tp.Rect.Width + tp.BorderWidth * 2, tp.Rect.Height + tp.BorderWidth * 2), Color.Black);
            DrawRectangleRec(tp.Rect, tp.Color);
            DrawText(tp.Text, (int)(tp.Rect.X + tp.Padding), (int)(tp.Rect.Y + tp.Padding), FontSize, Color.Black);
        }

        DrawText("grab a text particle by pressing with the mouse and throw it by releasing", 10, 10, 10, Color.DarkGray);
        DrawText("slice a text particle by pressing it with the mouse right button", 10, 30, 10, Color.DarkGray);
        DrawText("shatter a text particle keeping left shift pressed and pressing it with the mouse right button", 10, 50, 10, Color.DarkGray);
        DrawText("glue text particles by grabbing than and keeping left control pressed", 10, 70, 10, Color.DarkGray);
        DrawText("1 to 6 to reset", 10, 90, 10, Color.DarkGray);
        DrawText("when you have only one text particle, you can slice it by pressing a char", 10, 110, 10, Color.DarkGray);
        DrawText($"TEXT PARTICLE COUNT: {textParticles.Count}", 10, GetScreenHeight() - 30, 20, Color.Black);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public void Unload()
    {
    }

    //----------------------------------------------------------------------------------
    // Module Functions Definition
    //----------------------------------------------------------------------------------
    private void PrepareFirstTextParticle(string text)
    {
        TextParticle first = CreateTextParticle(
            text,
            GetScreenWidth() / 2.0f,
            GetScreenHeight() / 2.0f,
            Color.RayWhite
        );

        textParticles.Clear();
        textParticles.Add(first);
    }

    private static TextParticle CreateTextParticle(string text, float x, float y, Color color)
    {
        TextParticle tp = new()
        {
            Text = "",
            Rect = new Rectangle(x, y, 30, 30),
            Vel = new Vector2(GetRandomValue(-200, 200), GetRandomValue(-200, 200)),
            Ppos = new Vector2(0, 0),
            Padding = 5.0f,
            BorderWidth = 5.0f,
            Friction = 0.99f,
            Elasticity = 0.9f,
            Color = color,
            Grabbed = false
        };

        // Emulate C TextCopy() into a fixed size buffer
        if (text.Length > MaxTextLength - 1)
        {
            text = text.Substring(0, MaxTextLength - 1);
        }
        tp.Text = text;

        tp.Rect.Width = MeasureText(tp.Text, FontSize) + tp.Padding * 2;
        tp.Rect.Height = FontSize + tp.Padding * 2;
        return tp;
    }

    private void SliceTextParticle(TextParticle tp, int particlePos, int sliceLength)
    {
        int length = tp.Text.Length;

        if ((length > 1) && ((textParticles.Count + length) < MaxTextParticles))
        {
            for (int i = 0; i < length; i += sliceLength)
            {
                string text = sliceLength == 1 ? tp.Text[i].ToString() : Subtext(tp.Text, i, sliceLength);
                textParticles.Add(CreateTextParticle(
                    text,
                    tp.Rect.X + i * tp.Rect.Width / length,
                    tp.Rect.Y,
                    new Color(GetRandomValue(0, 255), GetRandomValue(0, 255), GetRandomValue(0, 255), 255)
                ));
            }
            RealocateTextParticles(particlePos);
        }
    }

    private void SliceTextParticleByChar(TextParticle tp, char charToSlice)
    {
        string[] tokens = tp.Text.Split(charToSlice);
        int tokenCount = tokens.Length;

        if (tokenCount > 1)
        {
            int textLength = tp.Text.Length;
            for (int i = 0; i < textLength; i++)
            {
                if (tp.Text[i] == charToSlice)
                {
                    textParticles.Add(CreateTextParticle(
                        charToSlice.ToString(),
                        tp.Rect.X,
                        tp.Rect.Y,
                        new Color(GetRandomValue(0, 255), GetRandomValue(0, 255), GetRandomValue(0, 255), 255)
                    ));
                }
            }
            for (int i = 0; i < tokenCount; i++)
            {
                int tokenLength = tokens[i].Length;
                textParticles.Add(CreateTextParticle(
                    tokens[i],
                    tp.Rect.X + i * tp.Rect.Width / tokenLength,
                    tp.Rect.Y,
                    new Color(GetRandomValue(0, 255), GetRandomValue(0, 255), GetRandomValue(0, 255), 255)
                ));
            }
            RealocateTextParticles(0);
        }
    }

    private void ShatterTextParticle(TextParticle tp, int particlePos)
    {
        SliceTextParticle(tp, particlePos, 1);
    }

    private void GlueTextParticles(TextParticle grabbed, TextParticle target)
    {
        int p1 = textParticles.IndexOf(grabbed);
        int p2 = textParticles.IndexOf(target);

        if ((p1 != -1) && (p2 != -1))
        {
            TextParticle tp = CreateTextParticle(
                grabbed.Text + target.Text,
                grabbed.Rect.X,
                grabbed.Rect.Y,
                Color.RayWhite
            );
            tp.Grabbed = true;
            textParticles.Add(tp);
            grabbed.Grabbed = false;
            if (p1 < p2)
            {
                RealocateTextParticles(p2);
                RealocateTextParticles(p1);
            }
            else
            {
                RealocateTextParticles(p1);
                RealocateTextParticles(p2);
            }
        }
    }

    private void RealocateTextParticles(int particlePos)
    {
        textParticles.RemoveAt(particlePos);
    }

    // Extract a substring, clamping length to the available characters (like raylib TextSubtext)
    private static string Subtext(string text, int position, int length)
    {
        if (position >= text.Length)
        {
            return "";
        }

        int maxLength = text.Length - position;
        if (length > maxLength)
        {
            length = maxLength;
        }

        return text.Substring(position, length);
    }

    // C# equivalents of raylib TextTo* helpers (behaviour kept identical)
    private static string TextToUpper(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            sb.Append((c >= 'a' && c <= 'z') ? (char)(c - 32) : c);
        }
        return sb.ToString();
    }

    private static string TextToLower(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            sb.Append((c >= 'A' && c <= 'Z') ? (char)(c + 32) : c);
        }
        return sb.ToString();
    }

    private static string TextToPascal(string text)
    {
        var sb = new StringBuilder(text.Length);

        if (text.Length > 0)
        {
            sb.Append(char.ToUpperInvariant(text[0]));

            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == '_' && (i + 1) < text.Length)
                {
                    sb.Append(char.ToUpperInvariant(text[i + 1]));
                    i++;
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
        }

        return sb.ToString();
    }

    private static string TextToSnake(string text)
    {
        var sb = new StringBuilder(text.Length);

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c >= 'A' && c <= 'Z')
            {
                if (i > 0)
                {
                    sb.Append('_');
                }
                sb.Append((char)(c + 32));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    private static string TextToCamel(string text)
    {
        var sb = new StringBuilder(text.Length);

        if (text.Length > 0)
        {
            sb.Append(char.ToLowerInvariant(text[0]));

            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == '_' && (i + 1) < text.Length)
                {
                    sb.Append(char.ToUpperInvariant(text[i + 1]));
                    i++;
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
        }

        return sb.ToString();
    }

    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        InitWindow(screenWidth, screenHeight, "raylib [text] example - strings management");

        SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
        //--------------------------------------------------------------------------------------

        var game = new StringsManagement();
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
