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

    //TODO span
    /// <summary>
    /// Audio context data, depends on type
    /// </summary>
    public void* CtxData;

    public readonly CBool IsValid => Raylib.IsMusicValid(this);

    public static Music Load(string fileName)
    {
        return Raylib.LoadMusicStream(fileName);
    }

    public readonly void Play()
    {
        Raylib.PlayMusicStream(this);
    }

    public readonly void Pause()
    {
        Raylib.PauseMusicStream(this);
    }

    public void Update()
    {
        Raylib.UpdateMusicStream(this);
    }

    public readonly void Stop()
    {
        Raylib.StopMusicStream(this);
    }

    /// <summary>
    /// Set pan of this music where 0.5 is centered
    /// </summary>
    public void SetPan(float pan)
    {
        Raylib.SetMusicPan(this, pan);
    }

    /// <summary>
    /// Set pitch for this music where 1.0 is base level
    /// </summary>}
    public void SetPitch(float pitch)
    {
        Raylib.SetMusicPitch(this, pitch);
    }

    /// <summary>
    /// Set music volume from 0.0 to 1.0
    /// </summary>
    public void SetVolume(float volume)
    {
        Raylib.SetMusicVolume(this, volume);
    }

    /// <summary>
    /// Seek music to a position (in seconds)
    /// </summary>
    public void Seek(float position)
    {
        Raylib.SeekMusicStream(this, position);
    }

    public void Unload()
    {
        Raylib.UnloadMusicStream(this);
    }
}
