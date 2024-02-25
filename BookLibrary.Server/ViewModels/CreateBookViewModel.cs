using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.ViewModels;

public class CreateBookViewModel
{
    [Required(ErrorMessage = "The Name field is required.")]
    public string Name { get; init; }

    [Required(ErrorMessage = "The Description field is required.")]
    public string Description { get; init; }
    
    [Required(ErrorMessage = "The Pages field is required.")]
    public List<string> Pages { get; init; } = new List<string>();

    [Required(ErrorMessage = "The Price field is required.")]
    public decimal Price { get; init; }

    [Required(ErrorMessage = "At least one genre must be selected.")]
    public List<int> GenreIds { get; init; }

    [Required(ErrorMessage = "At least one author must be selected.")]
    public List<int> AuthorIds { get; init; }

    public ICollection<IdKeyValue> Genres { get; set; }

    public ICollection<IdKeyValue> Authors { get; set; }
}