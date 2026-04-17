using Microsoft.AspNetCore.Mvc;

namespace JWT.Repository
{
    public interface IAuthService
    {
        Task<LoginResponseDto>? Login(LoginRequestDto dto);
        Task<LoginResponseDto>? Refresh(string refreshToken);
    }
}
