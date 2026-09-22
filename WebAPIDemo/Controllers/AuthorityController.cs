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
            if (Authenticator.Authenticate(credential.ClientId, credential.Secret))
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                var secretKey = _configuration["SecurityKey"] ?? string.Empty;
                return Ok(new
                {
                    access_token = Authenticator.CreateToken(credential.ClientId,expiresAt, secretKey),
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

        

    }
}
