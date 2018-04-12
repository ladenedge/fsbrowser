
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FSBrowser.Tests
{
    public class AppSettingsConfigTests
    {
        AppSettingsConfig Config => new AppSettingsConfig();

        [Test]
        public void HomeDirectory_ReadsAppSetting()
        {
            Assert.That(Config.HomeDirectory, Is.EqualTo("root"), "See app.config setting.");
        }
    }
}
