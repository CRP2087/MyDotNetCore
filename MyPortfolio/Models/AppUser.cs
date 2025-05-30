using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        //[Required]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
