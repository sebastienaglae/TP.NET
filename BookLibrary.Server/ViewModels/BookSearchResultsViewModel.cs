using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.ViewModels;

public class BookSearchResultsViewModel
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int Page { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "Items per page must be between 1 and 100")]
    public int ItemsPerPage { get; set; } = 10;
}