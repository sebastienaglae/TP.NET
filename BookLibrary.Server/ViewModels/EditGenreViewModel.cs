using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.ViewModels;

public class EditGenreViewModel
{
    public int Id { get; init; }
    
    [Required(ErrorMessage = "The Name field is required.")]
    [Display(Name = "Name")]
    public string Name { get; set; }
}