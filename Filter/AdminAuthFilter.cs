using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SymphonyLimited.Filter
{
    public class AdminAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var adminUser = context.HttpContext.Session.GetString("AdminUser");

            if (string.IsNullOrEmpty(adminUser))
            {
                context.Result = new RedirectToActionResult("LogIn", "Admin", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
