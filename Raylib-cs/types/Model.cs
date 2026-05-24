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

    /// <summary>
    /// Bone name as string
    /// </summary>
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
    public Transform* BindPose;

    public Span<Transform> BindPoseAsSpan()
    {
        if (BindPose == null || BoneCount <= 0)
        {
            return Span<Transform>.Empty;
        }

        return new Span<Transform>(BindPose, BoneCount);
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
// typedef Transform *ModelAnimPose; It's just a pointer array.

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

    /// <summary>
    /// Bones animated transformation matrices as span. Based on Skeleton.BoneCount length
    /// </summary>
    public Span<Matrix4x4> BoneMatricesAsSpan()
    {
        if (BoneMatrices == null || Skeleton.BoneCount <= 0)
        {
            return Span<Matrix4x4>.Empty;
        }

        return new Span<Matrix4x4>(BoneMatrices, Skeleton.BoneCount);
    }

    /// <summary>
    /// Current animation pose as span. Based on Skeleton.BoneCount length
    /// </summary>
    public Span<Transform> CurrentPoseAsSpan()
    {
        if (CurrentPose == null || Skeleton.BoneCount <= 0)
        {
            return Span<Transform>.Empty;
        }

        return new Span<Transform>(CurrentPose, Skeleton.BoneCount);
    }

    /// <summary>
    /// Mesh material number as span. Based on MaterialCount length
    /// </summary>
    public Span<int> MeshMaterialAsSpan()
    {
        if (MeshMaterial == null || MaterialCount <= 0)
        {
            return Span<int>.Empty;
        }

        return new Span<int>(MeshMaterial, MaterialCount);
    }

    /// <summary>
    /// Meshes as span. Based on MeshCount length
    /// </summary>
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
    /// Number of animation key frames
    /// </summary>
    public readonly int KeyFrameCount;

    /// <summary>
    /// Animation sequence keyframe poses [keyframe][pose]
    /// </summary>
    public Transform** KeyframePoses;


    /// <summary>
    /// Animation sequence keyframe poses as span. Based on KeyFrameCount length
    /// </summary>
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

    /// <summary>
    /// Animation name as string
    /// </summary>
    public string NameToString()
    {
        fixed (sbyte* name = Name)
        {
            return Utf8StringUtils.GetUTF8String(name);
        }
    }
}
