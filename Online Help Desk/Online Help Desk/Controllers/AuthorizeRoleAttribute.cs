using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Online_Help_Desk.Models;


namespace Online_Help_Desk.Attributes
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly RoleEnum _role;

        public AuthorizeRoleAttribute(RoleEnum role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetInt32("UserRole");

            if (userRole == null || (int)_role != userRole)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "Auth" },
                        { "action", "Login" }
                    });
            }

            base.OnActionExecuting(context);
        }
    }
}

