using System.Text.Json.Serialization;

namespace Password_Hasing.Service
{
    public class PasswordVerificationResult
    {
        public bool Verified { get; set; }
        public bool NeedRehash { get; set; }
        //[JsonIgnore]
        public string? NewHash { get; set; }
    }
}
