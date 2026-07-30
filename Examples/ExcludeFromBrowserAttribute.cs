namespace Examples;

/// <summary>
/// Marks an <see cref="IExample"/> that is omitted from the browser host (Web/Host.cs)
/// because of a wasm/WebGL platform limitation. Desktop runs are unaffected.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ExcludeFromBrowserAttribute : Attribute
{
    /// <summary>Why the example cannot run in the browser.</summary>
    public string Reason
    {
        get;
    }

    public ExcludeFromBrowserAttribute(string reason = null)
    {
        Reason = reason;
    }
}
