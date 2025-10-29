using System.ComponentModel.DataAnnotations;

namespace Auth.Dtos {
    public class EmailDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
