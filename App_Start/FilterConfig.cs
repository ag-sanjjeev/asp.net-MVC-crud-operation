using Crud_Operation.CustomFilters;
using System.Web;
using System.Web.Mvc;

namespace Crud_Operation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomAuthentication());
        }
    }
}
