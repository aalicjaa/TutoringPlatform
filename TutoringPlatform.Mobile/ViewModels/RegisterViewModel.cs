using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TutoringPlatform.Mobile.Api;

namespace TutoringPlatform.Mobile.ViewModels;

public sealed class RegisterViewModel : INotifyPropertyChanged
{
    private readonly AuthApi _authApi;

    private bool _isBusy;
    private string _error = "";

    private string _firstName = "";
    private string _lastName = "";
    private string _email = "";
    private string _password = "";
    private string _passwordConfirm = "";
    private string _role = "Student";

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            ((Command)RegisterCommand).ChangeCanExecute();
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

    public string FirstName { get => _firstName; set { value ??= ""; if (_firstName == value) return; _firstName = value; OnPropertyChanged(); } }
    public string LastName { get => _lastName; set { value ??= ""; if (_lastName == value) return; _lastName = value; OnPropertyChanged(); } }
    public string Email { get => _email; set { value ??= ""; if (_email == value) return; _email = value; OnPropertyChanged(); } }
    public string Password { get => _password; set { value ??= ""; if (_password == value) return; _password = value; OnPropertyChanged(); } }
    public string PasswordConfirm { get => _passwordConfirm; set { value ??= ""; if (_passwordConfirm == value) return; _passwordConfirm = value; OnPropertyChanged(); } }

    public string Role
    {
        get => _role;
        set
        {
            value ??= "Student";
            if (_role == value) return;
            _role = value;
            OnPropertyChanged();
        }
    }

    public IList<string> RoleOptions { get; } = new List<string> { "Student", "Tutor" };

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    public RegisterViewModel(AuthApi authApi)
    {
        _authApi = authApi;
        RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
        GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//login"));
    }

    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        Error = "";

        var first = (FirstName ?? "").Trim();
        var last = (LastName ?? "").Trim();
        var email = (Email ?? "").Trim();
        var pass = Password ?? "";
        var pass2 = PasswordConfirm ?? "";
        var role = (Role ?? "Student").Trim();

        if (first.Length < 2) { Error = "Podaj imię."; return; }
        if (last.Length < 2) { Error = "Podaj nazwisko."; return; }
        if (email.Length < 5 || !email.Contains('@')) { Error = "Podaj poprawny email."; return; }
        if (pass.Length < 6) { Error = "Hasło min. 6 znaków."; return; }
        if (!string.Equals(pass, pass2, StringComparison.Ordinal)) { Error = "Hasła nie są takie same."; return; }
        if (role != "Student" && role != "Tutor") { Error = "Wybierz rolę."; return; }

        IsBusy = true;

        try
        {
            var result = await _authApi.RegisterAndLoginAsync(first, last, email, pass, role);
            if (!result.Ok)
            {
                Error = result.Error ?? "Rejestracja nie powiodła się";
                return;
            }

            await Shell.Current.GoToAsync("//main/tutors");
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

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}