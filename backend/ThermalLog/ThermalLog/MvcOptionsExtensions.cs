using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace ThermalLog
{

    /// <summary>
    /// 
    /// </summary>
    public static class MvcOptionsExtensions 
    {
        /// <summary>
        /// サイト全体におけるエンドポイントのプリフィックスを設定します
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="routeAttribute"></param>
        public static void UseRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
        }

        /// <summary>
        /// サイト全体におけるエンドポイントのプリフィックスを設定します
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="prefix"></param>
        public static void UseRoutePrefix(this MvcOptions opts, string
        prefix)
        {
            opts.UseRoutePrefix(new RouteAttribute(prefix));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _routePrefix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        public RoutePrefixConvention(IRouteTemplateProvider route)
        {
            _routePrefix = new AttributeRouteModel(route);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        {
            foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
                }
                else
                {
                    selector.AttributeRouteModel = _routePrefix;
                }
            }
        }
    }

}
