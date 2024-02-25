using System.Collections.ObjectModel;
using System.Windows.Input;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using MessageBox = System.Windows.MessageBox;

namespace BookLibrary.Client.ViewModel;

public class ListBooks
{
    private readonly LibraryService? _libraryService = Ioc.Default.GetService<LibraryService>();

    public ListBooks()
    {
        RemoveAuthorCommand = new RelayCommand<Author>(OnRemoveAuthorCommand);
        RemoveGenreCommand = new RelayCommand<Genre>(OnRemoveGenreCommand);
        ShowBookDetailsCommand = new RelayCommand<Book>(OnShowBookDetailsCommand);
        SearchCommand = new AsyncRelayCommand(OnSearch);
        _libraryService.ResetBooks();

        OnSearch();
    }

    public ObservableCollection<Author?> FilteredAuthors { get; } = [];
    public ObservableCollection<Genre?> FilteredGenres { get; } = [];
    public ObservableCollection<Book> Books { get; } = Ioc.Default.GetService<LibraryService>()?.Books!;
    public ICommand RemoveAuthorCommand { get; }
    public ICommand RemoveGenreCommand { get; }
    public ICommand ShowBookDetailsCommand { get; }
    public ICommand SearchCommand { get; }

    private async void OnShowBookDetailsCommand(Book? obj)
    {
        if (obj == null)
            return;

        await _libraryService?.LoadBookById(obj.Id)!;
        Ioc.Default.GetService<INavigationService>()?.Navigate<BookDetails>(typeof(BookDetails), obj.Id);
    }

    private void OnRemoveAuthorCommand(Author? obj)
    {
        if (obj == null)
            return;

        FilteredAuthors.Remove(obj);
    }

    private void OnRemoveGenreCommand(Genre? obj)
    {
        if (obj == null)
            return;

        FilteredGenres.Remove(obj);
    }

    public void AddAuthor(Author? argsSelectedItem)
    {
        if (argsSelectedItem == null || FilteredAuthors.Any(a => a != null && a.Id == argsSelectedItem.Id)) return;
        FilteredAuthors.Add(argsSelectedItem);
    }

    public void AddGenre(Genre? argsSelectedItem)
    {
        if (argsSelectedItem == null || FilteredGenres.Any(a => a != null && a.Id == argsSelectedItem.Id)) return;
        FilteredGenres.Add(argsSelectedItem);
    }

    public async Task<List<Genre>> GetGenres(string query)
    {
        return await _libraryService?.GetGenres(query)!;
    }

    public async Task<List<Author>> GetAuthors(string query)
    {
        return await _libraryService?.GetAuthors(query)!;
    }

    public async Task OnSearch()
    {
        try
        {
            var authors = FilteredAuthors.Where(a => a != null).Select(a => a!.Id).ToList();
            var genres = FilteredGenres.Where(a => a != null).Select(a => a!.Id).ToList();

            await _libraryService?.LoadBooks(genres, authors)!;
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching books");
        }
    }
}