using TutoringPlatform.Mobile.ViewModels;

namespace TutoringPlatform.Mobile.Pages;

public partial class MyBookingsPage : ContentPage
{
    private readonly MyBookingsViewModel _vm;

    public MyBookingsPage(MyBookingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}