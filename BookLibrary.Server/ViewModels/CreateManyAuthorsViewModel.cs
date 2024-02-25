using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.ViewModels;

public class CreateManyAuthorsViewModel
{
    public List<CreateManyAuthorViewModel> Authors { get; set; }
}

public class CreateManyAuthorViewModel
{
    [Required][MinLength(1)]  public string FirstName { get; set; }
    [Required][MinLength(1)] public string LastName { get; set; }
}