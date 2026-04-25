namespace Password_Hasing.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


/*
 SQL TABLE 

 CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    Password NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
 */