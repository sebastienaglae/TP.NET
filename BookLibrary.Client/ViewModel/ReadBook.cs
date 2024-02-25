using System.ComponentModel;
using System.Speech.Synthesis;
using System.Windows;
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
    private SpeechSynthesizer synthesizer;
    private bool isPaused = false;
    public Book Book => Ioc.Default.GetRequiredService<LibraryService>().Book;

    public ReadBook()
    {
        InitializeSpeechSynthesizer();

        PreviousPageCommand = new RelayCommand<bool>(OnPreviousPageCommand);
        NextPageCommand = new RelayCommand<bool>(OnNextPageCommand);
        ClickPageOneCommand = new RelayCommand<MouseButtonEventArgs>(OnTextClick);

        if (synthesizer == null)
            return;
        PlayCommand = new RelayCommand(OnPlayCommand);
        PauseCommand = new RelayCommand(OnPauseCommand,
            () => !isPaused && synthesizer.State.Equals(SynthesizerState.Speaking));
        ResumeCommand = new RelayCommand(OnResumeCommand,
            () => isPaused && synthesizer.State.Equals(SynthesizerState.Paused));
        StopCommand = new RelayCommand(OnStopCommand,
            () => synthesizer.State.Equals(SynthesizerState.Speaking) || isPaused);
    }

    private void OnTextClick(MouseButtonEventArgs? obj)
    {
        // get the world under the mouse
        var text = obj?.OriginalSource.ToString();
        if (text == null)
            return;
        
    }

    private void InitializeSpeechSynthesizer()
    {
        synthesizer = new SpeechSynthesizer();
        synthesizer.SpeakCompleted += (sender, e) =>
        {
            isPaused = false;
            CommandManager.InvalidateRequerySuggested();
        };
    }

    private void OnResumeCommand()
    {
        synthesizer.Resume();
        isPaused = false;
    }

    private void OnStopCommand()
    {
        synthesizer.SpeakAsyncCancelAll();
    }

    private void OnPauseCommand()
    {
        synthesizer.Pause();
        isPaused = true;
    }

    private void OnPlayCommand()
    {
        var textToSpeak = "Hello, this is a test."; // Replace with your text
        synthesizer.SpeakAsync(textToSpeak);
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
    public bool CanPlay => !isPaused && synthesizer.State.Equals(SynthesizerState.Ready);
    public bool CanPause => !isPaused && synthesizer.State.Equals(SynthesizerState.Speaking);
    public bool CanResume => isPaused && synthesizer.State.Equals(SynthesizerState.Paused);
    public bool CanStop => synthesizer.State.Equals(SynthesizerState.Speaking) || isPaused;
    public ICommand ClickPageOneCommand { get; }

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