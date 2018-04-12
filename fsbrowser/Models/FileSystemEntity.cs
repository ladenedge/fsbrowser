
using System;
using System.IO.Abstractions;

namespace FSBrowser.Models
{
    public class FileSystemEntity
    {
        public FileSystemEntity(FileSystemInfoBase fsinfo)
        {
            Info = fsinfo ?? throw new ArgumentNullException(nameof(fsinfo));
        }

        FileSystemInfoBase Info { get; set; }

        public string Name => Info.Name;
        public string Path => Info.FullName;
        public long Size => Info is FileInfoBase finfo ? finfo.Length : 0;
        public DateTime CTime => Info.CreationTimeUtc;
        public DateTime MTime => Info.LastWriteTimeUtc;
        public string MimeType => Info.GetMimeType();
        public bool ReadOnly => Info.IsReadOnly();

        public string Self { get; set; }
        public string Parent { get; set; }
        public string Children { get; set; }
    }
}
