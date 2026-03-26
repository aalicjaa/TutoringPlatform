using TutoringPlatform.Mobile.ViewModels;

namespace TutoringPlatform.Mobile.Pages;

public partial class TutorDetailsPage : ContentPage, IQueryAttributable
{
    private readonly TutorDetailsViewModel _vm;

    public TutorDetailsPage(TutorDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var raw) && raw is string idStr && Guid.TryParse(idStr, out var id))
        {
            await _vm.InitializeAsync(id);
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        await _vm.LoadSlotsAsync();
    }
}