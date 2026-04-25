using Password_Hasing.Dto;
using Password_Hasing.Models;
using Password_Hasing.Repository;

namespace Password_Hasing.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterDto?> RegisterAsync(RegisterRequest request)
        {
            var checkUser = await _userRepository.EmailExistsAsync(request.Email);
            if (checkUser) return null;

            var hashedPassword =  _passwordHasher.Hash(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                PasswordHash = hashedPassword,
            };

            await _userRepository.AddAsync(user);

            return new RegisterDto
            {
                Email = request.Email,
                Password = request.Password,
                hashedPassword = hashedPassword,
            };
        }

        public async Task<LoginDto> LoginAsync(LoginRequest request)
        {
            var result = await _userRepository.GetByEmailAsync(request.Email);

            if (result == null || string.IsNullOrEmpty(result.Email))
                return new LoginDto {ErrorMessage = "Email Not Exist" };


            if (!_passwordHasher.Verify(request.Password, result.PasswordHash))
                return new LoginDto {ErrorMessage = "Password is Incorrect"};


            return new LoginDto
            {
                Username = result.Username,
                Email = result.Email,
                Password = result.Password,
                PasswordHash = result.PasswordHash,
            };
        }

    }
}
