using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.Attributes;
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

            //4. Verify the token and extract claims
            //if(!await Authenticator.VerifyTokenAsync(tokenString, securityKey))
            //{
            //    context.Result = new UnauthorizedResult();
            //    return;
            //}

            var claims = await Authenticator.VerifyTokenAsync(tokenString, securityKey);
            if(claims == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            else
            {
                //get claims requirement
                var requiredClaims = context.ActionDescriptor.EndpointMetadata
                    .OfType<RequiredClaimAttribute>()
                    .ToList();

                if (requiredClaims != null)
                {
                    bool allClaimsPresent = false;

                    foreach (var reqClaim in requiredClaims)
                    {
                        var claim = claims.FirstOrDefault(c => c.Type.ToLower() == reqClaim.ClaimType.ToLower()
                        && c.Value.ToLower() == reqClaim.ClaimValue.ToLower());

                        if (claim != null)
                        {
                            allClaimsPresent = true;
                        }
                        else
                        {
                            allClaimsPresent = false;
                            break;
                        }
                    }

                    if (!allClaimsPresent)
                    {
                        context.Result = new StatusCodeResult(403); // Forbidden


                    }
                }
            }

        }
    }
}
