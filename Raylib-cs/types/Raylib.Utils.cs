using System.Runtime.InteropServices;
using System.Numerics;
using System;

namespace Raylib_cs;

public static unsafe partial class Raylib
{
    /// <summary>Initialize window and OpenGL context</summary>
    public static void InitWindow(int width, int height, string title)
    {
        using Utf8Buffer str1 = title.ToUtf8Buffer();
        InitWindow(width, height, str1.AsPointer());
    }

    /// <summary>Set title for window (only PLATFORM_DESKTOP)</summary>
    public static void SetWindowTitle(string title)
    {
        using Utf8Buffer str1 = title.ToUtf8Buffer();
        SetWindowTitle(str1.AsPointer());
    }

    /// <summary>Set icon for window (multiple images, RGBA 32bit)</summary>
    public static void SetWindowIcons(Image[] images)
    {
        if (images == null || images.Length == 0)
        {
            return;
        }

        fixed (Image* imagesPtr = images)
        {
            SetWindowIcons(imagesPtr, images.Length);
        }
    }

    /// <summary>Get the human-readable, UTF-8 encoded name of the specified monitor</summary>
    public static string GetMonitorName_(int monitor)
    {
        return Utf8StringUtils.GetUTF8String(GetMonitorName(monitor));
    }

    /// <summary>Get clipboard text content</summary>
    public static string GetClipboardText_()
    {
        return Utf8StringUtils.GetUTF8String(GetClipboardText());
    }

