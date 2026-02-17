using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;

namespace Raylib_cs;

[SuppressUnmanagedCodeSecurity]
public static unsafe partial class Rlgl
{
    /// <summary>
    /// Used by DllImport to load the native library
    /// </summary>
    public const string NativeLibName = "raylib";

    public const int DEFAULT_BATCH_BUFFER_ELEMENTS = 8192;
    public const int DEFAULT_BATCH_BUFFERS = 1;
    public const int DEFAULT_BATCH_DRAWCALLS = 256;
    public const int MAX_BATCH_ACTIVE_TEXTURES = 4;
    public const int MAX_MATRIX_STACK_SIZE = 32;
    public const float CULL_DISTANCE_NEAR = 0.01f;
    public const float CULL_DISTANCE_FAR = 1000.0f;

    // Texture parameters (equivalent to OpenGL defines)
    public const int TEXTURE_WRAP_S = 0x2802;
    public const int TEXTURE_WRAP_T = 0x2803;
    public const int TEXTURE_MAG_FILTER = 0x2800;
    public const int TEXTURE_MIN_FILTER = 0x2801;

    public const int TEXTURE_FILTER_NEAREST = 0x2600;
    public const int TEXTURE_FILTER_LINEAR = 0x2601;
    public const int TEXTURE_FILTER_MIP_NEAREST = 0x2700;
    public const int TEXTURE_FILTER_NEAREST_MIP_LINEAR = 0x2702;
    public const int TEXTURE_FILTER_LINEAR_MIP_NEAREST = 0x2701;
    public const int TEXTURE_FILTER_MIP_LINEAR = 0x2703;
    public const int TEXTURE_FILTER_ANISOTROPIC = 0x3000;
    public const int TEXTURE_MIPMAP_BIAS_RATIO = 0x4000;

    public const int TEXTURE_WRAP_REPEAT = 0x2901;
    public const int TEXTURE_WRAP_CLAMP = 0x812F;
    public const int TEXTURE_WRAP_MIRROR_REPEAT = 0x8370;
    public const int TEXTURE_WRAP_MIRROR_CLAMP = 0x8742;

    // GL equivalent data types
    public const int UNSIGNED_BYTE = 0x1401;
    public const int FLOAT = 0x1406;

    // Buffer usage hint
    public const int STREAM_DRAW = 0x88E0;
    public const int STREAM_READ = 0x88E1;
    public const int STREAM_COPY = 0x88E2;
    public const int STATIC_DRAW = 0x88E4;
    public const int STATIC_READ = 0x88E5;
    public const int STATIC_COPY = 0x88E6;
    public const int DYNAMIC_DRAW = 0x88E8;
    public const int DYNAMIC_READ = 0x88E9;
    public const int DYNAMIC_COPY = 0x88EA;

    // GL blending factors
    public const int ZERO = 0;
    public const int ONE = 1;
    public const int SRC_COLOR = 0x0300;
    public const int ONE_MINUS_SRC_COLOR = 0x0301;
    public const int SRC_ALPHA = 0x0302;
    public const int ONE_MINUS_SRC_ALPHA = 0x0303;
    public const int DST_ALPHA = 0x0304;
    public const int ONE_MINUS_DST_ALPHA = 0x0305;
    public const int DST_COLOR = 0x0306;
    public const int ONE_MINUS_DST_COLOR = 0x0307;
    public const int SRC_ALPHA_SATURATE = 0x0308;
    public const int CONSTANT_COLOR = 0x8001;
    public const int ONE_MINUS_CONSTANT_COLOR = 0x8002;
    public const int CONSTANT_ALPHA = 0x8003;
    public const int ONE_MINUS_CONSTANT_ALPHA = 0x8004;

    // GL blending functions/equations
    public const int FUNC_ADD = 0x8006;
    public const int MIN = 0x8007;
    public const int MAX = 0x8008;
    public const int FUNC_SUBTRACT = 0x800A;
    public const int FUNC_REVERSE_SUBTRACT = 0x800B;
    public const int BLEND_EQUATION = 0x8009;
    public const int BLEND_EQUATION_RGB = 0x8009;
    public const int BLEND_EQUATION_ALPHA = 0x883D;
    public const int BLEND_DST_RGB = 0x80C8;
    public const int BLEND_SRC_RGB = 0x80C9;
    public const int BLEND_DST_ALPHA = 0x80CA;
    public const int BLEND_SRC_ALPHA = 0x80CB;
    public const int BLEND_COLOR = 0x8005;

