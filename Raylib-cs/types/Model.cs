using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Bone information
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct BoneInfo
{
    /// <summary>
    /// Bone name (char[32])
    /// </summary>
    public fixed sbyte Name[32];

    /// <summary>
    /// Bone parent
    /// </summary>
    public int Parent;

    public string NameToString()
    {
        fixed (sbyte* name = Name)
        {
            return Utf8StringUtils.GetUTF8String(name);
        }
    }
}

/// <summary>
/// Skeleton, animation bones hierarchy
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ModelSkeleton
{
    /// <summary>
    /// Number of bones
    /// </summary>
    public int BoneCount;

    /// <summary>
    /// Bones information (skeleton)
    /// </summary>
    public BoneInfo* Bones;

    /// <summary>
    /// Bones base transformation (Transform[])
    /// </summary>
    public Transform* ModelAnimPose;

    public Span<Transform> ModelAnimPoseAsSpan()
    {
        return new Span<Transform>(ModelAnimPose, BoneCount);
    }

    public Span<BoneInfo> BonesAsSpan()
    {

        if (Bones == null || BoneCount <= 0)
        {
            return Span<BoneInfo>.Empty;
        }

        return new Span<BoneInfo>(Bones, BoneCount);
    }
}

// Note:
// Anim pose, an array of Transform[]
// typedef Transform *ModelAnimPose; It's just an pointer array.

/// <summary>
/// Model type
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Model
{
    /// <summary>
    /// Local transform matrix
    /// </summary>
    public Matrix4x4 Transform;

    /// <summary>
    /// Number of meshes
    /// </summary>
    public int MeshCount;

    /// <summary>
    /// Number of materials
    /// </summary>
    public int MaterialCount;

    /// <summary>
    /// Meshes array (Mesh *)
    /// </summary>
    public Mesh* Meshes;

    /// <summary>
    /// Materials array (Material *)
    /// </summary>
    public Material* Materials;

    /// <summary>
    /// Mesh material number (int *)
    /// </summary>
    public int* MeshMaterial;

    /// <summary>
    /// Skeleton for animation
    /// </summary>
    public ModelSkeleton Skeleton;

    /// <summary>
    /// Current animation pose (Transform[])
    /// </summary>
    public Transform* CurrentPose;

    /// <summary>
    /// Bones animated transformation matrices
    /// </summary>
    public Matrix4x4* BoneMatrices;

    public Span<Matrix4x4> BoneMatricesAsSpan()
    {
        return new Span<Matrix4x4>(BoneMatrices, Skeleton.BoneCount);
    }

    public Span<Transform> CurrentPoseAsSpan()
    {
        return new Span<Transform>(CurrentPose, Skeleton.BoneCount);
    }

    public Span<int> MeshMaterialAsSpan()
    {
        if (MeshMaterial == null || MeshCount <= 0)
        {
            return Span<int>.Empty;
        }

        return new Span<int>(MeshMaterial, MeshCount);
    }

    public Span<Mesh> MeshesAsSpan()
    {
        if (Meshes == null || MeshCount <= 0)
        {
            return Span<Mesh>.Empty;
        }

        return new Span<Mesh>(Meshes, MeshCount);
    }
}

/// <summary>
/// ModelAnimation, contains a full animation sequence
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ModelAnimation
{
    /// <summary>
    /// Animation name (char[32])
    /// </summary>
    public fixed sbyte Name[32];

    /// <summary>
    /// Number of bones
    /// </summary>
    public readonly int BoneCount;

    /// <summary>
    /// Number of animation frames
    /// </summary>
    public readonly int KeyFrameCount;

    /// <summary>
    /// Animation sequence keyframe poses [keyframe][pose]
    /// </summary>
    public Transform** KeyframePoses;

    public Span<Transform> GetKeyFramePoseAsSpan(int frame)
    {
        if (KeyframePoses == null || frame < 0 || frame >= KeyFrameCount)
        {
            return Span<Transform>.Empty;
        }

        var pose = KeyframePoses[frame];

        if (pose == null || BoneCount <= 0)
        {
            return Span<Transform>.Empty;
        }

        return new Span<Transform>(KeyframePoses[frame], KeyFrameCount);
    }

    public string NameToString()
    {
        fixed (sbyte* name = Name)
        {
            return Utf8StringUtils.GetUTF8String(name);
        }
    }
}
