using System.Collections.Generic;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.ViewModels;

public record BookResultsViewModel(IEnumerable<Book> Books, PaginationInfo PaginationInfo);