    /// <summary>Set clipboard text content</summary>
    public static void SetClipboardText(string text)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        SetClipboardText(str1.AsPointer());
    }

    /// <summary>Open URL with default system browser (if available)</summary>
    public static void OpenURL(string url)
    {
        using Utf8Buffer str1 = url.ToUtf8Buffer();
        OpenURL(str1.AsPointer());
    }

    /// <summary>Set internal gamepad mappings (SDL_GameControllerDB)</summary>
    public static int SetGamepadMappings(string mappings)
    {
        using Utf8Buffer str1 = mappings.ToUtf8Buffer();
        return SetGamepadMappings(str1.AsPointer());
    }

    /// <summary>Load shader from files and bind default locations</summary>
    public static Shader LoadShader(string vsFileName, string fsFileName)
    {
        using AnsiBuffer str1 = vsFileName.ToAnsiBuffer();
        using AnsiBuffer str2 = fsFileName.ToAnsiBuffer();
        return LoadShader(str1.AsPointer(), str2.AsPointer());
    }

    /// <summary>Load shader from code string and bind default locations</summary>
    public static Shader LoadShaderFromMemory(string vsCode, string fsCode)
    {
        using Utf8Buffer str1 = vsCode.ToUtf8Buffer();
        using Utf8Buffer str2 = fsCode.ToUtf8Buffer();
        return LoadShaderFromMemory(str1.AsPointer(), str2.AsPointer());
    }

    /// <summary>Get shader uniform location</summary>
    public static int GetShaderLocation(Shader shader, string uniformName)
    {
        using Utf8Buffer str1 = uniformName.ToUtf8Buffer();
        return GetShaderLocation(shader, str1.AsPointer());
    }

    /// <summary>Get shader attribute location</summary>
    public static int GetShaderLocationAttrib(Shader shader, string attribName)
    {
        using Utf8Buffer str1 = attribName.ToUtf8Buffer();
        return GetShaderLocationAttrib(shader, str1.AsPointer());
    }

    /// <summary>Takes a screenshot of current screen (saved a .png)</summary>
    public static void TakeScreenshot(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        TakeScreenshot(str1.AsPointer());
    }

    /// <summary>Check file extension</summary>
    public static CBool IsFileExtension(string fileName, string ext)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        using AnsiBuffer str2 = ext.ToAnsiBuffer();
        return IsFileExtension(str1.AsPointer(), str2.AsPointer());
    }

    /// <summary>Get file modification time (last write time)</summary>
    public static long GetFileModTime(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return GetFileModTime(str1.AsPointer()).Value;
    }

    /// <summary>Load image from file into CPU memory (RAM)</summary>
    public static Image LoadImage(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadImage(str1.AsPointer());
    }

    /// <summary>Load image sequence from memory buffer</summary>
    public static Image LoadImageAnimFromMemory(string fileType, byte[] fileData, out int frames)
    {
        using AnsiBuffer type = fileType.ToAnsiBuffer();
        fixed (byte* data = fileData)
        {
            int frameCount;
            var result = LoadImageAnimFromMemory(type.AsPointer(), data, fileData.Length, &frameCount);
            frames = frameCount;
            return result;
        }
    }

    /// <summary>Load image from RAW file data</summary>
    public static Image LoadImageRaw(string fileName, int width, int height, PixelFormat format, int headerSize)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadImageRaw(str1.AsPointer(), width, height, format, headerSize);
    }

    /// <summary>Load image sequence from file (frames appended to image.data)</summary>
    public static Image LoadImageAnim(string fileName, out int frames)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        fixed (int* p = &frames)
        {
            return LoadImageAnim(str1.AsPointer(), p);
        }
    }

    /// <summary>
    /// Load image from managed memory, fileType refers to extension: i.e. ".png"
    /// </summary>
    public static Image LoadImageFromMemory(string fileType, byte[] fileData)
    {
        using AnsiBuffer fileTypeNative = fileType.ToAnsiBuffer();
        fixed (byte* fileDataNative = fileData)
        {
            Image image = LoadImageFromMemory(fileTypeNative.AsPointer(), fileDataNative, fileData.Length);
            return image;
        }
    }

    /// <summary>Export image data to file</summary>
    public static CBool ExportImage(Image image, string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return ExportImage(image, str1.AsPointer());
    }

    /// <summary>Export image as code file defining an array of bytes</summary>
    public static CBool ExportImageAsCode(Image image, string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return ExportImageAsCode(image, str1.AsPointer());
    }

    /// <summary>Show trace log messages (LOG_DEBUG, LOG_INFO, LOG_WARNING, LOG_ERROR)</summary>
    public static void TraceLog(TraceLogLevel logLevel, string text)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        TraceLog(logLevel, str1.AsPointer());
    }

    /// <summary>Set shader uniform value vector</summary>
    public static void SetShaderValueV<T>(
        Shader shader,
        int locIndex,
        T[] values,
        ShaderUniformDataType uniformType,
        int count
    ) where T : unmanaged
    {
        SetShaderValueV(shader, locIndex, (Span<T>)values, uniformType, count);
    }

    /// <summary>Set shader uniform value vector</summary>
    public static void SetShaderValueV<T>(
        Shader shader,
        int locIndex,
        Span<T> values,
        ShaderUniformDataType uniformType,
        int count
    ) where T : unmanaged
    {
        fixed (T* valuePtr = values)
        {
            SetShaderValueV(shader, locIndex, valuePtr, uniformType, count);
        }
    }

    /// <summary>Set shader uniform value</summary>
    public static void SetShaderValue<T>(Shader shader, int locIndex, T value, ShaderUniformDataType uniformType)
        where T : unmanaged
    {
        SetShaderValue(shader, locIndex, &value, uniformType);
    }

    /// <summary>Set shader uniform value</summary>
    public static void SetShaderValue<T>(
        Shader shader,
        int locIndex,
        T[] values,
        ShaderUniformDataType uniformType
    ) where T : unmanaged
    {
        SetShaderValue(shader, locIndex, (Span<T>)values, uniformType);
    }

    /// <summary>Set shader uniform value</summary>
    public static void SetShaderValue<T>(
        Shader shader,
        int locIndex,
        Span<T> values,
        ShaderUniformDataType uniformType
    ) where T : unmanaged
    {
        fixed (T* valuePtr = values)
        {
            SetShaderValue(shader, locIndex, valuePtr, uniformType);
        }
    }

    /// <summary>C++ style memory allocator</summary>
    public static T* New<T>(uint count) where T : unmanaged
    {
        return (T*)MemAlloc(count * (uint)sizeof(T));
    }

    /// <summary>Save data to file from an unmanaged type</summary>
    /// <returns>True if the operation was successfully</returns>
    public static CBool SaveFileData<T>(T data, string fileName) where T : unmanaged
    {
        using AnsiBuffer ansiBuffer = fileName.ToAnsiBuffer();
        return SaveFileData(ansiBuffer.AsPointer(), &data, sizeof(T));
    }

    /// <summary>Save data to file from an unmanaged type</summary>
    /// <returns>True if the operation was successfully</returns>
    public static CBool SaveFileData<T>(T[] data, string fileName) where T : unmanaged
    {
        if (data == null || data.Length == 0)
        {
            return false;
        }

        fixed (T* ptr = data)
        {
            using AnsiBuffer ansiBuffer = fileName.ToAnsiBuffer();
            return SaveFileData(ansiBuffer.AsPointer(), ptr, sizeof(T) * data.Length);
        }
    }

    /// <summary>Export data to code (.h), returns true on success</summary>
    public static CBool ExportDataAsCode(byte[] data, string fileName)
    {
        fixed (byte* ptr = data)
        {
            using AnsiBuffer name = fileName.ToAnsiBuffer();
            return ExportDataAsCode(ptr, data.Length, name.AsPointer());
        }
    }

    /// <summary>Load file data as byte array (read)</summary>
    public static byte* LoadFileData(string fileName, ref int bytesRead)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        fixed (int* p = &bytesRead)
        {
            return LoadFileData(str1.AsPointer(), p);
        }
    }

    public static byte[] LoadFileData(string fileName)
    {
        int length = 0;
        byte* data = LoadFileData(fileName, ref length);

        if (data == null)
        {
            return Array.Empty<byte>();
        }


        byte[] arr = new byte[length];
        Marshal.Copy((IntPtr)data, arr, 0, length);
        UnloadFileData(data);
        return arr;
    }

    /// <summary>
    /// Load file data as an array of unmanaged types
    /// </summary>
    public static T[] LoadFileData<T>(string fileName) where T : unmanaged
    {
        int length = 0;
        byte* data = LoadFileData(fileName, ref length);
        Span<T> values = new Span<T>(data, length / sizeof(T));
        T[] arr = values.ToArray();
        UnloadFileData(data);
        return arr;
    }

    /// <summary>Get dropped files names (memory should be freed)</summary>
    public static string[] GetDroppedFiles()
    {
        FilePathList filePathList = LoadDroppedFiles();
        string[] files = new string[filePathList.Count];

        for (uint i = 0; i < filePathList.Count; i++)
        {
            files[i] = filePathList[i];
        }

        UnloadDroppedFiles(filePathList);

        return files;
    }

    /// <summary>Get gamepad internal name id</summary>
    public static string GetGamepadName_(int gamepad)
    {
        return Utf8StringUtils.GetUTF8String(GetGamepadName(gamepad));
    }

    /// <summary>Update camera position for selected mode</summary>
    public static void UpdateCamera(ref Camera3D camera, CameraMode mode)
    {
        fixed (Camera3D* c = &camera)
        {
            UpdateCamera(c, mode);
        }
    }

    /// <summary>Update camera movement/rotation</summary>
    public static void UpdateCameraPro(ref Camera3D camera, Vector3 movement, Vector3 rotation, float zoom)
    {
        fixed (Camera3D* c = &camera)
        {
            UpdateCameraPro(c, movement, rotation, zoom);
        }
    }

    /// <summary>Returns the cameras forward vector (normalized)</summary>
    public static Vector3 GetCameraForward(ref Camera3D camera)
    {
        fixed (Camera3D* c = &camera)
        {
            return GetCameraForward(c);
        }
    }

    /// <summary>
    /// Returns the cameras up vector (normalized)<br/>
    /// NOTE: The up vector might not be perpendicular to the forward vector
    /// </summary>
    public static Vector3 GetCameraUp(ref Camera3D camera)
    {
        fixed (Camera3D* c = &camera)
        {
            return GetCameraUp(c);
        }
    }

    /// <summary>Returns the cameras right vector (normalized)</summary>
    public static Vector3 GetCameraRight(ref Camera3D camera)
    {
        fixed (Camera3D* c = &camera)
        {
            return GetCameraRight(c);
        }
    }

    /// <summary>Moves the camera in its forward direction</summary>
    public static void CameraMoveForward(ref Camera3D camera, float distance, CBool moveInWorldPlane)
    {
        fixed (Camera3D* c = &camera)
        {
            CameraMoveForward(c, distance, moveInWorldPlane);
        }
    }

    /// <summary>Moves the camera in its up direction</summary>
    public static void CameraMoveUp(ref Camera3D camera, float distance)
    {
        fixed (Camera3D* c = &camera)
        {
            CameraMoveUp(c, distance);
        }
    }

    /// <summary>Moves the camera target in its current right direction</summary>
    public static void CameraMoveRight(ref Camera3D camera, float distance, CBool moveInWorldPlane)
    {
        fixed (Camera3D* c = &camera)
        {
            CameraMoveRight(c, distance, moveInWorldPlane);
        }
    }

    /// <summary>Moves the camera position closer/farther to/from the camera target</summary>
    public static void CameraMoveToTarget(ref Camera3D camera, float delta)
    {
        fixed (Camera3D* c = &camera)
        {
            CameraMoveToTarget(c, delta);
        }
    }

    /// <summary>
    /// Rotates the camera around its up vector<br/>
    /// If rotateAroundTarget is false, the camera rotates around its position
    /// </summary>
    public static void CameraYaw(ref Camera3D camera, float angle, CBool rotateAroundTarget)
    {
        fixed (Camera3D* c = &camera)
        {
            CameraYaw(c, angle, rotateAroundTarget);
        }
    }

    /// <summary>
    /// Rotates the camera around its right vector
    /// </summary>
    public static void CameraPitch(ref Camera3D camera,
        float angle,
        CBool lockView,
        CBool rotateAroundTarget,
        CBool rotateUp
    )
    {
        fixed (Camera3D* c = &camera)
        {
            CameraPitch(c, angle, lockView, rotateAroundTarget, rotateUp);
        }
    }

    /// <summary>Rotates the camera around its forward vector</summary>
    public static void CameraRoll(ref Camera3D camera, float angle)
    {
        fixed (Camera3D* c = &camera)
        {
            CameraRoll(c, angle);
        }
    }

    /// <summary>Returns the camera view matrix</summary>
    public static Matrix4x4 GetCameraViewMatrix(ref Camera3D camera)
    {
        fixed (Camera3D* c = &camera)
        {
            return GetCameraViewMatrix(c);
        }
    }

    /// <summary>Returns the camera projection matrix</summary>
    public static Matrix4x4 GetCameraProjectionMatrix(ref Camera3D camera, float aspect)
    {
        fixed (Camera3D* c = &camera)
        {
            return GetCameraProjectionMatrix(c, aspect);
        }
    }

    /// <summary>
    /// Check the collision between two lines defined by two points each, returns collision point by reference
    /// </summary>
    public static CBool CheckCollisionLines(
        Vector2 startPos1,
        Vector2 endPos1,
        Vector2 startPos2,
        Vector2 endPos2,
        ref Vector2 collisionPoint
    )
    {
        fixed (Vector2* p = &collisionPoint)
        {
            return CheckCollisionLines(startPos1, endPos1, startPos2, endPos2, p);
        }
    }

    public static CBool CheckCollisionPointPoly(Vector2 point, Vector2[] points)
    {
        fixed (Vector2* p = points)
        {
            return CheckCollisionPointPoly(point, p, points.Length);
        }
    }

    /// <summary>Generate image: grayscale image from text data</summary>
    public static Image GenImageText(int width, int height, string text)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        return GenImageText(width, height, str1.AsPointer());
    }

    /// <summary>Create an image from text (default font)</summary>
    public static Image ImageText(string text, int fontSize, Color color)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        return ImageText(str1.AsPointer(), fontSize, color);
    }

    /// <summary>Create an image from text (custom sprite font)</summary>
    public static Image ImageTextEx(Font font, string text, float fontSize, float spacing, Color tint)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        return ImageTextEx(font, str1.AsPointer(), fontSize, spacing, tint);
    }

    /// <summary>Convert image to POT (power-of-two)</summary>
    public static void ImageToPOT(ref Image image, Color fill)
    {
        fixed (Image* p = &image)
        {
            ImageToPOT(p, fill);
        }
    }

    /// <summary>Convert image data to desired format</summary>
    public static void ImageFormat(ref Image image, PixelFormat newFormat)
    {
        fixed (Image* p = &image)
        {
            ImageFormat(p, newFormat);
        }
    }

    /// <summary>Apply alpha mask to image</summary>
    public static void ImageAlphaMask(ref Image image, Image alphaMask)
    {
        fixed (Image* p = &image)
        {
            ImageAlphaMask(p, alphaMask);
        }
    }

    /// <summary>Clear alpha channel to desired color</summary>
    public static void ImageAlphaClear(ref Image image, Color color, float threshold)
    {
        fixed (Image* p = &image)
        {
            ImageAlphaClear(p, color, threshold);
        }
    }

    /// <summary>Crop image depending on alpha value</summary>
    public static void ImageAlphaCrop(ref Image image, float threshold)
    {
        fixed (Image* p = &image)
        {
            ImageAlphaCrop(p, threshold);
        }
    }

    /// <summary>Premultiply alpha channel</summary>
    public static void ImageAlphaPremultiply(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageAlphaPremultiply(p);
        }
    }

    /// <summary>Apply Gaussian blur using a box blur approximation</summary>
    public static void ImageBlurGaussian(ref Image image, int blurSize)
    {
        fixed (Image* p = &image)
        {
            ImageBlurGaussian(p, blurSize);
        }
    }

    /// <summary>Apply custom square convolution kernel to image</summary>
    public static void ImageKernelConvolution(ref Image image, float[] kernel)
    {
        fixed (Image* imagePtr = &image)
        {
            fixed (float* kernelPtr = kernel)
            {
                ImageKernelConvolution(imagePtr, kernelPtr, kernel.Length);
            }
        }
    }

    /// <summary>Crop an image to a defined rectangle</summary>
    public static void ImageCrop(ref Image image, Rectangle crop)
    {
        fixed (Image* p = &image)
        {
            ImageCrop(p, crop);
        }
    }

    /// <summary>Resize image (Bicubic scaling algorithm)</summary>
    public static void ImageResize(ref Image image, int newWidth, int newHeight)
    {
        fixed (Image* p = &image)
        {
            ImageResize(p, newWidth, newHeight);
        }
    }

    /// <summary>Resize image (Nearest-Neighbor scaling algorithm)</summary>
    public static void ImageResizeNN(ref Image image, int newWidth, int newHeight)
    {
        fixed (Image* p = &image)
        {
            ImageResizeNN(p, newWidth, newHeight);
        }
    }

    /// <summary>Resize canvas and fill with color</summary>
    public static void ImageResizeCanvas(
        ref Image image,
        int newWidth,
        int newHeight,
        int offsetX,
        int offsetY,
        Color color
    )
    {
        fixed (Image* p = &image)
        {
            ImageResizeCanvas(p, newWidth, newHeight, offsetX, offsetY, color);
        }
    }

    /// <summary>Generate all mipmap levels for a provided image</summary>
    public static void ImageMipmaps(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageMipmaps(p);
        }
    }

    /// <summary>Dither image data to 16bpp or lower (Floyd-Steinberg dithering)</summary>
    public static void ImageDither(ref Image image, int rBpp, int gBpp, int bBpp, int aBpp)
    {
        fixed (Image* p = &image)
        {
            ImageDither(p, rBpp, gBpp, bBpp, aBpp);
        }
    }

    /// <summary>Flip image vertically</summary>
    public static void ImageFlipVertical(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageFlipVertical(p);
        }
    }

    /// <summary>Flip image horizontally</summary>
    public static void ImageFlipHorizontal(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageFlipHorizontal(p);
        }
    }

    /// <summary>Rotate image by input angle in degrees (-359 to 359)</summary>
    public static void ImageRotate(ref Image image, int degrees)
    {
        fixed (Image* p = &image)
        {
            ImageRotate(p, degrees);
        }
    }

    /// <summary>Rotate image clockwise 90deg</summary>
    public static void ImageRotateCW(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageRotateCW(p);
        }
    }

    /// <summary>Rotate image counter-clockwise 90deg</summary>
    public static void ImageRotateCCW(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageRotateCCW(p);
        }
    }

    /// <summary>Modify image color: tint</summary>
    public static void ImageColorTint(ref Image image, Color color)
    {
        fixed (Image* p = &image)
        {
            ImageColorTint(p, color);
        }
    }

    /// <summary>Modify image color: invert</summary>
    public static void ImageColorInvert(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageColorInvert(p);
        }
    }

    /// <summary>Modify image color: grayscale</summary>
    public static void ImageColorGrayscale(ref Image image)
    {
        fixed (Image* p = &image)
        {
            ImageColorGrayscale(p);
        }
    }

    /// <summary>Modify image color: contrast (-100 to 100)</summary>
    public static void ImageColorContrast(ref Image image, float contrast)
    {
        fixed (Image* p = &image)
        {
            ImageColorContrast(p, contrast);
        }
    }

    /// <summary>Modify image color: brightness (-255 to 255)</summary>
    public static void ImageColorBrightness(ref Image image, int brightness)
    {
        fixed (Image* p = &image)
        {
            ImageColorBrightness(p, brightness);
        }
    }

    /// <summary>Modify image color: replace color</summary>
    public static void ImageColorReplace(ref Image image, Color color, Color replace)
    {
        fixed (Image* p = &image)
        {
            ImageColorReplace(p, color, replace);
        }
    }

    /// <summary>Clear image background with given color</summary>
    public static void ImageClearBackground(ref Image dst, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageClearBackground(p, color);
        }
    }

    /// <summary>Draw pixel within an image</summary>
    public static void ImageDrawPixel(ref Image dst, int posX, int posY, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawPixel(p, posX, posY, color);
        }
    }

    /// <summary>Draw pixel within an image (Vector version)</summary>
    public static void ImageDrawPixelV(ref Image dst, Vector2 position, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawPixelV(p, position, color);
        }
    }

    /// <summary>Draw line within an image</summary>
    public static void ImageDrawLine(
        ref Image dst,
        int startPosX,
        int startPosY,
        int endPosX,
        int endPosY,
        Color color
    )
    {
        fixed (Image* p = &dst)
        {
            ImageDrawLine(p, startPosX, startPosY, endPosX, endPosY, color);
        }
    }

    /// <summary>Draw line within an image (Vector version)</summary>
    public static void ImageDrawLineV(ref Image dst, Vector2 start, Vector2 end, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawLineV(p, start, end, color);
        }
    }

    /// <summary>Draw a line defining thickness within an image</summary>
    public static void ImageDrawLineEx(ref Image dst, Vector2 start, Vector2 end, int thick, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawLineEx(p, start, end, thick, color);
        }
    }

    /// <summary>Draw circle within an image</summary>
    public static void ImageDrawCircle(ref Image dst, int centerX, int centerY, int radius, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawCircle(p, centerX, centerY, radius, color);
        }
    }

    public static void ImageDrawCircleLines(ref Image dst, int centerX, int centerY, int radius, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawCircleLines(p, centerX, centerY, radius, color);
        }
    }

    public static void ImageDrawCircleLinesV(ref Image dst, Vector2 center, int radius, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawCircleLinesV(p, center, radius, color);
        }
    }

    /// <summary>Draw circle within an image (Vector version)</summary>
    public static void ImageDrawCircleV(ref Image dst, Vector2 center, int radius, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawCircleV(p, center, radius, color);
        }
    }

    /// <summary>Draw rectangle within an image</summary>
    public static void ImageDrawRectangle(ref Image dst, int posX, int posY, int width, int height, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawRectangle(p, posX, posY, width, height, color);
        }
    }

    /// <summary>Draw rectangle within an image (Vector version)</summary>
    public static void ImageDrawRectangleV(ref Image dst, Vector2 position, Vector2 size, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawRectangleV(p, position, size, color);
        }
    }

    /// <summary>Draw rectangle within an image</summary>
    public static void ImageDrawRectangleRec(ref Image dst, Rectangle rec, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawRectangleRec(p, rec, color);
        }
    }

    /// <summary>Draw rectangle lines within an image</summary>
    public static void ImageDrawRectangleLines(ref Image dst, Rectangle rec, int thick, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawRectangleLines(p, rec, thick, color);
        }
    }

    /// <summary>Draw triangle within an image</summary>
    public static void ImageDrawTriangle(ref Image dst, Vector2 v1, Vector2 v2, Vector2 v3, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawTriangle(p, v1, v2, v3, color);
        }
    }

    /// <summary>Draw triangle with interpolated colors within an image</summary>
    public static void ImageDrawTriangleEx(ref Image dst, Vector2 v1, Vector2 v2, Vector2 v3, Color c1, Color c2,
        Color c3)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawTriangleEx(p, v1, v2, v3, c1, c2, c3);
        }
    }

    /// <summary>Draw triangle outline within an image</summary>
    public static void ImageDrawTriangleLines(ref Image dst, Vector2 v1, Vector2 v2, Vector2 v3, Color color)
    {
        fixed (Image* p = &dst)
        {
            ImageDrawTriangleLines(p, v1, v2, v3, color);
        }
    }

    /// <summary>Draw a triangle fan defined by points within an image (first vertex is the center)</summary>
    public static void ImageDrawTriangleFan(ref Image dst, Vector2[] points, Color color)
    {
        fixed (Image* imagePtr = &dst)
        {
            fixed (Vector2* vec2Ptr = points)
            {
                ImageDrawTriangleFan(imagePtr, vec2Ptr, points.Length, color);
            }
        }
    }

    /// <summary>Draw a triangle strip defined by points within an image</summary>
    public static void ImageDrawTriangleStrip(ref Image dst, Vector2[] points, Color color)
    {
        fixed (Image* imagePtr = &dst)
        {
            fixed (Vector2* vec2Ptr = points)
            {
                ImageDrawTriangleStrip(imagePtr, vec2Ptr, points.Length, color);
            }
        }
    }

    /// <summary>Draw a source image within a destination image (tint applied to source)</summary>
    public static void ImageDraw(ref Image dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint)
    {
        fixed (Image* p = &dst)
        {
            ImageDraw(p, src, srcRec, dstRec, tint);
        }
    }

    /// <summary>Draw text (using default font) within an image (destination)</summary>
    public static void ImageDrawText(ref Image dst, string text, int x, int y, int fontSize, Color color)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        fixed (Image* p = &dst)
        {
            ImageDrawText(p, str1.AsPointer(), x, y, fontSize, color);
        }
    }

    /// <summary>Draw text (custom sprite font) within an image (destination)</summary>
    public static void ImageDrawTextEx(
        ref Image dst,
        Font font,
        string text,
        Vector2 position,
        int fontSize,
        float spacing,
        Color color
    )
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        fixed (Image* p = &dst)
        {
            ImageDrawTextEx(p, font, str1.AsPointer(), position, fontSize, spacing, color);
        }
    }

    /// <summary>Load texture from file into GPU memory (VRAM)</summary>
    public static Texture2D LoadTexture(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadTexture(str1.AsPointer());
    }

    /// <summary>Update GPU texture with new data</summary>
    public static void UpdateTexture<T>(Texture2D texture, T[] pixels) where T : unmanaged
    {
        UpdateTexture(texture, (ReadOnlySpan<T>)pixels);
    }

    /// <summary>Update GPU texture with new data</summary>
    public static void UpdateTexture<T>(Texture2D texture, ReadOnlySpan<T> pixels) where T : unmanaged
    {
        fixed (void* pixelPtr = pixels)
        {
            UpdateTexture(texture, pixelPtr);
        }
    }

    /// <summary>Update GPU texture rectangle with new data</summary>
    public static void UpdateTextureRec<T>(Texture2D texture, Rectangle rec, T[] pixels) where T : unmanaged
    {
        UpdateTextureRec(texture, rec, (ReadOnlySpan<T>)pixels);
    }

    /// <summary>Update GPU texture rectangle with new data</summary>
    public static void UpdateTextureRec<T>(Texture2D texture, Rectangle rec, ReadOnlySpan<T> pixels) where T : unmanaged
    {
        fixed (void* pixelPtr = pixels)
        {
            UpdateTextureRec(texture, rec, pixelPtr);
        }
    }

    /// <summary>Generate GPU mipmaps for a texture</summary>
    public static void GenTextureMipmaps(ref Texture2D texture)
    {
        fixed (Texture2D* p = &texture)
        {
            GenTextureMipmaps(p);
        }
    }

    /// <summary>Load font from file into GPU memory (VRAM)</summary>
    public static Font LoadFont(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadFont(str1.AsPointer());
    }

    /// <summary>
    /// Load font from file with extended parameters, use NULL for codepoints and 0 for codepointCount to load
    /// the default character set, font size is provided in pixels height
    /// </summary>
    public static Font LoadFontEx(string fileName, int fontSize, int[] codepoints, int codepointCount)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        fixed (int* p = codepoints)
        {
            return LoadFontEx(str1.AsPointer(), fontSize, p, codepointCount);
        }
    }

    /// <summary>
    /// Load font from managed memory, fileType refers to extension: i.e. ".ttf"
    /// </summary>
    public static Font LoadFontFromMemory(
        string fileType,
        byte[] fileData,
        int fontSize,
        int[] codepoints,
        int codepointCount
    )
    {
        using AnsiBuffer fileTypeNative = fileType.ToAnsiBuffer();
        fixed (byte* fileDataNative = fileData)
        {
            fixed (int* fontCharsNative = codepoints)
            {
                Font font = LoadFontFromMemory(
                    fileTypeNative.AsPointer(),
                    fileDataNative,
                    fileData.Length,
                    fontSize,
                    fontCharsNative,
                    codepointCount
                );
                return font;
            }
        }
    }

    /// <summary>Upload vertex data into GPU and provided VAO/VBO ids</summary>
    public static void UploadMesh(ref Mesh mesh, CBool dynamic)
    {
        fixed (Mesh* p = &mesh)
        {
            UploadMesh(p, dynamic);
        }
    }

    /// <summary> Update mesh vertex data in GPU for a specific buffer index </summary>
    public static void UpdateMeshBuffer<T>(Mesh mesh, int index, ReadOnlySpan<T> data, int offset) where T : unmanaged
    {
        fixed (void* dataPtr = data)
        {
            UpdateMeshBuffer(mesh, index, dataPtr, data.Length * sizeof(T), offset);
        }
    }

    /// <summary>Set texture for a material map type (MAP_DIFFUSE, MAP_SPECULAR...)</summary>
    public static void SetMaterialTexture(ref Material material, MaterialMapIndex mapType, Texture2D texture)
    {
        fixed (Material* p = &material)
        {
            SetMaterialTexture(p, mapType, texture);
        }
    }

    /// <summary>Set material for a mesh</summary>
    public static void SetModelMeshMaterial(ref Model model, int meshId, int materialId)
    {
        fixed (Model* p = &model)
        {
            SetModelMeshMaterial(p, meshId, materialId);
        }
    }

    /// <summary>Load model animations from file</summary>
    public static ModelAnimation* LoadModelAnimations(string fileName, ref int animCount)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        fixed (int* p = &animCount)
        {
            return LoadModelAnimations(str1.AsPointer(), p);
        }
    }

    /// <summary>Load model animations from file</summary>
    public static Span<ModelAnimation> LoadModelAnimations(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        int count;

        ModelAnimation* result = LoadModelAnimations(str1.AsPointer(), &count);
        return new Span<ModelAnimation>(result, count);
    }

    public static void UnloadModelAnimations(Span<ModelAnimation> animations)
    {
        fixed (ModelAnimation* ptr = animations)
        {
            UnloadModelAnimations(ptr, animations.Length);
        }
    }

    /// <summary>Compute mesh tangents</summary>
    public static void GenMeshTangents(ref Mesh mesh)
    {
        fixed (Mesh* p = &mesh)
        {
            GenMeshTangents(p);
        }
    }

    /// <summary>Update sound buffer with new data</summary>
    public static void UpdateSound<T>(Sound sound, ReadOnlySpan<T> data, int sampleCount) where T : unmanaged
    {
        fixed (T* dataPtr = data)
        {
            UpdateSound(sound, dataPtr, sampleCount);
        }
    }

    /// <summary>Update sound buffer with new data</summary>
    public static void UpdateSound<T>(Sound sound, ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (T* dataPtr = data)
        {
            UpdateSound(sound, dataPtr, data.Length);
        }
    }

    /// <summary>Update audio stream buffers with data</summary>
    public static void UpdateAudioStream<T>(AudioStream sound, ReadOnlySpan<T> data, int frameCount) where T : unmanaged
    {
        fixed (T* dataPtr = data)
        {
            UpdateAudioStream(sound, dataPtr, frameCount);
        }
    }

    /// <summary>Convert wave data to desired format</summary>
    public static void WaveFormat(ref Wave wave, int sampleRate, int sampleSize, int channels)
    {
        fixed (Wave* p = &wave)
        {
            WaveFormat(p, sampleRate, sampleSize, channels);
        }
    }

    /// <summary>Crop a wave to defined frames range</summary>
    public static void WaveCrop(ref Wave wave, int initFrame, int finalFrame)
    {
        fixed (Wave* p = &wave)
        {
            WaveCrop(p, initFrame, finalFrame);
        }
    }

    /// <summary>Draw lines sequence</summary>
    public static void DrawLineStrip(Vector2[] points, int pointCount, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawLineStrip(p, pointCount, color);
        }
    }

    /// <summary>Draw a triangle fan defined by points (first vertex is the center)</summary>
    public static void DrawTriangleFan(Vector2[] points, int pointCount, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawTriangleFan(p, pointCount, color);
        }
    }

    /// <summary>Draw a triangle strip defined by points</summary>
    public static void DrawTriangleStrip(Vector2[] points, int pointCount, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawTriangleStrip(p, pointCount, color);
        }
    }

    /// <summary>Draw spline: Linear, minimum 2 points</summary>
    public static void DrawSplineLinear(Vector2[] points, int pointCount, float thick, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawSplineLinear(p, pointCount, thick, color);
        }
    }

    /// <summary>Draw spline: B-Spline, minimum 4 points</summary>
    public static void DrawSplineBasis(Vector2[] points, int pointCount, float thick, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawSplineBasis(p, pointCount, thick, color);
        }
    }

    /// <summary>Draw spline: Catmull-Rom, minimum 4 points</summary>
    public static void DrawSplineCatmullRom(Vector2[] points, int pointCount, float thick, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawSplineCatmullRom(p, pointCount, thick, color);
        }
    }

    /// <summary>Draw spline: Quadratic Bezier, minimum 3 points (1 control point): [p1, c2, p3, c4...]</summary>
    public static void DrawSplineBezierQuadratic(Vector2[] points, int pointCount, float thick, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawSplineBezierQuadratic(p, pointCount, thick, color);
        }
    }

    /// <summary>Draw spline: Cubic Bezier, minimum 4 points (2 control points): [p1, c2, c3, p4, c5, c6...]</summary>
    public static void DrawSplineBezierCubic(Vector2[] points, int pointCount, float thick, Color color)
    {
        fixed (Vector2* p = points)
        {
            DrawSplineBezierCubic(p, pointCount, thick, color);
        }
    }

    /// <summary>Draw text (using default font)</summary>
    public static void DrawText(string text, int posX, int posY, int fontSize, Color color)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        DrawText(str1.AsPointer(), posX, posY, fontSize, color);
    }

    /// <summary>Draw text using font and additional parameters</summary>
    public static void DrawTextEx(Font font, string text, Vector2 position, float fontSize, float spacing, Color tint)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        DrawTextEx(font, str1.AsPointer(), position, fontSize, spacing, tint);
    }

    /// <summary>Draw text using Font and pro parameters (rotation)</summary>
    public static void DrawTextPro(
        Font font,
        string text,
        Vector2 position,
        Vector2 origin,
        float rotation,
        float fontSize,
        float spacing,
        Color tint
    )
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        DrawTextPro(font, str1.AsPointer(), position, origin, rotation, fontSize, spacing, tint);
    }

    /// <summary>Draw multiple characters (codepoint)</summary>
    public static void DrawTextCodepoints(
        Font font,
        int[] codepoints,
        Vector2 position,
        float fontSize,
        float spacing,
        Color tint
    )
    {
        fixed (int* codepointsPtr = codepoints)
        {
            DrawTextCodepoints(font, codepointsPtr, codepoints.Length, position, fontSize, spacing, tint);
        }
    }

    /// <summary>Measure string size for Font</summary>
    public static Vector2 MeasureTextCodepoints(
        Font font, int[] codepoints, float fontSize, float spacing
    )
    {
        fixed (int* codepointsPtr = codepoints)
        {
            return MeasureTextCodepoints(font, codepointsPtr, codepoints.Length, fontSize, spacing);
        }
    }

    /// <summary>Measure string width for default font</summary>
    public static int MeasureText(string text, int fontSize)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        return MeasureText(str1.AsPointer(), fontSize);
    }

    /// <summary>Measure string size for Font</summary>
    public static Vector2 MeasureTextEx(Font font, string text, float fontSize, float spacing)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        return MeasureTextEx(font, str1.AsPointer(), fontSize, spacing);
    }

    /// <summary>Get all codepoints in a string, codepoints count returned by parameters</summary>
    public static int[] LoadCodepoints(string text, ref int count)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        fixed (int* c = &count)
        {
            int* pointsPtr = LoadCodepoints(str1.AsPointer(), c);
            int[] codepoints = new ReadOnlySpan<int>(pointsPtr, count).ToArray();
            UnloadCodepoints(pointsPtr);
            return codepoints;
        }
    }

    /// <summary>Get total number of codepoints in a UTF8 encoded string</summary>
    public static int GetCodepointCount(string text)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        return GetCodepointCount(str1.AsPointer());
    }

    /// <summary>Get next codepoint in a UTF-8 encoded string, 0x3f('?') is returned on failure</summary>
    public static int GetCodepoint(string text, ref int codepointSize)
    {
        using Utf8Buffer str1 = text.ToUtf8Buffer();
        fixed (int* p = &codepointSize)
        {
            return GetCodepoint(str1.AsPointer(), p);
        }
    }

    /// <summary>Encode one codepoint into UTF-8 byte array (array length returned as parameter)</summary>
    public static string CodepointToUTF8(int codepoint, ref int utf8Size)
    {
        fixed (int* l1 = &utf8Size)
        {
            sbyte* ptr = CodepointToUTF8(codepoint, l1);
            return Utf8StringUtils.GetUTF8String(ptr);
        }
    }

    /// <summary>Load UTF-8 text encoded from codepoints array</summary>
    public static string LoadUTF8(int[] codepoints, int length)
    {
        fixed (int* c1 = codepoints)
        {
            sbyte* ptr = LoadUTF8(c1, length);
            string text = Utf8StringUtils.GetUTF8String(ptr);
            MemFree(ptr);
            return text;
        }
    }

    /// <summary>Draw a model (with texture if set)</summary>
    public static Model LoadModel(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadModel(str1.AsPointer());
    }

    /// <summary>Export mesh data to file, returns true on success</summary>
    public static CBool ExportMesh(Mesh mesh, string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return ExportMesh(mesh, str1.AsPointer());
    }

    /// <summary>Export mesh as code file (.h) defining multiple arrays of vertex attributes</summary>
    public static CBool ExportMeshAsCode(Mesh mesh, string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return ExportMeshAsCode(mesh, str1.AsPointer());
    }

    /// <summary>Draw a triangle strip defined by points</summary>
    public static void DrawTriangleStrip3D(Vector3[] points, int pointCount, Color color)
    {
        fixed (Vector3* p = points)
        {
            DrawTriangleStrip3D(p, pointCount, color);
        }
    }

    /// <summary>Draw multiple mesh instances with material and different transforms</summary>
    public static void DrawMeshInstanced(Mesh mesh, Material material, Matrix4x4[] transforms, int instances)
    {
        fixed (Matrix4x4* p = transforms)
        {
            DrawMeshInstanced(mesh, material, p, instances);
        }
    }

    /// <summary>Load wave data from file</summary>
    public static Wave LoadWave(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadWave(str1.AsPointer());
    }

    /// <summary>
    /// Load wave from managed memory, fileType refers to extension: i.e. ".wav"
    /// </summary>
    public static Wave LoadWaveFromMemory(string fileType, byte[] fileData)
    {
        using AnsiBuffer fileTypeNative = fileType.ToAnsiBuffer();

        fixed (byte* fileDataNative = fileData)
        {
            Wave wave = LoadWaveFromMemory(
                fileTypeNative.AsPointer(),
                fileDataNative,
                fileData.Length
            );

            return wave;
        }
    }

    /// <summary>Load sound from file</summary>
    public static Sound LoadSound(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadSound(str1.AsPointer());
    }

    /// <summary>Export wave data to file</summary>
    public static CBool ExportWave(Wave wave, string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return ExportWave(wave, str1.AsPointer());
    }

    /// <summary>Export wave sample data to code (.h)</summary>
    public static CBool ExportWaveAsCode(Wave wave, string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return ExportWaveAsCode(wave, str1.AsPointer());
    }

    /// <summary>Load music stream from file</summary>
    public static Music LoadMusicStream(string fileName)
    {
        using AnsiBuffer str1 = fileName.ToAnsiBuffer();
        return LoadMusicStream(str1.AsPointer());
    }

    /// <summary>
    /// Load music stream from managed memory, fileType refers to extension: i.e. ".wav"
    /// </summary>
    public static Music LoadMusicStreamFromMemory(string fileType, byte[] fileData)
    {
        using AnsiBuffer fileTypeNative = fileType.ToAnsiBuffer();

        fixed (byte* fileDataNative = fileData)
        {
            Music music = LoadMusicStreamFromMemory(
                fileTypeNative.AsPointer(),
                fileDataNative,
                fileData.Length
            );

            return music;
        }
    }

    /// <summary>
    /// Attach audio stream processor to the entire audio pipeline
    /// </summary>
    public static void AttachAudioMixedProcessor(AudioCallback<float> processor)
    {
        if (AudioMixed.Callback == null)
        {
            AudioMixed.Callback = processor;
            AttachAudioMixedProcessor(&AudioMixed.Processor);
        }
    }

    /// <summary>
    /// Detach audio stream processor from the entire audio pipeline
    /// </summary>
    public static void DetachAudioMixedProcessor(AudioCallback<float> processor)
    {
        if (AudioMixed.Callback == processor)
        {
            DetachAudioMixedProcessor(&AudioMixed.Processor);
            AudioMixed.Callback = null;
        }
    }

    public static string SubText(this string input, int position, int length)
    {
        return input.Substring(position, Math.Min(length, input.Length));
    }

    public static Material GetMaterial(ref Model model, int materialIndex)
    {
        return model.Materials[materialIndex];
    }

    public static Texture2D GetMaterialTexture(ref Model model, int materialIndex, MaterialMapIndex mapIndex)
    {
        return model.Materials[materialIndex].Maps[(int)mapIndex].Texture;
    }

    public static void SetMaterialTexture(
        ref Model model,
        int materialIndex,
        MaterialMapIndex mapIndex,
        ref Texture2D texture
    )
    {
        SetMaterialTexture(&model.Materials[materialIndex], mapIndex, texture);
    }

    public static void SetMaterialShader(ref Model model, int materialIndex, ref Shader shader)
    {
        model.Materials[materialIndex].Shader = shader;
    }

    /// <summary>Get a ray trace from mouse position</summary>
    [ObsoleteAttribute("This method is obsolete. Call GetScreenToWorldRay instead.")]
    public static Ray GetMouseRay(Vector2 mousePosition, Camera3D camera)
    {
        return GetScreenToWorldRay(mousePosition, camera);
    }

    /// <summary>Load automation events list from file, NULL for empty list, capacity = MAX_AUTOMATION_EVENTS</summary>
    public static AutomationEventList LoadAutomationEventList(string fileName)
    {
        using Utf8Buffer str1 = fileName.ToUtf8Buffer();
        return LoadAutomationEventList(str1.AsPointer());
    }

    /// <summary>Export automation events list as text file</summary>
    public static CBool ExportAutomationEventList(AutomationEventList list, string fileName)
    {
        using Utf8Buffer str1 = fileName.ToUtf8Buffer();
        return ExportAutomationEventList(list, str1.AsPointer());
    }

    /// <summary>Set automation event list to record to</summary>
    public static void SetAutomationEventList(ref AutomationEventList list)
    {
        fixed (AutomationEventList* p = &list)
        {
            SetAutomationEventList(p);
        }
    }

    public static int MakeDirectory(string path)
    {
        using AnsiBuffer pathBuffer = path.ToAnsiBuffer();
        return MakeDirectory(pathBuffer.AsPointer());
    }

    public static string GetApplicationDirectoryString()
    {
        return Utf8StringUtils.GetUTF8String(GetApplicationDirectory());
    }

    public static string GetWorkingDirectoryString()
    {
        return Utf8StringUtils.GetUTF8String(GetWorkingDirectory());
    }

    /// <summary>
    /// Load a sequence of random numbers, store them and return them but not before unloading them.
    /// </summary>
    /// <param name="count">Amount of random numbers to load</param>
    /// <param name="min">Minimum random value</param>
    /// <param name="max">Maximum random value</param>
    /// <returns>An array filled with the random numbers</returns>
    public static int[] GetRandomSequence(uint count, int min, int max)
    {
        int temp = min;
        min = Math.Min(min, max);
        max = Math.Max(temp, max);
        int* sequence = LoadRandomSequence(count, min, max);
        int[] output = new int[count];
        //Marshal.Copy((IntPtr)sequence, output, 0, count);
        for (uint i = 0; i < count; i++)
        {
            output[i] = sequence[i];
        }

        UnloadRandomSequence(sequence);
        return output;
    }

    /// <summary>
    /// Load a sequence of random numbers, store them and return them but not before unloading them.
    /// </summary>
    /// <param name="count">Amount of random numbers to load</param>
    /// <param name="min">Minimum random value</param>
    /// <param name="max">Maximum random value</param>
    /// <returns>An array filled with the random numbers</returns>
    public static float[] GetRandomSequence(uint count, float min, float max)
    {
        float temp = min;
        min = Math.Min(min, max);
        max = Math.Max(temp, max);
        const int maxi = 100000;
        int* sequence = LoadRandomSequence(count, 0, maxi);
        float[] output = new float[count];
        for (uint i = 0; i < count; i++)
        {
            int val = sequence[i];
            float norm = (float)val / (float)maxi;
            output[i] = Raymath.Lerp(min, max, norm);
        }

        return output;
    }

    /// <summary>
    /// Create a file in the specified path to save the specified text
    /// </summary>
    public static void SaveFileText(string fileName, string text)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        using AnsiBuffer textBuffer = text.ToAnsiBuffer();
        SaveFileText(fileBuffer.AsPointer(), textBuffer.AsPointer());
    }

    /// <summary>Rename file (if exists)</summary>
    public static int FileRename(string filename, string fileRename)
    {

        using AnsiBuffer fileBuffer = filename.ToAnsiBuffer();
        using AnsiBuffer textBuffer = fileRename.ToAnsiBuffer();
        return FileRename(fileBuffer.AsPointer(), textBuffer.AsPointer());
    }

    /// <summary>Remove file (if exists)</summary>
    public static int FileRemove(string filename)
    {
        using AnsiBuffer fileBuffer = filename.ToAnsiBuffer();
        return FileRemove(fileBuffer.AsPointer());
    }

    /// <summary>Copy file from one path to another, dstPath created if it doesn't exist</summary>
    public static int FileCopy(string srcPath, string dstPath)
    {
        using AnsiBuffer srcBuffer = srcPath.ToAnsiBuffer();
        using AnsiBuffer dstBuffer = dstPath.ToAnsiBuffer();
        return FileCopy(srcBuffer.AsPointer(), dstBuffer.AsPointer());
    }

    /// <summary>Move file from one path to another, dstPath created if it doesn't exist</summary>
    public static int FileMove(string srcPath, string dstPath)
    {
        using AnsiBuffer srcBuffer = srcPath.ToAnsiBuffer();
        using AnsiBuffer dstBuffer = dstPath.ToAnsiBuffer();
        return FileMove(srcBuffer.AsPointer(), dstBuffer.AsPointer());
    }

    /// <summary>Replace text in an existing file</summary>
    public static int FileTextReplace(string fileName, string search, string replacement)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        using AnsiBuffer searchBuffer = search.ToAnsiBuffer();
        using AnsiBuffer replaceBuffer = replacement.ToAnsiBuffer();
        return FileTextReplace(fileBuffer.AsPointer(), searchBuffer.AsPointer(), replaceBuffer.AsPointer());
    }

    /// <summary>Find text in existing file</summary>
    public static int FileTextFindIndex(string fileName, string search)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        using AnsiBuffer searchBuffer = search.ToAnsiBuffer();
        return FileTextFindIndex(fileBuffer.AsPointer(), searchBuffer.AsPointer());
    }

    /// <summary>Check if file exists</summary>
    public static CBool FileExists(string fileName)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        return FileExists(fileBuffer.AsPointer());
    }

    /// <summary>Check if a directory path exists</summary>
    public static CBool DirectoryExists(string dirPath)
    {
        using AnsiBuffer dirBuffer = dirPath.ToAnsiBuffer();
        return DirectoryExists(dirBuffer.AsPointer());
    }

    /// <summary>Get file length in bytes</summary>
    public static int GetFileLength(string fileName)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        return GetFileLength(fileBuffer.AsPointer());
    }

    /// <summary>Get string to extension for a filename string (includes dot: '.png')</summary>
    public static string GetFileExtension(string fileName)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        return Utf8StringUtils.GetUTF8String(GetFileExtension(fileBuffer.AsPointer()));
    }

    /// <summary>Get string to filename for a path string</summary>
    public static string GetFileName(string fileName)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        return Utf8StringUtils.GetUTF8String(GetFileName(fileBuffer.AsPointer()));
    }

    /// <summary>Get filename string without extension </summary>
    public static string GetFileNameWithoutExt(string fileName)
    {
        using AnsiBuffer fileBuffer = fileName.ToAnsiBuffer();
        return Utf8StringUtils.GetUTF8String(GetFileNameWithoutExt(fileBuffer.AsPointer()));
    }

    /// <summary>Get full path for a given fileName with path</summary>
    public static string GetDirectoryPath(string filePath)
    {
        using AnsiBuffer fileBuffer = filePath.ToAnsiBuffer();
        return Utf8StringUtils.GetUTF8String(GetDirectoryPath(fileBuffer.AsPointer()));
    }

    /// <summary>Get previous directory path for a given path</summary>
    public static string GetPrevDirectoryPath(string dirPath)
    {
        using AnsiBuffer dirBuffer = dirPath.ToAnsiBuffer();
        return Utf8StringUtils.GetUTF8String(GetPrevDirectoryPath(dirBuffer.AsPointer()));
    }

    /// <summary>Get current working directory</summary>
    public static string GetWorkingDirectoryAsString()
    {
        return Utf8StringUtils.GetUTF8String(GetWorkingDirectory());
    }

    /// <summary>Get the directory of the running application</summary>
    public static string GetApplicationDirectoryAsString()
    {
        return Utf8StringUtils.GetUTF8String(GetApplicationDirectory());
    }

    /// <summary>Change working directory, return true on success</summary>
    public static CBool ChangeDirectory(string dirPath)
    {
        using AnsiBuffer dirBuffer = dirPath.ToAnsiBuffer();
        return ChangeDirectory(dirBuffer.AsPointer());
    }

    /// <summary>Check if a given path is a file or a directory</summary>
    public static CBool IsPathFile(string path)
    {
        using AnsiBuffer pathBuffer = path.ToAnsiBuffer();
        return IsPathFile(pathBuffer.AsPointer());
    }

    /// <summary>Load directory filepaths</summary>
    public static FilePathList LoadDirectoryFiles(string dirPath)
    {
        using AnsiBuffer dirBuffer = dirPath.ToAnsiBuffer();
        return LoadDirectoryFiles(dirBuffer.AsPointer());
    }

    /// <summary>Get the file count in a directory</summary>
    public static int GetDirectoryFileCount(string dirPath)
    {
        using AnsiBuffer dirBuffer = dirPath.ToAnsiBuffer();
        return GetDirectoryFileCount(dirBuffer.AsPointer());
    }

    /// <summary>Get the file count in a directory with extension filtering and recursive directory scan. Use 'DIR' in the filter string to include directories in the result</summary>
    public static int GetDirectoryFileCountEx(string dirPath, string filter, CBool scanSubdirs)
    {
        using AnsiBuffer dirBuffer = dirPath.ToAnsiBuffer();
        using AnsiBuffer filterBuffer = filter.ToAnsiBuffer();
        return GetDirectoryFileCountEx(dirBuffer.AsPointer(), filterBuffer.AsPointer(), scanSubdirs);
    }

    /// <summary>Load directory filepaths with extension filtering and subdir scan; some filters available: "*.*", "FILES*", "DIRS*"</summary>
    public static FilePathList LoadDirectoryFilesEx(string basePath, string filter, CBool scanSubDirs)
    {
        using AnsiBuffer baseBuffer = basePath.ToAnsiBuffer();
        using AnsiBuffer filterBuffer = filter.ToAnsiBuffer();
        return LoadDirectoryFilesEx(baseBuffer.AsPointer(), filterBuffer.AsPointer(), scanSubDirs);
    }


    /// <summary>Compress data (DEFLATE algorithm)</summary>
    public static byte[] CompressData(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            int compressedSize;
            var compressedPtr = CompressData(dataPtr, data.Length, &compressedSize);

            if (compressedPtr == null || compressedSize <= 0)
            {
                return Array.Empty<byte>();
            }

            try
            {
                var result = new byte[compressedSize];

                fixed (byte* resultPtr = result)
                {
                    Buffer.MemoryCopy(
                        compressedPtr,
                        resultPtr,
                        compressedSize,
                        compressedSize
                    );
                }

                return result;
            }
            finally
            {
                MemFree(compressedPtr);
            }
        }
    }

    /// <summary>Decompress data (DEFLATE algorithm)</summary>
    public static byte[] DecompressData(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            int dataSize;
            var compressedPtr = DecompressData(dataPtr, data.Length, &dataSize);

            if (compressedPtr == null || dataSize <= 0)
            {
                return Array.Empty<byte>();
            }

            try
            {
                var result = new byte[dataSize];

                fixed (byte* resultPtr = result)
                {
                    Buffer.MemoryCopy(
                        compressedPtr,
                        resultPtr,
                        dataSize,
                        dataSize
                    );
                }

                return result;
            }
            finally
            {
                MemFree(compressedPtr);
            }
        }
    }

    /// <summary>Encode data to Base64 string</summary>
    public static byte[] EncodeDataBase64(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            int outputSize;
            var compressedPtr = EncodeDataBase64(dataPtr, data.Length, &outputSize);

            if (compressedPtr == null || outputSize <= 0)
            {
                return Array.Empty<byte>();
            }

            try
            {
                var result = new byte[outputSize];

                fixed (byte* resultPtr = result)
                {
                    Buffer.MemoryCopy(
                        compressedPtr,
                        resultPtr,
                        outputSize,
                        outputSize
                    );
                }

                return result;
            }
            finally
            {
                MemFree(compressedPtr);
            }
        }
    }

    /// <summary>Decode data to Base64 string</summary>
    public static byte[] DecodeDataBase64(string data)
    {
        using Utf8Buffer ptr = data.ToUtf8Buffer();

        int outputSize;
        var compressedPtr = DecodeDataBase64(ptr.AsPointer(), &outputSize);

        if (compressedPtr == null || outputSize <= 0)
        {
            return Array.Empty<byte>();
        }

        try
        {
            var result = new byte[outputSize];

            fixed (byte* resultPtr = result)
            {
                Buffer.MemoryCopy(
                    compressedPtr,
                    resultPtr,
                    outputSize,
                    outputSize
                );
            }

            return result;
        }
        finally
        {
            MemFree(compressedPtr);
        }
    }

    /// <summary>Compute CRC32 hash code</summary>
    public static uint ComputeCRC32(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            return ComputeCRC32(dataPtr, data.Length);
        }
    }

    /// <summary>Compute MD5 hash code, returns uint[4] array</summary>
    public static uint[] ComputeMD5(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            var result = ComputeMD5(dataPtr, data.Length);
            return
            [
                result[0],
                result[1],
                result[2],
                result[3],
            ];
        }
    }

    /// <summary>Compute SHA1 hash code, returns uint[5] array</summary>
    public static uint[] ComputeSHA1(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            var result = ComputeSHA1(dataPtr, data.Length);
            return
            [
                result[0],
                result[1],
                result[2],
                result[3],
                result[4],
            ];
        }
    }

    /// <summary>Compute SHA256 hash code, returns int[8] (32 bytes)</summary>
    public static uint[] ComputeSHA256(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            var result = ComputeSHA256(dataPtr, data.Length);
            return
            [
                result[0],
                result[1],
                result[2],
                result[3],
                result[4],
                result[5],
                result[6],
                result[7],
            ];
        }
    }

    /// <summary>
    /// Loads text from a file, reads it, saves it, unloads the file, and returns the loaded text.
    /// </summary>
    /// <returns>The text content of the file on the given path</returns>
    public static string LoadFileText(string fileName)
    {
        using AnsiBuffer nameBuffer = fileName.ToAnsiBuffer();
        sbyte* data = LoadFileText(nameBuffer.AsPointer());
        string text = new string(data);
        UnloadFileText(data);
        return text;
    }

    public static Vector2 GetScreenCenter()
    {
        Vector2 center = new Vector2();
        center.X = GetScreenWidth() / 2.0f;
        center.Y = GetScreenHeight() / 2.0f;
        return center;
    }

}
