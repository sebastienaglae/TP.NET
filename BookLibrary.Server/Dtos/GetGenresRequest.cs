using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.Models.Api;

public record GetGenresRequest(
    string Query,
    [Range(0, int.MaxValue, ErrorMessage = "Offset must be positive")] int Offset,
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")] int Limit = 20
);