    // ------------------------------------------------------------------------------------
    // Functions Declaration - Matrix operations
    // ------------------------------------------------------------------------------------

    /// <summary>Choose the current matrix to be transformed</summary>
    [DllImport(NativeLibName, EntryPoint = "rlMatrixMode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MatrixMode(int mode);

    /// <inheritdoc cref="MatrixMode(int)"/>
    public static void MatrixMode(MatrixMode mode)
    {
        MatrixMode((int)mode);
    }

    /// <summary>Push the current matrix to stack</summary>
    [DllImport(NativeLibName, EntryPoint = "rlPushMatrix", CallingConvention = CallingConvention.Cdecl)]
    public static extern void PushMatrix();

    /// <summary>Pop lattest inserted matrix from stack</summary>
    [DllImport(NativeLibName, EntryPoint = "rlPopMatrix", CallingConvention = CallingConvention.Cdecl)]
    public static extern void PopMatrix();

    /// <summary>Reset current matrix to identity matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadIdentity", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoadIdentity();

    /// <summary>Multiply the current matrix by a translation matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlTranslatef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Translatef(float x, float y, float z);

    /// <summary>Multiply the current matrix by a rotation matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlRotatef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Rotatef(float angle, float x, float y, float z);

    /// <summary>Multiply the current matrix by a scaling matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlScalef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Scalef(float x, float y, float z);

    /// <summary>
    /// Multiply the current matrix by another matrix<br/>
    /// Current Matrix can be set via <see cref="MatrixMode(int)"/>
    /// </summary>
    [DllImport(NativeLibName, EntryPoint = "rlMultMatrixf", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MultMatrixf(float* matf);

    /// <inheritdoc cref="MultMatrixf(float*)"/>
    public static void MultMatrixf(Matrix4x4 matf)
    {
        Float16 f = Raymath.MatrixToFloatV(matf);
        MultMatrixf(f.v);
    }

    [DllImport(NativeLibName, EntryPoint = "rlFrustum", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Frustum(
        double left,
        double right,
        double bottom,
        double top,
        double znear,
        double zfar
    );

    [DllImport(NativeLibName, EntryPoint = "rlOrtho", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Ortho(
        double left,
        double right,
        double bottom,
        double top,
        double znear,
        double zfar
    );

    /// <summary>Set the viewport area</summary>
    [DllImport(NativeLibName, EntryPoint = "rlViewport", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Viewport(int x, int y, int width, int height);

    /// <summary>Set clip planes distances</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetClipPlanes", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetClipPlanes(double nearPlane, double farPlane);

    /// <summary>Get cull plane distance near</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetCullDistanceNear", CallingConvention = CallingConvention.Cdecl)]
    public static extern double GetCullDistanceNear();

    /// <summary>Get cull plane distance far</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetCullDistanceFar", CallingConvention = CallingConvention.Cdecl)]
    public static extern double GetCullDistanceFar();


    // ------------------------------------------------------------------------------------
    // Functions Declaration - Vertex level operations
    // ------------------------------------------------------------------------------------

    /// <summary>Initialize drawing mode (how to organize vertex)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlBegin", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Begin(int mode);

    public static void Begin(DrawMode mode)
    {
        Begin((int)mode);
    }

    /// <summary>Finish vertex providing</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnd", CallingConvention = CallingConvention.Cdecl)]
    public static extern void End();

    /// <summary>Define one vertex (position) - 2 int</summary>
    [DllImport(NativeLibName, EntryPoint = "rlVertex2i", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Vertex2i(int x, int y);

    /// <summary>Define one vertex (position) - 2 float</summary>
    [DllImport(NativeLibName, EntryPoint = "rlVertex2f", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Vertex2f(float x, float y);

    /// <summary>Define one vertex (position) - 3 float</summary>
    [DllImport(NativeLibName, EntryPoint = "rlVertex3f", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Vertex3f(float x, float y, float z);

    /// <summary>Define one vertex (texture coordinate) - 2 float</summary>
    [DllImport(NativeLibName, EntryPoint = "rlTexCoord2f", CallingConvention = CallingConvention.Cdecl)]
    public static extern void TexCoord2f(float x, float y);

    /// <summary>Define one vertex (normal) - 3 float</summary>
    [DllImport(NativeLibName, EntryPoint = "rlNormal3f", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Normal3f(float x, float y, float z);

    /// <summary>Define one vertex (color) - 4 byte</summary>
    [DllImport(NativeLibName, EntryPoint = "rlColor4ub", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Color4ub(byte r, byte g, byte b, byte a);

    /// <summary>Define one vertex (color) - 3 float</summary>
    [DllImport(NativeLibName, EntryPoint = "rlColor3f", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Color3f(float x, float y, float z);

    /// <summary>Define one vertex (color) - 4 float</summary>
    [DllImport(NativeLibName, EntryPoint = "rlColor4f", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Color4f(float x, float y, float z, float w);


    // ------------------------------------------------------------------------------------
    // Functions Declaration - OpenGL equivalent functions (common to 1.1, 3.3+, ES2)
    // NOTE: This functions are used to completely abstract raylib code from OpenGL layer
    // ------------------------------------------------------------------------------------

    /// <summary>Vertex buffers state</summary>

    /// <summary>Enable vertex array (VAO, if supported)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableVertexArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern CBool EnableVertexArray(uint vaoId);

    /// <summary>Disable vertex array (VAO, if supported)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableVertexArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableVertexArray();

    /// <summary>Enable vertex buffer (VBO)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableVertexBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableVertexBuffer(uint id);

    /// <summary>Disable vertex buffer (VBO)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableVertexBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableVertexBuffer();

    /// <summary>Enable vertex buffer element (VBO element)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableVertexBufferElement", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableVertexBufferElement(uint id);

    /// <summary>Disable vertex buffer element (VBO element)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableVertexBufferElement", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableVertexBufferElement();

    /// <summary>Enable vertex attribute index</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableVertexAttribute", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableVertexAttribute(uint index);

    /// <summary>Disable vertex attribute index</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableVertexAttribute", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableVertexAttribute(uint index);


    // Textures state

    /// <summary>Select and active a texture slot</summary>
    [DllImport(NativeLibName, EntryPoint = "rlActiveTextureSlot", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ActiveTextureSlot(int slot);

    /// <summary>Enable texture</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableTexture(uint id);

    /// <summary>Disable texture</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableTexture();

    /// <summary>Enable texture cubemap</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableTextureCubemap", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableTextureCubemap(uint id);

    /// <summary>Disable texture cubemap</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableTextureCubemap", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableTextureCubemap();

    /// <summary>Set texture parameters (filter, wrap)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlTextureParameters", CallingConvention = CallingConvention.Cdecl)]
    public static extern void TextureParameters(uint id, int param, int value);

    /// <summary>Set cubemap parameters (filter, wrap)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlCubemapParameters", CallingConvention = CallingConvention.Cdecl)]
    public static extern void CubemapParameters(uint id, int param, int value);


    // Shader state

    /// <summary>Enable shader program</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableShader", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableShader(uint id);

    /// <summary>Disable shader program</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableShader", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableShader();


    // Framebuffer state

    /// <summary>Enable render texture (fbo)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableFramebuffer(uint id);

    /// <summary>Disable render texture (fbo), return to default framebuffer</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableFramebuffer();

    /// <summary>Get the currently active render texture (fbo), 0 for default framebuffer</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetActiveFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetActiveFramebuffer();

    /// <summary>Blit active framebuffer to main framebuffer</summary>
    [DllImport(NativeLibName, EntryPoint = "rlBlitFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BlitFramebuffer();

    /// <summary>Bind framebuffer (FBO)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlBindFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BindFramebuffer(uint target, uint framebuffer);

    /// <summary>Activate multiple draw color buffers</summary>
    [DllImport(NativeLibName, EntryPoint = "rlActiveDrawBuffers", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ActiveDrawBuffers(int count);


    // General render state

    /// <summary>Enable color blending</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableColorBlend", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableColorBlend();

    /// <summary>Disable color blending</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableColorBlend", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableColorBlend();

    /// <summary>Enable depth test</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableDepthTest", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableDepthTest();

    /// <summary>Disable depth test</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableDepthTest", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableDepthTest();

    /// <summary>Enable depth write</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableDepthMask", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableDepthMask();

    /// <summary>Disable depth write</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableDepthMask", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableDepthMask();

    /// <summary>Enable backface culling</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableBackfaceCulling", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableBackfaceCulling();

    /// <summary>Disable backface culling</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableBackfaceCulling", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableBackfaceCulling();

    /// <summary>Color mask control</summary>
    [DllImport(NativeLibName, EntryPoint = "rlColorMask", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ColorMask(CBool r, CBool g, CBool b, CBool a);

    /// <summary>Set face culling mode</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetCullFace", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetCullFace(int mode);

    /// <summary>Enable scissor test</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableScissorTest", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableScissorTest();

    /// <summary>Disable scissor test</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableScissorTest", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableScissorTest();

    /// <summary>Scissor test</summary>
    [DllImport(NativeLibName, EntryPoint = "rlScissor", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Scissor(int x, int y, int width, int height);

    /// <summary>Enable wire mode</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableWireMode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableWireMode();

    /// <summary>Enable point mode</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnablePointMode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnablePointMode();

    /// <summary>Disable wire mode</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableWireMode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableWireMode();

    /// <summary>Set the line drawing width</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetLineWidth", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetLineWidth(float width);

    /// <summary>Get the line drawing width</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetLineWidth", CallingConvention = CallingConvention.Cdecl)]
    public static extern float GetLineWidth();

    /// <summary>Enable line aliasing</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableSmoothLines", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableSmoothLines();

    /// <summary>Disable line aliasing</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableSmoothLines", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableSmoothLines();

    /// <summary>Enable stereo rendering</summary>
    [DllImport(NativeLibName, EntryPoint = "rlEnableStereoRender", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EnableStereoRender();

    /// <summary>Disable stereo rendering</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDisableStereoRender", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisableStereoRender();

    /// <summary>Check if stereo render is enabled</summary>
    [DllImport(NativeLibName, EntryPoint = "rlIsStereoRenderEnabled", CallingConvention = CallingConvention.Cdecl)]
    public static extern CBool IsStereoRenderEnabled();

    /// <summary>Clear color buffer with color</summary>
    [DllImport(NativeLibName, EntryPoint = "rlClearColor", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ClearColor(byte r, byte g, byte b, byte a);

    /// <summary>Clear used screen buffers (color and depth)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlClearScreenBuffers", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ClearScreenBuffers();

    /// <summary>Check and log OpenGL error codes</summary>
    [DllImport(NativeLibName, EntryPoint = "rlCheckErrors", CallingConvention = CallingConvention.Cdecl)]
    public static extern void CheckErrors();

    /// <summary>Set blending mode</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetBlendMode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetBlendMode(BlendMode mode);

    /// <summary>Set blending mode factor and equation (using OpenGL factors)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetBlendFactors", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetBlendFactors(int glSrcFactor, int glDstFactor, int glEquation);

    /// <summary>Set blending mode factors and equations separately (using OpenGL factors)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetBlendFactorsSeparate", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetBlendFactorsSeparate(
        int glSrcRGB,
        int glDstRGB,
        int glSrcAlpha,
        int glDstAlpha,
        int glEqRGB,
        int glEqAlpha
    );


    // ------------------------------------------------------------------------------------
    // Functions Declaration - rlgl functionality
    // ------------------------------------------------------------------------------------

    /// <summary>Initialize rlgl (buffers, shaders, textures, states)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlglInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GlInit(int width, int height);

    /// <summary>De-inititialize rlgl (buffers, shaders, textures)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlglClose", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GlClose();

    /// <summary>Load OpenGL extensions</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadExtensions", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoadExtensions(void* loader);

    /// <summary>Get current OpenGL version</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetVersion", CallingConvention = CallingConvention.Cdecl)]
    public static extern GlVersion GetVersion();

    /// <summary>Get default framebuffer width</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetFramebufferWidth", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetFramebufferWidth();

    /// <summary>Get default framebuffer height</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetFramebufferHeight", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetFramebufferHeight();

    /// <summary>Get default texture</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetTextureIdDefault", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetTextureIdDefault();

    /// <summary>Get default shader</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetShaderIdDefault", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetShaderIdDefault();

    /// <summary>Get default shader locations</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetShaderLocsDefault", CallingConvention = CallingConvention.Cdecl)]
    public static extern int* GetShaderLocsDefault();

    // Render batch management

    /// <summary>Load a render batch system<br/>
    /// NOTE: rlgl provides a default render batch to behave like OpenGL 1.1 immediate mode<br/>
    /// but this render batch API is exposed in case custom batches are required</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadRenderBatch", CallingConvention = CallingConvention.Cdecl)]
    public static extern RenderBatch LoadRenderBatch(int numBuffers, int bufferElements);

    /// <summary>Unload render batch system</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUnloadRenderBatch", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadRenderBatch(RenderBatch batch);

    /// <summary>Draw render batch data (Update->Draw->Reset)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDrawRenderBatch", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DrawRenderBatch(RenderBatch* batch);

    /// <summary>Set the active render batch for rlgl (NULL for default internal)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetRenderBatchActive", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetRenderBatchActive(RenderBatch* batch);

    /// <summary>Update and draw internal render batch</summary>
    [DllImport(NativeLibName, EntryPoint = "rlDrawRenderBatchActive", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DrawRenderBatchActive();

    /// <summary>Check internal buffer overflow for a given number of vertex</summary>
    [DllImport(NativeLibName, EntryPoint = "rlCheckRenderBatchLimit", CallingConvention = CallingConvention.Cdecl)]
    public static extern CBool CheckRenderBatchLimit(int vCount);

    /// <summary>Set current texture for render batch and check buffers limits</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetTexture(uint id);


    // Vertex buffers management

    /// <summary>Load vertex array (vao) if supported</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadVertexArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadVertexArray();

    /// <summary>Load a vertex buffer attribute</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadVertexBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadVertexBuffer(void* buffer, int size, CBool dynamic);

    /// <summary>Load a new attributes element buffer</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadVertexBufferElement", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadVertexBufferElement(void* buffer, int size, CBool dynamic);

    /// <summary>Update GPU buffer with new data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUpdateVertexBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UpdateVertexBuffer(uint bufferId, void* data, int dataSize, int offset);

    /// <summary>Update vertex buffer elements with new data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUpdateVertexBufferElements", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UpdateVertexBufferElements(uint id, void* data, int dataSize, int offset);

    [DllImport(NativeLibName, EntryPoint = "rlUnloadVertexArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadVertexArray(uint vaoId);

    [DllImport(NativeLibName, EntryPoint = "rlUnloadVertexBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadVertexBuffer(uint vboId);

    /// <summary>Set vertex attribute data configuration</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetVertexAttribute", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetVertexAttribute(
        uint index,
        int compSize,
        int type,
        CBool normalized,
        int stride,
        int offset
    );

    [DllImport(NativeLibName, EntryPoint = "rlSetVertexAttributeDivisor", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetVertexAttributeDivisor(uint index, int divisor);

    /// <summary>Set vertex attribute default value</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetVertexAttributeDefault", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetVertexAttributeDefault(int locIndex, void* value, int attribType, int count);

    [DllImport(NativeLibName, EntryPoint = "rlDrawVertexArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DrawVertexArray(int offset, int count);

    [DllImport(NativeLibName, EntryPoint = "rlDrawVertexArrayElements", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DrawVertexArrayElements(int offset, int count, void* buffer);

    [DllImport(NativeLibName, EntryPoint = "rlDrawVertexArrayInstanced", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DrawVertexArrayInstanced(int offset, int count, int instances);

    [DllImport(NativeLibName, EntryPoint = "rlDrawVertexArrayElementsInstanced", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DrawVertexArrayElementsInstanced(
        int offset,
        int count,
        void* buffer,
        int instances
    );


    // Textures data management

    /// <summary>Load texture in GPU</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadTexture(void* data, int width, int height, PixelFormat format, int mipmapCount);

    /// <summary>Load depth texture/renderbuffer (to be attached to fbo)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadTextureDepth", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadTextureDepth(int width, int height, CBool useRenderBuffer);

    /// <summary>Load texture cubemap data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadTextureCubemap", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadTextureCubemap(void* data, int size, PixelFormat format, int mipmapCount);

    /// <summary>Update GPU texture with new data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUpdateTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UpdateTexture(
        uint id,
        int offsetX,
        int offsetY,
        int width,
        int height,
        PixelFormat format,
        void* data
    );

    /// <summary>Get OpenGL internal formats</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetGlTextureFormats", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetGlTextureFormats(
        PixelFormat format,
        uint* glInternalFormat,
        uint* glFormat,
        uint* glType
    );

    /// <summary>Get OpenGL internal formats</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetPixelFormatName", CallingConvention = CallingConvention.Cdecl)]
    public static extern sbyte* GetPixelFormatName(PixelFormat format);

    /// <summary>Unload texture from GPU memory</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUnloadTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadTexture(uint id);

    /// <summary>Generate mipmap data for selected texture</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGenTextureMipmaps", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GenTextureMipmaps(uint id, int width, int height, PixelFormat format, int* mipmaps);

    /// <summary>Read texture pixel data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlReadTexturePixels", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* ReadTexturePixels(uint id, int width, int height, PixelFormat format);

    /// <summary>Read screen pixel data (color buffer)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlReadScreenPixels", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* ReadScreenPixels(int width, int height);


    // Framebuffer management (fbo)

    /// <summary>Load an empty framebuffer</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadFramebuffer();

    /// <summary>Attach texture/renderbuffer to a framebuffer</summary>
    [DllImport(NativeLibName, EntryPoint = "rlFramebufferAttach", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FramebufferAttach(
        uint fboId,
        uint texId,
        FramebufferAttachType attachType,
        FramebufferAttachTextureType texType,
        int mipLevel
    );

    /// <summary>Verify framebuffer is complete</summary>
    [DllImport(NativeLibName, EntryPoint = "rlFramebufferComplete", CallingConvention = CallingConvention.Cdecl)]
    public static extern CBool FramebufferComplete(uint id);

    /// <summary>Delete framebuffer from GPU</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUnloadFramebuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadFramebuffer(uint id);


    // Shaders management

    /// <summary>Load shader from code strings</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadShaderCode", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadShaderCode(sbyte* vsCode, sbyte* fsCode);

    /// <summary>Compile custom shader and return shader id<br/>
    /// (type: VERTEX_SHADER, FRAGMENT_SHADER, COMPUTE_SHADER)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlCompileShader", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint CompileShader(sbyte* shaderCode, int type);

    /// <summary>Load custom shader program</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadShaderProgram", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadShaderProgram(uint vShaderId, uint fShaderId);

    /// <summary>Unload shader program</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUnloadShaderProgram", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadShaderProgram(uint id);

    /// <summary>Get shader location uniform</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetLocationUniform", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocationUniform(uint shaderId, sbyte* uniformName);

    /// <summary>Get shader location attribute</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetLocationAttrib", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocationAttrib(uint shaderId, sbyte* attribName);

    /// <summary>Set shader value uniform</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetUniform", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUniform(int locIndex, void* value, int uniformType, int count);

    /// <summary>Set shader value matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetUniformMatrix", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUniformMatrix(int locIndex, Matrix4x4 mat);

    /// <summary>Set shader value matrices</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetUniformMatrices", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUniformMatrices(int locIndex, Matrix4x4* mat, int count);

    /// <summary>Set shader value sampler</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetUniformSampler", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUniformSampler(int locIndex, uint textureId);

    /// <summary>Set shader currently active (id and locations)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetShader", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetShader(uint id, int* locs);


    // Compute shader management

    /// <summary>Load compute shader program</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadComputeShaderProgram", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadComputeShaderProgram(uint shaderId);

    /// <summary>Dispatch compute shader (equivalent to *draw* for graphics pilepine)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlComputeShaderDispatch", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ComputeShaderDispatch(uint groupX, uint groupY, uint groupZ);


    // Shader buffer storage object management (ssbo)

    /// <summary>Load shader storage buffer object (SSBO)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadShaderBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint LoadShaderBuffer(uint size, void* data, int usageHint);

    /// <summary>Unload shader storage buffer object (SSBO)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUnloadShaderBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UnloadShaderBuffer(uint ssboId);

    /// <summary>Update SSBO buffer data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlUpdateShaderBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UpdateShaderBuffer(uint id, void* data, uint dataSize, uint offset);

    /// <summary>Bind SSBO buffer data</summary>
    [DllImport(NativeLibName, EntryPoint = "rlBindShaderBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BindShaderBuffer(uint id, uint index);

    /// <summary>Read SSBO buffer data (GPU->CPU)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlReadShaderBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ReadShaderBuffer(uint id, void* dest, uint count, uint offset);

    /// <summary>Copy SSBO data between buffers</summary>
    [DllImport(NativeLibName, EntryPoint = "rlCopyShaderBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void CopyShaderBuffer(
        uint destId,
        uint srcId,
        uint destOffset,
        uint srcOffset,
        uint count
    );

    /// <summary>Get SSBO buffer size</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetShaderBufferSize", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetShaderBufferSize(uint id);


    // Buffer management

    /// <summary>Bind image texture</summary>
    [DllImport(NativeLibName, EntryPoint = "rlBindImageTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BindImageTexture(uint id, uint index, int format, CBool readOnly);


    // Matrix state management

    /// <summary>Get internal modelview matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetMatrixModelview", CallingConvention = CallingConvention.Cdecl)]
    public static extern Matrix4x4 GetMatrixModelview();

    /// <summary>Get internal projection matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetMatrixProjection", CallingConvention = CallingConvention.Cdecl)]
    public static extern Matrix4x4 GetMatrixProjection();

    /// <summary>Get internal accumulated transform matrix</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetMatrixTransform", CallingConvention = CallingConvention.Cdecl)]
    public static extern Matrix4x4 GetMatrixTransform();

    /// <summary>Get internal projection matrix for stereo render (selected eye)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetMatrixProjectionStereo", CallingConvention = CallingConvention.Cdecl)]
    public static extern Matrix4x4 GetMatrixProjectionStereo(int eye);

    /// <summary>Get internal view offset matrix for stereo render (selected eye)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlGetMatrixViewOffsetStereo", CallingConvention = CallingConvention.Cdecl)]
    public static extern Matrix4x4 GetMatrixViewOffsetStereo(int eye);

    /// <summary>Set a custom projection matrix (replaces internal projection matrix)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetMatrixProjection", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetMatrixProjection(Matrix4x4 view);

    /// <summary>Set a custom modelview matrix (replaces internal modelview matrix)</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetMatrixModelview", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetMatrixModelView(Matrix4x4 proj);

    /// <summary>Set eyes projection matrices for stereo rendering</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetMatrixProjectionStereo", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetMatrixProjectionStereo(Matrix4x4 left, Matrix4x4 right);

    /// <summary>Set eyes view offsets matrices for stereo rendering</summary>
    [DllImport(NativeLibName, EntryPoint = "rlSetMatrixViewOffsetStereo", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetMatrixViewOffsetStereo(Matrix4x4 left, Matrix4x4 right);


    // Quick and dirty cube/quad buffers load->draw->unload

    /// <summary>Load and draw a cube</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadDrawCube", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoadDrawCube();

    /// <summary>Load and draw a quad</summary>
    [DllImport(NativeLibName, EntryPoint = "rlLoadDrawQuad", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoadDrawQuad();
}
