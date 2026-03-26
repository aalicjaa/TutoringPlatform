using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TutoringPlatform.Mobile.Api;

namespace TutoringPlatform.Mobile.ViewModels;

public sealed class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthApi _authApi;

    private string _email = "";
    private string _password = "";
    private bool _isBusy;
    private string _error = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Email
    {
        get => _email;
        set
        {
            value ??= "";
            if (_email == value) return;
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            value ??= "";
            if (_password == value) return;
            _password = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            ((Command)LoginCommand).ChangeCanExecute();
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

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel(AuthApi authApi)
    {
        _authApi = authApi;
        LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//register"));
    }

    private async Task LoginAsync()
    {
        if (IsBusy) return;

        var email = (Email ?? "").Trim();
        var password = Password ?? "";

        if (email.Length < 5 || !email.Contains('@'))
        {
            Error = "Podaj poprawny email";
            return;
        }

        if (password.Length < 1)
        {
            Error = "Podaj hasło";
            return;
        }

        IsBusy = true;
        Error = "";

        try
        {
            var (ok, err) = await _authApi.LoginAsync(email, password);
            if (!ok)
            {
                Error = err ?? "Nie udało się zalogować";
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