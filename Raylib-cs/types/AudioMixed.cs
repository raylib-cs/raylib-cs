using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

namespace Raylib_cs;

internal static unsafe class AudioMixed
{
    public static AudioCallback<float> Callback = null;

    [UnmanagedCallersOnly(CallConvs = new Type[]
    { typeof(CallConvCdecl) })]
    public static void Processor(void* buffer, uint frames)
    {
        // The buffer is stereo audio, so we need to double our frame count.
        frames = Math.Min(frames * 2, int.MaxValue);

        Span<float> floats = new(buffer, (int)frames);
        Callback?.Invoke(floats);
    }
}
