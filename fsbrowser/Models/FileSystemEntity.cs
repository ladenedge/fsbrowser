
using System;
using System.IO.Abstractions;

namespace FSBrowser.Models
{
    /// <summary>
    /// Our main business object, a fileystem entity.
    /// </summary>
    public class FileSystemEntity
    {
        /// <summary>
        /// Constructs a new filesystem entity from an Info class.
        /// </summary>
        /// <param name="fsinfo">The Info on which to base this entity.</param>
        public FileSystemEntity(FileSystemInfoBase fsinfo)
        {
            Info = fsinfo ?? throw new ArgumentNullException(nameof(fsinfo));
        }

        FileSystemInfoBase Info { get; set; }

        // Filesystem metadata properties.
        public string Name => Info.Name;
        public string Path => Info.FullName;
        public long Size => Info is FileInfoBase finfo ? finfo.Length : 0;
        public DateTime CTime => Info.CreationTimeUtc;
        public DateTime MTime => Info.LastWriteTimeUtc;
        public string MimeType => Info.GetMimeType();
        public bool ReadOnly => Info.IsReadOnly();

        // Various href properties.
        public string Self { get; set; }
        public string Parent { get; set; }
        public string Children { get; set; }
    }
}
