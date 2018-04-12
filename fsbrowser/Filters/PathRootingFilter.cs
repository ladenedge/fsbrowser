
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Filter for incoming paths to root them according to the configuration.
    /// </summary>
    public class PathRootingFilter : PathFilter
    {
        public PathRootingFilter(IConfig config, IFileSystem fs)
        {
            Config = config;
            FS = fs;
        }

        IConfig Config { get; set; }
        IFileSystem FS { get; set; }

        /// <summary>
        /// Prepends the configured root directory to the supplied path.
        /// </summary>
        /// <param name="path">The incoming path.</param>
        /// <param name="intendedType">The intended type on the action.</param>
        /// <returns>The supplied path, rooted.</returns>
        protected override object AdjustPath(string path, Type intendedType)
        {
            if (FS.Path.IsPathRooted(path) && !path.StartsWith(Config.HomeDirectory))
                throw new ArgumentException("Path component must not be rooted", nameof(path));

            path = FS.Path.Combine(Config.HomeDirectory, path);
            path = FS.Path.GetFullPath(path);

            // Some security on staying within our little chroot jail.
            return path.StartsWith(Config.HomeDirectory) ? path : Config.HomeDirectory;
        }
    }
}
