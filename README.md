![Raylib-cs Logo](Raylib-cs/logo/raylib-cs_256x256.png "Raylib-cs Logo")

# Raylib-cs

C# bindings for raylib, a simple and easy-to-use library to learn video games programming (www.raylib.com)

[![GitHub contributors](https://img.shields.io/github/contributors/raylib-cs/raylib-cs)](https://github.com/raylib-cs/raylib-cs/graphs/contributors)
[![License](https://img.shields.io/badge/license-zlib%2Flibpng-blue.svg)](LICENSE)
[![Chat on Discord](https://img.shields.io/discord/426912293134270465.svg?logo=discord)](https://discord.gg/raylib)
[![GitHub stars](https://img.shields.io/github/stars/raylib-cs/raylib-cs?style=social)](https://github.com/raylib-cs/raylib-cs/stargazers)
[![Build](https://github.com/raylib-cs/raylib-cs/workflows/Build/badge.svg)](https://github.com/raylib-cs/raylib-cs/actions?query=workflow%3ABuild)

Raylib-cs targets net8.0, net10.0 and uses the [official 6.0 release](https://github.com/raysan5/raylib/releases/tag/6.0)
to build the native libraries.

### Version 8.1.0 of Raylib-cs supports the following runtimes:
- **browser-wasm** *(raylib-6.0_webassembly)* 🆕
- **linux-x64** *(raylib-6.0_linux_amd64)*
- **osx-arm64** *(raylib-6.0_macos)*
- **osx-x64** *(raylib-6.0_macos)*
- **win-x64** *(raylib-6.0_win64_msvc16)*
- **win-x86** *(raylib-6.0_win32_msvc16)*

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

3. Download/build the native libraries for the platforms you want using the [official 6.0 release](https://github.com/raysan5/raylib/releases/tag/6.0). **NOTE: the MSVC version is required for Windows platforms**

4. Set up the native libraries, so they are in the same directory as the executable/can be found in the [search path](https://www.mono-project.com/docs/advanced/pinvoke/).

5. Start coding!

## Building from source

The `Examples` and `Raylib-cs.Tests` projects consume the binding as a NuGet package, and the in-repo
version may not be published on nuget.org yet. Pack it once into the local feed before the first build:

```
dotnet pack Raylib-cs -c Release -o nuget
dotnet build
```

`NuGet.config` points restore at the local `./nuget` feed, so both projects pick up the freshly packed
package. If you skip the pack step on a fresh clone, restore fails with NU1301 (the `./nuget` source
doesn't exist) or NU1102 (Raylib-cs not found) — running the pack command fixes both.

When iterating on the binding itself, note that NuGet caches the extracted package by version and
ignores repacks of the same version. After repacking, delete the old package from `./nuget` and clear
the cached copy (`dotnet nuget locals global-packages --clear`, or delete
`~/.nuget/packages/raylib-cs/<version>`) before restoring again.

## Hello, World!

```csharp
using Raylib_cs;

namespace HelloWorld;

internal static class Program
{
    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
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

## Building the examples locally

The `Examples` project references the sibling `Raylib-cs` project by default, so local changes to the
bindings are picked up directly:

```
dotnet run --project Examples
```

To build the examples against the Raylib-cs NuGet package instead (the version set by
`RaylibCsVersion` in [Directory.Build.props](Directory.Build.props)), set `UseRaylibCsPackage`:

```
dotnet run --project Examples -p:UseRaylibCsPackage=true
```

## Contributing

Feel free to open an issue. If you'd like to contribute, please fork the repository and make
changes as you'd like. Pull requests are welcome.

If you want to request features or report bugs related to raylib directly (in contrast to this binding), please refer to the [author's project repo](https://github.com/raysan5/raylib).

## License

See [LICENSE](LICENSE) for details.
