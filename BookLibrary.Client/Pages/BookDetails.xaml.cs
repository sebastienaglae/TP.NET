namespace BookLibrary.Client.Pages;

public partial class BookDetails
{
    private readonly ViewModel.BookDetails _viewModel;

    public BookDetails()
    {
        _viewModel = new ViewModel.BookDetails();
        DataContext = _viewModel;
        InitializeComponent();
    }
}