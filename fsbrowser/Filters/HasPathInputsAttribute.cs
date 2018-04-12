
using System;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Identifies an action (or a controller with actions) that accepts paths as inputs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HasPathInputsAttribute : Attribute { }
}
