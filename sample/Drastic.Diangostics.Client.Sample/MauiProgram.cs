using Drastic.Diagnostics.Client;
using Microsoft.Extensions.Logging;

namespace Drastic.Diangostics.Client.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

#if WINDOWS
		builder.Services.AddSingleton<IAppClientFactory, Drastic.Diagnostics.Client.WinUI.WinUIAppClientFactory>();
#elif ANDROID
        builder.Services.AddSingleton<IAppClientFactory, Drastic.Diagnostics.Client.Android.AndroidAppClientFactory>();
#elif IOS || MACCATALYST
		builder.Services.AddSingleton<IAppClientFactory, Drastic.Diagnostics.Client.iOS.iOSAppClientFactory>();
#endif

        builder
            .UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
