using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_SchoolSystem.Filters
{
    public class EmpDataFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
           
        }
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            Controller controller = filterContext.Controller as Controller;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (controller != null)
            {
                if (ERP_SchoolSystem.Classes.UserInfo.GetUserTypeID() == 1 || ERP_SchoolSystem.Classes.UserInfo.GetUserTypeID() == 2)
                {
                    if(ERP_SchoolSystem.Classes.UserInfo.IsEmpAdded() == false)
                    {
                        filterContext.Result = new RedirectResult("~/Home/UpdateEmpData");
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }
}