using System;
using Microsoft.AspNetCore.Mvc;

namespace MyBills.Mvc.Extensions
{
    public static class Url
    {
        /// <summary>
        /// Extension method to active flag to menu item
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string MakeActive(this IUrlHelper urlHelper, string controller)
        {
            var result = "active";
            var controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();

            if (!controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }

            return result;
        }
    }
}
