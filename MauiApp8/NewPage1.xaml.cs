namespace MauiApp8;

public partial class HelpPage : ContentPage
{
    public HelpPage()
    {
        InitializeComponent();

    }
    private async void Home_Clicked(object sender, EventArgs e)
    {
        // For example, navigate to the MainPage itself or pop to root
        await Navigation.PopToRootAsync();
    }
}
