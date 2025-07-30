using System;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Audio stream type<br/>
/// NOTE: Useful to create custom audio streams not bound to a specific file
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct AudioStream
{
    //TODO: convert
    /// <summary>
    /// Pointer to internal data(rAudioBuffer *) used by the audio system
    /// </summary>
    public IntPtr Buffer;

    /// <summary>
    /// Pointer to internal data processor, useful for audio effects
    /// </summary>
    public IntPtr Processor;

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

    public readonly CBool IsValid => Raylib.IsAudioStreamValid(this);
    public readonly CBool IsPlaying => Raylib.IsAudioStreamPlaying(this);
    public readonly CBool IsProcessed => Raylib.IsAudioStreamProcessed(this);

    public static AudioStream Load(uint sampleRate, uint sampleSize, uint channels)
    {
        return Raylib.LoadAudioStream(sampleRate, sampleSize, channels);
    }

    public readonly void Play()
    {
        Raylib.PlayAudioStream(this);
    }

    public readonly void Pause()
    {
        Raylib.PauseAudioStream(this);
    }

    public readonly void Resume()
    {
        Raylib.ResumeAudioStream(this);
    }

    public unsafe void Update(void* data, int frameCount)
    {
        Raylib.UpdateAudioStream(this, data, frameCount);
    }

    public readonly void Stop()
    {
        Raylib.StopAudioStream(this);
    }

    public static void SetBufferSizeDefault(int size)
    {
        Raylib.SetAudioStreamBufferSizeDefault(size);
    }

    /// <summary>
    /// Set pan for this audio stream where 0.5 is centered
    /// </summary>
    /// <param name="pan"></param>
    public void SetPan(float pan)
    {
        Raylib.SetAudioStreamPan(this, pan);
    }

    /// <summary>
    /// Set pitch for this audio stream where 1.0 si base level
    /// </summary>
    public void SetPitch(float pitch)
    {
        Raylib.SetAudioStreamPitch(this, pitch);
    }

    /// <summary>
    /// Set the volume from 0.0 to 1.0
    /// </summary>
    public void SetVolume(float volume)
    {
        Raylib.SetAudioStreamVolume(this, volume);
    }

    public void Unload()
    {
        Raylib.UnloadAudioStream(this);
    }
}
