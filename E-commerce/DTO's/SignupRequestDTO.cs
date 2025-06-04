using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTO_s
{
    public class SignupRequestDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        public string password { get; set; }
    }
}
