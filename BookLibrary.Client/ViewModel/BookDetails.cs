using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace BookLibrary.Client.ViewModel;

public class BookDetails
{
    private readonly LibraryService _libraryService = Ioc.Default.GetRequiredService<LibraryService>();
    public Book Book => Ioc.Default.GetRequiredService<LibraryService>().Book;
    public ObservableCollection<Author> Authors { get; } = [];
    public ObservableCollection<Genre> Genres { get; } = [];
}