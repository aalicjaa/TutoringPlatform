namespace TutoringPlatform.Mobile;

public partial class App : Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Light;
        MainPage = shell;
    }
}