using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Wave type, defines audio wave data
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Wave
{
    /// <summary>
    /// Number of samples
    /// </summary>
    public uint SampleCount;

    /// <summary>
    /// Frequency (samples per second)
    /// </summary>
    public uint SampleRate;

    /// <summary>
    /// Bit depth (bits per sample): 8, 16, 32 (24 not supported)
    /// </summary>
    public uint SampleSize;

    /// <summary>
    /// Number of channels (1-mono, 2-stereo)
    /// </summary>
    public uint Channels;

    // TODO: SPAN<byte>?
    /// <summary>
    /// Buffer data pointer
    /// </summary>
    public void* Data;
}
