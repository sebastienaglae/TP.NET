using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Server.Dtos;

public record GetAuthorsRequest(
    string Query,
    [Range(0, int.MaxValue, ErrorMessage = "Offset must be positive")]
    int Offset,
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    int Limit = 20
);