using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Vertex data definning a mesh<br/>
/// NOTE: Data stored in CPU memory (and GPU)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Mesh
{
    /// <summary>
    ///  Creates a mesh ready for default vertex data allocation
    /// </summary>
    public Mesh(int vertexCount, int triangleCount)
    {
        VertexCount = vertexCount;
        TriangleCount = triangleCount;
    }

    /// <summary>
    ///  Number of vertices stored in arrays
    /// </summary>
    public int VertexCount;

    /// <summary>
    /// Number of triangles stored (indexed or not)
    /// </summary>
    public int TriangleCount;

    #region Default vertex data

    /// <summary>
    /// Vertex position (XYZ - 3 components per vertex) (shader-location = 0)
    /// </summary>
    public float* Vertices = default;

    /// <summary>
    /// Vertex texture coordinates (UV - 2 components per vertex) (shader-location = 1)
    /// </summary>
    public float* TexCoords = default;

    /// <summary>
    /// Vertex second texture coordinates (useful for lightmaps) (shader-location = 5)
    /// </summary>
    public float* TexCoords2 = default;

    /// <summary>
    /// Vertex normals (XYZ - 3 components per vertex) (shader-location = 2)
    /// </summary>
    public float* Normals = default;

    /// <summary>
    /// Vertex tangents (XYZW - 4 components per vertex) (shader-location = 4)
    /// </summary>
    public float* Tangents = default;

    /// <summary>
    /// Vertex colors (RGBA - 4 components per vertex) (shader-location = 3)
    /// </summary>
    public byte* Colors = default;

    /// <summary>
    /// Vertex indices (in case vertex data comes indexed)
    /// </summary>
    public ushort* Indices = default;

    /// <summary>
    /// Allocate <see cref="Vertices"/>
    /// </summary>
    public void AllocVertices()
    {
        Vertices = Raylib.New<float>(3 * (uint)VertexCount);
    }

    /// <summary>
    /// Allocate <see cref="TexCoords"/>
    /// </summary>
    public void AllocTexCoords()
    {
        TexCoords = Raylib.New<float>(2 * (uint)VertexCount);
    }

    /// <summary>
    /// Allocate <see cref="TexCoords2"/>
    /// </summary>
    public void AllocTexCoords2()
    {
        TexCoords2 = Raylib.New<float>(2 * (uint)VertexCount);
    }

    /// <summary>
    /// Allocate <see cref="Normals"/>
    /// </summary>
    public void AllocNormals()
    {
        Normals = Raylib.New<float>(3 * (uint)VertexCount);
    }

    /// <summary>
    /// Allocate <see cref="Tangents"/>
    /// </summary>
    public void AllocTangents()
    {
        Tangents = Raylib.New<float>(4 * (uint)VertexCount);
    }

    /// <summary>
    /// Allocate <see cref="Colors"/>
    /// </summary>
    public void AllocColors()
    {
        Colors = Raylib.New<byte>(4 * (uint)VertexCount);
    }

    /// <summary>
    /// Allocate <see cref="Indices"/>
    /// </summary>
    public void AllocIndices()
    {
        Indices = Raylib.New<ushort>(3 * (uint)TriangleCount);
    }

    /// <summary>
    /// Access <see cref="Vertices"/>
    /// </summary>
    public readonly Span<T> VerticesAs<T>() where T : unmanaged
    {
        return new(Vertices, 3 * VertexCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="TexCoords"/>
    /// </summary>
    public readonly Span<T> TexCoordsAs<T>() where T : unmanaged
    {
        return new Span<T>(TexCoords, 2 * VertexCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="TexCoords2"/>
    /// </summary>
    public readonly Span<T> TexCoords2As<T>() where T : unmanaged
    {
        return new Span<T>(TexCoords2, 2 * VertexCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="Normals"/>
    /// </summary>
    public readonly Span<T> NormalsAs<T>() where T : unmanaged
    {
        return new Span<T>(Normals, 3 * VertexCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="Tangents"/>
    /// </summary>
    public readonly Span<T> TangentsAs<T>() where T : unmanaged
    {
        return new Span<T>(Tangents, 4 * VertexCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="Colors"/>
    /// </summary>
    public readonly Span<T> ColorsAs<T>() where T : unmanaged
    {
        return new Span<T>(Colors, 4 * VertexCount * sizeof(byte) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="Indices"/>
    /// </summary>
    public readonly Span<T> IndicesAs<T>() where T : unmanaged
    {
        return new Span<T>(Indices, 3 * TriangleCount * sizeof(ushort) / sizeof(T));
    }

    #endregion

    #region Animation vertex data

    /// <summary>
    /// Animated vertex positions (after bones transformations)
    /// </summary>
    public float* AnimVertices = default;

    /// <summary>
    /// Animated normals (after bones transformations)
    /// </summary>
    public float* AnimNormals = default;

    /// <summary>
    /// Vertex bone ids, up to 4 bones influence by vertex (skinning)
    /// </summary>
    public byte* BoneIds = default;

    /// <summary>
    /// Vertex bone weight, up to 4 bones influence by vertex (skinning)
    /// </summary>
    public float* BoneWeights = default;

    /// <summary>
    /// Bones animated transformation matrices
    /// </summary>
    public Matrix4x4* BoneMatrices = default;

    /// <summary>
    /// Number of bones
    /// </summary>
    public int BoneCount;

    #endregion

    #region OpenGL identifiers

    /// <summary>
    /// OpenGL Vertex Array Object id
    /// </summary>
    public uint VaoId = default;

    /// <summary>
    /// OpenGL Vertex Buffer Objects id (default vertex data, uint[])
    /// </summary>
    public uint* VboId = default;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="Vertices"/>
    /// </summary>
    public const int VboIdIndexVertices = 0;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="TexCoords"/>
    /// </summary>
    public const int VboIdIndexTexCoords = 1;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="Normals"/>
    /// </summary>
    public const int VboIdIndexNormals = 2;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="Colors"/>
    /// </summary>
    public const int VboIdIndexColors = 3;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="Tangents"/>
    /// </summary>
    public const int VboIdIndexTangents = 4;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="TexCoords2"/>
    /// </summary>
    public const int VboIdIndexTexCoords2 = 5;

    /// <summary>
    /// Default <see cref="VboId"/> index for <see cref="Indices"/>
    /// </summary>
    public const int VboIdIndexIndices = 6;

    #endregion
}
