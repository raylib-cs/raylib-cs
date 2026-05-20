using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Raylib_cs;

// NOTE: Helper types to be used instead of array return types for *ToFloat functions
public unsafe struct Float3
{
    public fixed float v[3];
}

public unsafe struct Float16
{
    public fixed float v[16];
}

[SuppressUnmanagedCodeSecurity]
public static unsafe partial class Raymath
{
    /// <summary>
    /// Used by LibraryImport to load the native library
    /// </summary>
    public const string NativeLibName = Raylib.NativeLibName;

    /// <summary>Clamp float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Clamp(float value, float min, float max);

    /// <summary>Calculate linear interpolation between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Lerp(float start, float end, float amount);

    /// <summary>Normalize input value within input range</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Normalize(float value, float start, float end);

    /// <summary>Remap input value within input range to output range</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Remap(float value,
        float inputStart, float inputEnd,
        float outputStart, float outputEnd
    );

    /// <summary>Wrap input value from min to max</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Wrap(float value, float min, float max);

    /// <summary>Check whether two given floats are almost equal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FloatEquals(float x, float y);

    /// <summary>Vector with components value 0.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Zero();

    /// <summary>Vector with components value 1.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2One();

    /// <summary>Add two vectors (v1 + v2)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Add(Vector2 v1, Vector2 v2);

    /// <summary>Add vector and float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2AddValue(Vector2 v, float add);

    /// <summary>Subtract two vectors (v1 - v2)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Subtract(Vector2 v1, Vector2 v2);

    /// <summary>Subtract vector by float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2SubtractValue(Vector2 v, float sub);

    /// <summary>Calculate vector length</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2Length(Vector2 v);

    /// <summary>Calculate vector square length</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2LengthSqr(Vector2 v);

    /// <summary>Calculate two vectors dot product</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2DotProduct(Vector2 v1, Vector2 v2);

    /// <summary>Calculate two vectors cross product</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2CrossProduct(Vector2 v1, Vector2 v2);

    /// <summary>Calculate distance between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2Distance(Vector2 v1, Vector2 v2);

    /// <summary>Calculate square distance between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2DistanceSqr(Vector2 v1, Vector2 v2);

    /// <summary>
    /// Calculate the signed angle from v1 to v2, relative to the origin (0, 0)
    /// NOTE: Coordinate system convention: positive X right, positive Y down
    /// positive angles appear clockwise, and negative angles appear counterclockwise
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2Angle(Vector2 v1, Vector2 v2);

    /// <summary>
    /// Calculate angle defined by a two vectors line
    /// NOTE: Parameters need to be normalized
    /// Current implementation should be aligned with glm::angle
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector2LineAngle(Vector2 start, Vector2 end);

    /// <summary>Scale vector (multiply by value)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Scale(Vector2 v, float scale);

    /// <summary>Multiply vector by vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Multiply(Vector2 v1, Vector2 v2);

    /// <summary>Negate vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Negate(Vector2 v);

    /// <summary>Divide vector by vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Divide(Vector2 v1, Vector2 v2);

    /// <summary>Normalize provided vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Normalize(Vector2 v);

    /// <summary>Transforms a Vector2 by a given Matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Transform(Vector2 v, Matrix4x4 mat);

    /// <summary>Calculate linear interpolation between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Lerp(Vector2 v1, Vector2 v2, float amount);

    /// <summary>Calculate reflected vector to normal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Reflect(Vector2 v, Vector2 normal);

    /// <summary>Rotate vector by angle</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Rotate(Vector2 v, float angle);

    /// <summary>Move Vector towards target</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2MoveTowards(Vector2 v, Vector2 target, float maxDistance);

    /// <summary>Invert the given vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Invert(Vector2 v);

    /// <summary>
    /// Clamp the components of the vector between min and max values specified by the given vectors
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Clamp(Vector2 v, Vector2 min, Vector2 max);

    /// <summary>Clamp the magnitude of the vector between two min and max values</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2ClampValue(Vector2 v, float min, float max);

    /// <summary>Check whether two given vectors are almost equal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Vector2Equals(Vector2 p, Vector2 q);

    /// <summary>Compute the direction of a refracted ray</summary>
    /// <param name="v">normalized direction of the incoming ray</param>
    /// <param name="n">normalized normal vector of the interface of two optical media</param>
    /// <param name="r">
    /// ratio of the refractive index of the medium from where the ray comes
    /// to the refractive index of the medium on the other side of the surface
    /// </param>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector2Refract(Vector2 v, Vector2 n, float r);

    /// <summary>Vector with components value 0.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Zero();

    /// <summary>Vector with components value 1.0f</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3One();

    /// <summary>Add two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Add(Vector3 v1, Vector3 v2);

    /// <summary>Add vector and float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3AddValue(Vector3 v, float add);

    /// <summary>Subtract two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Subtract(Vector3 v1, Vector3 v2);

    /// <summary>Subtract vector and float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3SubtractValue(Vector3 v, float sub);

    /// <summary>Multiply vector by scalar</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Scale(Vector3 v, float scalar);

    /// <summary>Multiply vector by vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Multiply(Vector3 v1, Vector3 v2);

    /// <summary>Calculate two vectors cross product</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3CrossProduct(Vector3 v1, Vector3 v2);

    /// <summary>Calculate one vector perpendicular vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Perpendicular(Vector3 v);

    /// <summary>Calculate vector length</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector3Length(Vector3 v);

    /// <summary>Calculate vector square length</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector3LengthSqr(Vector3 v);

    /// <summary>Calculate two vectors dot product</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector3DotProduct(Vector3 v1, Vector3 v2);

    /// <summary>Calculate distance between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector3Distance(Vector3 v1, Vector3 v2);

    /// <summary>Calculate square distance between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float Vector3DistanceSqr(Vector3 v1, Vector3 v2);

    /// <summary>Calculate angle between two vectors in XY and XZ</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector2 Vector3Angle(Vector3 v1, Vector3 v2);

    /// <summary>Negate provided vector (invert direction)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Negate(Vector3 v);

    /// <summary>Divide vector by vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Divide(Vector3 v1, Vector3 v2);

    /// <summary>Normalize provided vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Normalize(Vector3 v);

    /// <summary>Calculate the projection of the vector v1 on to v2</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Project(Vector3 v1, Vector3 v2);

    /// <summary>Calculate the rejection of the vector v1 on to v2</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Reject(Vector3 v1, Vector3 v2);

    /// <summary>
    /// Orthonormalize provided vectors<br/>
    /// Makes vectors normalized and orthogonal to each other<br/>
    /// Gram-Schmidt function implementation
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void Vector3OrthoNormalize(Vector3* v1, Vector3* v2);

    /// <summary>Transforms a Vector3 by a given Matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Transform(Vector3 v, Matrix4x4 mat);

    /// <summary>Transform a vector by quaternion rotation</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3RotateByQuaternion(Vector3 v, Quaternion q);

    /// <summary>Rotates a vector around an axis</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3RotateByAxisAngle(Vector3 v, Vector3 axis, float angle);

    /// <summary>Move Vector towards target</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3MoveTowards(Vector3 v, Vector3 target, float maxDistance);

    /// <summary>Calculate linear interpolation between two vectors</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Lerp(Vector3 v1, Vector3 v2, float amount);

    /// <summary>
    /// Calculate cubic hermite interpolation between two vectors and their tangents
    /// as described in the GLTF 2.0 specification: https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html#interpolation-cubic
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3CubicHermite(Vector3 v1, Vector3 tangent1, Vector3 v2, Vector3 tangent2, float amount);

    /// <summary>Calculate reflected vector to normal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Reflect(Vector3 v, Vector3 normal);

    /// <summary>Get min value for each pair of components</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Min(Vector3 v1, Vector3 v2);

    /// <summary>Get max value for each pair of components</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Max(Vector3 v1, Vector3 v2);

    /// <summary>
    /// Compute barycenter coordinates (u, v, w) for point p with respect to triangle (a, b, c)<br/>
    /// NOTE: Assumes P is on the plane of the triangle
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Barycenter(Vector3 p, Vector3 a, Vector3 b, Vector3 c);

    /// <summary>
    /// Projects a Vector3 from screen space into object space<br/>
    /// NOTE: We are avoiding calling other raymath functions despite available
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Unproject(Vector3 source, Matrix4x4 projection, Matrix4x4 view);

    /// <summary>Get Vector3 as float array</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Float3 Vector3ToFloatV(Vector3 v);

    /// <summary>Invert the given vector</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Invert(Vector3 v);

    /// <summary>
    /// Clamp the components of the vector between
    /// min and max values specified by the given vectors
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Clamp(Vector3 v, Vector3 min, Vector3 max);

    /// <summary>Clamp the magnitude of the vector between two values</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3ClampValue(Vector3 v, float min, float max);

    /// <summary>Check whether two given vectors are almost equal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int Vector3Equals(Vector3 p, Vector3 q);

    /// <summary>
    /// Compute the direction of a refracted ray where v specifies the
    /// normalized direction of the incoming ray, n specifies the
    /// normalized normal vector of the interface of two optical media,
    /// and r specifies the ratio of the refractive index of the medium
    /// from where the ray comes to the refractive index of the medium
    /// on the other side of the surface
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 Vector3Refract(Vector3 v, Vector3 n, float r);


    /// <summary>Compute matrix determinant</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float MatrixDeterminant(Matrix4x4 mat);

    /// <summary>Get the trace of the matrix (sum of the values along the diagonal)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float MatrixTrace(Matrix4x4 mat);

    /// <summary>Transposes provided matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixTranspose(Matrix4x4 mat);

    /// <summary>Invert provided matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixInvert(Matrix4x4 mat);

    /// <summary>Get identity matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixIdentity();

    /// <summary>Add two matrices</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixAdd(Matrix4x4 left, Matrix4x4 right);

    /// <summary>Subtract two matrices (left - right)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixSubtract(Matrix4x4 left, Matrix4x4 right);

    /// <summary>
    /// Get two matrix multiplication<br/>
    /// NOTE: When multiplying matrices... the order matters!
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixMultiply(Matrix4x4 left, Matrix4x4 right);

    /// <summary>
    /// Multiply matrix components by value
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixMultiplyValue(Matrix4x4 left, float value);

    /// <summary>Get translation matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixTranslate(float x, float y, float z);

    /// <summary>
    /// Create rotation matrix from axis and angle<br/>
    /// NOTE: Angle should be provided in radians
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixRotate(Vector3 axis, float angle);

    /// <summary>Get x-rotation matrix (angle in radians)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixRotateX(float angle);

    /// <summary>Get y-rotation matrix (angle in radians)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixRotateY(float angle);

    /// <summary>Get z-rotation matrix (angle in radians)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixRotateZ(float angle);

    /// <summary>Get xyz-rotation matrix (angles in radians)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixRotateXYZ(Vector3 ang);

    /// <summary>Get zyx-rotation matrix (angles in radians)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixRotateZYX(Vector3 ang);

    /// <summary>Get scaling matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixScale(float x, float y, float z);

    /// <summary>Get perspective projection matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixFrustum(
        double left,
        double right,
        double bottom,
        double top,
        double nearPlane,
        double farPlane
    );

    /// <summary>
    /// Get perspective projection matrix<br/>
    /// NOTE: Angle should be provided in radians
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixPerspective(double fovy, double aspect, double near, double far);

    /// <summary>Get orthographic projection matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixOrtho(
        double left,
        double right,
        double bottom,
        double top,
        double near,
        double far
    );

    /// <summary>Get camera look-at matrix (view matrix)</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixLookAt(Vector3 eye, Vector3 target, Vector3 up);

    /// <summary>Get float array of matrix data</summary>
    [LibraryImport(Raylib.NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Float16 MatrixToFloatV(Matrix4x4 m);


    /// <summary>Add 2 quaternions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionAdd(Quaternion q1, Quaternion q2);

    /// <summary>Add quaternion and float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionAddValue(Quaternion q, float add);

    /// <summary>Subtract 2 quaternions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionSubtract(Quaternion q1, Quaternion q2);

    /// <summary>Subtract quaternion and float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionSubtractValue(Quaternion q, float add);

    /// <summary>Get identity quaternion</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionIdentity();

    /// <summary>Computes the length of a quaternion</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float QuaternionLength(Quaternion q);

    /// <summary>Normalize provided quaternion</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionNormalize(Quaternion q);

    /// <summary>Invert provided quaternion</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionInvert(Quaternion q);

    /// <summary>Calculate two quaternion multiplication</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionMultiply(Quaternion q1, Quaternion q2);

    /// <summary>Scale quaternion by float value</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionScale(Quaternion q, float mul);

    /// <summary>Divide two quaternions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionDivide(Quaternion q1, Quaternion q2);

    /// <summary>Calculate linear interpolation between two quaternions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionLerp(Quaternion q1, Quaternion q2, float amount);

    /// <summary>Calculate slerp-optimized interpolation between two quaternions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionNlerp(Quaternion q1, Quaternion q2, float amount);

    /// <summary>Calculates spherical linear interpolation between two quaternions</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionSlerp(Quaternion q1, Quaternion q2, float amount);

    /// <summary>
    /// Calculate quaternion cubic spline interpolation using Cubic Hermite Spline algorithm
    /// as described in the GLTF 2.0 specification: https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html#interpolation-cubic
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionCubicHermiteSpline(
        Quaternion q1,
        Quaternion outTangent1,
        Quaternion q2,
        Quaternion inTangent2,
        float t
    );

    /// <summary>Calculate quaternion based on the rotation from one vector to another</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionFromVector3ToVector3(Vector3 from, Vector3 to);

    /// <summary>Get a quaternion for a given rotation matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionFromMatrix(Matrix4x4 mat);

    /// <summary>Get a matrix for a given quaternion</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 QuaternionToMatrix(Quaternion q);

    /// <summary>
    /// Get rotation quaternion for an angle and axis<br/>
    /// NOTE: angle must be provided in radians
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionFromAxisAngle(Vector3 axis, float angle);

    /// <summary>Get the rotation angle and axis for a given quaternion</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void QuaternionToAxisAngle(Quaternion q, Vector3* outAxis, float* outAngle);

    /// <summary>
    /// Get the quaternion equivalent to Euler angles<br/>
    /// NOTE: Rotation order is ZYX
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionFromEuler(float pitch, float yaw, float roll);

    /// <summary>
    /// Get the Euler angles equivalent to quaternion (roll, pitch, yaw)<br/>
    /// NOTE: Angles are returned in a Vector3 struct in radians
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Vector3 QuaternionToEuler(Quaternion q);

    /// <summary>Transform a quaternion given a transformation matrix</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Quaternion QuaternionTransform(Quaternion q, Matrix4x4 mat);

    /// <summary>Check whether two given quaternions are almost equal</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int QuaternionEquals(Quaternion p, Quaternion q);

    /// <summary>
    /// Compose a transformation matrix from rotational, translational and scaling components
    /// TODO: This function is not following raymath conventions defined in header: NOT self-contained
    /// </summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Matrix4x4 MatrixCompose(Vector3 translation, Quaternion rotation, Vector3 scale);

    /// <summary>Decompose a transformation matrix into its rotational, translational and scaling components</summary>
    [LibraryImport(NativeLibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void MatrixDecompose(
        Matrix4x4 mat,
        Vector3* translation,
        Quaternion* rotation,
        Vector3* scale
    );
}
