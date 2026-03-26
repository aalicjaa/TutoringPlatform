using TutoringPlatform.Mobile.Models;
using TutoringPlatform.Mobile.ViewModels;

namespace TutoringPlatform.Mobile.Pages;

public partial class TutorsPage : ContentPage
{
    private readonly TutorsViewModel _vm;

    public TutorsPage(TutorsViewModel vm)
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

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection?.FirstOrDefault() as TutorListItem;
        if (item == null) return;

        ((CollectionView)sender).SelectedItem = null;

        if (item.Id == Guid.Empty) return;

        await Shell.Current.GoToAsync($"tutor-details?id={Uri.EscapeDataString(item.Id.ToString())}");
    }
}