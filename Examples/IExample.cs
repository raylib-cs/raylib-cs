namespace Examples;

/// <summary>
/// A runnable raylib example. Each example class implements this interface directly: loop-spanning
/// state from the original example's Main lives in instance fields, (re)initialized in
/// <see cref="Init"/> so re-selecting an example resets it.
///
/// <para>
/// Desktop runs the example via its <c>static Main()</c>, a thin driver that owns the window
/// (InitWindow/CloseWindow, SetTargetFPS and friends) and drives <see cref="Init"/>,
/// <see cref="Update"/>, and <see cref="Unload"/> around a blocking
/// <c>while (!WindowShouldClose())</c> loop. In the browser, <c>Web/Host.cs</c> owns the single
/// window and calls <see cref="Update"/> one frame at a time from JavaScript, so examples never
/// block. Platform divergences (e.g. GLSL 100 vs 330 shaders) are guarded with <c>#if BROWSER</c>,
/// preferably around a single constant so both platforms share one code path.
/// </para>
/// </summary>
public interface IExample
{
    /// <summary>Display name shown in the navigation dropdown.</summary>
    string Name { get; }

    /// <summary>Window title, matching the example's standalone <c>Main()</c>.</summary>
    string Title { get; }

    /// <summary>Config flags the desktop runner applies before window creation.</summary>
    ConfigFlags ConfigFlags => 0;

    /// <summary>Target FPS the desktop runner sets after window creation.</summary>
    int TargetFps => 60;

    /// <summary>Whether the desktop runner disables the cursor (relative mouse movement).</summary>
    bool CursorDisabled => false;

    /// <summary>Whether the desktop runner hides the cursor.</summary>
    bool CursorHidden => false;

    /// <summary>One-time setup.</summary>
    void Init();

    /// <summary>Render one frame, including BeginDrawing/EndDrawing.</summary>
    void Update();

    /// <summary>Free resources.</summary>
    void Unload();
}
