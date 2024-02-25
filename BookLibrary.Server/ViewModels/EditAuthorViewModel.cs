using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.ViewModels;

public class EditAuthorViewModel
{
    public int Id { get; init; }
    
    [Required(ErrorMessage = "The Name field is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; init; }
    
    [Required(ErrorMessage = "The Last Name field is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; init; }
}