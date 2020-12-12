using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RateLimiting.Models.Authentication;
using RateLimiting.Security.Services;

namespace RateLimiting.Security.Auth.BasicAuth 
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;

        public BasicAuthFilter(string realm)
        {
            _realm = realm;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
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
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
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
    }
}