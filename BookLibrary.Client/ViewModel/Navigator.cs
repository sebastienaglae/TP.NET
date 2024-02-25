using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using Wpf.Ui.Input;

namespace BookLibrary.Client.ViewModel;

internal class Navigator : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand GoToBookDetailsCommand => new RelayCommand<string>(GoToBook);

    public void GoToHome()
    {
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<Pages.ListBooks>();
    }
    
    public void GoToBook(string id)
    {
        MessageBox.Show($"Navigating to book {id}");
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<BookDetails>(4);
    }
    
}