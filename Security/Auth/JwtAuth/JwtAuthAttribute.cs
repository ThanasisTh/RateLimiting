using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using RateLimiting.Security.Entities;
using RateLimiting.Security.Services;
using Microsoft.Extensions.DependencyInjection;

namespace RateLimiting.Security.Auth.JwtAuth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthAttribute : Attribute, IAuthorizationFilter, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) {}

        public void OnActionExecuting(ActionExecutingContext context)
        {
            int len = 0;
            var rateLimitService = context.HttpContext.RequestServices.GetRequiredService<IUserService>(); 
            var user = (User)context.HttpContext.Items["User"];
            try {
                len = Int32.Parse(context.HttpContext.Request.Query["len"]);
                if (len > 0) {
                    int remainingBandwidth = rateLimitService.consumeBandwidth(user, len);
                    if (remainingBandwidth < 0) {
                        ReturnRateExceededResult(context, remainingBandwidth);
                        return;
                    }
                    context.HttpContext.Response.Headers["X-Rate-Limit"] = remainingBandwidth.ToString();
                }
            }
            catch (FormatException) {
                ReturnBadResult(context);
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

        private void ReturnBadResult(ActionExecutingContext context)
        {
            context.Result = new BadRequestResult();
        }

        private void ReturnRateExceededResult(ActionExecutingContext context, int remainingBandwidth) {
            context.Result = new ObjectResult("Request exceeds remaining rate limit :(");
        }
        
    }
}
