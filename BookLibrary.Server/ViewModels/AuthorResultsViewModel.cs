using System.Collections.Generic;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.ViewModels;

public record AuthorResultsViewModel(IEnumerable<Author> Authors, PaginationInfo PaginationInfo);