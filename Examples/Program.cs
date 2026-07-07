#nullable enable
using System.Diagnostics;

namespace Examples;

internal static class Program
{
    private static unsafe void Main(string[] args)
    {
        Raylib.SetTraceLogCallback(&Logging.LogConsole);

        if (args.Length > 0)
        {
            var example = GetExample(args[0]);
            if (example == null)
            {
                Console.WriteLine($"Unknown example: {args[0]}");
                return;
            }

            RunExample(example);
            return;
        }

        foreach (var example in ExampleRegistry.DesktopExamples)
        {
            RunExampleProcess(Environment.ProcessPath!, example.GetType().Name);
        }
    }

    private static IExample? GetExample(string name)
    {
        return Array.Find(ExampleRegistry.DesktopExamples, x =>
            x.GetType().Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Drives an example the same way its standalone Main() does: window setup from the
    /// example's properties, then Init/Update/Unload around a blocking frame loop.
    /// </summary>
    private static void RunExample(IExample example)
    {
        if (example.ConfigFlags != 0)
        {
            SetConfigFlags(example.ConfigFlags);
        }

        InitWindow(example.Width, example.Height, example.Title);

        if (example.CursorDisabled)
        {
            DisableCursor();
        }
        else if (example.CursorHidden)
        {
            HideCursor();
        }

        SetTargetFPS(example.TargetFps);

        try
        {
            example.Init();

            while (!example.ShouldClose)
            {
                example.Update();
            }
        }
        finally
        {
            try
            {
                example.Unload();
            }
            finally
            {
                CloseWindow();
            }
        }
    }

    private static void RunExampleProcess(
        string fileName,
        string exampleName
    )
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = exampleName
        };
        using var process = Process.Start(processStartInfo);
        process?.WaitForExit();
    }
}
