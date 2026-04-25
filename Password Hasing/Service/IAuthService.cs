using Password_Hasing.Dto;

namespace Password_Hasing.Service
{
    public interface IAuthService
    {
        Task<RegisterDto?> RegisterAsync(RegisterRequest request);
        Task<LoginDto> LoginAsync(LoginRequest request);
    }
}
