using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// File path list
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct FilePathList
{
    /// <summary>
    /// Filepaths max entries
    /// </summary>
    public uint Capacity;

    /// <summary>
    /// Filepaths entries count
    /// </summary>
    public uint Count;

    /// <summary>
    /// Filepaths entries
    /// </summary>
    public byte** Paths;

    public string this[uint i]
    {
        get
        {
            if (i >= 0 && i < Count)
            {
                return Marshal.PtrToStringUTF8((System.IntPtr)Paths[i]);
            }
            else
            {
                throw new System.IndexOutOfRangeException();
            }
        }
    }
}
