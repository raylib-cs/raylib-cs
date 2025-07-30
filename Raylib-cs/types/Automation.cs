using System;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>Automation event</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AutomationEvent
{
    /// <summary>Event frame</summary>
    public uint Frame;

    /// <summary>Event type (AutomationEventType)</summary>
    public uint Type;

    /// <summary>Event parameters (if required)</summary>
    public fixed int Params[4];

    public readonly void Play()
    {
        Raylib.PlayAutomationEvent(this);
    }
}

/// <summary>Automation event list</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AutomationEventList
{
    /// <summary>Events max entries (MAX_AUTOMATION_EVENTS)</summary>
    public uint Capacity;

    /// <summary>Events entries count</summary>
    public uint Count;

    /// <summary>Events entries</summary>
    public AutomationEvent* Events;

    /// <inheritdoc cref="Events"/>
    public readonly ReadOnlySpan<AutomationEvent> EventsAsSpan => new ReadOnlySpan<AutomationEvent>(Events, (int)Count);

    public static AutomationEventList Load(string fileName)
    {
        return Raylib.LoadAutomationEventList(fileName);
    }

    public readonly CBool Export(string fileName)
    {
        return Raylib.ExportAutomationEventList(this, fileName);
    }

    public void Unload()
    {
        Raylib.UnloadAutomationEventList(this);
    }

}
