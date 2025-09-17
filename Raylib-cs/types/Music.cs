using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Music stream type (audio file streaming from memory)<br/>
/// NOTE: Anything longer than ~10 seconds should be streamed
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Music
{
    /// <summary>
    /// Audio stream
    /// </summary>
    public AudioStream Stream;

    /// <summary>
    /// Total number of samples
    /// </summary>
    public uint FrameCount;

    /// <summary>
    /// Music looping enable
    /// </summary>
    public CBool Looping;

    /// <summary>
    /// Type of music context (audio filetype)
    /// </summary>
    public int CtxType;

    // TODO span
    /// <summary>
    /// Audio context data, depends on type
    /// </summary>
    public void* CtxData;
}
