using Microsoft.AspNetCore.Mvc;

namespace JWT.Repository
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> Login(LoginRequestDto dto, CancellationToken cancellationToken);
        Task<LoginResponseDto?> Refresh(string refreshToken);
    }
}
