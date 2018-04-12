
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
        public static string GetMimeType(this FileSystemInfoBase fsinfo)
        {
            return fsinfo.IsDirectory() ? "inode/directory" : MimeMapping.GetMimeMapping(fsinfo.Name);
        }

        public static bool IsDirectory(this FileSystemInfoBase fsinfo)
        {
            return (fsinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static bool IsReadOnly(this FileSystemInfoBase fsinfo)
        {
            return (fsinfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

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
            [typeof(DirectoryInfoBase)] = (fs, path) => fs.DirectoryInfo.FromDirectoryName(path),
            [typeof(FileSystemInfoBase)] = (fs, path) => fs.File.Exists(path) ? InfoFactories[typeof(FileInfoBase)](fs, path) : InfoFactories[typeof(DirectoryInfoBase)](fs, path),
        };

        public static FileSystemInfoBase PathToInfo(this IFileSystem fs, string path, Type infoType)
        {
            if (!InfoFactories.ContainsKey(infoType))
                throw new KeyNotFoundException($"Could not convert to Info from '{infoType}'");

            var factory = InfoFactories[infoType];
            return factory(fs, path);
        }

        public static bool TryPathToInfo(this IFileSystem fs, string path, Type infoType, out FileSystemInfoBase info)
        {
            info = null;
            if (!InfoFactories.ContainsKey(infoType))
                return false;

            var factory = InfoFactories[infoType];
            info = factory(fs, path);

            return true;
        }
    }
}
