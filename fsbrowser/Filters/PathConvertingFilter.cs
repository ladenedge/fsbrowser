
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using FSBrowser.Models;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Filter for incoming paths to convert them to the appropriate Infos.
    /// </summary>
    public class PathConvertingFilter : PathFilter
    {
        public PathConvertingFilter(IFileSystem fs) => FS = fs;

        IFileSystem FS { get; set; }

        protected override object AdjustPath(string path, Type intendedType)
        {
            return FS.PathToInfo(path, intendedType);
        }
    }
}
