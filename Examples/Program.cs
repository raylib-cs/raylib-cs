using System.Diagnostics;
using Examples.Core;
using Examples.Shapes;
using Examples.Textures;
using Examples.Text;
using Examples.Models;
using Examples.Shaders;
using Examples.Audio;

namespace Examples;

public class ExampleInfo
{
    public ExampleInfo(string name, Func<int> main)
    {
        this.Name = name;
        this.Main = main;
    }

    public string Name
    {
        get; set;
    }

    public Func<int> Main
    {
        get; set;
    }
}

public class ExampleList
{
    public static ExampleInfo[] AllExamples = new[]
    {
        // Core
        new ExampleInfo("DeltaTime", DeltaTime.Main),
        new ExampleInfo("InputGesturesTestBed", InputGesturesTestBed.Main),
        new ExampleInfo("InputVirtualControls", InputVirtualControls.Main),
        new ExampleInfo("Camera2dPlatformer", Camera2dPlatformer.Main),
        new ExampleInfo("Camera2dDemo", Camera2dDemo.Main),
        new ExampleInfo("Camera3dFirstPerson", Camera3dFirstPerson.Main),
        new ExampleInfo("Camera3dFree", Camera3dFree.Main),
        new ExampleInfo("Camera3dMode", Camera3dMode.Main),
        new ExampleInfo("Picking3d", Picking3d.Main),
        new ExampleInfo("BasicScreenManager", BasicScreenManager.Main),
        new ExampleInfo("BasicWindow", BasicWindow.Main),
        new ExampleInfo("CustomLogging", CustomLogging.Main),
        new ExampleInfo("DropFiles", DropFiles.Main),
        new ExampleInfo("InputGamepad", InputGamepad.Main),
        new ExampleInfo("InputGestures", InputGestures.Main),
        new ExampleInfo("InputKeys", InputKeys.Main),
        new ExampleInfo("InputMouseWheel", InputMouseWheel.Main),
        new ExampleInfo("InputMouse", InputMouse.Main),
        new ExampleInfo("InputMultitouch", InputMultitouch.Main),
        new ExampleInfo("RandomValues", RandomValues.Main),
        new ExampleInfo("ScissorTest", ScissorTest.Main),
        new ExampleInfo("SmoothPixelPerfect", SmoothPixelPerfect.Main),
        new ExampleInfo("SplitScreen", SplitScreen.Main),
        new ExampleInfo("StorageValues", StorageValues.Main),
        new ExampleInfo("VrSimulator", VrSimulator.Main),
        new ExampleInfo("WindowFlags", WindowFlags.Main),
        new ExampleInfo("WindowLetterbox", WindowLetterbox.Main),
        new ExampleInfo("WorldScreen", WorldScreen.Main),

        // Shapes
        new ExampleInfo("BasicShapes", BasicShapes.Main),
        new ExampleInfo("BouncingBall", BouncingBall.Main),
        new ExampleInfo("CollisionArea", CollisionArea.Main),
        new ExampleInfo("ColorsPalette", ColorsPalette.Main),
        new ExampleInfo("EasingsBallAnim", EasingsBallAnim.Main),
        new ExampleInfo("EasingsBoxAnim", EasingsBoxAnim.Main),
        new ExampleInfo("EasingsRectangleArray", EasingsRectangleArray.Main),
        new ExampleInfo("FollowingEyes", FollowingEyes.Main),
        new ExampleInfo("LinesBezier", LinesBezier.Main),
        new ExampleInfo("LogoRaylibAnim", LogoRaylibAnim.Main),
        new ExampleInfo("LogoRaylibShape", LogoRaylibShape.Main),
        new ExampleInfo("RectangleScaling", RectangleScaling.Main),

        // Textures
        new ExampleInfo("BackgroundScrolling", BackgroundScrolling.Main),
        new ExampleInfo("BlendModes", BlendModes.Main),
        new ExampleInfo("Bunnymark", Bunnymark.Main),
        new ExampleInfo("DrawTiled", DrawTiled.Main),
        new ExampleInfo("ImageDrawing", ImageDrawing.Main),
        new ExampleInfo("ImageGeneration", ImageGeneration.Main),
        new ExampleInfo("ImageLoading", ImageLoading.Main),
        new ExampleInfo("ImageProcessing", ImageProcessing.Main),
        new ExampleInfo("ImageText", ImageText.Main),
        new ExampleInfo("LogoRaylibTexture", LogoRaylibTexture.Main),
        new ExampleInfo("MousePainting", MousePainting.Main),
        new ExampleInfo("NpatchDrawing", NpatchDrawing.Main),
        new ExampleInfo("ParticlesBlending", ParticlesBlending.Main),
        new ExampleInfo("TexturedCurve", TexturedCurve.Main),
        new ExampleInfo("Polygon", Polygon.Main),
        new ExampleInfo("RawData", RawData.Main),
        new ExampleInfo("SpriteAnim", SpriteAnim.Main),
        new ExampleInfo("SpriteButton", SpriteButton.Main),
        new ExampleInfo("SpriteExplosion", SpriteExplosion.Main),
        new ExampleInfo("SrcRecDstRec", SrcRecDstRec.Main),
        new ExampleInfo("ToImage", ToImage.Main),

        // Text
        new ExampleInfo("CodepointsLoading", CodepointsLoading.Main),
        new ExampleInfo("FontFilters", FontFilters.Main),
        new ExampleInfo("FontLoading", FontLoading.Main),
        new ExampleInfo("FontSdf", FontSdf.Main),
        new ExampleInfo("FontSpritefont", FontSpritefont.Main),
        new ExampleInfo("FormatText", FormatText.Main),
        new ExampleInfo("InputBox", InputBox.Main),
        new ExampleInfo("RaylibFonts", RaylibFonts.Main),
        new ExampleInfo("RectangleBounds", RectangleBounds.Main),
        new ExampleInfo("WritingAnim", WritingAnim.Main),

        // Models
        new ExampleInfo("LoadingIqm", LoadingIqm.Main),
        new ExampleInfo("BillboardDemo", BillboardDemo.Main),
        new ExampleInfo("BoxCollisions", BoxCollisions.Main),
        new ExampleInfo("CubicmapDemo", CubicmapDemo.Main),
        new ExampleInfo("ModelCubeTexture", ModelCubeTexture.Main),
        new ExampleInfo("FirstPersonMaze", FirstPersonMaze.Main),
        new ExampleInfo("GeometricShapes", GeometricShapes.Main),
        new ExampleInfo("HeightmapDemo", HeightmapDemo.Main),
        new ExampleInfo("MeshDemo", MeshDemo.Main),
        new ExampleInfo("ModelLoading", ModelLoading.Main),
        new ExampleInfo("MeshGeneration", MeshGeneration.Main),
        new ExampleInfo("MeshPicking", MeshPicking.Main),
        new ExampleInfo("OrthographicProjection", OrthographicProjection.Main),
        new ExampleInfo("SolarSystem", SolarSystem.Main),
        new ExampleInfo("SkyboxDemo", SkyboxDemo.Main),
        new ExampleInfo("WavingCubes", WavingCubes.Main),
        new ExampleInfo("YawPitchRoll", YawPitchRoll.Main),

        // Shaders
        new ExampleInfo("BasicLighting", BasicLighting.Main),
        new ExampleInfo("BasicPbr", BasicPbr.Main),
        new ExampleInfo("CustomUniform", CustomUniform.Main),
        new ExampleInfo("Eratosthenes", Eratosthenes.Main),
        new ExampleInfo("Fog", Fog.Main),
        new ExampleInfo("HotReloading", HotReloading.Main),
        new ExampleInfo("HybridRender", HybridRender.Main),
        new ExampleInfo("JuliaSet", JuliaSet.Main),
        new ExampleInfo("ModelShader", ModelShader.Main),
        new ExampleInfo("MultiSample2d", MultiSample2d.Main),
        new ExampleInfo("PaletteSwitch", PaletteSwitch.Main),
        new ExampleInfo("PostProcessing", PostProcessing.Main),
        new ExampleInfo("Raymarching", Raymarching.Main),
        new ExampleInfo("MeshInstancing", MeshInstancing.Main),
        new ExampleInfo("ShapesTextures", ShapesTextures.Main),
        new ExampleInfo("SimpleMask", SimpleMask.Main),
        new ExampleInfo("Spotlight", Spotlight.Main),
        new ExampleInfo("TextureDrawing", TextureDrawing.Main),
        new ExampleInfo("TextureOutline", TextureOutline.Main),
        new ExampleInfo("TextureWaves", TextureWaves.Main),
        new ExampleInfo("WriteDepth", WriteDepth.Main),

        // Audio
        new ExampleInfo("ModulePlaying", ModulePlaying.Main),
        new ExampleInfo("MusicStreamDemo", MusicStreamDemo.Main),
        new ExampleInfo("SoundLoading", SoundLoading.Main),
    };

    public static ExampleInfo GetExample(string name)
    {
        var example = Array.Find(ExampleList.AllExamples, x =>
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
        );
        return example;
    }
}

class Program
{
    static unsafe void Main(string[] args)
    {
        Raylib.SetTraceLogCallback(&Logging.LogConsole);

        if (args.Length > 0)
        {
            var example = ExampleList.GetExample(args[0]);
            example?.Main?.Invoke();
            return;
        }

        foreach (var example in ExampleList.AllExamples)
        {
            RunExampleProcess(Environment.ProcessPath, example.Name);
        }
    }

    static void RunExampleProcess(
        string fileName,
        string exampleName
    )
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = exampleName
        };
        using var process = Process.Start(processStartInfo);
        process?.WaitForExit();
    }
}
