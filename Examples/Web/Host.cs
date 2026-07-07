using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace Examples.Web;

/// <summary>
/// Browser host: owns the single raylib window and the "current" example. The frame loop is
/// driven from JavaScript (main.js, via requestAnimationFrame) since a blocking C# loop would
/// freeze the page. Methods marked [JSExport] are called from main.js.
///
/// <para>
/// Every switch applies <see cref="IExample.TargetFps"/> and resets the cursor to visible.
/// Cursor hiding, <see cref="IExample.ConfigFlags"/>, and per-example window size are not
/// honored in the browser; the canvas stays 800x450 and the pointer stays visible.
/// </para>
/// </summary>
public partial class Host
{
    public const int screenWidth = 800;
    public const int screenHeight = 450;

    private static readonly List<IExample> _examples = new(ExampleRegistry.BrowserExamples);

    private static IExample _current;

    public static void Main()
    {
        InitWindow(screenWidth, screenHeight, "raylib-cs web examples");
        SetTargetFPS(60);

        // main.js selects the first example, so its init failures surface through SetExample.
    }

    /// <summary>Render one frame of the current example; false if it threw and was torn down.</summary>
    [JSExport]
    public static bool UpdateFrame()
    {
        if (_current == null)
        {
            return true;
        }

        try
        {
            _current.Update();
            return true;
        }
        catch (Exception ex)
        {
            // Don't let one misbehaving example kill the whole page; log and stop driving it.
            Console.WriteLine($"[Examples.Web] '{_current.Name}' threw during Update: {ex}");
            TryUnload(_current);
            _current = null;
            return false;
        }
    }

    /// <summary>Switch the active example by name; false on unknown name or failed Init.</summary>
    [JSExport]
    public static bool SetExample(string name)
    {
        var next = _examples.Find(e => e.Name == name);
        if (next == null)
        {
            return false;
        }

        if (_current != null)
        {
            TryUnload(_current);
            _current = null;
        }

        // Reset the baseline; an example may hide/lock the cursor from its own Update(), and
        // CursorDisabled/CursorHidden are deliberately never applied in the browser.
        EnableCursor();
        ShowCursor();
        SetTargetFPS(next.TargetFps);

        try
        {
            next.Init();
            _current = next;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Examples.Web] '{next.Name}' failed to initialize: {ex}");
            TryUnload(next);
            return false;
        }
    }

    /// <summary>Target FPS of the current example; main.js paces the frame loop with it.</summary>
    [JSExport]
    public static int GetCurrentTargetFps()
    {
        return _current?.TargetFps ?? 60;
    }

    /// <summary>Newline-separated example names, used to populate the nav dropdown.</summary>
    [JSExport]
    public static string GetExampleNames()
    {
        return string.Join("\n", _examples.ConvertAll(e => e.Name));
    }

    /// <summary>
    /// Map browser CSS mouse coordinates to the fixed 800x450 framebuffer when the canvas
    /// is CSS-scaled (integer/fit/native display modes in main.js).
    /// </summary>
    [JSExport]
    public static void SetMouseScaleFromDisplay(float scaleX, float scaleY)
    {
        SetMouseOffset(0, 0);
        SetMouseScale(scaleX, scaleY);
    }

    /// <summary>Best-effort Unload that never lets a failure cascade.</summary>
    private static void TryUnload(IExample example)
    {
        try
        {
            example.Unload();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Examples.Web] '{example.Name}' threw during Unload: {ex}");
        }
    }
}
