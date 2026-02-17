![Raylib-cs Logo](Raylib-cs/logo/raylib-cs_256x256.png "Raylib-cs Logo")

# Raylib-cs

C# bindings for raylib, a simple and easy-to-use library to learn video games programming (www.raylib.com)

[![GitHub contributors](https://img.shields.io/github/contributors/raylib-cs/raylib-cs)](https://github.com/raylib-cs/raylib-cs/graphs/contributors)
[![License](https://img.shields.io/badge/license-zlib%2Flibpng-blue.svg)](LICENSE)
[![Chat on Discord](https://img.shields.io/discord/426912293134270465.svg?logo=discord)](https://discord.gg/raylib)
[![GitHub stars](https://img.shields.io/github/stars/raylib-cs/raylib-cs?style=social)](https://github.com/raylib-cs/raylib-cs/stargazers)
[![Build](https://github.com/raylib-cs/raylib-cs/workflows/Build/badge.svg)](https://github.com/raylib-cs/raylib-cs/actions?query=workflow%3ABuild)

Raylib-cs targets net8.0, net9.0, net10.0 and uses the [official 5.5 release](https://github.com/raysan5/raylib/releases/tag/5.5)
binary distribution of raylib.

### Version 8.0.0 of Raylib-cs supports the following runtimes:
- **browser-wasm** *(raylib-5.5_webassembly)* 🆕
- **linux-x64** *(raylib-5.5_linux_amd64)*
- **osx-arm64** *(raylib-5.5_macos)* 🆕
- **osx-x64** *(raylib-5.5_macos)*
- **win-x64** *(raylib-5.5_win64_msvc16)*
- **win-x86** *(raylib-5.5_win32_msvc16)*

The following frameworks are deprecated (fall back to Version 7.x if required):
- net6.0 (end of life)


## Status

Raylib-cs is passively maintained. Occasional updates may be released from time to time. Pull requests may be
accepted if they don't have a large maintenance burden.

## Installation - NuGet

This is the preferred method to get started.

1) Pick a folder in which you would like to start a raylib project. For example, "MyRaylibCSProj."
2) Then from a terminal (for example, a VSCode terminal), whilst in the directory you just created
    run the following commands. (Please keep in mind .NET should already be installed on your system)

```
dotnet new console
```
```
dotnet add package Raylib-cs
```

[![NuGet](https://img.shields.io/nuget/dt/raylib-cs)](https://www.nuget.org/packages/Raylib-cs/)

If you need to edit the Raylib-cs source, then you will need to add the bindings as a project (see below).

If you are new to using NuGet (or you've forgotten) and are trying to run the above command in the command prompt,
remember that you need to be *inside the intended project directory* (not just inside the solution directory); 
otherwise the command won't work.

## Installation - Manual

1. Download/clone the repo

2. Add [Raylib-cs/Raylib-cs.csproj](Raylib-cs/Raylib-cs.csproj) to your project as an existing project.

3. Download/build the native libraries for the platforms you want using the [official 5.5 release](https://github.com/raysan5/raylib/releases/tag/5.5).
   **NOTE: the MSVC version is required for Windows platforms**

4. Set up the native libraries, so they are in the same directory as the executable/can be found in the [search path](https://www.mono-project.com/docs/advanced/pinvoke/).

5. Start coding!

## Hello, World!

```csharp
using Raylib_cs;

namespace HelloWorld;

internal static class Program
{
    // STAThread is required if you deploy using NativeAOT on Windows - See https://github.com/raylib-cs/raylib-cs/issues/301
    [System.STAThread]
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "Hello World");

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
```

## Contributing

Feel free to open an issue. If you'd like to contribute, please fork the repository and make
changes as you'd like. Pull requests are welcome.

If you want to request features or report bugs related to raylib directly (in contrast to this binding), please refer to the [author's project repo](https://github.com/raysan5/raylib).

## License

See [LICENSE](LICENSE) for details.
