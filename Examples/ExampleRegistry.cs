using System.Diagnostics.CodeAnalysis;

namespace Examples;

/// <summary>
/// Discovers every <see cref="IExample"/> implementation in this assembly via reflection:
/// <see cref="DesktopExamples"/> (Program.cs) is all of them, <see cref="BrowserExamples"/>
/// (Web/Host.cs) is everything not marked <see cref="ExcludeFromBrowserAttribute"/>.
/// Ordering is by category (browser dropdown grouping), then display name.
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

    private static readonly IExample[] AllExamples = DiscoverAll();

    public static readonly IExample[] DesktopExamples = AllExamples;

    public static readonly IExample[] BrowserExamples =
        Array.FindAll(AllExamples, e => !e.GetType().IsDefined(typeof(ExcludeFromBrowserAttribute), inherit: false));

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
