using BookLibrary.Client.Services;
using BookLibrary.Client.ViewModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibrary.Client;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        // Si vous avez besoin de rajouter des services, vous pouvez le déclarer ici
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<INavigationService>(new NavigationService())
                .AddSingleton(new LibraryService())
                .BuildServiceProvider());

        Ioc.Default.GetRequiredService<INavigationService>().Navigate<ListBooks>();
    }
}