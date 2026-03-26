using Microsoft.Extensions.Logging;
using TutoringPlatform.Mobile.Api;
using TutoringPlatform.Mobile.Config;
using TutoringPlatform.Mobile.Pages;
using TutoringPlatform.Mobile.Storage;
using TutoringPlatform.Mobile.ViewModels;

namespace TutoringPlatform.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<TokenStorage>();
        builder.Services.AddSingleton<AuthMessageHandler>();

        builder.Services.AddSingleton(sp =>
        {
            var tokenStorage = sp.GetRequiredService<TokenStorage>();

            var httpHandler = new HttpClientHandler();

#if DEBUG
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

            var handler = new AuthMessageHandler(tokenStorage)
            {
                InnerHandler = httpHandler
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiOptions.BaseUrl)
            };
        });

        builder.Services.AddSingleton<AuthApi>();
        builder.Services.AddSingleton<TutorsApi>();
        builder.Services.AddSingleton<BookingsApi>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TutorsViewModel>();
        builder.Services.AddTransient<MyBookingsViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TutorsPage>();
        builder.Services.AddTransient<MyBookingsPage>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<TutorDetailsViewModel>();
        builder.Services.AddTransient<TutorDetailsPage>();
        builder.Services.AddTransient<BookingsApi>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<TutorDetailsApi>();
        builder.Services.AddTransient<TutorDetailsViewModel>();
        builder.Services.AddTransient<TutorDetailsPage>();
        builder.Services.AddTransient<AvailabilityApi>();




        return builder.Build();
    }
}