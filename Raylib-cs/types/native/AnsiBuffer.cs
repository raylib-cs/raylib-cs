using System.Runtime.InteropServices;
using System;

namespace Raylib_cs;

/// <summary>
/// Converts text to a Ansi buffer for passing to native code
/// </summary>
public readonly ref struct AnsiBuffer
{
    private readonly IntPtr _data;

    public AnsiBuffer(string text)
    {
        _data = Marshal.StringToHGlobalAnsi(text);
    }

    public unsafe sbyte* AsPointer()
    {
        return (sbyte*)_data.ToPointer();
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(_data);
    }
}

public static class AnsiStringUtils
{
    public static AnsiBuffer ToAnsiBuffer(this string text)
    {
        return new AnsiBuffer(text);
    }
}
