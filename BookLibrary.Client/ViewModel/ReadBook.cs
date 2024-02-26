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
    private bool _hasNextPage;
    private bool _hasPreviousPage;
    private SpeechSynthesizer _synthesizer;
    private bool _isPaused;
    private string _textToSpeak = string.Empty;
    public Book Book => Ioc.Default.GetRequiredService<LibraryService>().Book;

    public ReadBook()
    {
        InitializeSpeechSynthesizer();

        PreviousPageCommand = new RelayCommand<bool>(OnPreviousPageCommand);
        NextPageCommand = new RelayCommand<bool>(OnNextPageCommand);
        PageOneSelectionChangedCommand = new RelayCommand<RoutedEventArgs>(StartFromCaret);
        PageTwoSelectionChangedCommand = new RelayCommand<RoutedEventArgs>(StartFromCaret);

        if (_synthesizer == null)
            return;
        PlayCommand = new RelayCommand(OnPlayCommand);
        PauseCommand = new RelayCommand(OnPauseCommand);
        ResumeCommand = new RelayCommand(OnResumeCommand);
        StopCommand = new RelayCommand(OnStopCommand);
    }

    private void StartFromCaret(RoutedEventArgs? parameter)
    {
        if (parameter?.Source is RichTextBox richTextBox)
        {
            var caretPosition = richTextBox.CaretPosition;
            var textRange = new TextRange(caretPosition, richTextBox.Document.ContentEnd);
            var textFromCaretToStart = textRange.Text;
            OnStopCommand();
            _synthesizer.SpeakAsync(textFromCaretToStart);
        }
    }

    private void InitializeSpeechSynthesizer()
    {
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SpeakCompleted += (sender, e) =>
        {
            _isPaused = false;
            CommandManager.InvalidateRequerySuggested();
        };
        _synthesizer.SpeakProgress += (sender, e) =>
        {
            SpeakProgress = e.CharacterPosition / e.CharacterCount;
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
        var textToSpeak = "Bonjour a toi !"; // Replace with your text
        _synthesizer.SpeakAsync(textToSpeak);
    }

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

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
    }

    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ResumeCommand { get; }
    private string _pageOneText = "Hello, this is a test.";

    public string PageOneText
    {
        get { return _pageOneText; }
        set
        {
            _pageOneText = value;
            OnPropertyChanged(nameof(PageOneText));
        }
    }

    private string _pageTwoText = "Hello, this is not a test.";

    public string PageTwoText
    {
        get { return _pageTwoText; }
        set
        {
            _pageTwoText = value;
            OnPropertyChanged(nameof(PageTwoText));
        }
    }

    public ICommand PageOneSelectionChangedCommand { get; }
    public ICommand PageTwoSelectionChangedCommand { get; }
    private double _speakProgress;

    public double SpeakProgress
    {
        get { return _speakProgress; }
        set
        {
            _speakProgress = value;
            OnPropertyChanged(nameof(SpeakProgress));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnNextPageCommand(bool obj)
    {
        OnStopCommand();
    }

    private void OnPreviousPageCommand(bool obj)
    {
        OnStopCommand();
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}