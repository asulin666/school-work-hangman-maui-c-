using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiApp8;

public partial class StartupPage : ContentPage
{
    public StartupPage()
    {
        InitializeComponent();

        // Load best score from Preferences
        int bestScore = Preferences.Get("best_score", 0);
        BestScoreLabel.Text = $"Best Score: {bestScore}";
    }

    private async void OnStartGameClicked(object sender, EventArgs e)
    {
        string playerName = NameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            await DisplayAlert("Error", "Please enter your name before starting.", "OK");
            return;
        }

        // Pass player name to MainPage
        await Navigation.PushAsync(new MainPage());
    }

    private async void OnInstructionsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage());
    }
}
