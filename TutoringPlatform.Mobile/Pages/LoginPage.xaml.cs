using TutoringPlatform.Mobile.ViewModels;

namespace TutoringPlatform.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}