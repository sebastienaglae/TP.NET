using System.ComponentModel;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using BookLibrary.Client.Models;
using BookLibrary.Client.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace BookLibrary.Client.ViewModel;

public class ReadBook : INotifyPropertyChanged
{
    private int _currentPage;
    private string _currentPages;
    private bool _hasNextPage;
    private bool _hasPreviousPage;
    private bool _isPaused;
    private string _pageOneText = "";

    private string _pageTwoText = "";
    private double _speakProgress;
    private SpeechSynthesizer _synthesizer;
    private string _textToSpeak;


    public ReadBook()
    {
        InitializeSpeechSynthesizer();

        PreviousPageCommand = new RelayCommand(OnPreviousPageCommand);
        NextPageCommand = new RelayCommand(OnNextPageCommand);
        PageOneSelectionChangedCommand = new RelayCommand<RoutedEventArgs>(StartFromCaret);
        PageTwoSelectionChangedCommand = new RelayCommand<RoutedEventArgs>(StartFromCaret);

        LoadPages(0);

        if (_synthesizer == null)
            return;
        PlayCommand = new RelayCommand(OnPlayCommand);
        PauseCommand = new RelayCommand(OnPauseCommand);
        ResumeCommand = new RelayCommand(OnResumeCommand);
        StopCommand = new RelayCommand(OnStopCommand);
    }

    public Book Book => Ioc.Default.GetRequiredService<LibraryService>().Book;

    public bool HasPreviousPage
    {
        get => _hasPreviousPage;
        set
        {
            if (_hasPreviousPage != value)
            {
                _hasPreviousPage = value;
                OnPropertyChanged(nameof(HasPreviousPage));
            }
        }
    }

    public bool HasNextPage
    {
        get => _hasNextPage;
        set
        {
            if (_hasNextPage != value)
            {
                _hasNextPage = value;
                OnPropertyChanged(nameof(HasNextPage));
            }
        }
    }

    public string CurrentPages
    {
        get => _currentPages;
        set
        {
            if (_currentPages != value)
            {
                _currentPages = value;
                OnPropertyChanged(nameof(CurrentPages));
            }
        }
    }

    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ResumeCommand { get; }

    public string PageOneText
    {
        get => _pageOneText;
        set
        {
            _pageOneText = value;
            OnPropertyChanged(nameof(PageOneText));
        }
    }

    public string PageTwoText
    {
        get => _pageTwoText;
        set
        {
            _pageTwoText = value;
            OnPropertyChanged(nameof(PageTwoText));
        }
    }

    public ICommand PageOneSelectionChangedCommand { get; }
    public ICommand PageTwoSelectionChangedCommand { get; }

    public double SpeakProgress
    {
        get => _speakProgress;
        set
        {
            _speakProgress = value;
            OnPropertyChanged(nameof(SpeakProgress));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void UpdateCurrentPages()
    {
        CurrentPages = $"{_currentPage + 1} - {_currentPage + 2}";
    }

    private void StartFromCaret(RoutedEventArgs? parameter)
    {
        if (parameter?.Source is RichTextBox richTextBox)
        {
            var caretPosition = richTextBox.CaretPosition;
            var textRange = new TextRange(caretPosition, richTextBox.Document.ContentEnd);
            _textToSpeak = textRange.Text;
            OnStopCommand();
            _synthesizer.SpeakAsync(_textToSpeak);
        }
    }

    private void InitializeSpeechSynthesizer()
    {
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SpeakCompleted += (sender, e) =>
        {
            _isPaused = false;
            SpeakProgress = 100;
            CommandManager.InvalidateRequerySuggested();
        };
        _synthesizer.SpeakProgress += (sender, e) =>
        {
            SpeakProgress = (double)(e.CharacterPosition + e.CharacterCount) * 100 / _textToSpeak.Length;
        };
    }

    private void OnResumeCommand()
    {
        _synthesizer.Resume();
        _isPaused = false;
    }

    private void OnStopCommand()
    {
        _synthesizer.SpeakAsyncCancelAll();
        _synthesizer.Resume();
        _isPaused = false;
    }

    private void OnPauseCommand()
    {
        _synthesizer.Pause();
        _isPaused = true;
    }

    private void OnPlayCommand()
    {
        _textToSpeak = PageOneText + PageTwoText;
        _synthesizer.SpeakAsync(_textToSpeak);
    }

    private void OnNextPageCommand()
    {
        OnStopCommand();
        LoadPages(_currentPage + 2);
    }

    private void OnPreviousPageCommand()
    {
        OnStopCommand();
        LoadPages(_currentPage - 2);
    }

    private void LoadPages(int page)
    {
        _currentPage = page;
        UpdateCurrentPages();
        HasPreviousPage = _currentPage > 0;
        HasNextPage = _currentPage + 2 < Book.Pages.Length;
        PageOneText = _currentPage < Book.Pages.Length ? Book.Pages[_currentPage] : "";
        PageTwoText = _currentPage + 1 < Book.Pages.Length ? Book.Pages[_currentPage + 1] : "";
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}