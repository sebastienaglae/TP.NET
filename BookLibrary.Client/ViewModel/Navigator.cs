using System.ComponentModel;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace BookLibrary.Client.ViewModel;

internal class Navigator : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public void GoToHome()
    {
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<Pages.ListBooks>();
    }
}