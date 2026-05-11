using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Reflection;
using System.Security;
using System.IO;

[assembly: DisableRuntimeMarshalling]

namespace Raylib_cs;

[SuppressUnmanagedCodeSecurity]
public static unsafe partial class Raylib
{
    /// <summary>
    /// Used by LibraryImport to load the native library
    /// </summary>
    public const string NativeLibName = "raylib";

    public const string RAYLIB_VERSION = "5.5";

    public const float DEG2RAD = MathF.PI / 180.0f;
    public const float RAD2DEG = 180.0f / MathF.PI;

    static Raylib()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), (libraryName, assembly, searchPath) =>
        {
            IntPtr handle = IntPtr.Zero;
            string libraryPath = GetLibraryPath(libraryName);

            if (NativeLibrary.TryLoad(libraryPath, out handle))
            {
                return handle;
            }

            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle))
            {
                return handle;
            }

            throw new DllNotFoundException(
                $"Failed to load {libraryName} using {libraryPath}."
            );
        });
    }

    public static string GetLibraryPath(string libraryName)
    {
        string appBase = AppContext.BaseDirectory;
        string rid = GetNormalizedRuntimeIdentifier();

        string path = Path.Combine(appBase, "runtimes", rid, "native");

        string fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"{libraryName}.dll"
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? $"lib{libraryName}.dylib"
                : $"lib{libraryName}.so";

        return Path.Combine(path, fileName);
    }

    /// <summary>
    /// Gets a normalized runtime identifier that's consistent across different installation methods.
    /// https://github.com/dotnet/runtime/issues/114156#issuecomment-2773234611
    /// </summary>
    public static string GetNormalizedRuntimeIdentifier()
    {
        string rid = RuntimeInformation.RuntimeIdentifier;

        // If already in the expected format, return as is
        if (rid == "win-x64" || rid == "win-arm64" ||
            rid == "linux-x64" || rid == "linux-arm64" ||
            rid == "linux-musl-x64" || rid == "linux-musl-arm64" ||
            rid == "osx-x64" || rid == "osx-arm64")
        {
            return rid;
        }

        // Handle OS-specific RIDs from native repositories

        // Extract architecture (should be the part after the last dash)
        string architecture = "x64"; // Default
        if (rid.Contains("-"))
        {
            architecture = rid.Substring(rid.LastIndexOf('-') + 1);
        }

        // Determine OS and variant
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return $"win-{architecture}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Check if it's Alpine Linux (musl-based)
            if (rid.Contains("alpine") || IsAlpineLinux())
            {
                return $"linux-musl-{architecture}";
            }
            return $"linux-{architecture}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return $"osx-{architecture}";
        }

        // Fallback to the original RID if we can't normalize it
        return rid;
    }

    /// <summary>
    /// Checks if the current Linux distribution is Alpine (musl-based).
    /// </summary>
    private static bool IsAlpineLinux()
    {
        try
        {
            // Check for /etc/os-release file which contains distribution info
            if (File.Exists("/etc/os-release"))
            {
                string content = File.ReadAllText("/etc/os-release");
                return content.Contains("ID=alpine") || content.Contains("ID=\"alpine\"");
            }

            // Alternative check for /etc/alpine-release
            return File.Exists("/etc/alpine-release");
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get color with alpha applied, alpha goes from 0.0f to 1.0f<br/>
    /// NOTE: Added for compatability with previous versions
    /// </summary>
    public static Color Fade(Color color, float alpha) => ColorAlpha(color, alpha);

    //------------------------------------------------------------------------------------
    // Window and Graphics Device Functions (Module: core)
    //------------------------------------------------------------------------------------

    // Window-related functions

    /// <summary>Initialize window and OpenGL context</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void InitWindow(int width, int height, sbyte* title);

    /// <summary>Check if KEY_ESCAPE pressed or Close icon pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool WindowShouldClose();

    /// <summary>Close window and unload OpenGL context</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CloseWindow();

    /// <summary>Check if window has been initialized successfully</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowReady();

    /// <summary>Check if window is currently fullscreen</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowFullscreen();

    /// <summary>Check if window is currently hidden</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowHidden();

    /// <summary>Check if window is currently minimized</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowMinimized();

    /// <summary>Check if window is currently maximized</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowMaximized();

    /// <summary>Check if window is currently focused</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowFocused();

    /// <summary>Check if window has been resized last frame</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowResized();

    /// <summary>Check if one specific window flag is enabled</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWindowState(ConfigFlags flag);

    /// <summary>Set window configuration state using flags</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool SetWindowState(ConfigFlags flag);

    /// <summary>Clear window configuration state flags</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ClearWindowState(ConfigFlags flag);

    /// <summary>Toggle window state: fullscreen/windowed, resizes monitor to match window resolution</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ToggleFullscreen();

    /// <summary>Toggle window state: borderless windowed, resizes window to match monitor resolution</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ToggleBorderlessWindowed();

    /// <summary>Set window state: maximized, if resizable</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void MaximizeWindow();

    /// <summary>Set window state: minimized, if resizable</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void MinimizeWindow();

    /// <summary>Set window state: not minimized/maximized</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void RestoreWindow();

    /// <summary>Set icon for window (single image, RGBA 32bit)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowIcon(Image image);

    /// <summary>Set icon for window (multiple images, RGBA 32bit)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowIcons(Image* images, int count);

    /// <summary>Set title for window</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowTitle(sbyte* title);

    /// <summary>Set window position on screen</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowPosition(int x, int y);

    /// <summary>Set monitor for the current window (fullscreen mode)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowMonitor(int monitor);

    /// <summary>Set window minimum dimensions (for FLAG_WINDOW_RESIZABLE)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowMinSize(int width, int height);

    /// <summary>Set window maximum dimensions (for FLAG_WINDOW_RESIZABLE)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowMaxSize(int width, int height);

    /// <summary>Set window dimensions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowSize(int width, int height);

    /// <summary>Set window opacity [0.0f..1.0f]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowOpacity(float opacity);

    /// <summary>Set window focused</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetWindowFocused();

    /// <summary>Get native window handle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void* GetWindowHandle();

    /// <summary>Get current screen width</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetScreenWidth();

    /// <summary>Get current screen height</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetScreenHeight();

    /// <summary>Get current render width (it considers HiDPI)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetRenderWidth();

    /// <summary>Get current render height (it considers HiDPI)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetRenderHeight();

    /// <summary>Get number of connected monitors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMonitorCount();

    /// <summary>Get current monitor where window is placed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetCurrentMonitor();

    /// <summary>Get specified monitor position</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetMonitorPosition(int monitor);

    /// <summary>Get specified monitor width</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMonitorWidth(int monitor);

    /// <summary>Get specified monitor height</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMonitorHeight(int monitor);

    /// <summary>Get specified monitor physical width in millimetres</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMonitorPhysicalWidth(int monitor);

    /// <summary>Get specified monitor physical height in millimetres</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMonitorPhysicalHeight(int monitor);

    /// <summary>Get specified monitor refresh rate</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMonitorRefreshRate(int monitor);

    /// <summary>Get window position XY on monitor</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetWindowPosition();

    /// <summary>Get window scale DPI factor</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetWindowScaleDPI();

    /// <summary>Get the human-readable, UTF-8 encoded name of the specified monitor</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetMonitorName(int monitor);

    /// <summary>Get clipboard text content</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetClipboardText();

    /// <summary>Get clipboard image content (only works on Windows)</summary>
    [UnsupportedOSPlatform("browser")]
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GetClipboardImage();

    /// <summary>Set clipboard text content</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetClipboardText(sbyte* text);

    /// <summary>Enable waiting for events on EndDrawing(), no automatic event polling</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EnableEventWaiting();

    /// <summary>Disable waiting for events on EndDrawing(), automatic events polling</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DisableEventWaiting();

    // Custom frame control functions
    // NOTE: Those functions are intended for advance users that want full control over the frame processing
    // By default EndDrawing() does this job: draws everything + SwapScreenBuffer() + manage frame timming + PollInputEvents()
    // To avoid that behaviour and control frame processes manually, enable in config.h: SUPPORT_CUSTOM_FRAME_CONTROL

    /// <summary>Swap back buffer with front buffer (screen drawing)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SwapScreenBuffer();

    /// <summary>Register all input events</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PollInputEvents();

    /// <summary>Wait for some time (halt program execution)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void WaitTime(double seconds);

    // Cursor-related functions

    /// <summary>Shows cursor</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ShowCursor();

    /// <summary>Hides cursor</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void HideCursor();

    /// <summary>Check if cursor is not visible</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsCursorHidden();

    /// <summary>Enables cursor (unlock cursor)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EnableCursor();

    /// <summary>Disables cursor (lock cursor)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DisableCursor();

    /// <summary>Check if cursor is on the screen</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsCursorOnScreen();


    // Drawing-related functions

    /// <summary>Set background color (framebuffer clear color)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ClearBackground(Color color);

    /// <summary>Setup canvas (framebuffer) to start drawing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginDrawing();

    /// <summary>End canvas drawing and swap buffers (double buffering)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndDrawing();

    /// <summary>Initialize 2D mode with custom camera (2D)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginMode2D(Camera2D camera);

    /// <summary>Ends 2D mode with custom camera</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndMode2D();

    /// <summary>Initializes 3D mode with custom camera (3D)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginMode3D(Camera3D camera);

    /// <summary>Ends 3D mode and returns to default 2D orthographic mode</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndMode3D();

    /// <summary>Initializes render texture for drawing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginTextureMode(RenderTexture2D target);

    /// <summary>Ends drawing to render texture</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndTextureMode();

    /// <summary>Begin custom shader drawing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginShaderMode(Shader shader);

    /// <summary>End custom shader drawing (use default shader)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndShaderMode();

    /// <summary>Begin blending mode (alpha, additive, multiplied)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginBlendMode(BlendMode mode);

    /// <summary>End blending mode (reset to default: alpha blending)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndBlendMode();

    /// <summary>Begin scissor mode (define screen area for following drawing)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginScissorMode(int x, int y, int width, int height);

    /// <summary>End scissor mode</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndScissorMode();

    /// <summary>Begin stereo rendering (requires VR simulator)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BeginVrStereoMode(VrStereoConfig config);

    /// <summary>End stereo rendering (requires VR simulator)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EndVrStereoMode();


    // VR stereo config functions for VR simulator

    /// <summary>Load VR stereo config for VR simulator device parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial VrStereoConfig LoadVrStereoConfig(VrDeviceInfo device);

    /// <summary>Unload VR stereo configs</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadVrStereoConfig(VrStereoConfig config);


    // Shader management functions

    /// <summary>Load shader from files and bind default locations</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Shader LoadShader(sbyte* vsFileName, sbyte* fsFileName);

    /// <summary>Load shader from code strings and bind default locations</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Shader LoadShaderFromMemory(sbyte* vsCode, sbyte* fsCode);

    /// <summary>Check if a shader is valid (loaded on GPU)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsShaderValid(Shader shader);

    /// <summary>Get shader uniform location</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetShaderLocation(Shader shader, sbyte* uniformName);

    /// <summary>Get shader attribute location</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetShaderLocationAttrib(Shader shader, sbyte* attribName);

    /// <summary>Set shader uniform value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetShaderValue(
        Shader shader,
        int locIndex,
        void* value,
        ShaderUniformDataType uniformType
    );

    /// <summary>Set shader uniform value vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetShaderValueV(
        Shader shader,
        int locIndex,
        void* value,
        ShaderUniformDataType uniformType,
        int count
    );

    /// <summary>Set shader uniform value (matrix 4x4)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetShaderValueMatrix(Shader shader, int locIndex, Matrix4x4 mat);

    /// <summary>Set shader uniform value for texture (sampler2d)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetShaderValueTexture(Shader shader, int locIndex, Texture2D texture);

    /// <summary>Unload shader from GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadShader(Shader shader);


    // Screen-space-related functions

    /// <summary>Get a ray trace from screen position (i.e mouse)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Ray GetScreenToWorldRay(Vector2 position, Camera3D camera);

    /// <summary>Get a ray trace from screen position (i.e mouse) in a viewport</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Ray GetScreenToWorldRayEx(Vector2 position, Camera3D camera, int width, int height);

    /// <summary>Get camera transform matrix (view matrix)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 GetCameraMatrix(Camera3D camera);

    /// <summary>Get camera 2d transform matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 GetCameraMatrix2D(Camera2D camera);

    /// <summary>Get the screen space position for a 3d world space position</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetWorldToScreen(Vector3 position, Camera3D camera);

    /// <summary>Get size position for a 3d world space position</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetWorldToScreenEx(Vector3 position, Camera3D camera, int width, int height);

    /// <summary>Get the screen space position for a 2d camera world space position</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetWorldToScreen2D(Vector2 position, Camera2D camera);

    /// <summary>Get the world space position for a 2d camera screen space position</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetScreenToWorld2D(Vector2 position, Camera2D camera);


    // Timing-related functions

    /// <summary>Set target FPS (maximum)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetTargetFPS(int fps);

    /// <summary>Get current FPS</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetFPS();

    /// <summary>Get time in seconds for last frame drawn</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetFrameTime();

    /// <summary>Get elapsed time in seconds since InitWindow()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial double GetTime();


    // Misc. functions

    /// <summary>Get a random value between min and max (both included)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetRandomValue(int min, int max);

    /// <summary>Set the seed for the random number generator</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int SetRandomSeed(uint seed);

    /// <summary>Load random values sequence, no values repeated</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int* LoadRandomSequence(uint count, int min, int max);

    /// <summary>Unload random values sequence</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadRandomSequence(int* sequence);

    /// <summary>Takes a screenshot of current screen (saved a .png)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TakeScreenshot(sbyte* fileName);

    /// <summary>Setup window configuration flags (view FLAGS)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetConfigFlags(ConfigFlags flags);

    /// <summary>Show trace log messages (LOG_DEBUG, LOG_INFO, LOG_WARNING, LOG_ERROR)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TraceLog(TraceLogLevel logLevel, sbyte* text);

    /// <summary>Set the current threshold (minimum) log level</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetTraceLogLevel(TraceLogLevel logLevel);

    /// <summary>Internal memory allocator</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void* MemAlloc(uint size);

    /// <summary>Internal memory reallocator</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void* MemRealloc(void* ptr, uint size);

    /// <summary>Internal memory free</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void MemFree(void* ptr);


    // Set custom callbacks
    // WARNING: Callbacks setup is intended for advance users

    /// <summary>Set custom trace log</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetTraceLogCallback(delegate* unmanaged[Cdecl]<int, sbyte*, sbyte*, void> callback);

    /// <summary>Set custom file binary data loader</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetLoadFileDataCallback(delegate* unmanaged[Cdecl]<sbyte*, int*, byte*> callback);

    /// <summary>Set custom file binary data saver</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetSaveFileDataCallback(
        delegate* unmanaged[Cdecl]<sbyte*, void*, int, CBool> callback
    );

    /// <summary>Set custom file text data loader</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetLoadFileTextCallback(delegate* unmanaged[Cdecl]<sbyte*, sbyte*> callback);

    /// <summary>Set custom file text data saver</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetSaveFileTextCallback(delegate* unmanaged[Cdecl]<sbyte*, sbyte*, CBool> callback);


    // Files management functions

    /// <summary>Load file data as byte array (read)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial byte* LoadFileData(sbyte* fileName, int* dataSize);

    /// <summary>Unload file data allocated by LoadFileData()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadFileData(byte* data);

    /// <summary>Save data to file from byte array (write), returns true on success</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool SaveFileData(sbyte* fileName, void* data, int dataSize);

    /// <summary>Export data to code (.h), returns true on success</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportDataAsCode(byte* data, int dataSize, sbyte* fileName);

    // Load text data from file (read), returns a '\0' terminated string
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* LoadFileText(sbyte* fileName);

    // Unload file text data allocated by LoadFileText()
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadFileText(sbyte* text);

    // Save text data to file (write), string must be '\0' terminated, returns true on success
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool SaveFileText(sbyte* fileName, sbyte* text);

    // Check if file exists
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool FileExists(sbyte* fileName);

    // Check if a directory path exists
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool DirectoryExists(sbyte* dirPath);

    /// <summary>Check file extension (including point: .png, .wav)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsFileExtension(sbyte* fileName, sbyte* ext);

    /// <summary>Get file length in bytes</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetFileLength(sbyte* fileName);

    /// <summary>Get pointer to extension for a filename string (includes dot: '.png')</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetFileExtension(sbyte* fileName);

    /// <summary>Get pointer to filename for a path string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetFileName(sbyte* filePath);

    /// <summary>Get filename string without extension (uses static string)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetFileNameWithoutExt(sbyte* filePath);

    /// <summary>Get full path for a given fileName with path (uses static string)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetDirectoryPath(sbyte* filePath);

    /// <summary>Get previous directory path for a given path (uses static string)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetPrevDirectoryPath(sbyte* dirPath);

    /// <summary>Get current working directory (uses static string)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetWorkingDirectory();

    /// <summary>Get the directory of the running application (uses static string)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetApplicationDirectory();

    /// <summary>Create directories (including full path requested), returns 0 on success</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int MakeDirectory(sbyte* dirPath);

    /// <summary>Load directory filepaths</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial FilePathList LoadDirectoryFiles(sbyte* dirPath, int* count);

    /// <summary>Load directory filepaths with extension filtering and recursive directory scan. Use 'DIR' in the filter string to include directories in the result</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial FilePathList LoadDirectoryFilesEx(sbyte* basePath, sbyte* filter, CBool scanSubdirs);

    /// <summary>Unload filepaths</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadDirectoryFiles(FilePathList files);

    /// <summary>Check if a given path is a file or a directory</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsPathFile(sbyte* path);

    /// <summary>Check if fileName is valid for the platform/OS</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsFileNameValid(sbyte* fileName);

    /// <summary>Change working directory, return true on success</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ChangeDirectory(sbyte* dir);

    /// <summary>Check if a file has been dropped into window</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsFileDropped();

    /// <summary>Load dropped filepaths</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial FilePathList LoadDroppedFiles();

    /// <summary>Unload dropped filepaths</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadDroppedFiles(FilePathList files);

    /// <summary>Get file modification time (last write time)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial long GetFileModTime(sbyte* fileName);


    // Compression/Encoding functionality

    /// <summary>Compress data (DEFLATE algorithm), memory must be MemFree()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial byte* CompressData(byte* data, int dataSize, int* compDataSize);

    /// <summary>Decompress data (DEFLATE algorithm), memory must be MemFree()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial byte* DecompressData(byte* compData, int compDataSize, int* dataSize);

    /// <summary>Encode data to Base64 string, memory must be MemFree()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* EncodeDataBase64(byte* data, int dataSize, int* outputSize);

    /// <summary>Decode Base64 string data, memory must be MemFree()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial byte* DecodeDataBase64(byte* data, int* outputSize);

    /// <summary>Compute CRC32 hash code</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial uint ComputeCRC32(byte* data, int dataSize);

    /// <summary>Compute MD5 hash code, returns static int[4] (16 bytes)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial uint* ComputeMD5(byte* data, int dataSize);

    /// <summary>Compute SHA1 hash code, returns static int[5] (20 bytes)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial uint* ComputeSHA1(byte* data, int dataSize);

    /// <summary>Open URL with default system browser (if available)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void OpenURL(sbyte* url);

    // Automation events functionality

    /// <summary>Load automation events list from file, NULL for empty list, capacity = MAX_AUTOMATION_EVENTS</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial AutomationEventList LoadAutomationEventList(sbyte* fileName);

    /// <summary>Unload automation events list from file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadAutomationEventList(AutomationEventList list);

    /// <summary>Export automation events list as text file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportAutomationEventList(AutomationEventList list, sbyte* fileName);

    /// <summary>Set automation event list to record to</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAutomationEventList(AutomationEventList* list);

    /// <summary>Set automation event internal base frame to start recording</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAutomationEventBaseFrame(int frame);

    /// <summary>Start recording automation events (AutomationEventList must be set)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void StartAutomationEventRecording();

    /// <summary>Stop recording automation events</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void StopAutomationEventRecording();

    /// <summary>Play a recorded automation event</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PlayAutomationEvent(AutomationEvent ev);

    //------------------------------------------------------------------------------------
    // Input Handling Functions (Module: core)
    //------------------------------------------------------------------------------------

    // Input-related functions: keyboard

    /// <summary>Detect if a key has been pressed once</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsKeyPressed(KeyboardKey key);

    /// <summary>Detect if a key has been pressed again</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsKeyPressedRepeat(KeyboardKey key);

    /// <summary>Detect if a key is being pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsKeyDown(KeyboardKey key);

    /// <summary>Detect if a key has been released once</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsKeyReleased(KeyboardKey key);

    /// <summary>Detect if a key is NOT being pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsKeyUp(KeyboardKey key);

    /// <summary>Set a custom key to exit program (default is ESC)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetExitKey(KeyboardKey key);

    /// <summary>
    /// Get key pressed (keycode), call it multiple times for keys queued, returns 0 when the queue is empty
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetKeyPressed();

    /// <summary>
    /// Get char pressed (unicode), call it multiple times for chars queued, returns 0 when the queue is empty
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetCharPressed();


    // Input-related functions: gamepads

    /// <summary>Detect if a gamepad is available</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsGamepadAvailable(int gamepad);

    /// <summary>Get gamepad internal name id</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* GetGamepadName(int gamepad);

    /// <summary>Detect if a gamepad button has been pressed once</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsGamepadButtonPressed(int gamepad, GamepadButton button);

    /// <summary>Detect if a gamepad button is being pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsGamepadButtonDown(int gamepad, GamepadButton button);

    /// <summary>Detect if a gamepad button has been released once</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsGamepadButtonReleased(int gamepad, GamepadButton button);

    /// <summary>Detect if a gamepad button is NOT being pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsGamepadButtonUp(int gamepad, GamepadButton button);

    /// <summary>Get the last gamepad button pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetGamepadButtonPressed();

    /// <summary>Get gamepad axis count for a gamepad</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetGamepadAxisCount(int gamepad);

    /// <summary>Get axis movement value for a gamepad axis</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetGamepadAxisMovement(int gamepad, GamepadAxis axis);

    /// <summary>Set internal gamepad mappings (SDL_GameControllerDB)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int SetGamepadMappings(sbyte* mappings);

    /// <summary>Set gamepad vibration for both motors (duration in seconds)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetGamepadVibration(int gamepad, float leftMotor, float rightMotor, float duration);

    // Input-related functions: mouse

    /// <summary>Detect if a mouse button has been pressed once</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMouseButtonPressed(MouseButton button);

    /// <summary>Detect if a mouse button is being pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMouseButtonDown(MouseButton button);

    /// <summary>Detect if a mouse button has been released once</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMouseButtonReleased(MouseButton button);

    /// <summary>Detect if a mouse button is NOT being pressed</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMouseButtonUp(MouseButton button);

    /// <summary>Get mouse position X</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMouseX();

    /// <summary>Get mouse position Y</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetMouseY();

    /// <summary>Get mouse position XY</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetMousePosition();

    /// <summary>Get mouse delta between frames</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetMouseDelta();

    /// <summary>Set mouse position XY</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMousePosition(int x, int y);

    /// <summary>Set mouse offset</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMouseOffset(int offsetX, int offsetY);

    /// <summary>Set mouse scaling</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMouseScale(float scaleX, float scaleY);

    /// <summary>Get mouse wheel movement for X or Y, whichever is larger</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetMouseWheelMove();

    /// <summary>Get mouse wheel movement for both X and Y</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetMouseWheelMoveV();

    /// <summary>Set mouse cursor</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMouseCursor(MouseCursor cursor);


    // Input-related functions: touch

    /// <summary>Get touch position X for touch point 0 (relative to screen size)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetTouchX();

    /// <summary>Get touch position Y for touch point 0 (relative to screen size)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetTouchY();

    /// <summary>Get touch position XY for a touch point index (relative to screen size)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetTouchPosition(int index);

    /// <summary>Get touch point identifier for given index</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetTouchPointId(int index);

    /// <summary>Get number of touch points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetTouchPointCount();

    //------------------------------------------------------------------------------------
    // Gestures and Touch Handling Functions (Module: gestures)
    //------------------------------------------------------------------------------------

    /// <summary>Enable a set of gestures using flags</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetGesturesEnabled(Gesture flags);

    /// <summary>Check if a gesture has been detected</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsGestureDetected(Gesture gesture);

    /// <summary>Get latest detected gesture</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Gesture GetGestureDetected();

    /// <summary>Get gesture hold time in seconds</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetGestureHoldDuration();

    /// <summary>Get gesture drag vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetGestureDragVector();

    /// <summary>Get gesture drag angle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetGestureDragAngle();

    /// <summary>Get gesture pinch delta</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetGesturePinchVector();

    /// <summary>Get gesture pinch angle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetGesturePinchAngle();


    //------------------------------------------------------------------------------------
    // Camera System Functions (Module: camera)
    //------------------------------------------------------------------------------------

    /// <summary>Update camera position for selected mode</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateCamera(Camera3D* camera, CameraMode mode);

    /// <summary>Update camera movement/rotation</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateCameraPro(Camera3D* camera, Vector3 movement, Vector3 rotation, float zoom);

    /// <summary>Returns the cameras forward vector (normalized)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 GetCameraForward(Camera3D* camera);

    /// <summary>
    /// Returns the cameras up vector (normalized)<br/>
    /// NOTE: The up vector might not be perpendicular to the forward vector
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 GetCameraUp(Camera3D* camera);

    /// <summary>Returns the cameras right vector (normalized)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 GetCameraRight(Camera3D* camera);


    // Camera movement

    /// <summary>Moves the camera in its forward direction</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraMoveForward(Camera3D* camera, float distance, CBool moveInWorldPlane);

    /// <summary>Moves the camera in its up direction</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraMoveUp(Camera3D* camera, float distance);

    /// <summary>Moves the camera target in its current right direction</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraMoveRight(Camera3D* camera, float distance, CBool moveInWorldPlane);

    /// <summary>Moves the camera position closer/farther to/from the camera target</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraMoveToTarget(Camera3D* camera, float delta);


    // Camera rotation

    /// <summary>
    /// Rotates the camera around its up vector<br/>
    /// If rotateAroundTarget is false, the camera rotates around its position
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraYaw(Camera3D* camera, float angle, CBool rotateAroundTarget);

    /// <summary>
    /// Rotates the camera around its right vector
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraPitch(
        Camera3D* camera,
        float angle,
        CBool lockView,
        CBool rotateAroundTarget,
        CBool rotateUp
    );

    /// <summary>Rotates the camera around its forward vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CameraRoll(Camera3D* camera, float angle);

    /// <summary>Returns the camera view matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 GetCameraViewMatrix(Camera3D* camera);

    /// <summary>Returns the camera projection matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 GetCameraProjectionMatrix(Camera3D* camera, float aspect);


    //------------------------------------------------------------------------------------
    // Basic Shapes Drawing Functions (Module: shapes)
    //------------------------------------------------------------------------------------

    /// <summary>
    /// Set texture and rectangle to be used on shapes drawing<br/>
    /// NOTE: It can be useful when using basic shapes and one single font.<br/>
    /// Defining a white rectangle would allow drawing everything in a single draw call.
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetShapesTexture(Texture2D texture, Rectangle source);

    /// <summary>Get texture that is used for shapes drawing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Texture2D GetShapesTexture();

    /// <summary>Get texture source rectangle that is used for shapes drawing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Rectangle GetShapesTextureRectangle();

    // Basic shapes drawing functions

    /// <summary>Draw a pixel using geometry [Can be slow, use with care]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPixel(int posX, int posY, Color color);

    /// <summary>Draw a pixel using geometry (Vector version) [Can be slow, use with care]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPixelV(Vector2 position, Color color);

    /// <summary>Draw a line</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawLine(int startPosX, int startPosY, int endPosX, int endPosY, Color color);

    /// <summary>Draw a line (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawLineV(Vector2 startPos, Vector2 endPos, Color color);

    /// <summary>Draw a line defining thickness</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color);

    /// <summary>Draw a line using cubic-bezier curves in-out</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawLineBezier(Vector2 startPos, Vector2 endPos, float thick, Color color);

    /// <summary>Draw lines sequence</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawLineStrip(Vector2* points, int pointCount, Color color);

    /// <summary>Draw a color-filled circle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircle(int centerX, int centerY, float radius, Color color);

    /// <summary>Draw a piece of a circle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircleSector(
        Vector2 center,
        float radius,
        float startAngle,
        float endAngle,
        int segments,
        Color color
    );

    /// <summary>Draw circle sector outline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircleSectorLines(
        Vector2 center,
        float radius,
        float startAngle,
        float endAngle,
        int segments,
        Color color
    );

    /// <summary>Draw a gradient-filled circle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircleGradient(
        int centerX,
        int centerY,
        float radius,
        Color inner,
        Color outer
    );

    /// <summary>Draw a color-filled circle (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircleV(Vector2 center, float radius, Color color);

    /// <summary>Draw circle outline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircleLines(int centerX, int centerY, float radius, Color color);

    /// <summary>Draw circle outline (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircleLinesV(Vector2 center, float radius, Color color);

    /// <summary>Draw ellipse</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawEllipse(int centerX, int centerY, float radiusH, float radiusV, Color color);

    /// <summary>Draw ellipse outline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawEllipseLines(int centerX, int centerY, float radiusH, float radiusV, Color color);

    /// <summary>Draw ring</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRing(
        Vector2 center,
        float innerRadius,
        float outerRadius,
        float startAngle,
        float endAngle,
        int segments,
        Color color
    );

    /// <summary>Draw ring outline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRingLines(
        Vector2 center,
        float innerRadius,
        float outerRadius,
        float startAngle,
        float endAngle,
        int segments,
        Color color
    );

    /// <summary>Draw a color-filled rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangle(int posX, int posY, int width, int height, Color color);

    /// <summary>Draw a color-filled rectangle (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleV(Vector2 position, Vector2 size, Color color);

    /// <summary>Draw a color-filled rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleRec(Rectangle rec, Color color);

    /// <summary>Draw a color-filled rectangle with pro parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectanglePro(Rectangle rec, Vector2 origin, float rotation, Color color);

    /// <summary>Draw a vertical-gradient-filled rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleGradientV(
        int posX,
        int posY,
        int width,
        int height,
        Color top,
        Color bottom
    );

    /// <summary>Draw a horizontal-gradient-filled rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleGradientH(
        int posX,
        int posY,
        int width,
        int height,
        Color left,
        Color right
    );

    /// <summary>Draw a gradient-filled rectangle with custom vertex colors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleGradientEx(
        Rectangle rec,
        Color topLeft,
        Color bottomLeft,
        Color topRight,
        Color bottomRight
    );

    /// <summary>Draw rectangle outline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleLines(int posX, int posY, int width, int height, Color color);

    /// <summary>Draw rectangle outline with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleLinesEx(Rectangle rec, float lineThick, Color color);

    /// <summary>Draw rectangle with rounded edges</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleRounded(Rectangle rec, float roundness, int segments, Color color);

    /// <summary>Draw rectangle lines with rounded edges</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleRoundedLines(
        Rectangle rec,
        float roundness,
        int segments,
        Color color
    );

    /// <summary>Draw rectangle with rounded edges outline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRectangleRoundedLinesEx(
        Rectangle rec,
        float roundness,
        int segments,
        float lineThick,
        Color color
    );

    /// <summary>Draw a color-filled triangle (vertex in counter-clockwise order!)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color);

    /// <summary>Draw triangle outline (vertex in counter-clockwise order!)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTriangleLines(Vector2 v1, Vector2 v2, Vector2 v3, Color color);

    /// <summary>Draw a triangle fan defined by points (first vertex is the center)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTriangleFan(Vector2* points, int pointCount, Color color);

    /// <summary>Draw a triangle strip defined by points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTriangleStrip(Vector2* points, int pointCount, Color color);

    /// <summary>Draw a regular polygon (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPoly(Vector2 center, int sides, float radius, float rotation, Color color);

    /// <summary>Draw a polygon outline of n sides</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPolyLines(Vector2 center, int sides, float radius, float rotation, Color color);

    /// <summary>Draw a polygon outline of n sides with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPolyLinesEx(
        Vector2 center,
        int sides,
        float radius,
        float rotation,
        float lineThick,
        Color color
    );

    // Splines drawing functions

    /// <summary>Draw spline: Linear, minimum 2 points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineLinear(Vector2* points, int pointCount, float thick, Color color);

    /// <summary>Draw spline: B-Spline, minimum 4 points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineBasis(Vector2* points, int pointCount, float thick, Color color);

    /// <summary>Draw spline: Catmull-Rom, minimum 4 points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineCatmullRom(Vector2* points, int pointCount, float thick, Color color);

    /// <summary>Draw spline: Quadratic Bezier, minimum 3 points (1 control point): [p1, c2, p3, c4...]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineBezierQuadratic(Vector2* points, int pointCount, float thick, Color color);

    /// <summary>Draw spline: Cubic Bezier, minimum 4 points (2 control points): [p1, c2, c3, p4, c5, c6...]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineBezierCubic(Vector2* points, int pointCount, float thick, Color color);

    /// <summary>Draw spline segment: Linear, 2 points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineSegmentLinear(Vector2 p1, Vector2 p2, float thick, Color color);

    /// <summary>Draw spline segment: B-Spline, 4 points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineSegmentBasis(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float thick, Color color);

    /// <summary>Draw spline segment: Catmull-Rom, 4 points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineSegmentCatmullRom(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float thick, Color color);

    /// <summary>Draw spline segment: Quadratic Bezier, 2 points, 1 control point</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineSegmentBezierQuadratic(Vector2 p1, Vector2 c2, Vector2 p3, float thick, Color color);

    /// <summary>Draw spline segment: Cubic Bezier, 2 points, 2 control points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSplineSegmentBezierCubic(Vector2 p1, Vector2 c2, Vector2 c3, Vector2 p4, float thick, Color color);

    // Spline segment point evaluation functions, for a given t [0.0f .. 1.0f]

    /// <summary>Get (evaluate) spline point: Linear</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetSplinePointLinear(Vector2 startPos, Vector2 endPos, float t);

    /// <summary>Get (evaluate) spline point: B-Spline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetSplinePointBasis(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t);

    /// <summary>Get (evaluate) spline point: Catmull-Rom</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetSplinePointCatmullRom(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t);

    /// <summary>Get (evaluate) spline point: Quadratic Bezier</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetSplinePointBezierQuad(Vector2 p1, Vector2 c2, Vector2 p3, float t);

    /// <summary>Get (evaluate) spline point: Cubic Bezier</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 GetSplinePointBezierCubic(Vector2 p1, Vector2 c2, Vector2 c3, Vector2 p4, float t);

    // Basic shapes collision detection functions

    /// <summary>Check collision between two rectangles</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionRecs(Rectangle rec1, Rectangle rec2);

    /// <summary>Check collision between two circles</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionCircles(
        Vector2 center1,
        float radius1,
        Vector2 center2,
        float radius2
    );

    /// <summary>Check collision between circle and rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionCircleRec(Vector2 center, float radius, Rectangle rec);

    /// <summary>Check if circle collides with a line created betweeen two points [p1] and [p2]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionCircleLine(Vector2 center, float radius, Vector2 p1, Vector2 p2);

    /// <summary>Check if point is inside rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionPointRec(Vector2 point, Rectangle rec);

    /// <summary>Check if point is inside circle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionPointCircle(Vector2 point, Vector2 center, float radius);

    /// <summary>Check if point is inside a triangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionPointTriangle(Vector2 point, Vector2 p1, Vector2 p2, Vector2 p3);

    /// <summary>Check if point is within a polygon described by array of vertices</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionPointPoly(Vector2 point, Vector2* points, int pointCount);

    /// <summary>
    /// Check the collision between two lines defined by two points each, returns collision point by reference
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionLines(
        Vector2 startPos1,
        Vector2 endPos1,
        Vector2 startPos2,
        Vector2 endPos2,
        Vector2* collisionPoint
    );

    /// <summary>
    /// Check if point belongs to line created between two points [p1] and [p2] with defined margin in pixels [threshold]
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionPointLine(Vector2 point, Vector2 p1, Vector2 p2, int threshold);

    /// <summary>Get collision rectangle for two rectangles collision</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Rectangle GetCollisionRec(Rectangle rec1, Rectangle rec2);


    //------------------------------------------------------------------------------------
    // Texture Loading and Drawing Functions (Module: textures)
    //------------------------------------------------------------------------------------

    // Image loading functions
    // NOTE: This functions do not require GPU access

    /// <summary>Load image from file into CPU memory (RAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image LoadImage(sbyte* fileName);

    /// <summary>Load image from RAW file data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image LoadImageRaw(
        sbyte* fileName,
        int width,
        int height,
        PixelFormat format,
        int headerSize
    );

    /// <summary>Load image sequence from file (frames appended to image.data)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image LoadImageAnim(sbyte* fileName, int* frames);

    /// <summary>Load image from memory buffer, fileType refers to extension: i.e. ".png"</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image LoadImageFromMemory(sbyte* fileType, byte* fileData, int dataSize);

    /// <summary>Load image from GPU texture data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image LoadImageFromTexture(Texture2D texture);

    /// <summary>Load image from screen buffer and (screenshot)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image LoadImageFromScreen();

    /// <summary>Check if an image is valid (data and parameters)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsImageValid(Image image);

    /// <summary>Unload image from CPU memory (RAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadImage(Image image);

    /// <summary>Export image data to file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportImage(Image image, sbyte* fileName);

    /// <summary>Export image to memory buffer</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial byte* ExportImageToMemory(Image image, sbyte* fileType, int* fileSize);

    /// <summary>Export image as code file defining an array of bytes</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportImageAsCode(Image image, sbyte* fileName);


    // Image generation functions

    /// <summary>Generate image: plain color</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageColor(int width, int height, Color color);

    /// <summary>Generate image: linear gradient, direction in degrees [0..360], 0=Vertical gradient</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageGradientLinear(int width, int height, int direction, Color start, Color end);

    /// <summary>Generate image: radial gradient</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageGradientRadial(
        int width,
        int height,
        float density,
        Color inner,
        Color outer
    );

    /// <summary>Generate image: square gradient</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageGradientSquare(
        int width,
        int height,
        float density,
        Color inner,
        Color outer);

    /// <summary>Generate image: checked</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageChecked(
        int width,
        int height,
        int checksX,
        int checksY,
        Color col1,
        Color col2
    );

    /// <summary>Generate image: white noise</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageWhiteNoise(int width, int height, float factor);

    /// <summary>Generate image: perlin noise</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImagePerlinNoise(int width, int height, int offsetX, int offsetY, float scale);

    /// <summary>Generate image: cellular algorithm, bigger tileSize means bigger cells</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageCellular(int width, int height, int tileSize);

    /// <summary>Generate image: grayscale image from text data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageText(int width, int height, sbyte* text);


    // Image manipulation functions

    /// <summary>Create an image duplicate (useful for transformations)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image ImageCopy(Image image);

    /// <summary>Create an image from another image piece</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image ImageFromImage(Image image, Rectangle rec);

    /// <summary>Create an image from a selected channel of another image (GRAYSCALE)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image ImageFromChannel(Image image, int selectedChannel);

    /// <summary>Create an image from text (default font)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image ImageText(sbyte* text, int fontSize, Color color);

    /// <summary>Create an image from text (custom sprite font)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image ImageTextEx(Font font, sbyte* text, float fontSize, float spacing, Color tint);

    /// <summary>Convert image data to desired format</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageFormat(Image* image, PixelFormat newFormat);

    /// <summary>Convert image to POT (power-of-two)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageToPOT(Image* image, Color fill);

    /// <summary>Crop an image to a defined rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageCrop(Image* image, Rectangle crop);

    /// <summary>Crop image depending on alpha value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageAlphaCrop(Image* image, float threshold);

    /// <summary>Clear alpha channel to desired color</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageAlphaClear(Image* image, Color color, float threshold);

    /// <summary>Apply alpha mask to image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageAlphaMask(Image* image, Image alphaMask);

    /// <summary>Premultiply alpha channel</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageAlphaPremultiply(Image* image);

    /// <summary>Apply Gaussian blur using a box blur approximation</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageBlurGaussian(Image* image, int blurSize);

    /// <summary>Apply custom square convolution kernel to image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageKernelConvolution(Image* image, float* kernel, int kernelSize);

    /// <summary>Resize image (Bicubic scaling algorithm)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageResize(Image* image, int newWidth, int newHeight);

    /// <summary>Resize image (Nearest-Neighbor scaling algorithm)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageResizeNN(Image* image, int newWidth, int newHeight);

    /// <summary>Resize canvas and fill with color</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageResizeCanvas(
        Image* image,
        int newWidth,
        int newHeight,
        int offsetX,
        int offsetY,
        Color color
    );

    /// <summary>Generate all mipmap levels for a provided image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageMipmaps(Image* image);

    /// <summary>Dither image data to 16bpp or lower (Floyd-Steinberg dithering)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDither(Image* image, int rBpp, int gBpp, int bBpp, int aBpp);

    /// <summary>Flip image vertically</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageFlipVertical(Image* image);

    /// <summary>Flip image horizontally</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageFlipHorizontal(Image* image);

    /// <summary>Rotate image by input angle in degrees (-359 to 359)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageRotate(Image* image, int degrees);

    /// <summary>Rotate image clockwise 90deg</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageRotateCW(Image* image);

    /// <summary>Rotate image counter-clockwise 90deg</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageRotateCCW(Image* image);

    /// <summary>Modify image color: tint</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageColorTint(Image* image, Color color);

    /// <summary>Modify image color: invert</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageColorInvert(Image* image);

    /// <summary>Modify image color: grayscale</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageColorGrayscale(Image* image);

    /// <summary>Modify image color: contrast (-100 to 100)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageColorContrast(Image* image, float contrast);

    /// <summary>Modify image color: brightness (-255 to 255)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageColorBrightness(Image* image, int brightness);

    /// <summary>Modify image color: replace color</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageColorReplace(Image* image, Color color, Color replace);

    /// <summary>Load color data from image as a Color array (RGBA - 32bit)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color* LoadImageColors(Image image);

    /// <summary>Load colors palette from image as a Color array (RGBA - 32bit)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color* LoadImagePalette(Image image, int maxPaletteSize, int* colorCount);

    /// <summary>Unload color data loaded with LoadImageColors()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadImageColors(Color* colors);

    /// <summary>Unload colors palette loaded with LoadImagePalette()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadImagePalette(Color* colors);

    /// <summary>Get image alpha border rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Rectangle GetImageAlphaBorder(Image image, float threshold);

    /// <summary>Get image pixel color at (x, y) position</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color GetImageColor(Image image, int x, int y);


    // Image drawing functions
    // NOTE: Image software-rendering functions (CPU)

    /// <summary>Clear image background with given color</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageClearBackground(Image* dst, Color color);

    /// <summary>Draw pixel within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawPixel(Image* dst, int posX, int posY, Color color);

    /// <summary>Draw pixel within an image (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawPixelV(Image* dst, Vector2 position, Color color);

    /// <summary>Draw line within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawLine(
        Image* dst,
        int startPosX,
        int startPosY,
        int endPosX,
        int endPosY,
        Color color
    );

    /// <summary>Draw line within an image (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawLineV(Image* dst, Vector2 start, Vector2 end, Color color);

    /// <summary>Draw a line defining thickness within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawLineEx(Image* dst, Vector2 start, Vector2 end, int thick, Color color);

    /// <summary>Draw circle within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawCircle(Image* dst, int centerX, int centerY, int radius, Color color);

    /// <summary>Draw circle within an image (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawCircleV(Image* dst, Vector2 center, int radius, Color color);

    /// <summary>Draw circle outline within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawCircleLines(Image* dst, int centerX, int centerY, int radius, Color color);

    /// <summary>Draw circle outline within an image (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawCircleLinesV(Image* dst, Vector2 center, int radius, Color color);

    /// <summary>Draw rectangle within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawRectangle(
        Image* dst,
        int posX,
        int posY,
        int width,
        int height,
        Color color
    );

    /// <summary>Draw rectangle within an image (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawRectangleV(Image* dst, Vector2 position, Vector2 size, Color color);

    /// <summary>Draw rectangle within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawRectangleRec(Image* dst, Rectangle rec, Color color);

    /// <summary>Draw rectangle lines within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawRectangleLines(Image* dst, Rectangle rec, int thick, Color color);

    /// <summary>Draw triangle within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawTriangle(Image* dst, Vector2 v1, Vector2 v2, Vector2 v3, Color color);

    /// <summary>Draw triangle with interpolated colors within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawTriangleEx(Image* dst, Vector2 v1, Vector2 v2, Vector2 v3, Color c1, Color c2, Color c3);

    /// <summary>Draw triangle outline within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawTriangleLines(Image* dst, Vector2 v1, Vector2 v2, Vector2 v3, Color color);

    /// <summary>Draw a triangle fan defined by points within an image (first vertex is the center)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawTriangleFan(Image* dst, Vector2* points, int pointCount, Color color);

    /// <summary>Draw a triangle strip defined by points within an image</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawTriangleStrip(Image* dst, Vector2* points, int pointCount, Color color);

    /// <summary>Draw a source image within a destination image (tint applied to source)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDraw(Image* dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint);

    /// <summary>Draw text (using default font) within an image (destination)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawText(Image* dst, sbyte* text, int x, int y, int fontSize, Color color);

    /// <summary>Draw text (custom sprite font) within an image (destination)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ImageDrawTextEx(
        Image* dst,
        Font font,
        sbyte* text,
        Vector2 position,
        float fontSize,
        float spacing,
        Color tint
    );


    // Texture loading functions
    // NOTE: These functions require GPU access

    /// <summary>Load texture from file into GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Texture2D LoadTexture(sbyte* fileName);

    /// <summary>Load texture from image data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Texture2D LoadTextureFromImage(Image image);

    /// <summary>Load cubemap from image, multiple image cubemap layouts supported</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Texture2D LoadTextureCubemap(Image image, CubemapLayout layout);

    /// <summary>Load texture for rendering (framebuffer)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial RenderTexture2D LoadRenderTexture(int width, int height);

    /// <summary>Check if a texture is valid (loaded in GPU)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsTextureValid(Texture2D texture);

    /// <summary>Unload texture from GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadTexture(Texture2D texture);

    /// <summary>Check if a render texture is valid (loaded in GPU)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsRenderTextureValid(RenderTexture2D target);

    /// <summary>Unload render texture from GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadRenderTexture(RenderTexture2D target);

    /// <summary>Update GPU texture with new data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateTexture(Texture2D texture, void* pixels);

    /// <summary>Update GPU texture rectangle with new data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateTextureRec(Texture2D texture, Rectangle rec, void* pixels);


    // Texture configuration functions

    /// <summary>Generate GPU mipmaps for a texture</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void GenTextureMipmaps(Texture2D* texture);

    /// <summary>Set texture scaling filter mode</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetTextureFilter(Texture2D texture, TextureFilter filter);

    /// <summary>Set texture wrapping mode</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetTextureWrap(Texture2D texture, TextureWrap wrap);


    // Texture drawing functions

    /// <summary>Draw a Texture2D</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTexture(Texture2D texture, int posX, int posY, Color tint);

    /// <summary>Draw a Texture2D with position defined as Vector2</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextureV(Texture2D texture, Vector2 position, Color tint);

    /// <summary>Draw a Texture2D with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextureEx(
        Texture2D texture,
        Vector2 position,
        float rotation,
        float scale,
        Color tint
    );

    /// <summary>Draw a part of a texture defined by a rectangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextureRec(Texture2D texture, Rectangle source, Vector2 position, Color tint);

    /// <summary>Draw a part of a texture defined by a rectangle with 'pro' parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTexturePro(
        Texture2D texture,
        Rectangle source,
        Rectangle dest,
        Vector2 origin,
        float rotation,
        Color tint
    );

    /// <summary>Draws a texture (or part of it) that stretches or shrinks nicely</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextureNPatch(
        Texture2D texture,
        NPatchInfo nPatchInfo,
        Rectangle dest,
        Vector2 origin,
        float rotation,
        Color tint
    );


    // Color/pixel related functions

    /// <summary>Get hexadecimal value for a Color (0xRRGGBBAA)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ColorToInt(Color color);

    /// <summary>Get color normalized as float [0..1]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector4 ColorNormalize(Color color);

    /// <summary>Get color from normalized values [0..1]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorFromNormalized(Vector4 normalized);

    /// <summary>Get HSV values for a Color, hue [0..360], saturation/value [0..1]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 ColorToHSV(Color color);

    /// <summary>Get a Color from HSV values, hue [0..360], saturation/value [0..1]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorFromHSV(float hue, float saturation, float value);

    /// <summary>Get color multiplied with another color</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorTint(Color color, Color tint);

    /// <summary>Get color with brightness correction, brightness factor goes from -1.0f to 1.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorBrightness(Color color, float factor);

    /// <summary>Get color with contrast correction, contrast values between -1.0f and 1.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorContrast(Color color, float contrast);

    /// <summary>Get color with alpha applied, alpha goes from 0.0f to 1.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorAlpha(Color color, float alpha);

    /// <summary>Get src alpha-blended into dst color with tint</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorAlphaBlend(Color dst, Color src, Color tint);

    /// <summary>Get color lerp interpolation between two colors, factor [0.0f..1.0f]</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color ColorLerp(Color color1, Color color2, float factor);

    /// <summary>Get Color structure from hexadecimal value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color GetColor(uint hexValue);

    /// <summary>Get Color from a source pixel pointer of certain format</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Color GetPixelColor(void* srcPtr, PixelFormat format);

    /// <summary>Set color formatted into destination pixel pointer</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetPixelColor(void* dstPtr, Color color, PixelFormat format);

    /// <summary>Get pixel data size in bytes for certain format</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetPixelDataSize(int width, int height, PixelFormat format);


    //------------------------------------------------------------------------------------
    // Font Loading and Text Drawing Functions (Module: text)
    //------------------------------------------------------------------------------------

    // Font loading/unloading functions

    /// <summary>Get the default Font</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Font GetFontDefault();

    /// <summary>Load font from file into GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Font LoadFont(sbyte* fileName);

    /// <summary>
    /// Load font from file with extended parameters, use NULL for codepoints and 0 for codepointCount to load
    /// the default character set, font size is provided in pixels height
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Font LoadFontEx(sbyte* fileName, int fontSize, int* codepoints, int codepointCount);

    /// <summary>Load font from Image (XNA style)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Font LoadFontFromImage(Image image, Color key, int firstChar);

    /// <summary>Load font from memory buffer, fileType refers to extension: i.e. ".ttf"</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Font LoadFontFromMemory(
        sbyte* fileType,
        byte* fileData,
        int dataSize,
        int fontSize,
        int* codepoints,
        int codepointCount
    );

    /// <summary>Check if a font is valid (font data loaded, WARNING: GPU texture not checked)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsFontValid(Font font);

    /// <summary>Load font data for further use</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial GlyphInfo* LoadFontData(
        byte* fileData,
        int dataSize,
        int fontSize,
        int* fontChars,
        int glyphCount,
        FontType type
    );

    /// <summary>Generate image font atlas using chars info</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Image GenImageFontAtlas(
        GlyphInfo* chars,
        Rectangle** recs,
        int glyphCount,
        int fontSize,
        int padding,
        int packMethod
    );

    /// <summary>Unload font chars info data (RAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadFontData(GlyphInfo* chars, int glyphCount);

    /// <summary>Unload Font from GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadFont(Font font);

    /// <summary>Export font as code file, returns true on success</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportFontAsCode(Font font, sbyte* fileName);


    // Text drawing functions

    /// <summary>Shows current FPS</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawFPS(int posX, int posY);

    /// <summary>Draw text (using default font)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawText(sbyte* text, int posX, int posY, int fontSize, Color color);

    /// <summary>Draw text using font and additional parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextEx(
        Font font,
        sbyte* text,
        Vector2 position,
        float fontSize,
        float spacing,
        Color tint
    );

    /// <summary>Draw text using Font and pro parameters (rotation)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextPro(
        Font font,
        sbyte* text,
        Vector2 position,
        Vector2 origin,
        float rotation,
        float fontSize,
        float spacing,
        Color tint
    );

    /// <summary>Draw one character (codepoint)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextCodepoint(
        Font font,
        int codepoint,
        Vector2 position,
        float fontSize,
        Color tint
    );

    /// <summary>Draw multiple characters (codepoint)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTextCodepoints(
        Font font,
        int* codepoints,
        int count,
        Vector2 position,
        float fontSize,
        float spacing,
        Color tint
    );

    // Text font info functions

    /// <summary>Set vertical line spacing when drawing with line-breaks</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetTextLineSpacing(int spacing);

    /// <summary>Measure string width for default font</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int MeasureText(sbyte* text, int fontSize);

    /// <summary>Measure string size for Font</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 MeasureTextEx(Font font, sbyte* text, float fontSize, float spacing);

    /// <summary>
    /// Get glyph index position in font for a codepoint (unicode character), fallback to '?' if not found
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetGlyphIndex(Font font, int character);

    /// <summary>
    /// Get glyph font info data for a codepoint (unicode character), fallback to '?' if not found
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial GlyphInfo GetGlyphInfo(Font font, int codepoint);

    /// <summary>
    /// Get glyph rectangle in font atlas for a codepoint (unicode character), fallback to '?' if not found
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Rectangle GetGlyphAtlasRec(Font font, int codepoint);


    // Text codepoints management functions (unicode characters)

    /// <summary>Load UTF-8 text encoded from codepoints array</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* LoadUTF8(int* codepoints, int length);

    /// <summary>Unload UTF-8 text encoded from codepoints array</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadUTF8(sbyte* text);

    /// <summary>Load all codepoints from a UTF-8 text string, codepoints count returned by parameter</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int* LoadCodepoints(sbyte* text, int* count);

    /// <summary>Unload codepoints data from memory</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadCodepoints(int* codepoints);

    /// <summary>Get total number of codepoints in a UTF8 encoded string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetCodepointCount(sbyte* text);

    /// <summary>Get next codepoint in a UTF-8 encoded string, 0x3f('?') is returned on failure</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetCodepoint(sbyte* text, int* codepointSize);

    /// <summary>Get next codepoint in a UTF-8 encoded string; 0x3f('?') is returned on failure</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetCodepointNext(sbyte* text, int* codepointSize);

    /// <summary>Get previous codepoint in a UTF-8 encoded string, 0x3f('?') is returned on failure</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int GetCodepointPrevious(sbyte* text, int* codepointSize);

    /// <summary>Encode one codepoint into UTF-8 byte array (array length returned as parameter)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* CodepointToUTF8(int codepoint, int* utf8Size);


    // Text strings management functions (no UTF-8 strings, only byte chars)
    // NOTE: Some strings allocate memory internally for returned strings, just be careful!

    /// <summary>Copy one string to another, returns bytes copied</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TextCopy(sbyte* dst, sbyte* src);

    /// <summary>Check if two text string are equal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool TextIsEqual(sbyte* text1, sbyte* text2);

    /// <summary>Get text length, checks for '\0' ending</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial uint TextLength(sbyte* text);

    /// <summary>Text formatting with variables (sprintf style)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextFormat(sbyte* text);

    /// <summary>Get a piece of a text string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextSubtext(sbyte* text, int position, int length);

    /// <summary>Replace text string (WARNING: memory must be freed!)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextReplace(sbyte* text, sbyte* replace, sbyte* by);

    /// <summary>Insert text in a position (WARNING: memory must be freed!)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextInsert(sbyte* text, sbyte* insert, int position);

    /// <summary>Join text strings with delimiter</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextJoin(sbyte** textList, int count, sbyte* delimiter);

    /// <summary>Split text into multiple strings</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte** TextSplit(sbyte* text, char delimiter, int* count);

    /// <summary>Append text at specific position and move cursor!</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void TextAppend(sbyte* text, sbyte* append, int* position);

    /// <summary>Find first text occurrence within a string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TextFindIndex(sbyte* text, sbyte* find);

    /// <summary>Get upper case version of provided string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextToUpper(sbyte* text);

    /// <summary>Get lower case version of provided string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextToLower(sbyte* text);

    /// <summary>Get Pascal case notation version of provided string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextToPascal(sbyte* text);

    /// <summary>Get Snake case notation version of provided string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextToSnake(sbyte* text);

    /// <summary>Get Camel case notation version of provided string</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial sbyte* TextToCamel(sbyte* text);

    /// <summary>Get integer value from text (negative values not supported)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int TextToInteger(sbyte* text);

    /// <summary>Get float value from text (negative values not supported)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float TextToFloat(sbyte* text);

    //------------------------------------------------------------------------------------
    // Basic 3d Shapes Drawing Functions (Module: models)
    //------------------------------------------------------------------------------------

    // Basic geometric 3D shapes drawing functions

    /// <summary>Draw a line in 3D world space</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawLine3D(Vector3 startPos, Vector3 endPos, Color color);

    /// <summary>Draw a point in 3D space, actually a small line</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPoint3D(Vector3 position, Color color);

    /// <summary>Draw a circle in 3D world space</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCircle3D(
        Vector3 center,
        float radius,
        Vector3 rotationAxis,
        float rotationAngle,
        Color color
    );

    /// <summary>Draw a color-filled triangle (vertex in counter-clockwise order!)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTriangle3D(Vector3 v1, Vector3 v2, Vector3 v3, Color color);

    /// <summary>Draw a triangle strip defined by points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawTriangleStrip3D(Vector3* points, int pointCount, Color color);

    /// <summary>Draw cube</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCube(Vector3 position, float width, float height, float length, Color color);

    /// <summary>Draw cube (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCubeV(Vector3 position, Vector3 size, Color color);

    /// <summary>Draw cube wires</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCubeWires(Vector3 position, float width, float height, float length, Color color);

    /// <summary>Draw cube wires (Vector version)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCubeWiresV(Vector3 position, Vector3 size, Color color);

    /// <summary>Draw sphere</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSphere(Vector3 centerPos, float radius, Color color);

    /// <summary>Draw sphere with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSphereEx(Vector3 centerPos, float radius, int rings, int slices, Color color);

    /// <summary>Draw sphere wires</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawSphereWires(Vector3 centerPos, float radius, int rings, int slices, Color color);

    /// <summary>Draw a cylinder/cone</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCylinder(
        Vector3 position,
        float radiusTop,
        float radiusBottom,
        float height,
        int slices,
        Color color
    );

    /// <summary>Draw a cylinder with base at startPos and top at endPos</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCylinderEx(
        Vector3 startPos,
        Vector3 endPos,
        float startRadius,
        float endRadius,
        int sides,
        Color color
    );

    /// <summary>Draw a cylinder/cone wires</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCylinderWires(
        Vector3 position,
        float radiusTop,
        float radiusBottom,
        float height,
        int slices,
        Color color
    );

    /// <summary>Draw a cylinder wires with base at startPos and top at endPos</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCylinderWiresEx(
        Vector3 startPos,
        Vector3 endPos,
        float startRadius,
        float endRadius,
        int sides,
        Color color
    );

    /// <summary>Draw a capsule with the center of its sphere caps at startPos and endPos</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCapsule(
        Vector3 startPos,
        Vector3 endPos,
        float radius,
        int slices,
        int rings,
        Color color
    );

    /// <summary>Draw capsule wireframe with the center of its sphere caps at startPos and endPos</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawCapsuleWires(
        Vector3 startPos,
        Vector3 endPos,
        float radius,
        int slices,
        int rings,
        Color color
    );

    /// <summary>Draw a plane XZ</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawPlane(Vector3 centerPos, Vector2 size, Color color);

    /// <summary>Draw a ray line</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawRay(Ray ray, Color color);

    /// <summary>Draw a grid (centered at (0, 0, 0))</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawGrid(int slices, float spacing);


    //------------------------------------------------------------------------------------
    // Model 3d Loading and Drawing Functions (Module: models)
    //------------------------------------------------------------------------------------

    // Model management functions

    /// <summary>Load model from files (meshes and materials)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Model LoadModel(sbyte* fileName);

    /// <summary>Load model from generated mesh (default material)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Model LoadModelFromMesh(Mesh mesh);

    /// <summary>Check if a model is valid (loaded in GPU, VAO/VBOs)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsModelValid(Model model);

    /// <summary>Unload model from memory (RAM and/or VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadModel(Model model);

    /// <summary>Compute model bounding box limits (considers all meshes)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial BoundingBox GetModelBoundingBox(Model model);


    // Model drawing functions

    /// <summary>Draw a model (with texture if set)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawModel(Model model, Vector3 position, float scale, Color tint);

    /// <summary>Draw a model with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawModelEx(
        Model model,
        Vector3 position,
        Vector3 rotationAxis,
        float rotationAngle,
        Vector3 scale,
        Color tint
    );

    /// <summary>Draw a model wires (with texture if set)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawModelWires(Model model, Vector3 position, float scale, Color tint);

    /// <summary>Draw a model wires (with texture if set) with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawModelWiresEx(
        Model model,
        Vector3 position,
        Vector3 rotationAxis,
        float rotationAngle,
        Vector3 scale,
        Color tint
    );

    /// <summary>Draw a model as points</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawModelPoints(Model model, Vector3 position, float scale, Color tint);

    /// <summary>Draw a model as points with extended parameters</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawModelPointsEx(
        Model model,
        Vector3 position,
        Vector3 rotationAxis,
        float rotationAngle,
        Vector3 scale,
        Color tint
    );

    /// <summary>Draw bounding box (wires)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawBoundingBox(BoundingBox box, Color color);

    /// <summary>Draw a billboard texture</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawBillboard(
        Camera3D camera,
        Texture2D texture,
        Vector3 center,
        float scale,
        Color tint
    );

    /// <summary>Draw a billboard texture defined by source</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawBillboardRec(
        Camera3D camera,
        Texture2D texture,
        Rectangle source,
        Vector3 position,
        Vector2 size,
        Color tint
    );

    /// <summary>Draw a billboard texture defined by source and rotation</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawBillboardPro(
        Camera3D camera,
        Texture2D texture,
        Rectangle source,
        Vector3 position,
        Vector3 up,
        Vector2 size,
        Vector2 origin,
        float rotation,
        Color tint
    );


    // Mesh management functions

    /// <summary>Upload vertex data into GPU and provided VAO/VBO ids</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UploadMesh(Mesh* mesh, CBool dynamic);

    /// <summary>Update mesh vertex data in GPU for a specific buffer index</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateMeshBuffer(Mesh mesh, int index, void* data, int dataSize, int offset);

    /// <summary>Unload mesh from memory (RAM and/or VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadMesh(Mesh mesh);

    /// <summary>Draw a 3d mesh with material and transform</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawMesh(Mesh mesh, Material material, Matrix4x4 transform);

    /// <summary>Draw multiple mesh instances with material and different transforms</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DrawMeshInstanced(Mesh mesh, Material material, Matrix4x4* transforms, int instances);

    /// <summary>Compute mesh bounding box limits</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial BoundingBox GetMeshBoundingBox(Mesh mesh);

    /// <summary>Compute mesh tangents</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void GenMeshTangents(Mesh* mesh);

    /// <summary>Export mesh data to file, returns true on success</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportMesh(Mesh mesh, sbyte* fileName);

    /// <summary>Export mesh as code file (.h) defining multiple arrays of vertex attributes</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportMeshAsCode(Mesh mesh, sbyte* fileName);

    // Mesh generation functions

    /// <summary>Generate polygonal mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshPoly(int sides, float radius);

    /// <summary>Generate plane mesh (with subdivisions)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshPlane(float width, float length, int resX, int resZ);

    /// <summary>Generate cuboid mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshCube(float width, float height, float length);

    /// <summary>Generate sphere mesh (standard sphere)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshSphere(float radius, int rings, int slices);

    /// <summary>Generate half-sphere mesh (no bottom cap)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshHemiSphere(float radius, int rings, int slices);

    /// <summary>Generate cylinder mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshCylinder(float radius, float height, int slices);

    /// <summary>Generate cone/pyramid mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshCone(float radius, float height, int slices);

    /// <summary>Generate torus mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshTorus(float radius, float size, int radSeg, int sides);

    /// <summary>Generate trefoil knot mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshKnot(float radius, float size, int radSeg, int sides);

    /// <summary>Generate heightmap mesh from image data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshHeightmap(Image heightmap, Vector3 size);

    /// <summary>Generate cubes-based map mesh from image data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Mesh GenMeshCubicmap(Image cubicmap, Vector3 cubeSize);


    // Material loading/unloading functions

    //TODO: safe Helper method
    /// <summary>Load materials from model file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Material* LoadMaterials(sbyte* fileName, int* materialCount);

    /// <summary>Load default material (Supports: DIFFUSE, SPECULAR, NORMAL maps)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Material LoadMaterialDefault();

    /// <summary>Check if a material is valid (shader assigned, map textures loaded in GPU)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMaterialValid(Material material);

    /// <summary>Unload material from GPU memory (VRAM)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadMaterial(Material material);

    /// <summary>Set texture for a material map type (MAP_DIFFUSE, MAP_SPECULAR...)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMaterialTexture(Material* material, MaterialMapIndex mapType, Texture2D texture);

    /// <summary>Set material for a mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetModelMeshMaterial(Model* model, int meshId, int materialId);


    // Model animations loading/unloading functions

    /// <summary>Load model animations from file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial ModelAnimation* LoadModelAnimations(sbyte* fileName, int* animCount);

    /// <summary>Update model animation pose (CPU)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateModelAnimation(Model model, ModelAnimation anim, int frame);

    /// <summary>Update model animation mesh bone matrices (GPU skinning)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateModelAnimationBones(Model model, ModelAnimation anim, int frame);

    /// <summary>Unload animation data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadModelAnimation(ModelAnimation anim);

    /// <summary>Unload animation array data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadModelAnimations(ModelAnimation* animations, int animCount);

    /// <summary>Check model animation skeleton match</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsModelAnimationValid(Model model, ModelAnimation anim);

    // Collision detection functions

    /// <summary>Detect collision between two spheres</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionSpheres(
        Vector3 center1,
        float radius1,
        Vector3 center2,
        float radius2
    );

    /// <summary>Detect collision between two bounding boxes</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionBoxes(BoundingBox box1, BoundingBox box2);

    /// <summary>Detect collision between box and sphere</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool CheckCollisionBoxSphere(BoundingBox box, Vector3 center, float radius);

    /// <summary>Detect collision between ray and sphere</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial RayCollision GetRayCollisionSphere(Ray ray, Vector3 center, float radius);

    /// <summary>Detect collision between ray and box</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial RayCollision GetRayCollisionBox(Ray ray, BoundingBox box);

    /// <summary>Get collision info between ray and mesh</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial RayCollision GetRayCollisionMesh(Ray ray, Mesh mesh, Matrix4x4 transform);

    /// <summary>Get collision info between ray and triangle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial RayCollision GetRayCollisionTriangle(Ray ray, Vector3 p1, Vector3 p2, Vector3 p3);

    /// <summary>Get collision info between ray and quad</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial RayCollision GetRayCollisionQuad(Ray ray, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4);


    //------------------------------------------------------------------------------------
    // Audio Loading and Playing Functions (Module: audio)
    //------------------------------------------------------------------------------------

    // Audio device management functions

    /// <summary>Initialize audio device and context</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void InitAudioDevice();

    /// <summary>Close the audio device and context</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CloseAudioDevice();

    /// <summary>Check if audio device has been initialized successfully</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsAudioDeviceReady();

    /// <summary>Set master volume (listener)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMasterVolume(float volume);

    /// <summary>Get master volume (listener)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetMasterVolume();


    // Wave/Sound loading/unloading functions

    /// <summary>Load wave data from file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Wave LoadWave(sbyte* fileName);

    /// <summary>Load wave from memory buffer, fileType refers to extension: i.e. ".wav"</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Wave LoadWaveFromMemory(sbyte* fileType, byte* fileData, int dataSize);

    /// <summary>Checks if wave data is valid (data loaded and parameters)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsWaveValid(Wave wave);

    /// <summary>Load sound from file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Sound LoadSound(sbyte* fileName);

    /// <summary>Load sound from wave data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Sound LoadSoundFromWave(Wave wave);

    /// <summary>Create a new sound that shares the same sample data as the source sound, does not own the sound data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Sound LoadSoundAlias(Sound source);

    /// <summary>Checks if a sound is valid (data loaded and buffers initialized)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsSoundValid(Sound sound);

    /// <summary>Update sound buffer with new data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateSound(Sound sound, void* data, int sampleCount);

    /// <summary>Unload wave data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadWave(Wave wave);

    /// <summary>Unload sound</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadSound(Sound sound);

    /// <summary>Unload a sound alias (does not deallocate sample data)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadSoundAlias(Sound alias);

    /// <summary>Export wave data to file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportWave(Wave wave, sbyte* fileName);

    /// <summary>Export wave sample data to code (.h)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool ExportWaveAsCode(Wave wave, sbyte* fileName);


    // Wave/Sound management functions

    /// <summary>Play a sound</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PlaySound(Sound sound);

    /// <summary>Stop playing a sound</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void StopSound(Sound sound);

    /// <summary>Pause a sound</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PauseSound(Sound sound);

    /// <summary>Resume a paused sound</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ResumeSound(Sound sound);

    /// <summary>Check if a sound is currently playing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsSoundPlaying(Sound sound);

    /// <summary>Set volume for a sound (1.0 is max level)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetSoundVolume(Sound sound, float volume);

    /// <summary>Set pitch for a sound (1.0 is base level)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetSoundPitch(Sound sound, float pitch);

    /// <summary>Set pan for a sound (0.5 is center)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetSoundPan(Sound sound, float pan);

    /// <summary>Copy a wave to a new wave</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Wave WaveCopy(Wave wave);

    /// <summary>Crop a wave to defined frames range</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void WaveCrop(Wave* wave, int initFrame, int finalFrame);

    /// <summary>Convert wave data to desired format</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void WaveFormat(Wave* wave, int sampleRate, int sampleSize, int channels);

    /// <summary>Get samples data from wave as a floats array</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float* LoadWaveSamples(Wave wave);

    /// <summary>Unload samples data loaded with LoadWaveSamples()</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadWaveSamples(float* samples);

    // Music management functions

    /// <summary>Load music stream from file</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Music LoadMusicStream(sbyte* fileName);

    /// <summary>Load music stream from memory buffer, fileType refers to extension: i.e. ".wav"</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Music LoadMusicStreamFromMemory(sbyte* fileType, byte* data, int dataSize);

    /// <summary>Checks if a music stream is valid (context and buffers initialized)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMusicValid(Music music);

    /// <summary>Unload music stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadMusicStream(Music music);

    /// <summary>Start music playing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PlayMusicStream(Music music);

    /// <summary>Check if music is playing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsMusicStreamPlaying(Music music);

    /// <summary>Updates buffers for music streaming</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateMusicStream(Music music);

    /// <summary>Stop music playing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void StopMusicStream(Music music);

    /// <summary>Pause music playing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PauseMusicStream(Music music);

    /// <summary>Resume playing paused music</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ResumeMusicStream(Music music);

    /// <summary>Seek music to a position (in seconds)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SeekMusicStream(Music music, float position);

    /// <summary>Set volume for music (1.0 is max level)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMusicVolume(Music music, float volume);

    /// <summary>Set pitch for a music (1.0 is base level)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMusicPitch(Music music, float pitch);

    /// <summary>Set pan for a music (0.5 is center)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetMusicPan(Music music, float pan);

    /// <summary>Get music time length (in seconds)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetMusicTimeLength(Music music);

    /// <summary>Get current music time played (in seconds)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float GetMusicTimePlayed(Music music);


    // AudioStream management functions

    /// <summary>Init audio stream (to stream raw audio pcm data)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial AudioStream LoadAudioStream(uint sampleRate, uint sampleSize, uint channels);

    /// <summary>Checks if an audio stream is valid (buffers initialized)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsAudioStreamValid(AudioStream stream);

    /// <summary>Unload audio stream and free memory</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UnloadAudioStream(AudioStream stream);

    /// <summary>Update audio stream buffers with data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateAudioStream(AudioStream stream, void* data, int frameCount);

    /// <summary>Check if any audio stream buffers requires refill</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsAudioStreamProcessed(AudioStream stream);

    /// <summary>Play audio stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PlayAudioStream(AudioStream stream);

    /// <summary>Pause audio stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void PauseAudioStream(AudioStream stream);

    /// <summary>Resume audio stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ResumeAudioStream(AudioStream stream);

    /// <summary>Check if audio stream is playing</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial CBool IsAudioStreamPlaying(AudioStream stream);

    /// <summary>Stop audio stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void StopAudioStream(AudioStream stream);

    /// <summary>Set volume for audio stream (1.0 is max level)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAudioStreamVolume(AudioStream stream, float volume);

    /// <summary>Set pitch for audio stream (1.0 is base level)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAudioStreamPitch(AudioStream stream, float pitch);

    /// <summary>Set pan for audio stream (0.5 is centered)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAudioStreamPan(AudioStream stream, float pan);

    /// <summary>Default size for new audio streams</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAudioStreamBufferSizeDefault(int size);

    /// <summary>Audio thread callback to request new data</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SetAudioStreamCallback(
        AudioStream stream,
        delegate* unmanaged[Cdecl]<void*, uint, void> callback
    );

    /// <summary>Attach audio stream processor to stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void AttachAudioStreamProcessor(
        AudioStream stream,
        delegate* unmanaged[Cdecl]<void*, uint, void> processor
    );

    /// <summary>Detach audio stream processor from stream</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DetachAudioStreamProcessor(
        AudioStream stream,
        delegate* unmanaged[Cdecl]<void*, uint, void> processor
    );

    /// <summary>Attach audio stream processor to the entire audio pipeline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void AttachAudioMixedProcessor(
        delegate* unmanaged[Cdecl]<void*, uint, void> processor
    );

    /// <summary>Detach audio stream processor from the entire audio pipeline</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DetachAudioMixedProcessor(
        delegate* unmanaged[Cdecl]<void*, uint, void> processor
    );
}
