using Infrastructure.Logger;
using Infrastructure.ORM;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.JWT;

public class Jwt
{
    private readonly byte[] symmetricKey = Convert.FromBase64String(DBConn.SecretKey);
    public string GenerateToken(int id, string username,bool isAdmin)
    {
        try
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(symmetricKey);
            string algorithms = SecurityAlgorithms.HmacSha256Signature;

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "MohamadProject",
                Audience = "Users",
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(1),
                Subject = new ClaimsIdentity(new[] {
                new Claim("ID", id.ToString()),
                new Claim("USERNAME", username),
                new Claim(ClaimTypes.Role, isAdmin.ToString()),

            }),
                SigningCredentials = new SigningCredentials(securityKey, algorithms)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken stoken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(stoken);
        }
        catch (Exception ex)
        {
            new Loger().Write(ex, "JsonWebToken => GenerateToken => username = " + username);
            return "InvalidToken";
        }
    }

    public int ValidateToken(string jwtToken)
    {
        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidAudience = "Users",
            ValidIssuer = "MohamadProject",
            IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
        };
        ClaimsPrincipal principal = new JwtSecurityTokenHandler()
            .ValidateToken(jwtToken.Remove(0, 7), validationParameters, out SecurityToken validatedToken);
        return Convert.ToInt32(principal?.FindFirst("ID")?.Value);
    }
}
