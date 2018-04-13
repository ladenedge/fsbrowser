
using System;
using System.IO;
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
        public DirectoriesController(IFileSystem fs) => FS = fs;

        IFileSystem FS { get; set; }

        /// <summary>
        /// Gets directory metadata.
        /// </summary>
        [HttpGet, ETagged("path")]
        public ActionResult Index([FromPath] DirectoryInfoBase path)
        {
            return Json(EntityFor(path), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes a directory.
        /// </summary>
        [HttpDelete, ActionName("Index")]
        public ActionResult IndexDelete([FromPath] DirectoryInfoBase path)
        {
            path.Delete(true);
            return new EmptyResult();
        }

        /// <summary>
        /// Copies or moves a filesystem entity.
        /// </summary>
        [HttpPut, ActionName("Index")]
        public ActionResult IndexPaste([FromPath] DirectoryInfoBase path, [FromPath] FileSystemInfoBase source, bool removeSource = false)
        {
            var newPath = FS.Path.Combine(path.FullName, source.Name);
            var pasteTo = FS.GetPasterFor(source, removeSource);

            pasteTo(newPath);

            var entity = EntityFor(FS.PathToInfo(newPath, source.GetType()));
            return Json(entity);
        }

        /// <summary>
        /// Gets metadata for all of a directory's children.
        /// </summary>
        [HttpGet, ETagged("path", UnlessSet = "filter")]
        public ActionResult Children([FromPath] DirectoryInfoBase path, string filter, bool? recursive)
        {
            var opt = recursive.HasValue && recursive.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var infos = String.IsNullOrEmpty(filter) ? path.GetFileSystemInfos() : path.GetFileSystemInfos(filter, opt);
            return Json(infos.Select(i => EntityFor(i)), JsonRequestBehavior.AllowGet);
        }


        FileSystemEntity EntityFor(FileSystemInfoBase info)
        {
            var controller = info.IsDirectory() ? "directories" : "files";
            var parent = info.Parent();

            // Fills out the entity object with the controller's URL helper.
            return new FileSystemEntity(info)
            {
                Self = Url.Action("", controller, new { path = info.FullName }, Request.Url.Scheme),
                Parent = parent != null ? Url.Action("", "directories", new { path = parent.FullName }, Request.Url.Scheme) : null,
                Children = info.IsDirectory() ? Url.Action("children", "directories", new { path = info.FullName }, Request.Url.Scheme) : null
            };
        }
    }
}
