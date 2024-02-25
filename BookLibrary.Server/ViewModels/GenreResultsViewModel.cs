using System.Collections.Generic;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.ViewModels;

public record GenreResultsViewModel(IEnumerable<Genre> Genres, PaginationInfo PaginationInfo);