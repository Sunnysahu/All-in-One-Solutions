using System.Text.Json.Serialization;

namespace Password_Hasing.Dto
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        [JsonIgnore]
        public string ErrorMessage {  get; set; } = string.Empty;

    }
}
