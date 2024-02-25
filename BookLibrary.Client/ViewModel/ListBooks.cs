using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace BookLibrary.Client.ViewModel;

public sealed class ListBooks : INotifyPropertyChanged
{
    private readonly LibraryService _libraryService = Ioc.Default.GetRequiredService<LibraryService>();
    private string _authorText = "";

    private string _genreText = "";

    public ListBooks()
    {
        RemoveAuthorCommand = new RelayCommand<Author>(OnRemoveAuthorCommand);
        RemoveGenreCommand = new RelayCommand<Genre>(OnRemoveGenreCommand);
        ShowBookDetailsCommand = new RelayCommand<Book>(OnShowBookDetailsCommand);
        SearchCommand = new AsyncRelayCommand(OnSearch);
        AuthorTextChangedCommand = new RelayCommand<AutoSuggestBoxTextChangedEventArgs>(AuthorTextChanged);
        AuthorSuggestionChosenCommand =
            new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(AuthorSuggestionChosen);
        GenreTextChangedCommand = new RelayCommand<AutoSuggestBoxTextChangedEventArgs>(GenreTextChanged);
        GenreSuggestionChosenCommand = new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(GenreSuggestionChosen);
        PageLoadedCommand = new RelayCommand(OnLoadedCommand);
    }

    public ObservableCollection<Author> SuggestedAuthors => Ioc.Default.GetService<LibraryService>()?.SuggestedAuthors!;
    public ObservableCollection<Genre> SuggestedGenres => Ioc.Default.GetService<LibraryService>()?.SuggestedGenres!;
    public ObservableCollection<Author?> FilteredAuthors { get; } = [];
    public ObservableCollection<Genre?> FilteredGenres { get; } = [];
    public ObservableCollection<Book> Books { get; } = Ioc.Default.GetService<LibraryService>()?.Books!;
    public ICommand RemoveAuthorCommand { get; }
    public ICommand RemoveGenreCommand { get; }
    public ICommand ShowBookDetailsCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand AuthorTextChangedCommand { get; }
    public ICommand AuthorSuggestionChosenCommand { get; }

    public string AuthorText
    {
        get => _authorText;
        set
        {
            _authorText = value;
            OnPropertyChanged(nameof(AuthorText));
        }
    }

    public string GenreText
    {
        get => _genreText;
        set
        {
            _genreText = value;
            OnPropertyChanged(nameof(GenreText));
        }
    }

    public ICommand GenreTextChangedCommand { get; }
    public ICommand GenreSuggestionChosenCommand { get; }
    public ICommand PageLoadedCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async void OnLoadedCommand()
    {
        await _libraryService.LoadBooks(true);
    }

    private void AuthorSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs? obj)
    {
        if (obj == null)
            return;
        AddAuthor(obj.SelectedItem as Author);
    }

    private void GenreSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs? obj)
    {
        if (obj == null)
            return;
        AddGenre(obj.SelectedItem as Genre);
    }

    private async void AuthorTextChanged(AutoSuggestBoxTextChangedEventArgs? args)
    {
        if (string.IsNullOrEmpty(AuthorText))
        {
            SuggestedAuthors.Clear();
            return;
        }

        await _libraryService.LoadSuggestedAuthors(AuthorText);
    }

    private async void GenreTextChanged(AutoSuggestBoxTextChangedEventArgs? args)
    {
        if (string.IsNullOrEmpty(GenreText))
        {
            SuggestedGenres.Clear();
            return;
        }

        await _libraryService.LoadSuggestedGenres(GenreText);
    }


    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void OnShowBookDetailsCommand(Book? obj)
    {
        if (obj == null)
            return;

        await _libraryService.LoadBookById(obj.Id);
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<BookDetails>();
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

    private void AddAuthor(Author? argsSelectedItem)
    {
        if (argsSelectedItem == null || FilteredAuthors.Any(a => a != null && a.Id == argsSelectedItem.Id)) return;
        FilteredAuthors.Add(argsSelectedItem);
    }

    private void AddGenre(Genre? argsSelectedItem)
    {
        if (argsSelectedItem == null || FilteredGenres.Any(a => a != null && a.Id == argsSelectedItem.Id)) return;
        FilteredGenres.Add(argsSelectedItem);
    }

    private async Task OnSearch()
    {
        try
        {
            var authors = FilteredAuthors.Where(a => a != null).Select(a => a!.Id).ToList();
            var genres = FilteredGenres.Where(a => a != null).Select(a => a!.Id).ToList();

            await _libraryService.LoadBooks(genres, authors, true);
        }
        catch (Exception)
        {
            MessageBox.Show("Error while fetching books");
        }
    }
}