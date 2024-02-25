using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.Dtos;

public class LoginDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
    public string Username { get; set; }
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    public string Password { get; set; }
}