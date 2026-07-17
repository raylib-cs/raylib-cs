using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace Examples.Web;

/// <summary>
/// Browser host: owns the single raylib window and the "current" example. The frame loop is
/// driven from JavaScript (main.js, via requestAnimationFrame) since a blocking C# loop would
/// freeze the page. Methods marked [JSExport] are called from main.js.
/// </summary>
public partial class Host
{
    public const int screenWidth = 800;
    public const int screenHeight = 450;

    // Examples discovered from the assembly, minus desktop-only ones (see ExampleRegistry).
    private static readonly List<IExample> _examples = new(ExampleRegistry.BrowserExamples);

    private static IExample _current;

    public static void Main()
    {
        InitWindow(screenWidth, screenHeight, "raylib-cs web examples");
        SetTargetFPS(60);

        SetExample(_examples[0].Name);
    }

    /// <summary>Render one frame of the current example (called every requestAnimationFrame tick).</summary>
    [JSExport]
    public static void UpdateFrame()
    {
        try
        {
            _current?.Update();
        }
        catch (Exception ex)
        {
            // Don't let one misbehaving example kill the whole page; log and stop driving it.
            Console.WriteLine($"[Examples.Web] '{_current?.Name}' threw during Update: {ex}");
            _current = null;
        }
    }

    /// <summary>Switch the active example by name (called from the nav dropdown).</summary>
    [JSExport]
    public static void SetExample(string name)
    {
        var next = _examples.Find(e => e.Name == name);
        if (next == null)
        {
            return;
        }

        try
        {
            _current?.Unload();
            _current = next;
            _current.Init();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Examples.Web] '{next.Name}' failed to initialize: {ex}");
            _current = null;
        }
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
}
