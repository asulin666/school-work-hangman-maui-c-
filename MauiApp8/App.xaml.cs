namespace MauiApp8
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // עוטף את MainPage בתוך NavigationPage כדי לאפשר Navigation.PushAsync
            MainPage = new NavigationPage(new MainPage());

        }

    }
}
