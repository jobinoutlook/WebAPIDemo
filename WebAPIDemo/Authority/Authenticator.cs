using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPIDemo.Authority
{
    public class Authenticator
    {
        public static bool Authenticate(string clientId, string secret)
        {
            var _app = AppRepository.GetApplicationByClientId(clientId);
            
            if (_app == null) return false;

            return (_app.ClientId == clientId && _app.Secret == secret);
        }

        public static string CreateToken(string clientId, DateTime expiresAt, string strSecretKey)
        {
            //var securityKey = _configuration["SecurityKey"] ?? string.Empty;

            var signingCredentials = new SigningCredentials(
                   new SymmetricSecurityKey(System.Text.Encoding.UTF32.GetBytes(strSecretKey)),
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
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow,

            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public static async Task<bool> VerifyTokenAsync(string tokenString, string securityKey)
        {
            if(string.IsNullOrWhiteSpace(tokenString) || string.IsNullOrWhiteSpace(securityKey))
            {
                return false;
            };

            var keyBytes = System.Text.Encoding.UTF32.GetBytes(securityKey);
            var tokenHandler = new JsonWebTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var result = await tokenHandler.ValidateTokenAsync(tokenString, validationParameters);

                return result.IsValid;
            }
            catch (SecurityTokenMalformedException)
            {
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
