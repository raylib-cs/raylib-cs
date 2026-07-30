namespace Examples.Shared;

public struct PbrLight
{
    public PbrLightType Type;
    public bool Enabled;
    public Vector3 Position;
    public Vector3 Target;
    public Vector4 Color;
    public float Intensity;

    // Shader light parameters locations
    public int TypeLoc;
    public int EnabledLoc;
    public int PositionLoc;
    public int TargetLoc;
    public int ColorLoc;
    public int IntensityLoc;
}

public enum PbrLightType
{
    Directorional,
    Point,
    Spot
}

public class PbrLights
{
    public static PbrLight CreateLight(
        int lightsCount,
        PbrLightType type,
        Vector3 pos,
        Vector3 target,
        Color color,
        float intensity,
        Shader shader
    )
    {
        PbrLight light = new();

        light.Enabled = true;
        light.Type = type;
        light.Position = pos;
        light.Target = target;
        light.Color = new Vector4(
            color.R / 255.0f,
            color.G / 255.0f,
            color.B / 255.0f,
            color.A / 255.0f
        );
        light.Intensity = intensity;

        var enabledName = "lights[" + lightsCount + "].enabled";
        var typeName = "lights[" + lightsCount + "].type";
        var posName = "lights[" + lightsCount + "].position";
        var targetName = "lights[" + lightsCount + "].target";
        var colorName = "lights[" + lightsCount + "].color";
        var intensityName = "lights[" + lightsCount + "].intensity";

        light.EnabledLoc = GetShaderLocation(shader, enabledName);
        light.TypeLoc = GetShaderLocation(shader, typeName);
        light.PositionLoc = GetShaderLocation(shader, posName);
        light.TargetLoc = GetShaderLocation(shader, targetName);
        light.ColorLoc = GetShaderLocation(shader, colorName);
        light.IntensityLoc = GetShaderLocation(shader, intensityName);

        UpdateLightValues(shader, light);

        return light;
    }

    public static void UpdateLightValues(Shader shader, PbrLight light)
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
        Raylib.SetShaderValue(shader, light.PositionLoc, light.Position, ShaderUniformDataType.Vec3);

        // Send to shader light target position values
        Raylib.SetShaderValue(shader, light.TargetLoc, light.Target, ShaderUniformDataType.Vec3);

        // Send to shader light color values
        Raylib.SetShaderValue(shader, light.ColorLoc, light.Color, ShaderUniformDataType.Vec4);

        // Send to shader light intensity values
        Raylib.SetShaderValue(shader, light.IntensityLoc, light.Intensity, ShaderUniformDataType.Float);
    }
}
