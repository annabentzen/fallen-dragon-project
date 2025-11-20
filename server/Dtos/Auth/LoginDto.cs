using System.ComponentModel.DataAnnotations;

namespace DragonGame.Dtos.Auth
{
    /// <summary>
    /// Data for user login
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}