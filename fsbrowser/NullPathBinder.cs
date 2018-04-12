
using System;
using System.IO.Abstractions;
using System.Web.Mvc;
using FSBrowser.Models;

namespace FSBrowser
{
    /// <summary>
    /// Model binder that maps null path inputs to the home directory.
    /// </summary>
    public class NullPathBinder : DefaultModelBinder
    {
        public NullPathBinder(IConfig config, IFileSystem fs)
        {
            Config = config;
            FS = fs;
        }

        IConfig Config { get; set; }
        IFileSystem FS { get; set; }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (FS.TryPathToInfo(Config.HomeDirectory, modelType, out FileSystemInfoBase info))
                return info;

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}
