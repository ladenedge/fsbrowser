
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
    public class DirectoriesController : Controller
    {
        public DirectoriesController(IConfig config, IFileSystem fs)
        {
            Config = config;
            FS = fs;
        }

        IConfig Config { get; set; }
        IFileSystem FS { get; set; }

        public ActionResult Index([FromPath] DirectoryInfoBase path)
        {
            return Json(EntityFor(path), JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        [ActionName("Index")]
        public ActionResult IndexDelete([FromPath] DirectoryInfoBase path)
        {
            path.Delete(true);
            return new EmptyResult();
        }

        public ActionResult Children([FromPath] DirectoryInfoBase path)
        {
            return Json(path.GetFileSystemInfos().Select(i => EntityFor(i)), JsonRequestBehavior.AllowGet);
        }

        //[HttpPut]
        //public ActionResult Paste([FromPath] DirectoryInfoBase path, [FromPath] FileSystemInfoBase source)
        //{
        //    var dinfo = FS.DirectoryInfo.FromDirectoryName(dir);
        //    return Json(dinfo.GetFileSystemInfos());
        //}

        FileSystemEntity EntityFor(FileSystemInfoBase info)
        {
            var controller = info.IsDirectory() ? "directories" : "files";
            var parent = info.Parent();

            return new FileSystemEntity(info)
            {
                Self = Url.Action("", controller, new { path = info.FullName }, Request.Url.Scheme),
                Parent = parent != null ? Url.Action("", "directories", new { path = parent.FullName }, Request.Url.Scheme) : null,
                Children = info.IsDirectory() ? Url.Action("children", "directories", new { path = info.FullName }, Request.Url.Scheme) : null
            };
        }
    }
}
