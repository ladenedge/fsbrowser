
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Web.Mvc;
using FSBrowser.Filters;
using FSBrowser.Models;

namespace FSBrowser.Controllers
{
    [HasPathInputs]
    public class FilesystemController : Controller
    {
        public FilesystemController(IConfig config, IFileSystem fs)
        {
            Config = config;
            FS = fs;
        }

        IConfig Config { get; set; }
        IFileSystem FS { get; set; }

        public ActionResult Root([FromPath] DirectoryInfoBase path)
        {
            return Json(path.GetFileSystemInfos().Select(i => new FileSystemEntity(i, UriFor(i))), JsonRequestBehavior.AllowGet);
        }

        //[HttpPut]
        //public ActionResult Paste([FromPath] DirectoryInfoBase path, [FromPath] FileSystemInfoBase sourcePath)
        //{
        //    var dinfo = FS.DirectoryInfo.FromDirectoryName(dir);
        //    return Json(dinfo.GetFileSystemInfos());
        //}

        string UriFor(FileSystemInfoBase info)
        {
            var controller = info.IsDirectory() ? "filesystem" : "files";

            var trimmedPath = info.FullName.Replace(Config.HomeDirectory, String.Empty);
            trimmedPath = trimmedPath.Replace(@"\", @"/");

            return Url.Action("root", controller, new { path = trimmedPath }, Request.Url.Scheme);
        }
    }
}
