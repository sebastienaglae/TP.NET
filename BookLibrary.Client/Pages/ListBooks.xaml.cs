using System.Collections;
using BookLibrary.Client.Models;
using Wpf.Ui.Controls;

namespace BookLibrary.Client.Pages;

public partial class ListBooks
{
    private readonly ViewModel.ListBooks _viewModel;

    public ListBooks()
    {
        _viewModel = new ViewModel.ListBooks();
        DataContext = _viewModel;
        InitializeComponent();
    }

    private void Author_AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        _viewModel.AddAuthor(args.SelectedItem as Author);
    }

    protected virtual async Task AutoSuggestBox_TextChanged<T>(AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args, Func<string, Task<List<T>>> getItems)
    {
        var text = sender.Text;
        if (string.IsNullOrEmpty(text))
        {
            sender.OriginalItemsSource = new ArrayList();
            return;
        }

        var items = await getItems(text);
        sender.OriginalItemsSource = items.ToList();
    }

    private async void Author_AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        await AutoSuggestBox_TextChanged(sender, args, _viewModel.GetAuthors);
    }

    private async void Genre_AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        await AutoSuggestBox_TextChanged(sender, args, _viewModel.GetGenres);
    }

    private void Genre_AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        _viewModel.AddGenre(args.SelectedItem as Genre);
    }
}