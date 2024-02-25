using System.ComponentModel;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace BookLibrary.Client.ViewModel;

public class BookDetails : INotifyPropertyChanged
{
    private readonly LibraryService? _libraryService = Ioc.Default.GetService<LibraryService>();
    public Book Book => Ioc.Default.GetService<LibraryService>()?.Book!;


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void LoadBookDetails(int bookId)
    {
        await _libraryService?.LoadBookById(bookId);
    }
}