using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSBrowser.Filters;
using FSBrowser.Models;

namespace FSBrowser.Controllers
{
    [HasPathInputs]
    public class FilesystemController : Controller
	{
        public FilesystemController(IFileSystem fs) => FS = fs;

        IFileSystem FS { get; set; }

        public ActionResult Root([FromPath] DirectoryInfoBase path)
        {
            return Json(path.GetFileSystemInfos().Select(i => new FileSystemEntity(i)), JsonRequestBehavior.AllowGet);
        }

        //[HttpPut]
        //public ActionResult Paste([FromPath] DirectoryInfoBase path, [FromPath] FileSystemInfoBase sourcePath)
        //{
        //    var dinfo = FS.DirectoryInfo.FromDirectoryName(dir);
        //    return Json(dinfo.GetFileSystemInfos());
        //}

        //FileSystemEntity ToFilesystemEntity(FileSystemInfoBase info)
        //{
        //    if (info.IsDirectory())
        //    {
        //        var href = Url.Action("index", controller, new { )
        //    return new FileSystemEntity(info);
        //}
    }
}