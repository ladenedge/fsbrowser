
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FSBrowser.Filters;
using NUnit.Framework;

namespace FSBrowser.Models.Tests
{
    public class PathConvertingFilterTests
    {
        TestConvertingFilter Filter { get; set; }

        [SetUp]
        public void Init()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"c:\home\file.txt"] = new MockFileData("content"),
                [@"c:\home\dir\file.jpg"] = new MockFileData("content")
            });
            Filter = new TestConvertingFilter(fs);
        }

        [Test]
        public void AdjustPath_ThrowsOnUnexpectedType()
        {
            Assert.That(() => Filter.TestAdjustPath(@"c:\home\file.txt", typeof(DateTime)), Throws.InstanceOf<KeyNotFoundException>());
        }

        [Test]
        public void AdjustPath_ReturnsExpectedType()
        {
            var result = Filter.TestAdjustPath(@"c:\home\file.txt", typeof(FileInfoBase));
            Assert.That(result, Is.InstanceOf<FileInfoBase>());
        }
        
        [Test]
        public void AdjustPath_ConvertsFile()
        {
            var path = @"c:\home\file.txt";
            var result = Filter.TestAdjustPath(path, typeof(FileInfoBase)) as FileInfoBase;
            Assert.That(result.FullName, Is.EqualTo(path));
        }

        [Test]
        public void AdjustPath_ConvertsDir()
        {
            var path = @"c:\home\dir";
            var result = Filter.TestAdjustPath(path, typeof(DirectoryInfoBase)) as DirectoryInfoBase;
            Assert.That(result.FullName, Is.EqualTo(path));
        }

        [Test]
        public void AdjustPath_ConvertsFileFromOnlyPath()
        {
            var path = @"c:\home\file.txt";
            var result = Filter.TestAdjustPath(path, typeof(FileSystemInfoBase)) as FileSystemInfoBase;
            Assert.That(result.FullName, Is.EqualTo(path));
        }

        [Test]
        public void AdjustPath_ConvertsDirFromOnlyPath()
        {
            var path = @"c:\home\dir";
            var result = Filter.TestAdjustPath(path, typeof(FileSystemInfoBase)) as FileSystemInfoBase;
            Assert.That(result.FullName, Is.EqualTo(path));
        }

        class TestConvertingFilter : PathConvertingFilter
        {
            public TestConvertingFilter(IFileSystem fs) : base(fs) { }

            public object TestAdjustPath(string path, Type intendedType)
            {
                return base.AdjustPath(path, intendedType);
            }
        }
    }
}
