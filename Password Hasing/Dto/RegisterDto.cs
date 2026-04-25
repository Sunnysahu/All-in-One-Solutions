namespace Password_Hasing.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string hashedPassword { get; set; } = string.Empty;
    }
}
