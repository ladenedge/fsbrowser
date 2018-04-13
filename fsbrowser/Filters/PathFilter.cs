
using System;
using System.Linq;
using System.Web.Mvc;

namespace FSBrowser.Filters
{
    /// <summary>
    /// Base class for filters that manipulate parameters decorated with [FromPath].
    /// </summary>
    public abstract class PathFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allParms = filterContext.ActionDescriptor.GetParameters();
            var pathParms = allParms.Where(p => p.GetCustomAttributes(typeof(FromPathAttribute), true).Any());

            foreach (var parm in pathParms)
            {
                var path = filterContext.CurrentParameterValue(parm.ParameterName);
                filterContext.ActionParameters[parm.ParameterName] = AdjustPath(path, parm.ParameterType);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { }

        /// <summary>
        /// Derived types must implement this template method to handle the incoming path.
        /// </summary>
        /// <param name="path">The incoming path.</param>
        /// <param name="intendedType">The intended type on the action.</param>
        /// <returns>The adjusted path.</returns>
        protected abstract object AdjustPath(string path, Type intendedType);
    }
}
