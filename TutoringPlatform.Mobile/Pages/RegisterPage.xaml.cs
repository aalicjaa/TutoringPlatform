using TutoringPlatform.Mobile.ViewModels;

namespace TutoringPlatform.Mobile.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}