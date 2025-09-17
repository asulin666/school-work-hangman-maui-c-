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
    }

    private void OnCheckScoreClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(name))
        {
            ScoreLabel.Text = "Enter a real name!";
            return;
        }

        currentPlayerName = name;

        int savedScore = Preferences.Get($"score_{name}", 0);
        ScoreLabel.Text = $"Hello {name}, Score: {savedScore}";
        viewModel.SetPlayer(name, savedScore);
    }

    private void LetterClicked(object sender, EventArgs e)
    {
        if (sender is Button button && !string.IsNullOrEmpty(button.Text))
        {
            char letter = button.Text[0];
            viewModel.GuessLetter(letter);
        }
    }

    private async void Help_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage());
    }
}

// -------------------------------------------------------------
// GameViewModel Class
// -------------------------------------------------------------

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
    public string HangmanImage { get; set; } = "hangman0.png";

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
        string filePath = Path.Combine("C:\\Users\\Owner\\Desktop\\MauiApp8\\MauiApp8\\Resources\\Raw\\20k.txt");

        if (!File.Exists(filePath))
        {
            // fallback default words
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

    public void GuessLetter(char letter)
    {
        Console.WriteLine($"Letter guessed: {letter}");

        if (guessedLetters.Contains(letter)) return;
        guessedLetters.Add(letter);

        if (selectedWord.Contains(letter))
        {
            for (int i = 0; i < selectedWord.Length; i++)
            {
                if (selectedWord[i] == letter)
                    guessedWord[i] = letter;
            }
        }
        else
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
        HangmanImage = $"hangman{6 - Lives}.png";
        OnPropertyChanged(nameof(HangmanImage));
    }

    private void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));



private void Help_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new HelpPage();
    }


}





