using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookLibrary.Client.Models;
using BookLibrary.Client.OpenApi.Api;
using MessageBox = System.Windows.MessageBox;

namespace BookLibrary.Client.Services;

public class LibraryService : INotifyPropertyChanged
{
    private readonly IAuthorApi _authorApi = new AuthorApi();
    private readonly IBookApi _bookApi = new BookApi();
    private readonly IGenreApi _genreApi = new GenreApi();
    private Book _book;
    private bool _hasMore = true;
    private int _page;

    public ObservableCollection<Book> Books { get; } = [];

    public Book Book
    {
        get => _book;
        set
        {
            if (_book != value)
            {
                _book = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task<List<Genre>> GetGenres(string query)
    {
        try
        {
            var response = await _genreApi.GenreGetGenresAsync(query);
            return response?.Data.ToGenres() ?? new List<Genre>();
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching genres");
            return new List<Genre>();
        }
    }

    public async Task<List<Author>> GetAuthors(string query)
    {
        try
        {
            var response = await _authorApi.AuthorGetAuthorsAsync(query);
            return response?.Data.ToAuthors() ?? new List<Author>();
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching authors");
            return new List<Author>();
        }
    }

    public async Task LoadBooks(List<int> genres, List<int> authors)
    {
        try
        {
            if (!_hasMore)
                return;
            var response = await _bookApi.BookGetBooksAsync(genres, authors, _page);
            _hasMore = response.HasMore;
            response.Data?.ForEach(b => Books.Add(b.ToBook()));
            _page++;
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching books");
        }
    }

    public void ResetBooks()
    {
        _page = 0;
        _hasMore = true;
        Books.Clear();
    }

    public async Task LoadBookById(int bookId)
    {
        var result = await _bookApi.BookGetBookAsync(bookId);
        Book = result.ToBook();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}