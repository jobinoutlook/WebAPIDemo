using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebAPIDemo.Authority;

namespace WebAPIDemo.Controllers
{
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        IConfiguration _configuration;
        public AuthorityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AppCredential credential)
        {
            if (AppRepository.Authenticate(credential.ClientId, credential.Secret))
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(credential.ClientId,expiresAt),
                    expires_at = expiresAt,

                });
            }
            else
            {
                ModelState.AddModelError("Unauthorized", "You are not authorised.");
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status401Unauthorized
                };
                return new UnauthorizedObjectResult(problemDetails);

            }
        }

        private string CreateToken(string clientId,DateTime expiresAt)
        {
            var securityKey = _configuration["SecurityKey"]??string.Empty;
            
            var signingCredentials = new SigningCredentials(
                   new SymmetricSecurityKey(System.Text.Encoding.UTF32.GetBytes(securityKey)),
                   SecurityAlgorithms.HmacSha256Signature
                   );

            var app = AppRepository.GetApplicationByClientId(clientId);
            var claimsDictionary = new Dictionary<string, object>()
            {
                {"AppName",app?.ApplicationName??string.Empty },
                {"Read",(app?.Scopes??string.Empty).Contains("read")?"true":"false" },
                {"Write",(app?.Scopes??string.Empty).Contains("write")?"true":"false" },

            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
               SigningCredentials = signingCredentials,
               Claims = claimsDictionary,
               Expires= expiresAt,
               NotBefore = DateTime.UtcNow,     

            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

    }
}
