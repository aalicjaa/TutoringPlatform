namespace TutoringPlatform.Mobile.Config;

public static class ApiOptions
{
#if ANDROID
    public const string BaseUrl = "https://10.0.2.2:7168/";
#else
    public const string BaseUrl = "https://localhost:7168/";
#endif
}