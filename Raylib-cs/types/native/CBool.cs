using System.Runtime.InteropServices;

namespace Raylib_cs;

[StructLayout(LayoutKind.Sequential)]
public readonly struct CBool
{
    /* The values of booleans in C++ are stored in a single byte, which means it
     * only supports values from -128 to 127. It is possible to argument that
     * they only support a single bit, yes, but the minimum storage unit of a
     * computer is a sbyte, so that's what we'll be using when doing operations,
     * which can be later implicitely cast onto any type during usage.
     * 
     * It is wise to note that C booleans are any numeric value, but allocating an
     * Int64 (long) for every CBool instance is.. well, wildly memory-inefficient. Yes,
     * the process is basically treated as a 0-cost instantiation, but it's better to rely
     * on explicit motivation than to blindly trust the runtime judgement on its memory
     * management. 
     * 
     * 'value' is visible and constructable (if necessary), but impossible to modify or access.
     */
    public sbyte Value
    {
        init; private get;
    }

    // Constructors for easier usage.
    public CBool(bool value)
    {
        this.Value = (sbyte)(value ? 1 : 0);
    }
    public CBool(long value)
    {
        this.Value = (sbyte)(value != 0 ? 1 : 0);
    }

    // CBool -> Native
    // Allows for arithmetic between CBools and for assignment to greater integer variables.
    public static implicit operator sbyte(CBool x)
    {
        return x.Value;
    }

    // Allows for CBools to be implicitely assigned to a native boolean variable.
    public static implicit operator bool(CBool x)
    {
        return x.Value != 0;
    }

    // Native -> CBool
    // Allows native booleans to be implicitely constructed into CBools while passing parameters.
    public static implicit operator CBool(bool x)
    {
        return new CBool(x);
    }

    // Same goes for integer numeric values (any value, so an Int64 is used).
    public static implicit operator CBool(long x)
    {
        return new CBool(x);
    }

    /* Arithmetic overloads
     * Operations between CBools and integers are already covered by the implicit
     * sbyte cast. So no need to worry about those.
     * 
     * All casts return CBool, since there is no way to know if the assignment is
     * to a native boolean or integer, or a CBool.
     */

    // Addition
    public static CBool operator +(CBool left, CBool right)
    {
        return new CBool { Value = (sbyte)(left.Value + right.Value) };
    }

    // Subtraction
    public static CBool operator -(CBool left, CBool right)
    {
        return new CBool { Value = (sbyte)(left.Value - right.Value) };
    }

    // ToString override
    public override string ToString()
    {
        return ((bool)this).ToString();
    }
}
