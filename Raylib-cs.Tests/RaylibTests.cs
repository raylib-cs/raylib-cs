using System.Runtime.CompilerServices;
using Xunit;

namespace Raylib_cs.Tests;

using System.Collections.Generic;

public static class BlittableHelper
{
    public static bool IsBlittable<T>()
    {
        return !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
    }
}

public class RaylibTests
{
    private static void CheckType<T>() where T : unmanaged
    {
        Assert.True(BlittableHelper.IsBlittable<T>());
    }

    [Fact]
    public void CheckTypes()
    {
        CheckType<Color>();
        CheckType<Rectangle>();
        CheckType<Image>();
        CheckType<Texture2D>();
        CheckType<RenderTexture2D>();
        CheckType<NPatchInfo>();
        CheckType<GlyphInfo>();
        CheckType<Font>();
        CheckType<Camera2D>();
        CheckType<Camera3D>();
        CheckType<Mesh>();
        CheckType<Shader>();
        CheckType<MaterialMap>();
        CheckType<Material>();
        CheckType<Transform>();
        CheckType<BoneInfo>();
        CheckType<Model>();
        CheckType<ModelSkeleton>();
        CheckType<ModelAnimation>();
        CheckType<Ray>();
        CheckType<RayCollision>();
        CheckType<BoundingBox>();
        CheckType<Wave>();
        CheckType<AudioStream>();
        CheckType<Sound>();
        CheckType<Music>();
        CheckType<VrDeviceInfo>();
        CheckType<VrStereoConfig>();
        CheckType<RenderBatch>();
    }
}
