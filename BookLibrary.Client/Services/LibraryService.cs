﻿using System.Collections.ObjectModel;
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
    public ObservableCollection<Author> SuggestedAuthors { get; } = [];
    public ObservableCollection<Genre> SuggestedGenres { get; } = [];

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

    private async Task<List<Genre>> GetGenres(string query)
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

    private async Task<List<Author>> GetAuthors(string query)
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

    public async Task LoadBooks(bool reset = false)
    {
        await LoadBooks([], [], reset);
    }

    public async Task LoadBooks(List<int> genres, List<int> authors, bool reset = false)
    {
        if (reset)
            ResetBooks();

        try
        {
            if (!_hasMore)
                return;
            var response = await _bookApi.BookGetBooksAsync(genres, authors, _page);
            if (response == null)
            {
                MessageBox.Show("Error while fetching books");
                return;
            }

            _hasMore = response.HasMore;
            response.Data?.ForEach(b => Books.Add(b.ToBook()));
            _page++;
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching books");
        }
    }

    private void ResetBooks()
    {
        _page = 0;
        _hasMore = true;
        Books.Clear();
    }

    public async Task LoadBookById(int bookId)
    {
        try
        {
            var result = await _bookApi.BookGetBookAsync(bookId);
            if (result == null)
            {
                MessageBox.Show("Error while fetching book");
                return;
            }

            Book = result.ToBook();
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching book");
        }
    }

    public async Task LoadSuggestedAuthors(string query)
    {
        try
        {
            var authors = await GetAuthors(query);
            if (authors == null)
            {
                MessageBox.Show("Error while fetching authors");
                return;
            }

            SuggestedAuthors.Clear();
            authors.ToList().ForEach(a => SuggestedAuthors.Add(a));
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching authors");
        }
    }

    public async Task LoadSuggestedGenres(string query)
    {
        try
        {
            var genres = await GetGenres(query);
            if (genres == null)
            {
                MessageBox.Show("Error while fetching genres");
                return;
            }

            SuggestedGenres.Clear();
            genres.ToList().ForEach(g => SuggestedGenres.Add(g));
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching genres");
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}