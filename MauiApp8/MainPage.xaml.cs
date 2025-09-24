using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp8;

public partial class MainPage : ContentPage
{
    private GameViewModel viewModel;
    private string currentPlayerName;

    private List<Button> letterButtons;

    public MainPage()
    {
        InitializeComponent();

        viewModel = new GameViewModel();
        BindingContext = viewModel;

        viewModel.OnScoreChanged = (name, newScore) =>
        {
            if (currentPlayerName == name)
            {
                ScoreLabel.Text = $"Hello {name}, Score: {newScore}";
            }
        };

        letterButtons = new List<Button>
        {
            ButtonA, ButtonB, ButtonC, ButtonD, ButtonE, ButtonF, ButtonG, ButtonH,
            ButtonI, ButtonJ, ButtonK, ButtonL, ButtonM, ButtonN, ButtonO, ButtonP,
            ButtonQ, ButtonR, ButtonS, ButtonT, ButtonU, ButtonV, ButtonW, ButtonX,
            ButtonY, ButtonZ
        };

        StartGame();
    }
    private async void Help_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage());
    }


    private void StartGame()
    {
        ResetButtons();

        viewModel.StartGame();
    }

    private void OnCheckScoreClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(name))
        {

            ScoreLabel.Text = "Enter a name!";
            return;
        }
         
        currentPlayerName = name;

        int savedScore = Preferences.Get($"score_{name}", 0);
        ScoreLabel.Text = $"Hello {name}, Score: {savedScore}";
        viewModel.SetPlayer(name, savedScore);
    }

private void LrClicked(object sender, EventArgs e)
{
    if (sender is Button button && !string.IsNullOrEmpty(button.Text))
    {
        char letter = button.Text[0];  

        button.IsEnabled = true;

        viewModel.CheckGuess(letter);
    }
}
    private void ResetButtons()
    {
        foreach (var button in letterButtons)
        {
            button.IsEnabled = true;  
        }
    }
}

public class GameViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string currentPlayerName;
    private int lives;
    private string selectedWord;
    private char[] guessedWord;
    private List<char> guessedLetters = new();
    private string[] words;

    public int Score { get; set; }
    public string DisplayWord { get; set; } = "";
    public string HangmanImage { get; set; } = "hangman1.png";

    public int Lives
    {
        get => lives;
        set { lives = value; OnPropertyChanged(); }
    }

    public Action<string, int> OnScoreChanged;

    public ICommand RestartCommand { get; }

    public GameViewModel()
    {
        RestartCommand = new Command(StartGame);
        LoadWords();
        StartGame();
    }

    private void LoadWords()
    {
        string filePath = Path.Combine("C:\\Users\\roeea\\OneDrive\\Desktop\\school-work-hangman-maui-c-\\MauiApp8\\Resources\\raw\\test.txt");

        if (!File.Exists(filePath))
        {
            words = new[] { "MAUI", "HANGMAN", "DOTNET", "MOBILE", "DEVELOPER" };
            return;
        }

        words = File.ReadAllLines(filePath)
                    .Where(w => !string.IsNullOrWhiteSpace(w))
                    .Select(w => w.Trim().ToUpperInvariant())
                    .ToArray();
    }

    public void SetPlayer(string name, int score)
    {
        currentPlayerName = name;
        Score = Preferences.Get($"score_{name}", score);
    }

    private void SaveScore()
    {
        if (!string.IsNullOrEmpty(currentPlayerName))
        {
            Preferences.Set($"score_{currentPlayerName}", Score);
        }
    }

    public void CheckGuess(char guessedLetter)
    {
        if (guessedLetters.Contains(guessedLetter)) return;
        guessedLetters.Add(guessedLetter);

        bool isCorrectGuess = false;

        for (int i = 0; i < selectedWord.Length; i++)
        {
            if (selectedWord[i] == guessedLetter)
            {
                guessedWord[i] = guessedLetter;
                isCorrectGuess = true;
            }
        }

        if (!isCorrectGuess)
        {
            Lives--;
            UpdateHangmanImage();
        }

        UpdateWordDisplay();

        if (!new string(guessedWord).Contains("_"))
        {
            Score += 10;
            SaveScore();
            OnScoreChanged?.Invoke(currentPlayerName, Score);
            Application.Current.MainPage.DisplayAlert("You Win!", "You guessed the word!", "OK");
            StartGame();
        }
        else if (Lives <= 0)
        {
            Score = 0;
            SaveScore();
            OnScoreChanged?.Invoke(currentPlayerName, Score);
            Application.Current.MainPage.DisplayAlert("Game Over", $"The word was: {selectedWord}", "Restart");
            StartGame();
        }
    }


    public void StartGame()
    {
        if (words == null || words.Length == 0)
        {
            Application.Current.MainPage.DisplayAlert("Error", "Word list is empty!", "OK");
            return;
        }

        Random random = new();
        selectedWord = words[random.Next(words.Length)];

        guessedWord = new string('_', selectedWord.Length).ToCharArray();

        Lives = 6;
        guessedLetters.Clear();

        UpdateWordDisplay();
        UpdateHangmanImage();
    }
    private void UpdateWordDisplay()
    {
        DisplayWord = string.Join(" ", guessedWord);
        OnPropertyChanged(nameof(DisplayWord));  
    }

    private void UpdateHangmanImage()
    {
        HangmanImage = $"hangman{7 - Lives}.png";
        OnPropertyChanged(nameof(HangmanImage));
    }

    private void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
