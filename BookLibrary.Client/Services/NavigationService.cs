using System.Windows;
using BookLibrary.Client.ViewModel;
using Wpf.Ui.Controls;

namespace BookLibrary.Client.Services;

public interface INavigationService
{
    void Navigate<T>(params object[] args);
}

public class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Type> _viewMapping = new()
    {
        [typeof(ListBooks)] = typeof(Pages.ListBooks),
        [typeof(BookDetails)] = typeof(Pages.BookDetails),
        [typeof(ReadBook)] = typeof(Pages.ReadBook)
    };

    public void Navigate<T>(params object[] args)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var nav = mainWindow?.FindName("NavigationView") as NavigationView;
        nav?.Navigate(_viewMapping[typeof(T)]);
    }
}