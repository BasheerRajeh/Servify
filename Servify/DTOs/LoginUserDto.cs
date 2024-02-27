using System.ComponentModel.DataAnnotations;

namespace Servify.DTOs
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
