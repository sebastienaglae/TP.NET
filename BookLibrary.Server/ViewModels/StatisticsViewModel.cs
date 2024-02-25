using System.Collections.Generic;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.ViewModels;

public record StatisticsViewModel(long BooksCount, long AuthorsCount, long GenresCount, Dictionary<Author, int> AuthorsBooksCount, Dictionary<Genre, int> GenresBooksCount,
    long MinBookWords, long MedianBookWords, long MaxBookWords, 
    decimal MinBookPrice, decimal MedianBookPrice, decimal MaxBookPrice);