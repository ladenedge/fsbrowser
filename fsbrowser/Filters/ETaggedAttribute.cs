
using Ninject;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSBrowser.Models;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Handle ETag logic for a given parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ETaggedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Constructs a new ETagging attribute.
        /// </summary>
        /// <param name="parameterName">The parameter the ETag should be based on.</param>
        public ETaggedAttribute(string parameterName) => ParameterName = parameterName;

        [Inject]
        public IFileSystem FS { get; set; }
        string ParameterName { get; set; }

        /// <summary>
        /// Abort the action if the If-None-Match value.. matches.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var path = filterContext.Controller.ValueProvider.GetValue(ParameterName)?.AttemptedValue;
            var matchHeader = filterContext.HttpContext.Request.Headers["If-None-Match"];
            if (String.IsNullOrEmpty(path) || matchHeader == null)
                return;

            var info = FS.PathToInfo(path, typeof(FileSystemInfoBase));

            if (matchHeader == info.LastWriteTimeUtc.Ticks.ToString())
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }

        /// <summary>
        /// Supply the latest ETag for future requests.
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var path = filterContext.Controller.ValueProvider.GetValue(ParameterName)?.AttemptedValue;
            if (String.IsNullOrEmpty(path))
                return;

            var info = FS.PathToInfo(path, typeof(FileSystemInfoBase));

            filterContext.HttpContext.Response.Headers["ETag"] = info.LastWriteTimeUtc.Ticks.ToString();
        }
    }
}
