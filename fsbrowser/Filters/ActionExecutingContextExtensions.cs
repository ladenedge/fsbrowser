
using System;
using System.Linq;
using System.Web.Mvc;

namespace FSBrowser.Filters
{
    static class ActionExecutingContextExtensions
    {
        /// <summary>
        /// Returns either the value being passed to the action or, if that value is null,
        /// the value provided by the client.
        /// </summary>
        /// <param name="filterContext">The current filter context.</param>
        /// <param name="paramName">Name of the parameter to retrieve.</param>
        /// <returns>The current value of the named parameter.</returns>
        public static string CurrentParameterValue(this ActionExecutingContext filterContext, string paramName)
        {
            return filterContext.ActionParameters[paramName]?.ToString() ??
                   filterContext.Controller.ValueProvider.GetValue(paramName)?.AttemptedValue ??
                   String.Empty;
        }
    }
}
