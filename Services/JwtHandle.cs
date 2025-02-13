using autenticacion.Database;
using autenticacion.Dtos;
using autenticacion.Interface;
using autenticacion.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace autenticacion.Services
{
    public class JwtHandle : IJwtHandle
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBContext _context;

        public JwtHandle(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, DBContext context)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public TokenSettings GeneratedToken(ResponseAuthenticationDTO user)
        {
            var jwtSettings = _configuration.GetSection("JWTSettings").Get<JWTSettings>();

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.System, "AuthenticationApiSubject"),
            new Claim(ClaimTypes.Email, user.User.Email),
            new Claim(ClaimTypes.NameIdentifier, "User"),
            new Claim(ClaimTypes.SerialNumber, user.User.UserCode),
        };

            var now = DateTime.UtcNow;
            var expired = DateTime.UtcNow.AddMinutes(jwtSettings.DurationMinutes);

            var payload = new JwtPayload
            (
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims,
                now,
                expired
            );

            var token = new JwtSecurityToken(header, payload);

            var tokenNormalized = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenData = new TokenSettings
            {
                AccessToken = tokenNormalized,
                RefreshToken = "",
                ExpiresIn = expired
            };

            return tokenData;
        }
    }
}
