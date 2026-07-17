using System.Diagnostics.CodeAnalysis;
using Examples.Core;
using Examples.Models;
using Examples.Shapes;

namespace Examples;

/// <summary>
/// Discovers every <see cref="IExample"/> implementation in this assembly via reflection and
/// derives the per-platform lists: <see cref="DesktopExamples"/> (Program.cs) and
/// <see cref="BrowserExamples"/> (Web/Host.cs). Ordering is by category (browser dropdown
/// grouping), then display name.
/// </summary>
public static class ExampleRegistry
{
    /// <summary>Category order used for the browser dropdown and desktop run-all sequence.</summary>
    private static readonly string[] CategoryOrder =
    [
        "Core",
        "Shapes",
        "Models",
        "Textures",
        "Text",
        "Audio",
        "Shaders",
    ];

    /// <summary>Desktop examples omitted from the browser host (platform limitations).</summary>
    private static readonly Type[] DesktopExcludedFromBrowser =
    [
        typeof(DropFiles),
        typeof(LoadingThread),  // System.Threading.Thread is unsupported on single-threaded wasm
        typeof(SkyboxDemo),
    ];

    /// <summary>Browser-only shape examples not registered for desktop CLI runs.</summary>
    private static readonly Type[] BrowserOnly =
    [
        typeof(DrawCircleSector),
        typeof(DrawRectangleRounded),
        typeof(DrawRing),
    ];

    private static readonly IExample[] AllExamples = DiscoverAll();

    public static readonly IExample[] DesktopExamples =
        Array.FindAll(AllExamples, e => Array.IndexOf(BrowserOnly, e.GetType()) < 0);

    public static readonly IExample[] BrowserExamples =
        Array.FindAll(AllExamples, e => Array.IndexOf(DesktopExcludedFromBrowser, e.GetType()) < 0);

    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "The Examples assembly is rooted via TrimmerRootAssembly in Examples.csproj.")]
    [UnconditionalSuppressMessage("Trimming", "IL2067",
        Justification = "Same as IL2026: all example types and their parameterless constructors are rooted.")]
    private static IExample[] DiscoverAll()
    {
        return typeof(IExample).Assembly
            .GetTypes()
            .Where(t => typeof(IExample).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(t => (IExample)Activator.CreateInstance(t))
            .OrderBy(e => Array.IndexOf(CategoryOrder, Category(e)))
            .ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    // "Examples.Core" -> "Core"
    private static string Category(IExample example)
    {
        var ns = example.GetType().Namespace ?? "";
        return ns[(ns.LastIndexOf('.') + 1)..];
    }
}
