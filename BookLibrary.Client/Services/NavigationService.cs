using System.Windows;
using BookLibrary.Client.ViewModel;
using Wpf.Ui.Controls;

namespace BookLibrary.Client.Services;

public interface INavigationService
{
    void Navigate<T>(params object[] args);
}

/// <summary>
///     Aucune raison de toucher a autre chose que <see cref="_viewMapping" /> dans cette classe, elle permet de gérer la
///     navigation
/// </summary>
public class NavigationService : INavigationService
{
    /// <summary>
    ///     Vous pouvez rajouter des correspondances ViewModel <-> View ici si vous souhaitez rajouter des pages
    /// </summary>
    private readonly Dictionary<Type, Type> _viewMapping = new()
    {
        [typeof(ListBooks)] = typeof(Pages.ListBooks),
        [typeof(BookDetails)] = typeof(Pages.BookDetails)
    };

    public NavigationView Frame { get; }


    /// <summary>
    ///     Permet de changer la page afficher par la <see cref="Frame" />
    /// </summary>
    /// <typeparam name="T">Le type du ViewModel a afficher</typeparam>
    /// <param name="args">
    ///     les paramètres pour instancier le ViewModel. /!\ votre ViewModel doit avoir un constructeur
    ///     compatible avec vos paramètres
    /// </param>
    public void Navigate<T>(params object[] args)
    {
        // Page p = Activator.CreateInstance(this._viewMapping[typeof(T)]) as Page;
        // p.DataContext = Activator.CreateInstance(typeof(T), args);
        // NavigationView.Navigate(p.GetType());
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var nav = mainWindow?.FindName("NavigationView") as NavigationView;
        nav?.Navigate(_viewMapping[typeof(T)]);
    }
}