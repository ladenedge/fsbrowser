
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
    public class FilesController : Controller
	{
        public FilesController(IFileSystem fs) => FS = fs;

        IFileSystem FS { get; set; }

        public ActionResult Root([FromPath] FileInfoBase path)
        {
            var entity = new FileSystemEntity(path);
            return File(path.FullName, entity.MimeType);
        }
    }
}
