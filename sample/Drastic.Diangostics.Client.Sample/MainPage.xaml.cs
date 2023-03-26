using Drastic.Diagnostics;
using Drastic.Diagnostics.Client;
using Drastic.Diagnostics.Remote;

namespace Drastic.Diangostics.Client.Sample;

public partial class MainPage : ContentPage
{
    private IServiceProvider services;
    private IAppClientFactory appClientFactory;
    private AppClient appClient;

    public MainPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.appClientFactory = this.services.GetRequiredService<IAppClientFactory>();
        this.appClient = this.appClientFactory.GenerateAppClient(DiagnosticsProtocol.Instance);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string manager = string.Empty;

#if IOS || MACCATALYST
        manager = "UIKit";
#elif ANDROID
        manager = "Android";
#elif WINDOWS
        manager = "WinUI";
#endif

        var view = this.appClient.GetVisualTree(manager);
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        this.appClient.TryGetRepresentedView(this.Handler.PlatformView, false, out var view );
        if (view is InspectView inspectView)
        {
            await inspectView.Capture();
        }
    }
}

