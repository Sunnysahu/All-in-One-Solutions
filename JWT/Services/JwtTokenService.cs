using JWT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            // Claims = data stored inside JWT

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken( // This builds the JWT object (not string yet)

                issuer: _configuration["Jwt:Issuer"],      // Who created this token?
                audience: _configuration["Jwt:Audience"],  // Who is this token intended for?
                claims: claims,                                // Data inside token
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])), signingCredentials: creds);      // Attaches signature to token

            return new JwtSecurityTokenHandler().WriteToken(token); // Converts object → string

            /*
                Claims → Your ID card info
                Issuer → Government issuing ID
                Audience → Where ID is valid
                Signature → Official stamp
                Expiry → Expiration date
             */
        }
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
