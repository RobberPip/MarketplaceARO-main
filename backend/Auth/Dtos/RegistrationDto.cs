using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Dtos {
    public class RegistrationDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public bool CheckedAgreements { get; set; }
        [Required]
        public bool CheckedRules { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
    public class SendConfirmEmailDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

