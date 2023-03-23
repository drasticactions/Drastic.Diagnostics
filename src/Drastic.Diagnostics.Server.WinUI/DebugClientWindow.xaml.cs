using CommunityToolkit.Mvvm.DependencyInjection;
using Drastic.Diagnostics.Server.ViewModels;
using Microsoft.UI.Xaml;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Drastic.Diagnostics.Server.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugClientWindow : WindowEx
    {
        public DebugClientWindow(string? ip = default, int? port = default)
        {
            this.InitializeComponent();
            this.DebugViewModel = Ioc.Default.GetService<DebugClientViewModel>()!;

            this.DebugViewModel.Port = port;
            this.DebugViewModel.IPAddress = ip;

            var manager = WinUIEx.WindowManager.Get(this);
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            this.Width = 500;
            this.Height = 500;
        }

        public bool Invert(bool value) => !value;

        private DebugClientViewModel DebugViewModel { get; }
    }
}
