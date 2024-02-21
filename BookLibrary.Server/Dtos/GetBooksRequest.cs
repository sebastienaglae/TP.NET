using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.Dtos;

public record GetBooksRequest(
    [MaxLength(10, ErrorMessage = "You can select up to 10 genres")]
    List<int> Genre,
    [MaxLength(10, ErrorMessage = "You can select up to 10 authors")]
    List<int> Author,
    [Range(0, int.MaxValue, ErrorMessage = "Offset must be positive")]
    int Offset,
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    int Limit = 20
);