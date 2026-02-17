// For more information see the blog post: https://aakinshin.net/posts/blittable/
// Original code derived from: https://github.com/AndreyAkinshin/BlittableStructs/blob/master/BlittableStructs/BlittableHelper.cs

/*
The MIT License

Copyright (c) 2015 Andrey Akinshin

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Raylib_cs.Tests;

public static class BlittableHelper
{
    public static bool IsBlittable<T>()
    {
        return IsBlittableCache<T>.Value;
    }

    public static bool IsBlittable(this Type type)
    {
        if (type == typeof(decimal))
        {
            return false;
        }
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            return elementType is {IsValueType: true} && elementType.IsBlittable();
        }
        try
        {
            //was deprecated: FormatterServices.GetUninitializedObject(type);
            //TODO: There's probably a nicer way to do this. See fennecs.net
            var instance = RuntimeHelpers.GetUninitializedObject(type);
            GCHandle.Alloc(instance, GCHandleType.Pinned).Free();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static class IsBlittableCache<T>
    {
        public static readonly bool Value = typeof(T).IsBlittable();
    }
}
