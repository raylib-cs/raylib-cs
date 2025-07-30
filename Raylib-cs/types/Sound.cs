using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Sound source type
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Sound
{
    /// <summary>
    /// Audio stream
    /// </summary>
    public AudioStream Stream;

    /// <summary>
    /// Total number of frames (considering channels)
    /// </summary>
    public uint FrameCount;

    public readonly CBool IsValid => Raylib.IsSoundValid(this);
    public readonly CBool IsPlaying => Raylib.IsSoundPlaying(this);

    public static Sound Load(string fileName)
    {
        return Raylib.LoadSound(fileName);
    }

    public static Sound LoadFromWave(Wave wave)
    {
        return Raylib.LoadSoundFromWave(wave);
    }

    public readonly void Play()
    {
        Raylib.PlaySound(this);
    }

    public readonly void Pause()
    {
        Raylib.PauseSound(this);
    }

    public readonly void Resume()
    {
        Raylib.ResumeSound(this);
    }

    /// <summary>
    /// Set pan where 0.5 is center
    /// </summary>
    public void SetPan(float pan)
    {
        Raylib.SetSoundPan(this, pan);
    }

    /// <summary>
    /// Set pitch where 1.0 is base level
    /// </summary>
    public void SetPitch(float pitch)
    {
        Raylib.SetSoundPitch(this, pitch);
    }

    /// <summary>
    /// Set volume from 0.0 to 1.0
    /// </summary>
    public void SetVolume(float volume)
    {
        Raylib.SetSoundVolume(this, volume);
    }

    public void Unload()
    {
        Raylib.UnloadSound(this);
    }
}
