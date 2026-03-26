using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using TutoringPlatform.Mobile.Api;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.ViewModels;

public sealed class TutorDetailsViewModel : INotifyPropertyChanged
{
    private readonly TutorDetailsApi _detailsApi;
    private readonly AvailabilityApi _availabilityApi;
    private readonly BookingsApi _bookingsApi;

    private bool _isBusy;
    private string _error = "";

    private Guid _tutorProfileId;

    private string _fullName = "";
    private string _city = "";
    private string _bio = "";

    private DateTime _date = DateTime.Today;

    private TutorOfferDto? _selectedOffer;
    private AvailabilitySlotDto? _selectedSlot;

    public event PropertyChangedEventHandler? PropertyChanged;

    public TutorDetailsViewModel(TutorDetailsApi detailsApi, AvailabilityApi availabilityApi, BookingsApi bookingsApi)
    {
        _detailsApi = detailsApi;
        _availabilityApi = availabilityApi;
        _bookingsApi = bookingsApi;

        RefreshSlotsCommand = new Command(async () => await LoadSlotsAsync(), () => !IsBusy && TutorProfileId != Guid.Empty);
        ReserveCommand = new Command(async () => await ReserveAsync());
        PayCommand = new Command(async () => await PayAsync(), () => !IsBusy);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            ((Command)RefreshSlotsCommand).ChangeCanExecute();
            ((Command)PayCommand).ChangeCanExecute();
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

    public Guid TutorProfileId
    {
        get => _tutorProfileId;
        private set
        {
            if (_tutorProfileId == value) return;
            _tutorProfileId = value;
            OnPropertyChanged();
            ((Command)RefreshSlotsCommand).ChangeCanExecute();
        }
    }

    public string FullName
    {
        get => _fullName;
        private set
        {
            if (_fullName == value) return;
            _fullName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Initials));
        }
    }

    public string City
    {
        get => _city;
        private set
        {
            if (_city == value) return;
            _city = value;
            OnPropertyChanged();
        }
    }

    public string Bio
    {
        get => _bio;
        private set
        {
            if (_bio == value) return;
            _bio = value;
            OnPropertyChanged();
        }
    }

    public DateTime Date
    {
        get => _date;
        set
        {
            if (_date.Date == value.Date) return;
            _date = value.Date;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TutorOfferDto> Offers { get; } = new();
    public ObservableCollection<AvailabilitySlotDto> AvailableSlots { get; } = new();

    public TutorOfferDto? SelectedOffer
    {
        get => _selectedOffer;
        set
        {
            if (_selectedOffer == value) return;
            _selectedOffer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PriceText));
            OnPropertyChanged(nameof(SubjectLine));
            OnPropertyChanged(nameof(SubjectChips));
            _ = LoadSlotsAsync();
        }
    }

    public AvailabilitySlotDto? SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            if (_selectedSlot == value) return;

            if (_selectedSlot != null) _selectedSlot.IsSelected = false;
            _selectedSlot = value;
            if (_selectedSlot != null) _selectedSlot.IsSelected = true;

            OnPropertyChanged();
        }
    }

    public IEnumerable<string> SubjectChips =>
        Offers.Select(o => o.SubjectName)
              .Where(s => !string.IsNullOrWhiteSpace(s))
              .Distinct(StringComparer.OrdinalIgnoreCase);

    public string SubjectLine => string.Join(", ", SubjectChips);

    public string PriceText
    {
        get
        {
            if (SelectedOffer != null)
                return $"{SelectedOffer.Price:0} zł";

            if (Offers.Count == 0)
                return "Brak ceny";

            return $"Od {Offers.Min(o => o.Price):0} zł";
        }
    }

    public string Initials
    {
        get
        {
            var name = (FullName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return char.ToUpper(parts[0][0]).ToString();

            return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
        }
    }

    public ICommand RefreshSlotsCommand { get; }
    public ICommand ReserveCommand { get; }
    public ICommand PayCommand { get; }

    public async Task InitializeAsync(Guid tutorId, CancellationToken ct = default)
    {
        await LoadDetailsAsync(tutorId, ct);
        await LoadSlotsAsync(ct);
    }

    public async Task LoadDetailsAsync(Guid tutorId, CancellationToken ct = default)
    {
        if (IsBusy) return;

        IsBusy = true;
        Error = "";

        try
        {
            var dto = await _detailsApi.GetAsync(tutorId, ct);
            if (dto == null)
            {
                Error = "Nie udało się pobrać danych korepetytora.";
                return;
            }

            TutorProfileId = dto.TutorProfileId;

            FullName = (dto.DisplayName ?? "Korepetytor").Trim();
            City = (dto.City ?? "").Trim();
            Bio = (dto.Bio ?? "").Trim();

            Offers.Clear();
            foreach (var o in dto.Offers ?? new List<TutorOfferDto>())
                Offers.Add(o);

            if (SelectedOffer == null && Offers.Count > 0)
                SelectedOffer = Offers[0];

            OnPropertyChanged(nameof(SubjectChips));
            OnPropertyChanged(nameof(SubjectLine));
            OnPropertyChanged(nameof(PriceText));
        }
        catch (Exception ex)
        {
            Error = ex.ToString();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadSlotsAsync(CancellationToken ct = default)
    {
        if (IsBusy) return;
        if (TutorProfileId == Guid.Empty) return;

        IsBusy = true;
        Error = "";

        try
        {
            foreach (var s in AvailableSlots) s.IsSelected = false;
            AvailableSlots.Clear();
            SelectedSlot = null;

            var dateUtc = Date.Date;
            var slots = await _availabilityApi.GetFreeAsync(TutorProfileId, dateUtc, ct);

            foreach (var s in slots)
                AvailableSlots.Add(s);
        }
        catch (Exception ex)
        {
            Error = ex.ToString();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ReserveAsync()
    {
        if (IsBusy) return;

        var offer = SelectedOffer ?? (Offers.Count > 0 ? Offers[0] : null);
        if (offer == null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Uwaga", "Wybierz ofertę (przedmiot) przed rezerwacją.", "OK");
            });
            return;
        }

        if (SelectedSlot == null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Uwaga", "Wybierz termin przed rezerwacją.", "OK");
            });
            return;
        }

        try
        {
            IsBusy = true;
            Error = "";

            var start = SelectedSlot.StartUtc;
            var end = SelectedSlot.EndUtc;

            await _bookingsApi.CreateAsync(offer.Id, start, end);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Lekcja została zarezerwowana.", "OK");
            });

            await LoadSlotsAsync();
        }
        catch (Exception ex)
        {
            Error = ex.ToString();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się zarezerwować lekcji. Sprawdź szczegóły w Error.", "OK");
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PayAsync()
    {
        await Task.Delay(1);
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}