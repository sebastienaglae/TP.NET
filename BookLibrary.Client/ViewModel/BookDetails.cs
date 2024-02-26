using System.Collections.ObjectModel;
using System.Windows.Input;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace BookLibrary.Client.ViewModel;

public class BookDetails
{
    public BookDetails()
    {
        ShowReadBook = new RelayCommand(OnShowReadBook);
    }

    public Book Book => Ioc.Default.GetRequiredService<LibraryService>().Book;
    public ObservableCollection<Author> Authors { get; } = [];
    public ObservableCollection<Genre> Genres { get; } = [];
    public ICommand ShowReadBook { get; }

    private void OnShowReadBook()
    {
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<ReadBook>();
    }
}