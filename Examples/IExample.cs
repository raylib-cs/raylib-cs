namespace Examples;

/// <summary>
/// A runnable raylib example. Loop-spanning state lives in instance fields, (re)initialized in
/// <see cref="Init"/> so re-selecting an example resets it.
///
/// <para>
/// Desktop drives examples via <c>Program.cs</c> (each also keeps a thin standalone
/// <c>static Main()</c>); in the browser, <c>Web/Host.cs</c> owns the single window and calls
/// <see cref="Update"/> one frame at a time from JavaScript. Platform divergences are guarded
/// with <c>#if BROWSER</c>, preferably around a single constant.
/// </para>
/// </summary>
public interface IExample
{
    /// <summary>Display name shown in the navigation dropdown.</summary>
    string Name
    {
        get;
    }

    /// <summary>Window title, matching the example's standalone <c>Main()</c>.</summary>
    string Title
    {
        get;
    }

    /// <summary>Desktop window size, matching the standalone <c>Main()</c>. The browser canvas is fixed at 800x450.</summary>
    int Width => 800;

    /// <inheritdoc cref="Width"/>
    int Height => 450;

    /// <summary>Config flags the desktop runner applies before window creation. Ignored in the browser.</summary>
    ConfigFlags ConfigFlags => 0;

    /// <summary>Target FPS. The desktop runner always applies it; the browser host paces its frame loop with it.</summary>
    int TargetFps => 60;

    /// <summary>Whether the desktop runner disables the cursor. Ignored in the browser (the pointer stays visible).</summary>
    bool CursorDisabled => false;

    /// <summary>Whether the desktop runner hides the cursor. Ignored in the browser.</summary>
    bool CursorHidden => false;

    /// <summary>
    /// Whether the desktop runner should exit its frame loop; defaults to
    /// <see cref="Raylib.WindowShouldClose"/>. Examples that intercept the close request
    /// override this and poll WindowShouldClose in <see cref="Update"/> themselves.
    /// </summary>
    bool ShouldClose => WindowShouldClose();

    /// <summary>Set up the Example and preload necessary resources.</summary>
    void Init();

    /// <summary>Render one frame, including BeginDrawing/EndDrawing.</summary>
    void Update();

    /// <summary>Free resources.</summary>
    void Unload();
}
