
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FSBrowser.Filters;
using Moq;
using NUnit.Framework;

namespace FSBrowser.Filters.Tests
{
    public class PathInitializationFilterTests
    {
        TestInitializationFilter Filter { get; set; }

        [SetUp]
        public void Init()
        {
            var fs = new MockFileSystem();
            var config = Mock.Of<IConfig>();
            Mock.Get(config).SetupGet(c => c.HomeDirectory).Returns(@"c:\home");

            Filter = new TestInitializationFilter(config);
        }

        [Test]
        public void AdjustPath_DoesNotChangeSuppliedPath()
        {
            var expected = @"c:\away\dir";
            var result = Filter.TestAdjustPath(expected, null);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AdjustPath_DoesNotChangeSuppliedPath([Values(null, "")] string path)
        {
            var result = Filter.TestAdjustPath(path, null);
            Assert.That(result, Is.EqualTo(@"c:\home"));
        }

        class TestInitializationFilter : PathInitializationFilter
        {
            public TestInitializationFilter(IConfig config) : base(config) { }

            public object TestAdjustPath(string path, Type intendedType)
            {
                return base.AdjustPath(path, intendedType);
            }
        }
    }
}
