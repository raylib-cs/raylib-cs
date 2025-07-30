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

    //TODO: SPAN<byte>  ?
    /// <summary>
    /// Buffer data pointer
    /// </summary>
    public void* Data;

    public readonly CBool IsValid => Raylib.IsWaveValid(this);

    public static Wave Load(string fileName)
    {
        return Raylib.LoadWave(fileName);
    }

    public CBool Export(string fileName)
    {
        return Raylib.ExportWave(this, fileName);
    }

    public CBool ExportAsCode(string fileName)
    {
        return Raylib.ExportWaveAsCode(this, fileName);
    }

    public void Format(int sampleRate, int sampleSize, int channels)
    {
        Raylib.WaveFormat(ref this, sampleRate, sampleSize, channels);
    }

    public void Crop(int initFrame, int finalFrame)
    {
        Raylib.WaveCrop(ref this, initFrame, finalFrame);
    }

    public void Unload()
    {
        Raylib.UnloadWave(this);
    }
}
