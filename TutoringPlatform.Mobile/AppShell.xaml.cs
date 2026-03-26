using TutoringPlatform.Mobile.Pages;

namespace TutoringPlatform.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("tutor-details", typeof(TutorDetailsPage));
    }
}