using System.ComponentModel.DataAnnotations;

namespace Auth.Dtos {
    public class ChangePasswordDto {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

    public class ResetPasswordDto {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
