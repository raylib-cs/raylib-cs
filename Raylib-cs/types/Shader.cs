using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Shader location index
/// </summary>
public enum ShaderLocationIndex
{
    VertexPosition = 0,
    VertexTexcoord01,
    VertexTexcoord02,
    VertexNormal,
    VertexTangent,
    VertexColor,
    MatrixMvp,
    MatrixView,
    MatrixProjection,
    MatrixModel,
    MatrixNormal,
    VectorView,
    ColorDiffuse,
    ColorSpecular,
    ColorAmbient,
    MapAlbedo,
    MapMetalness,
    MapNormal,
    MapRoughness,
    MapOcclusion,
    MapEmission,
    MapHeight,
    MapCubemap,
    MapIrradiance,
    MapPrefilter,
    MapBrdf,
    VertexBoneIds,
    VertexBoneWeights,
    BoneMatrices,

    MapDiffuse = MapAlbedo,
    MapSpecular = MapMetalness,
}

/// <summary>
/// Shader attribute data types
/// </summary>
public enum ShaderAttributeDataType
{
    Float = 0,
    Vec2,
    Vec3,
    Vec4
}

/// <summary>
/// Shader uniform data type
/// </summary>
public enum ShaderUniformDataType
{
    Float = 0,
    Vec2,
    Vec3,
    Vec4,
    Int,
    IVec2,
    IVec3,
    IVec4,
    UInt,
    UIVec2,
    UIVec3,
    UIVec4,
    Sampler2D
}

/// <summary>
/// Shader type (generic)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Shader
{
    /// <summary>
    /// Shader program id
    /// </summary>
    public uint Id;

    /// <summary>
    /// Shader locations array (MAX_SHADER_LOCATIONS, int *)
    /// </summary>
    public int* Locs;

    public readonly CBool IsValid => Raylib.IsShaderValid(this);

    public static Shader Load(string vsFileName, string fsFileName)
    {
        return Raylib.LoadShader(vsFileName, fsFileName);
    }

    public static Shader LoadFromMemory(string vsCode, string fsCode)
    {
        return Raylib.LoadShaderFromMemory(vsCode, fsCode);
    }

    public void GetLocationAttrib(string attribName)
    {
        Raylib.GetShaderLocationAttrib(this, attribName);
    }

    public void SetValue<T>(int locationIndex, T value, ShaderUniformDataType uniformDataType) where T : unmanaged
    {
        void* val = &value;
        Raylib.SetShaderValue(this, locationIndex, val, uniformDataType);
    }

    public void SetValues<T>(int locationIndex, T[] values, ShaderUniformDataType uniformDataType) where T : unmanaged
    {
        Raylib.SetShaderValueV(this, locationIndex, values, uniformDataType, values.Length);
    }

    public void SetTexture(int locationIndex, Texture2D texture)
    {
        Raylib.SetShaderValueTexture(this, locationIndex, texture);
    }

    public void SetMatrix(int locationIndex, System.Numerics.Matrix4x4 matrix)
    {
        Raylib.SetShaderValueMatrix(this, locationIndex, matrix);
    }

    public void Unload()
    {
        Raylib.UnloadShader(this);
    }
}
