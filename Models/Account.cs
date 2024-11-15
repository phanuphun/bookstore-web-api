using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SampleWebAPI.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; } = null;
        public string? Phone { get; set; } = null;

        [Required]
        public string? Username { get; set; } = null;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? Password { get; set; } = null;

        [Required]
        [NotMapped]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
        public string? ConfirmPassword { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Status { get; set; } = "Customer";
    }

    public class Login(){
        [Required]
        public string Username {get; set;}
        [Required]
        public string Password {get; set;} 
    }

     
}