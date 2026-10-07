using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLayer.Service
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserEntity user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var expiresInMinutes = int.Parse(
                _configuration["Jwt:ExpiresInMinutes"] ?? "60"
            );

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "JWT Key is not configured."
                );
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.UserId.ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.FirstName
                ),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateResetToken(string email, int minutes = 15)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("token_type", "password_reset")
    };
            var _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var now = DateTime.UtcNow;
            var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public bool TryValidateResetToken(string token, out string? email)
        {
            email = null;
            if (string.IsNullOrWhiteSpace(token)) return false;
            var _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrWhiteSpace(_configuration["Jwt:Issuer"]),
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = !string.IsNullOrWhiteSpace(_configuration["Jwt:Audience"]),
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            try
            {
                var principal = handler.ValidateToken(token, parameters, out _);
                email = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? principal.FindFirst(ClaimTypes.Email)?.Value;
                return email != null && principal.HasClaim(c => c.Type == "token_type" && c.Value == "password_reset");
            }
            catch
            {
                return false;
            }
        }
    }
}