using JWT.Data;
using JWT.Models;
using JWT.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        private readonly JwtTokenService _jwt;

        public AuthService(AppDbContext context, IConfiguration configuration, IUserRepository repository, JwtTokenService jwt)
        {
            _context = context;
            _configuration = configuration;
            _repository = repository;
            _jwt = jwt;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto dto)
        {
            var user = await _repository.GetUserAsync(dto.Username, dto.Password);

            if (user == null) return null;

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

            await SaveRefreshToken(user.Id, refreshToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task<LoginResponseDto> Refresh(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow);

            if (token == null) return null;

            var user = await _context.Users.FindAsync(token.UserId);

            var newAccessToken = _jwt.GenerateAccessToken(user);
            var newRefreshToken = _jwt.GenerateRefreshToken();

            token.IsRevoked = true;

            await SaveRefreshToken(user.Id, newRefreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task SaveRefreshToken(int userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }
    }
}
