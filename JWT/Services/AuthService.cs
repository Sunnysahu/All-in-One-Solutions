using JWT.Data;
using JWT.Models;
using JWT.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

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

        public async Task<LoginResponseDto?> Login(LoginRequestDto dto, CancellationToken cancellationToken)
        {
            /*
            Manual cancel after time

            // Create linked token (inherits client cancellation too)
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Cancel token after 2 seconds (does NOT stop automatically)
            cts.CancelAfter(2000); 

            // Wait 5 sec BUT observe token → will cancel at 2 sec
            await Task.Delay(5000, cts.Token); 

            // Manual cancel here (optional, if not already cancelled)
            cts.Cancel(); 

            cts.Token.ThrowIfCancellationRequested(); // force stop if not already stopped
            */

            cancellationToken.ThrowIfCancellationRequested();
            // simulate delay BEFORE DB

            await Task.Delay(5000, cancellationToken);

            Console.WriteLine("Print");

            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine("Delay Over");

            var user = await _repository.GetUserAsync(dto.Username, dto.Password, cancellationToken);

            if (user == null) return null;

            cancellationToken.ThrowIfCancellationRequested();

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

            await SaveRefreshToken(user.Id, refreshToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            // -------------------
        }

        public async Task<LoginResponseDto?> Refresh(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow);

            if (token == null) return null;

            var user = await _context.Users.FindAsync(token.UserId);

            if (user == null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
                return null;
            }
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
