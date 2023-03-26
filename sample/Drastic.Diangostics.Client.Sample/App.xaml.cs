namespace Drastic.Diangostics.Client.Sample;

public partial class App : Application
{
	public App(IServiceProvider services)
	{
		InitializeComponent();

		MainPage = new MainPage(services);
	}
}
