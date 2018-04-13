
using System;
using System.IO.Abstractions;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Filter for empty paths to initialize them according to the configuration.
    /// </summary>
    public class PathInitializationFilter : PathFilter
    {
        public PathInitializationFilter(IConfig config) => Config = config;

        IConfig Config { get; set; }

        /// <summary>
        /// Converts empty paths to the configured home directory.
        /// </summary>
        /// <param name="path">The incoming path.</param>
        /// <param name="intendedType">The intended type on the action.</param>
        /// <returns>The supplied path, or our home directory.</returns>
        protected override object AdjustPath(string path, Type intendedType)
        {
            if (String.IsNullOrEmpty(path))
                return Config.HomeDirectory;
            if (intendedType == typeof(DirectoryInfoBase))
                return path.EndsWith(@"\") ? path : path + @"\";
            return path;
        }
    }
}
