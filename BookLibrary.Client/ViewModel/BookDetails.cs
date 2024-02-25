using System.Collections.ObjectModel;
using System.Windows.Input;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace BookLibrary.Client.ViewModel;

public class BookDetails
{
    private readonly LibraryService _libraryService = Ioc.Default.GetRequiredService<LibraryService>();

    public BookDetails()
    {
        ShowReadBook = new RelayCommand(OnShowReadBook);
        PreviousPageCommand = new RelayCommand(OnPreviousPageCommand);
        NextPageCommand = new RelayCommand(OnNextPageCommand);
    }

    public Book Book => Ioc.Default.GetRequiredService<LibraryService>().Book;
    public ObservableCollection<Author> Authors { get; } = [];
    public ObservableCollection<Genre> Genres { get; } = [];
    public ICommand ShowReadBook { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private void OnNextPageCommand()
    {
        throw new NotImplementedException();
    }

    private void OnPreviousPageCommand()
    {
        throw new NotImplementedException();
    }

    private void OnShowReadBook()
    {
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<ReadBook>();
    }
}