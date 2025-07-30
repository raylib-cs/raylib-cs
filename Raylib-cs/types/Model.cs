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
}

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
    /// Number of bones
    /// </summary>
    public int BoneCount;

    //TODO: Span
    /// <summary>
    /// Bones information (skeleton, BoneInfo *)
    /// </summary>
    public BoneInfo* Bones;

    //TODO: Span
    /// <summary>
    /// Bones base transformation (pose, Transform *)
    /// </summary>
    public Transform* BindPose;

    public readonly CBool IsValid => Raylib.IsModelValid(this);

    public static Model Load(string fileName)
    {
        return Raylib.LoadModel(fileName);
    }

    public static Model LoadFromMesh(Mesh mesh)
    {
        return Raylib.LoadModelFromMesh(mesh);
    }

    public readonly void Draw(Vector3 position)
    {
        Raylib.DrawModel(this, position, 1.0f, Color.White);
    }

    public readonly void Draw(Vector3 position, float scale)
    {
        Raylib.DrawModel(this, position, scale, Color.White);
    }

    public readonly void Draw(Vector3 position, float scale, Color color)
    {
        Raylib.DrawModel(this, position, scale, color);
    }

    public readonly void Draw(Vector3 position, Vector3 rotationAxis, float rotationAngle)
    {
        Raylib.DrawModelEx(this, position, rotationAxis, rotationAngle, Vector3.One, Color.White);
    }

    public readonly void Draw(Vector3 position, Vector3 rotationAxis, float rotationAngle, Vector3 scale)
    {
        Raylib.DrawModelEx(this, position, rotationAxis, rotationAngle, scale, Color.White);
    }

    public readonly void Draw(Vector3 position, Vector3 rotationAxis, float rotationAngle, Vector3 scale, Color color)
    {
        Raylib.DrawModelEx(this, position, rotationAxis, rotationAngle, scale, color);
    }

    public readonly void DrawWires(Vector3 position, Color color)
    {
        Raylib.DrawModelWires(this, position, 1.0f, color);
    }

    public readonly void DrawWires(Vector3 position, float scale)
    {
        Raylib.DrawModelWires(this, position, scale, Color.White);
    }

    public readonly void DrawWires(Vector3 position, float scale, Color color)
    {
        Raylib.DrawModelWires(this, position, scale, color);
    }

    public readonly void DrawWires(Vector3 position, Vector3 rotationAxis, float rotationAngle, Color color)
    {
        Raylib.DrawModelWiresEx(this, position, rotationAxis, rotationAngle, Vector3.One, color);
    }

    public readonly void DrawWires(Vector3 position, Vector3 rotationAxis, float rotationAngle, Vector3 scale, Color color)
    {
        Raylib.DrawModelWiresEx(this, position, rotationAxis, rotationAngle, scale, color);
    }

    public readonly void DrawPoints(Vector3 position, Color color)
    {
        Raylib.DrawModelPoints(this, position, 1.0f, color);
    }

    public readonly void DrawPoints(Vector3 position, float scale, Color color)
    {
        Raylib.DrawModelPoints(this, position, scale, color);
    }

    public readonly void DrawPoints(Vector3 position, Vector3 rotationAxis, float rotationAngle, Color color)
    {
        Raylib.DrawModelPointsEx(this, position, rotationAxis, rotationAngle, Vector3.One, color);
    }

    public readonly void DrawPoints(Vector3 position, Vector3 rotationAxis, float rotationAngle, Vector3 scale, Color color)
    {
        Raylib.DrawModelPointsEx(this, position, rotationAxis, rotationAngle, scale, color);
    }

    public void Unload()
    {
        Raylib.UnloadModel(this);
    }
}

/// <summary>
/// Model animation
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ModelAnimation
{
    /// <summary>
    /// Number of bones
    /// </summary>
    public readonly int BoneCount;

    /// <summary>
    /// Number of animation frames
    /// </summary>
    public readonly int FrameCount;

    /// <summary>
    /// Bones information (skeleton, BoneInfo *)
    /// </summary>
    public readonly BoneInfo* Bones;

    /// <inheritdoc cref="Bones"/>
    public readonly ReadOnlySpan<BoneInfo> BoneInfo => new ReadOnlySpan<BoneInfo>(Bones, BoneCount);

    /// <summary>
    /// Poses array by frame (Transform **)
    /// </summary>
    public readonly Transform** FramePoses;

    /// <summary>
    /// Animation name (char[32])
    /// </summary>
    public fixed sbyte Name[32];

    /// <inheritdoc cref="FramePoses"/>
    public readonly FramePosesCollection FramePosesColl => new FramePosesCollection(FramePoses, FrameCount, BoneCount);

    public readonly struct FramePosesCollection
    {
        readonly Transform** _framePoses;

        readonly int _frameCount;

        readonly int _boneCount;

        public readonly FramePoses this[int index] => new FramePoses(_framePoses[index], _boneCount);

        public readonly Transform this[int index1, int index2] => new FramePoses(_framePoses[index1], _boneCount)[index2];

        internal FramePosesCollection(Transform** framePoses, int frameCount, int boneCount)
        {
            this._framePoses = framePoses;
            this._frameCount = frameCount;
            this._boneCount = boneCount;
        }
    }

    public static ModelAnimation[] LoadAnimations(string fileName)
    {
        int count = 0;
        ModelAnimation* animations = Raylib.LoadModelAnimations(fileName, ref count);
        ModelAnimation[] output = new ModelAnimation[count];
        for (int i = 0; i < count; i++)
        {
            output[i] = animations[i];
        }
        return output;
    }

    public static void UnloadAnimations(ModelAnimation[] modelAnimations)
    {
        fixed (ModelAnimation* ptr = modelAnimations)
        {
            Raylib.UnloadModelAnimations(ptr, modelAnimations.Length);
        }
    }

    public void Unload()
    {
        Raylib.UnloadModelAnimation(this);
    }
}

public readonly unsafe struct FramePoses
{
    readonly Transform* _poses;

    readonly int _count;

    public readonly ref Transform this[int index] => ref _poses[index];

    internal FramePoses(Transform* poses, int count)
    {
        this._poses = poses;
        this._count = count;
    }
}
