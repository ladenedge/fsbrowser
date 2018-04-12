
using System;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Identifies a parameter that originates from the client as a string
    /// containing path information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FromPathAttribute : Attribute { }
}
