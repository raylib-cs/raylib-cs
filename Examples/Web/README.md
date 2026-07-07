# Examples/Web — raylib-cs in the browser (WebAssembly)

Runs the raylib examples in the browser via WebAssembly, with a dropdown to switch between them.
It proves the `Raylib-cs` NuGet package's `browser-wasm` support end-to-end: the package's
`buildTransitive` targets link the shipped `raylib.a` into the .NET wasm runtime.

The browser-wasm configuration only activates when publishing with
`RuntimeIdentifier=browser-wasm`; normal solution builds don't need the `wasm-tools` workload.

## Prerequisites

- .NET 10 SDK
- `dotnet workload install wasm-tools`
- The `Raylib-cs` package matching `$(RaylibCsVersion)` (see `Directory.Build.props`) — from
  nuget.org, or built locally into the repo's `./nuget` feed (`dotnet pack Raylib-cs -c Release --output nuget`).

## Build

```bash
dotnet publish Examples -f net10.0 -r browser-wasm -c Release
# -> Examples/bin/Release/net10.0/browser-wasm/AppBundle/
```

### Toolchain caveat

If the link step fails with `wasm-opt: Unknown option '--enable-bulk-memory-opt'`, the installed
`wasm-tools` workload is out of sync with the SDK (stale workload band or a system EMSDK on
PATH); fix with `dotnet workload update`. As a safety net, `Examples.csproj` defaults
browser-wasm publishes to unoptimized native linking; an optimized publish can override those
properties.

## Run

WebAssembly must be served over HTTP (not `file://`):

```bash
dotnet serve -d Examples/bin/Release/net10.0/browser-wasm/AppBundle    # dotnet tool install -g dotnet-serve
# or:  npx http-server Examples/bin/Release/net10.0/browser-wasm/AppBundle
```

Open the printed URL and use the **Example** dropdown to switch examples.

## Canvas scaling modes

The render buffer stays fixed at `800x450`; display scaling is CSS-only. Pick with the **Scale**
dropdown (or `?scale=` query param):

- `native` (default): exact `800x450` CSS pixels.
- `integer`: largest whole-number multiple that fits, centered with letterboxing (pixel-perfect).
- `fit`: fills the viewport preserving aspect ratio (can be fractional, less crisp).

Scaling is computed in device pixels and converted back to CSS via `devicePixelRatio`, so the
modes behave consistently across OS scale and browser zoom. The status bar shows the live numbers
(`DPR`, CSS size, backing size, scale): `integer` should stay crisp at any OS scale or zoom, and
`native` should always report `Scale 1.00`.

## How it works

A browser can't run raylib's blocking `while (!WindowShouldClose())` loop (it would freeze the
page), so frames are driven from JavaScript:

- `Host.Main()` calls `InitWindow` once; `main.js` then populates the dropdown and selects the
  first example via `Host.SetExample`, so init failures surface in the on-page error banner.
- `main.js` binds the page `<canvas>` to the runtime and calls `Host.UpdateFrame()` from a
  `requestAnimationFrame` loop paced to the current example's `TargetFps` (raylib's own limiter
  busy-waits and would peg the main thread). The `Host` methods it calls are `[JSExport]`.
- On switch, `Host.SetExample` unloads the previous example, resets the cursor to visible, and
  applies the next `TargetFps`. Cursor hiding and `ConfigFlags` are not honored in the browser.
  If `Init` or `Update` throws, the example is unloaded and an error banner is shown.
- Each example is a single `.cs` file implementing `IExample` (`Init` / `Update` / `Unload`); the
  host owns the window, so examples never call `InitWindow`/`CloseWindow`. Platform differences
  (e.g. GLSL 100 vs 330) are handled inline with `#if BROWSER` guards, not separate files.

## Adding more examples

Examples are **auto-discovered by reflection** (`ExampleRegistry.DiscoverAll`) — no list to edit.
Drop a new `.cs` file in the matching category folder implementing `IExample`, splitting the
original monolithic `Main` as:

```
Main() { <setup>; while(!WindowShouldClose()){ <body> } <cleanup> }
  ->  Init()  { <setup, minus InitWindow/SetTargetFPS> }   // loop-spanning locals become fields
      Update(){ <body> }                                   // keep BeginDrawing..EndDrawing
      Unload(){ <cleanup, minus CloseWindow> }
```

Keep the standalone `static Main()` as a thin driver so the example still runs on its own.

If an example can't run on single-threaded wasm, add its type to `DesktopExcludedFromBrowser` in
`ExampleRegistry.cs`; `BrowserOnly` is the inverse list. Assets under `resources/` are bundled
into the wasm virtual filesystem automatically.
