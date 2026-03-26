using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TutoringPlatform.Mobile.Api;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.ViewModels;

public sealed class TutorsViewModel : INotifyPropertyChanged
{
    private readonly TutorsApi _api;

    private bool _isBusy;
    private string _error = "";

    private string _query = "";
    private string? _selectedCity;
    private string? _selectedSubject;

    private string? _minPriceText;
    private string? _maxPriceText;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<TutorListItem> Items { get; } = new();
    public ObservableCollection<TutorListItem> FilteredItems { get; } = new();

    public ObservableCollection<string> Cities { get; } = new();
    public ObservableCollection<string> Subjects { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            (RefreshCommand as Command)?.ChangeCanExecute();
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

    public string Query
    {
        get => _query;
        set
        {
            var v = value ?? "";
            if (_query == v) return;
            _query = v;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    public string? SelectedCity
    {
        get => _selectedCity;
        set
        {
            if (_selectedCity == value) return;
            _selectedCity = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    public string? SelectedSubject
    {
        get => _selectedSubject;
        set
        {
            if (_selectedSubject == value) return;
            _selectedSubject = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    public string? MinPriceText
    {
        get => _minPriceText;
        set
        {
            if (_minPriceText == value) return;
            _minPriceText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PriceRangeLabel));
            ApplyFilters();
        }
    }

    public string? MaxPriceText
    {
        get => _maxPriceText;
        set
        {
            if (_maxPriceText == value) return;
            _maxPriceText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PriceRangeLabel));
            ApplyFilters();
        }
    }

    public int? MinPrice => TryParsePrice(MinPriceText);
    public int? MaxPrice => TryParsePrice(MaxPriceText);

    public string PriceRangeLabel
    {
        get
        {
            var min = MinPrice;
            var max = MaxPrice;
            if (min is null && max is null) return "Bez limitu ceny";
            if (min is not null && max is null) return $"od {min} zł";
            if (min is null && max is not null) return $"do {max} zł";
            return $"{min}-{max} zł";
        }
    }

    public string ResultsText => $"{FilteredItems.Count} wyników";

    public ICommand RefreshCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public TutorsViewModel(TutorsApi api)
    {
        _api = api;
        RefreshCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        ClearFiltersCommand = new Command(ClearFilters);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        Error = "";

        try
        {
            var data = await _api.GetAsync();

            Items.Clear();

            foreach (var dto in data)
            {
                var name = (dto.DisplayName ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                    name = "Korepetytor";

                var city = (dto.City ?? "").Trim();
                var bio = (dto.Bio ?? "").Trim();

                var subjectNames = dto.Subjects?
                    .Select(s => (s.Name ?? "").Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? new List<string>();

                var subjectLine = string.Join(", ", subjectNames);

                Items.Add(new TutorListItem
                {
                    Id = dto.Id,
                    FullName = name,
                    City = city,
                    Bio = bio,
                    SubjectLine = subjectLine,
                    PriceFrom = dto.MinOfferPrice
                });
            }

            RebuildFilterLists();
            ApplyFilters();
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

    private void ClearFilters()
    {
        Query = "";
        SelectedCity = null;
        SelectedSubject = null;
        MinPriceText = null;
        MaxPriceText = null;
    }

    private void ApplyFilters()
    {
        var q = (Query ?? "").Trim();
        var city = SelectedCity?.Trim();
        var subject = SelectedSubject?.Trim();
        var min = MinPrice;
        var max = MaxPrice;

        FilteredItems.Clear();

        foreach (var it in Items)
        {
            if (!string.IsNullOrWhiteSpace(city) &&
                !string.Equals((it.City ?? "").Trim(), city, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!string.IsNullOrWhiteSpace(subject))
            {
                var tokens = (it.SubjectLine ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x));

                if (!tokens.Any(t => string.Equals(t, subject, StringComparison.OrdinalIgnoreCase)))
                    continue;
            }

            if (min is not null)
            {
                if (!it.PriceFrom.HasValue) continue;
                if (it.PriceFrom.Value < min.Value) continue;
            }

            if (max is not null)
            {
                if (!it.PriceFrom.HasValue) continue;
                if (it.PriceFrom.Value > max.Value) continue;
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var hay = $"{it.FullName} {it.City} {it.SubjectLine} {it.Bio}";
                if (!hay.Contains(q, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            FilteredItems.Add(it);
        }

        OnPropertyChanged(nameof(ResultsText));
    }

    private int? TryParsePrice(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return int.TryParse(s.Trim(), out var v) ? v : null;
    }

    private void RebuildFilterLists()
    {
        Cities.Clear();
        Subjects.Clear();

        var citySet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var subjectSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var it in Items)
        {
            if (!string.IsNullOrWhiteSpace(it.City))
                citySet.Add(it.City.Trim());

            var tokens = (it.SubjectLine ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            foreach (var t in tokens)
                subjectSet.Add(t);
        }

        foreach (var c in citySet.OrderBy(x => x)) Cities.Add(c);
        foreach (var s in subjectSet.OrderBy(x => x)) Subjects.Add(s);
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}