using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Raylib_cs;

/// <summary>
/// Converts text to a UTF8 buffer for passing to native code
/// </summary>
public readonly ref struct Utf8Buffer
{
    private readonly IntPtr _data;

    public Utf8Buffer(string text)
    {
        _data = Marshal.StringToCoTaskMemUTF8(text);
    }

    public unsafe sbyte* AsPointer()
    {
        return (sbyte*)_data.ToPointer();
    }

    public void Dispose()
    {
        Marshal.ZeroFreeCoTaskMemUTF8(_data);
    }
}

public static class Utf8StringUtils
{
    public static Utf8Buffer ToUtf8Buffer(this string text)
    {
        return new Utf8Buffer(text);
    }

    public static byte[] ToUtf8String(this string text)
    {
        if (text == null)
        {
            return null;
        }

        int length = Encoding.UTF8.GetByteCount(text);

        byte[] byteArray = new byte[length + 1];
        int wrote = Encoding.UTF8.GetBytes(text, 0, text.Length, byteArray, 0);
        byteArray[wrote] = 0;

        return byteArray;
    }

    public static unsafe string GetUTF8String(sbyte* bytes)
    {
        return Marshal.PtrToStringUTF8((IntPtr)bytes);
    }

    public static byte[] GetUTF8Bytes(this string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }
}
