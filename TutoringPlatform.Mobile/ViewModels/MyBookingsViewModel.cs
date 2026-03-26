using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TutoringPlatform.Mobile.Api;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.ViewModels;

public sealed class MyBookingsViewModel : INotifyPropertyChanged
{
    private readonly BookingsApi _api;

    private bool _isBusy;
    private string _error = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<MyBookingDto> Items { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            ((Command)RefreshCommand).ChangeCanExecute();
            ((Command)CancelCommand).ChangeCanExecute();
        }
    }

    public string Error
    {
        get => _error;
        private set
        {
            if (_error == value) return;
            _error = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand CancelCommand { get; }

    public MyBookingsViewModel(BookingsApi api)
    {
        _api = api;
        RefreshCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        CancelCommand = new Command<MyBookingDto>(async (b) => await CancelAsync(b), (b) => !IsBusy && b != null);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        Error = "";

        try
        {
            var data = await _api.GetMineAsync();
            Items.Clear();
            foreach (var x in data) Items.Add(x);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CancelAsync(MyBookingDto booking)
    {
        if (IsBusy) return;

        var id =
            GetGuid(booking, "Id") ??
            GetGuid(booking, "BookingId") ??
            GetGuid(booking, "ReservationId");

        if (id is null)
        {
            Error = "Nie mogę anulować: brak ID rezerwacji w obiekcie.";
            return;
        }

        IsBusy = true;
        Error = "";

        try
        {
            await _api.CancelAsync(id.Value);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static Guid? GetGuid(object obj, string name)
    {
        var p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (p == null) return null;

        var v = p.GetValue(obj);
        if (v == null) return null;

        if (v is Guid g) return g;
        return Guid.TryParse(v.ToString(), out var parsed) ? parsed : null;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}