using System.Runtime.InteropServices;

namespace Raylib_cs;

/// <summary>
/// Material map index
/// </summary>
public enum MaterialMapIndex
{
    /// <summary>
    /// NOTE: Same as MATERIAL_MAP_DIFFUSE
    /// </summary>
    Albedo = 0,

    /// <summary>
    /// NOTE: Same as MATERIAL_MAP_SPECULAR
    /// </summary>
    Metalness,

    Normal,
    Roughness,
    Occlusion,
    Emission,
    Height,

    /// <summary>
    /// NOTE: Uses GL_TEXTURE_CUBE_MAP
    /// </summary>
    Cubemap,

    /// <summary>
    /// NOTE: Uses GL_TEXTURE_CUBE_MAP
    /// </summary>
    Irradiance,

    /// <summary>
    /// NOTE: Uses GL_TEXTURE_CUBE_MAP
    /// </summary>
    Prefilter,

    Brdf,

    Diffuse = Albedo,
    Specular = Metalness,
}

/// <summary>
/// Material texture map
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MaterialMap
{
    /// <summary>
    /// Material map texture
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// Material map color
    /// </summary>
    public Color Color;

    /// <summary>
    /// Material map value
    /// </summary>
    public float Value;
}

/// <summary>
/// Material type (generic)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Material
{
    /// <summary>
    /// Material shader
    /// </summary>
    public Shader Shader;

    // TODO: convert
    /// <summary>
    /// Material maps
    /// </summary>
    public MaterialMap* Maps;

    /// <summary>
    /// Material generic parameters (if required)
    /// </summary>
    public fixed float Param[4];
}
