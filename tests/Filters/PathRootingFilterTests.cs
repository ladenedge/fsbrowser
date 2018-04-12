
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FSBrowser.Filters;
using Moq;
using NUnit.Framework;

namespace FSBrowser.Models.Tests
{
    public class PathRootingFilterTests
    {
        TestRootingFilter Filter { get; set; }

        [SetUp]
        public void Init()
        {
            var fs = new MockFileSystem();
            var config = Mock.Of<IConfig>();
            Mock.Get(config).SetupGet(c => c.HomeDirectory).Returns(@"c:\home");

            Filter = new TestRootingFilter(config, fs);
        }

        [Test]
        public void AdjustPath_ThrowsWhenPathIsRootedToNonHomeDirectory()
        {
            Assert.That(() => Filter.TestAdjustPath(@"c:\temp", null), Throws.ArgumentException);
        }

        [Test]
        public void AdjustPath_DoesNotChangeHomedPath()
        {
            var expected = @"c:\home\dir";
            var result = Filter.TestAdjustPath(expected, null);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AdjustPath_ConcatsHomeDirectory()
        {
            var result = Filter.TestAdjustPath(@"test\file", null);
            Assert.That(result, Is.EqualTo(@"c:\home\test\file"));
        }

        [Test]
        public void AdjustPath_FactorsOutParentDots()
        {
            var result = Filter.TestAdjustPath(@"test\..\test\..\test\file", null);
            Assert.That(result, Is.EqualTo(@"c:\home\test\file"));
        }

        [Test]
        public void AdjustPath_DisallowsLeavingJail()
        {
            var result = Filter.TestAdjustPath(@"..\file", null);
            Assert.That(result, Is.EqualTo(@"c:\home"));
        }

        class TestRootingFilter : PathRootingFilter
        {
            public TestRootingFilter(IConfig config, IFileSystem fs) : base(config, fs) { }

            public object TestAdjustPath(string path, Type intendedType)
            {
                return base.AdjustPath(path, intendedType);
            }
        }
    }
}
