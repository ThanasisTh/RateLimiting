using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RateLimiting.Models.Authentication;
using RateLimiting.Security.Services;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RateLimiting.Security.Auth.BasicAuth 
{
    /// <summary>
    /// <para>Class implementing <see cref="Attribute"/> and <see cref="IAuthorizationFilter"/> for <see cref="RandomController"/>, 
    /// filtering requests for authenticating registered instances of <see cref="User"/> using basic authentication.</para>
    /// <para> Also handles setting up the response object to these requests. </para>
    /// 
    /// <TODO> Filter for admin account </TODO> 
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : Attribute, IAuthorizationFilter, IActionFilter {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.DisplayName.Split(".", 4)[3].Split(" ", 2)[0] == nameof(RateLimiting.Controllers.RandomController.modifyLimit)) {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                var credentials = getCredentials(authHeader);
                if (credentials.Length == 2)
                {
                    if (IsAuthorized(context, credentials[0], credentials[1]))
                    {
                        return;
                    }
                }
            }
            try
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                var credentials = getCredentials(authHeader);
                if (credentials.Length == 2)
                {
                    if (IsAuthorized(context, credentials[0], credentials[1]))
                    {
                        ReturnOkResult(context);
                        return;
                    }
                }
                ReturnUnauthorizedResult(context);
            }
            catch (FormatException)
            {
                ReturnUnauthorizedResult(context);
            }
        }

        private bool IsAuthorized(AuthorizationFilterContext context, string username, string password)
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            return userService.isValidUser(username, password);
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = "";
            context.Result = new UnauthorizedResult();
        }

        private void ReturnOkResult(AuthorizationFilterContext context) {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            var credentials = getCredentials(authHeader);
            if (credentials.Length == 2)
            {
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                AuthenticateRequest model = new AuthenticateRequest(credentials[0], credentials[1]);
                var response = userService.Authenticate(model);
                context.Result = new OkObjectResult(response);
            }
        }

        private string[] getCredentials(string authHeader) {
            if (authHeader != null)
            {
                var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var credentials = Encoding.UTF8
                                        .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                        .Split(':', 2);
                    return credentials;
                }
            }
            return null;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }

}