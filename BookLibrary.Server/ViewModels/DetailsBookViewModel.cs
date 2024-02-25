using System.Collections.Generic;

namespace BookLibrary.Server.ViewModels;

public class DetailsBookViewModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public ICollection<string> Authors { get; set; }

    public ICollection<string> Genres { get; set; }
    public int Id { get; set; }
}