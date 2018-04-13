
using System;
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
        /// <summary>
        /// Gets a file's content.
        /// </summary>
        [HttpGet, ETagged("path")]
        public ActionResult Index([FromPath] FileInfoBase path)
        {
            var entity = new FileSystemEntity(path);
            return File(path.FullName, entity.MimeType);
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        [HttpDelete, ActionName("Index")]
        public ActionResult IndexDelete([FromPath] FileInfoBase path)
        {
            path.Delete();
            return new EmptyResult();
        }
    }
}
