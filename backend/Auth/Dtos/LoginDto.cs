using System.ComponentModel.DataAnnotations;

namespace Auth.Dtos {
    public class LoginDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
    public class ChangeLoginDto {
        [Required]
        public string Login { get; set; }
    }
}
