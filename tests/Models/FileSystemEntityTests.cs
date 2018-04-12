
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using NUnit.Framework;

namespace FSBrowser.Models.Tests
{
    public class FileSystemEntityTests
    {
        IFileSystem FS { get; set; }

        [SetUp]
        public void Init()
        {
            FS = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"c:\home\file.txt"] = new MockFileData("content"),
                [@"c:\home\file.jpg"] = new MockFileData("content"),
                [@"c:\home\file.unknown"] = new MockFileData("content")
            });
        }

        [Test]
        public void Name_IsFilename()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.Name, Is.EqualTo(info.Name));
        }

        [Test]
        public void Path_IsFullPath()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.Path, Is.EqualTo(info.FullName));
        }

        [Test]
        public void Size_MeasuresContentForFile()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.Size, Is.EqualTo("content".Length));
        }

        [Test]
        public void Size_IsZeroForDirectory()
        {
            var info = FS.DirectoryInfo.FromDirectoryName(@"c:\home");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.Size, Is.Zero);
        }

        [Test]
        public void CTime_IsCreationTime()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.CTime, Is.EqualTo(info.CreationTimeUtc));
        }

        [Test]
        public void MTime_IsModifiedTime()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.MTime, Is.EqualTo(info.LastWriteTimeUtc));
        }
        
        [TestCase("file.txt", "text/plain")]
        [TestCase("file.jpg", "image/jpeg")]
        [TestCase("file.unknown", "application/octet-stream")]
        public void MimeType_IsContentTypeForFile(string filename, string expectedType)
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\" + filename);
            var entity = new FileSystemEntity(info);
            Assert.That(entity.MimeType, Is.EqualTo(expectedType));
        }

        [Test]
        public void MimeType_IsInodeTypeForDirectory()
        {
            var info = FS.DirectoryInfo.FromDirectoryName(@"c:\home");
            var entity = new FileSystemEntity(info);
            Assert.That(entity.MimeType, Does.Contain("directory"));
        }

        [Test]
        public void ReadOnly_TrueWhenFileIsReadOnly()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            info.Attributes = FileAttributes.Normal | FileAttributes.ReadOnly;

            var entity = new FileSystemEntity(info);

            Assert.That(entity.ReadOnly, Is.True);
        }

        [Test]
        public void ReadOnly_FileWhenFileIsNormal()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            info.Attributes = FileAttributes.Normal;

            var entity = new FileSystemEntity(info);

            Assert.That(entity.ReadOnly, Is.False);
        }

        [Test]
        public void Href_SetInConstructor()
        {
            var info = FS.FileInfo.FromFileName(@"c:\home\file.txt");
            info.Attributes = FileAttributes.Normal;

            var entity = new FileSystemEntity(info, "href");

            Assert.That(entity.Href, Is.EqualTo("href"));
        }
    }
}
