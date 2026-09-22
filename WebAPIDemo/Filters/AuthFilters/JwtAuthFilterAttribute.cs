using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.Authority;

namespace WebAPIDemo.Filters.AuthFilters
{
    public class JwtAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //1. Get Authorization header from request
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string tokenString = authorizationHeader.ToString();

            //2. Get rid of Bearer prefix
            if (tokenString.StartsWith("Bearer "))
            {
                tokenString = tokenString.Substring(7);
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //3. Get Configuration and the Secret Key
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var securityKey = configuration?["SecurityKey"] ?? string.Empty;
            
            //4. Verify the token
            if(!await Authenticator.VerifyTokenAsync(tokenString, securityKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            return;
        }
    }
}
