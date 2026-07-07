# Examples/Web — raylib-cs in the browser (WebAssembly)

A small browser app that runs raylib examples in the browser via WebAssembly, with a dropdown to
switch between them. It exists to **prove the `Raylib-cs` NuGet package's `browser-wasm` support
works end-to-end**: the package's `buildTransitive/Raylib-cs.targets` automatically links the
shipped `runtimes/browser-wasm/native/raylib.a` (and adds `-sUSE_GLFW=3`) into the .NET wasm
runtime. The browser host lives in `Examples/Web`, while `Examples.csproj` remains the single
examples project.

The browser-wasm configuration is enabled only when publishing `Examples` with
`RuntimeIdentifier=browser-wasm`, so normal solution builds do not require the `wasm-tools`
workload.

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

If the link step fails with:

```
wasm-opt: Unknown option '--enable-bulk-memory-opt'
```

your installed `wasm-tools` workload (Binaryen) is out of sync with the SDK — emscripten passes a
feature flag the bundled `wasm-opt` doesn't understand (commonly caused by a stale workload band
or a conflicting system EMSDK on PATH/`$EMSDK`). Proper fix:

```bash
dotnet workload update
```

`Examples.csproj` defaults browser-wasm publishes to unoptimized native linking so the standard
publish command works on affected local toolchains. A fully optimized publish can override those
MSBuild properties after updating the workload/toolchain.

## Run

WebAssembly must be served over HTTP (not `file://`):

```bash
dotnet serve -d Examples/bin/Release/net10.0/browser-wasm/AppBundle    # dotnet tool install -g dotnet-serve
# or:  npx http-server Examples/bin/Release/net10.0/browser-wasm/AppBundle
```

Open the printed URL and use the **Example** dropdown to switch examples.

## Canvas scaling modes

The browser host keeps raylib's internal render buffer fixed to `800x450` and applies display
scaling in CSS. Use the **Scale** dropdown (or `?scale=` query param) to choose behavior:

- `native` (default): exact `800x450` CSS pixels (1x), no resizing.
- `integer`: largest whole-number multiple that fits the viewport, centered with letterboxing.
  Best for pixel-perfect presentation.
- `fit`: fills available viewport while preserving aspect ratio (can be fractional, less crisp on
  some DPI/zoom combinations).

Scaling is computed in **device pixels first** and then converted back to CSS pixels using current
`devicePixelRatio`, which decouples the modes from needing a specific browser zoom level on
125%/150% OS scale displays.

`main.js` also shows runtime diagnostics in the status bar (`DPR`, CSS size, backing size, scale)
to help debug OS scaling / browser zoom behavior across monitors.

### Manual validation matrix (expected behavior)

- OS scale `100%`, browser zoom `100%`: `integer` should appear crisp and stable (`Scale 1.00` or
  higher if viewport allows).
- OS scale `125%` and `150%`, browser zoom `100%`: `integer` should stay crisp (no subpixel CSS
  scaling); perceived physical size changes with OS scale are expected.
- Browser zoom `80%`, `100%`, `125%`: `integer` should continue using whole-number scaling;
  `fit` may show softening at non-integer effective scales.
- `native` mode should always report `CSS 800x450` and `Scale 1.00`.
- During window resize, there should be no frame-by-frame jitter because scale updates run on
  resize/mode changes, not per animation frame.

## How it works

A browser can't run raylib's blocking `while (!WindowShouldClose())` loop (it would freeze the
page), so frames are driven from JavaScript:

- `Host.Main()` calls `InitWindow` once and selects the default example (`main.js` runs it via
  `runMain()`).
- `main.js` binds the page `<canvas>` to the runtime, then calls `Host.UpdateFrame()` every
  `requestAnimationFrame` tick. `UpdateFrame`, `SetExample`, and `GetExampleNames` are `[JSExport]`.
- Each base example implements `IExample` (`Init` / `Update` / `Unload`) in `#if BROWSER` partials.
  The host owns the window;
  examples never call `InitWindow`/`CloseWindow`.

## Adding more examples

For a new desktop example (`../Core/...`, `../Shaders/...`, etc.), add a browser partial
(`*.Browser.cs`) that implements `IExample` and mirrors the split of its monolithic `Main`:

```
Main() { <setup>; while(!WindowShouldClose()){ <body> } <cleanup> }
  ->  Init()  { <setup, minus InitWindow/SetTargetFPS> }   // loop-spanning locals become fields
      Update(){ <body> }                                   // keep BeginDrawing..EndDrawing
      Unload(){ <cleanup, minus CloseWindow> }
```

Then add `new YourExample()` to the `Examples` list in `Host.cs`.

Examples that load files (`resources/...`) also need those assets in the wasm virtual filesystem.
`Examples.csproj` bundles `../resources/` into the browser app when publishing with
`RuntimeIdentifier=browser-wasm`.
