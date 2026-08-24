using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Services
{
    /// <summary>
    /// Service for JWT token generation and validation.
    /// </summary>
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        /// <summary>
        /// Initializes a new instance of the JwtTokenService class.
        /// </summary>
        /// <param name="configuration">The application configuration</param>
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _secretKey = _configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey is not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? "SimplePubManager";
            _audience = _configuration["Jwt:Audience"] ?? "SimplePubManager-Users";

            if (!int.TryParse(_configuration["Jwt:ExpirationMinutes"], out _expirationMinutes))
            {
                _expirationMinutes = 1440; // 24 hours default
            }

            if (_secretKey.Length < 32)
            {
                throw new InvalidOperationException("Jwt:SecretKey must be at least 32 characters");
            }
        }

        /// <summary>
        /// Generates a JWT token for a user.
        /// </summary>
        /// <param name="user">The user to generate a token for</param>
        /// <param name="organizationId">The organization ID to include in the token</param>
        /// <returns>A JWT token string</returns>
        public string GenerateToken(User user, Guid organizationId)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var now = DateTime.UtcNow;
            var expiryTime = now.AddMinutes(_expirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("organizationId", organizationId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("iat", now.ToString("O")),
                new Claim("exp", expiryTime.ToString("O"))
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryTime,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token and returns the claims if valid.
        /// </summary>
        /// <param name="token">The JWT token to validate</param>
        /// <returns>The ClaimsPrincipal if valid; otherwise null</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception)
            {
                // Token validation failed
                return null;
            }
        }
    }
}
