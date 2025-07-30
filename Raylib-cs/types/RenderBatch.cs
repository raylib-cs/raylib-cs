using System;
using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// RenderBatch type
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct RenderBatch
{
    /// <summary>
    /// Number of vertex buffers (multi-buffering support)
    /// </summary>
    public int BufferCount;

    /// <summary>
    /// Current buffer tracking in case of multi-buffering
    /// </summary>
    public int CurrentBuffer;

    /// <summary>
    /// Dynamic buffer(s) for vertex data
    /// </summary>
    public VertexBuffer* VertexBuffer;

    /// <summary>
    /// Draw calls array, depends on textureId
    /// </summary>
    public DrawCall* Draws;

    /// <summary>
    /// Draw calls counter
    /// </summary>
    public int DrawCounter;

    /// <summary>
    /// Current depth value for next draw
    /// </summary>
    public float CurrentDepth;
}

/// <summary>
/// Dynamic vertex buffers (position + texcoords + colors + indices arrays)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VertexBuffer
{
    /// <summary>
    /// Number of elements in the buffer (QUADS)
    /// </summary>
    public int ElementCount;

    /// <summary>
    /// Vertex position (XYZ - 3 components per vertex) (shader-location = 0)
    /// </summary>
    public float* Vertices;

    /// <summary>
    /// Vertex texture coordinates (UV - 2 components per vertex) (shader-location = 1)
    /// </summary>
    public float* TexCoords;

    /// <summary>
    /// Vertex normal (XYZ - 3 components per vertex) (shader-location = 2)
    /// </summary>
    public float* Normals;

    /// <summary>
    /// Vertex colors (RGBA - 4 components per vertex) (shader-location = 3)
    /// </summary>
    public byte* Colors;

    /// <summary>
    /// Vertex indices (in case vertex data comes indexed) (6 indices per quad)<br/>
    /// unsigned int* for GRAPHICS_API_OPENGL_11 or GRAPHICS_API_OPENGL_33<br/>
    /// unsigned short* for GRAPHICS_API_OPENGL_ES2
    /// </summary>
    public void* Indices;

    /// <summary>
    /// OpenGL Vertex Array Object id
    /// </summary>
    public uint VaoId;

    /// <summary>
    /// OpenGL Vertex Buffer Objects id (5 types of vertex data)
    /// </summary>
    public fixed uint VboId[5];

    /// <summary>
    /// Access <see cref="Vertices"/>
    /// </summary>
    public readonly Span<T> VerticesAs<T>() where T : unmanaged
    {
        return new(Vertices, ElementCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="TexCoords"/>
    /// </summary>
    public readonly Span<T> TexCoordsAs<T>() where T : unmanaged
    {
        return new(TexCoords, ElementCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="Normals"/>
    /// </summary>
    public readonly Span<T> NormalsAs<T>() where T : unmanaged
    {
        return new(Normals, ElementCount * sizeof(float) / sizeof(T));
    }

    /// <summary>
    /// Access <see cref="Colors"/>
    /// </summary>
    public readonly Span<T> ColorsAs<T>() where T : unmanaged
    {
        return new(Colors, ElementCount * sizeof(byte) / sizeof(T));
    }
}

/// <summary>
/// Draw call type<br/>
/// NOTE: Only texture changes register a new draw, other state-change-related elements are not
/// used at this moment (vaoId, shaderId, matrices), raylib just forces a batch draw call if any
/// of those state-change happens (this is done in core module)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct DrawCall
{
    /// <summary>
    /// Drawing mode: LINES, TRIANGLES, QUADS
    /// </summary>
    public DrawMode Mode;

    /// <summary>
    /// Number of vertices for the draw call
    /// </summary>
    public int VertexCount;

    /// <summary>
    /// Number of vertices required for index alignment (LINES, TRIANGLES)
    /// </summary>
    public int VertexAlignment;

    /// <summary>
    /// Texture id to be used on the draw -> Use to create new draw call if changes
    /// </summary>
    public uint TextureId;
}

public enum GlVersion
{
    OpenGl11 = 1,
    OpenGl21,
    OpenGl33,
    OpenGl43,
    OpenGlEs20
}

public enum FramebufferAttachType
{
    ColorChannel0 = 0,
    ColorChannel1,
    ColorChannel2,
    ColorChannel3,
    ColorChannel4,
    ColorChannel5,
    ColorChannel6,
    ColorChannel7,
    Depth = 100,
    Stencil = 200,
}

public enum FramebufferAttachTextureType
{
    CubemapPositiveX = 0,
    CubemapNegativeX,
    CubemapPositiveY,
    CubemapNegativeY,
    CubemapPositiveZ,
    CubemapNegativeZ,
    Texture2D = 100,
    Renderbuffer = 200,
}

/// <summary>
/// Matrix Modes (equivalent to OpenGL)
/// </summary>
public enum MatrixMode
{
    /// <summary>
    /// GL_MODELVIEW
    /// </summary>
    ModelView = 0x1700,

    /// <summary>
    /// GL_PROJECTION
    /// </summary>
    Projection = 0x1701,

    /// <summary>
    /// GL_TEXTURE
    /// </summary>
    Texture = 0x1702
}

/// <summary>
/// Primitive assembly draw modes
/// </summary>
public enum DrawMode
{
    /// <summary>
    /// GL_LINES
    /// </summary>
    Lines = 0x0001,

    /// <summary>
    /// GL_TRIANGLES
    /// </summary>
    Triangles = 0x0004,

    /// <summary>
    /// GL_QUADS
    /// </summary>
    Quads = 0x0007
}

/// <summary>
/// Texture parameters (equivalent to OpenGL defines)
/// </summary>
public enum TextureFilters
{
    /// <summary>
    /// RL_TEXTURE_FILTER_NEAREST
    /// <br/>
    /// GL_NEAREST
    /// </summary>
    Nearest = 0x2600,

    /// <summary>
    /// RL_TEXTURE_FILTER_LINEAR
    /// <br/>
    /// GL_LINEAR
    /// </summary>
    Linear = 0x2601,

    /// <summary>
    /// RL_TEXTURE_FILTER_MIP_NEAREST
    /// <br/>
    /// GL_NEAREST_MIPMAP_NEAREST
    /// </summary>
    MipNearest = 0x2700,

    /// <summary>
    /// RL_TEXTURE_FILTER_NEAREST_MIP_LINEAR
    /// <br/>
    /// GL_NEAREST_MIPMAP_LINEAR
    /// </summary>
    NearestMipLinear = 0x2702,

    /// <summary>
    /// RL_TEXTURE_FILTER_LINEAR_MIP_NEAREST
    /// <br/>
    /// GL_LINEAR_MIPMAP_NEAREST
    /// </summary>
    LinearMipNearest = 0x2701,

    /// <summary>
    /// RL_TEXTURE_FILTER_MIP_LINEAR
    /// <br/>
    /// GL_LINEAR_MIPMAP_LINEAR
    /// </summary>
    MipLinear = 0x2703,

    /// <summary>
    /// RL_TEXTURE_FILTER_ANISOTROPIC
    /// <br/>
    /// Anisotropic filter (custom identifier)
    /// </summary>
    Anisotropic = 0x3000
}

/// <summary>
/// GL Shader type
/// </summary>
public enum ShaderType
{
    /// <summary>
    /// RL_FRAGMENT_SHADER
    /// <br/>
    /// GL_FRAGMENT_SHADER
    /// </summary>
    Fragment = 0x8B30,

    /// <summary>
    /// RL_VERTEX_SHADER
    /// <br/>
    /// GL_VERTEX_SHADER
    /// </summary>
    Vertex = 0x8B31,

    /// <summary>
    /// RL_COMPUTE_SHADER
    /// <br/>
    /// GL_COMPUTE_SHADER
    /// </summary>
    Compute = 0x91b9
}
