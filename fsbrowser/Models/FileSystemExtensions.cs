
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Web;

namespace FSBrowser.Models
{
    static class FileSystemExtensions
    {
        /// <summary>
        /// Gets the MIME type of a filesystem object based on its type and extension.
        /// </summary>
        /// <param name="fsinfo">The entity to type.</param>
        /// <returns>The entity's MIME type.</returns>
        public static string GetMimeType(this FileSystemInfoBase fsinfo)
        {
            return fsinfo.IsDirectory() ? "inode/directory" : MimeMapping.GetMimeMapping(fsinfo.Name);
        }

        /// <summary>
        /// Gets whether an Info represents a directory.
        /// </summary>
        /// <param name="fsinfo">The Info to test for directoriness.</param>
        /// <returns><b>true</b> if the Info is a directory, otherwise <b>false</b>.</returns>
        public static bool IsDirectory(this FileSystemInfoBase fsinfo)
        {
            return (fsinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        /// Gets whether an Info is read-only.
        /// </summary>
        /// <param name="fsinfo">The Info to test for read-onlyness.</param>
        /// <returns><b>true</b> if the Info is a read-only, otherwise <b>false</b>.</returns>
        public static bool IsReadOnly(this FileSystemInfoBase fsinfo)
        {
            return (fsinfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        /// <summary>
        /// Gets the parent directory for the supplied Info.
        /// </summary>
        /// <param name="fsinfo">Info whose parent is to be retrieved.</param>
        /// <returns>The Info's parent directory.</returns>
        public static DirectoryInfoBase Parent(this FileSystemInfoBase fsinfo)
        {
            if (fsinfo is DirectoryInfoBase di)
                return di.Parent;
            else if (fsinfo is FileInfoBase fi)
                return fi.Directory;
            throw new ArgumentException("Unexpected FileSystemInfo implementation");
        }

        static IDictionary<Type, Func<IFileSystem, string, FileSystemInfoBase>> InfoFactories = new Dictionary<Type, Func<IFileSystem, string, FileSystemInfoBase>>
        {
            [typeof(FileInfoBase)] = (fs, path) => fs.FileInfo.FromFileName(path),
            [typeof(FileInfoWrapper)] = (fs, path) => fs.FileInfo.FromFileName(path),
            [typeof(DirectoryInfoBase)] = (fs, path) => fs.DirectoryInfo.FromDirectoryName(path),
            [typeof(DirectoryInfoWrapper)] = (fs, path) => fs.DirectoryInfo.FromDirectoryName(path),
            [typeof(FileSystemInfoBase)] = (fs, path) => fs.File.Exists(path) ? InfoFactories[typeof(FileInfoBase)](fs, path) : InfoFactories[typeof(DirectoryInfoBase)](fs, path),
        };

        /// <summary>
        /// Converts a path to an Info according to the supplied type.
        /// </summary>
        /// <param name="fs">Filesystem to use for Info creation.</param>
        /// <param name="path">Path to convert.</param>
        /// <param name="infoType">Type of info to expect.</param>
        /// <returns>An Info class corresponding to the supplied path.</returns>
        public static FileSystemInfoBase PathToInfo(this IFileSystem fs, string path, Type infoType)
        {
            if (!InfoFactories.ContainsKey(infoType))
                throw new KeyNotFoundException($"Could not convert to Info from '{infoType}'");

            var factory = InfoFactories[infoType];
            return factory(fs, path);
        }

        /// <summary>
        /// Attempts to convert a path to an Info according to the supplied type.
        /// </summary>
        /// <param name="fs">Filesystem to use for Info creation.</param>
        /// <param name="path">Path to convert.</param>
        /// <param name="infoType">Type of info to expect.</param>
        /// <param name="info">The converted Info, if successful.</param>
        /// <returns><b>true</b> if the conversion is successful, otherwise <b>false</b>.</returns>
        public static bool TryPathToInfo(this IFileSystem fs, string path, Type infoType, out FileSystemInfoBase info)
        {
            info = null;
            if (!InfoFactories.ContainsKey(infoType))
                return false;

            var factory = InfoFactories[infoType];
            info = factory(fs, path);

            return true;
        }

        /// <summary>
        /// Finds the appropriate function to paste an item to a new location.
        /// </summary>
        /// <param name="fs">Filesystem to use for pasting.</param>
        /// <param name="source">The source Info.</param>
        /// <param name="removeSource">Whether to remove the source item after pasting.</param>
        /// <returns>An action that may be called with the new destination for <paramref name="source"/>.</returns>
        public static Action<string> GetPasterFor(this IFileSystem fs, FileSystemInfoBase source, bool removeSource)
        {
            if (!source.IsDirectory() && removeSource)
                return (string dest) => fs.File.Move(source.FullName, dest);
            if (!source.IsDirectory() && !removeSource)
                return (string dest) => fs.File.Copy(source.FullName, dest);
            if (source.IsDirectory() && removeSource)
                return (string dest) => fs.Directory.Move(source.FullName, dest);
            if (source.IsDirectory() && !removeSource)
                return (string dest) => Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(source.FullName, dest);

            throw new ArgumentException($"Could not paste '{source.Name}");
        }
    }
}
