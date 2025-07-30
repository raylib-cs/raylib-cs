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

    //TODO: convert
    /// <summary>
    /// Material maps
    /// </summary>
    public MaterialMap* Maps;

    /// <summary>
    /// Material generic parameters (if required)
    /// </summary>
    public fixed float Param[4];

    public readonly CBool IsValid => Raylib.IsMaterialValid(this);

    /// <summary>
    /// Load default material
    /// </summary>
    public static Material Load()
    {
        return Raylib.LoadMaterialDefault();
    }

    /// <summary>
    /// Load materials from model file
    /// </summary>
    public static Material[] Load(string fileName)
    {
        using AnsiBuffer buffer = fileName.ToAnsiBuffer();
        int count = 0;
        Material* materials = Raylib.LoadMaterials(buffer.AsPointer(), &count);
        Material[] output = new Material[count];
        for (int i = 0; i < count; i++)
        {
            output[i] = materials[i];
        }
        return output;
    }

    public static Material GetMaterial(ref Model model, int materialIndex)
    {
        return Raylib.GetMaterial(ref model, materialIndex);
    }

    public static Texture2D GetTexture(ref Model model, int materialIndex, MaterialMapIndex mapIndex)
    {
        return Raylib.GetMaterialTexture(ref model, materialIndex, mapIndex);
    }

    public void SetTexture(MaterialMapIndex mapIndex, Texture2D texture)
    {
        Raylib.SetMaterialTexture(ref this, mapIndex, texture);
    }

    public static void SetShader(ref Model model, int materialIndex, ref Shader shader)
    {
        Raylib.SetMaterialShader(ref model, materialIndex, ref shader);
    }

    public void Unload()
    {
        Raylib.UnloadMaterial(this);
    }
}
