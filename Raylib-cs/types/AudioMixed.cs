using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

namespace Raylib_cs;

/// <summary>
/// Audio stream processor.
/// Used with <see cref="Raylib.AttachAudioMixedProcessor(AudioCallback{float})"/>
/// and <see cref="Raylib.DetachAudioMixedProcessor(AudioCallback{float})"/>
/// </summary>
public delegate void AudioCallback<T>(Span<T> buffer);

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
