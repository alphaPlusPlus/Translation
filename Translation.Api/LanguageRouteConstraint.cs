using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Translation.Api
{
    public class LanguageRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (!values.ContainsKey("language"))
            {
                return false;
            }

            var lang = values["language"].ToString();

            return lang == "en" || lang == "sv";
        }
    }
}