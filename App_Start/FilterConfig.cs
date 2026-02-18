using System.Web;
using System.Web.Mvc;

namespace MDS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new OutputCacheAttribute
            {
                VaryByParam = "*",
                Duration = 1,
                //NoStore = true,
            });
            filters.Add(new HandleErrorAttribute());
        }
    }
}