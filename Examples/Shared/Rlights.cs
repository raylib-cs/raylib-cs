using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Shared;

public struct Light
{
    public bool Enabled;
    public LightType Type;
    public Vector3 Position;
    public Vector3 Target;
    public Color Color;

    public int EnabledLoc;
    public int TypeLoc;
    public int PosLoc;
    public int TargetLoc;
    public int ColorLoc;
}

public enum LightType
{
    Directorional,
    Point
}

public static class Rlights
{
    public static Light CreateLight(
        int lightsCount,
        LightType type,
        Vector3 pos,
        Vector3 target,
        Color color,
        Shader shader
    )
    {
        Light light = new();

        light.Enabled = true;
        light.Type = type;
        light.Position = pos;
        light.Target = target;
        light.Color = color;

        var enabledName = "lights[" + lightsCount + "].enabled";
        var typeName = "lights[" + lightsCount + "].type";
        var posName = "lights[" + lightsCount + "].position";
        var targetName = "lights[" + lightsCount + "].target";
        var colorName = "lights[" + lightsCount + "].color";

        light.EnabledLoc = GetShaderLocation(shader, enabledName);
        light.TypeLoc = GetShaderLocation(shader, typeName);
        light.PosLoc = GetShaderLocation(shader, posName);
        light.TargetLoc = GetShaderLocation(shader, targetName);
        light.ColorLoc = GetShaderLocation(shader, colorName);

        UpdateLightValues(shader, light);

        return light;
    }

    public static void UpdateLightValues(Shader shader, Light light)
    {
        // Send to shader light enabled state and type
        Raylib.SetShaderValue(
            shader,
            light.EnabledLoc,
            light.Enabled ? 1 : 0,
            ShaderUniformDataType.Int
        );
        Raylib.SetShaderValue(shader, light.TypeLoc, (int)light.Type, ShaderUniformDataType.Int);

        // Send to shader light target position values
        Raylib.SetShaderValue(shader, light.PosLoc, light.Position, ShaderUniformDataType.Vec3);

        // Send to shader light target position values
        Raylib.SetShaderValue(shader, light.TargetLoc, light.Target, ShaderUniformDataType.Vec3);

        // Send to shader light color values
        var color = new[]
        {
                (float)light.Color.R / (float)255,
                (float)light.Color.G / (float)255,
                (float)light.Color.B / (float)255,
                (float)light.Color.A / (float)255
            };
        Raylib.SetShaderValue(shader, light.ColorLoc, color, ShaderUniformDataType.Vec4);
    }
}
