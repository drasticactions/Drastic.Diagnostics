// <copyright file="DebugAppClientWindow.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client.WinUI;
using Drastic.Diagnostics.Remote;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Drastic.Diagnostics.Server.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugAppClientWindow : WindowEx
    {

        private WinUIAppClient client;

        public DebugAppClientWindow()
        {
            this.InitializeComponent();

            this.client = new WinUIAppClient(DiagnosticsProtocol.Instance, "Test Client");

            var manager = WinUIEx.WindowManager.Get(this);
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            this.Width = 500;
            this.Height = 500;
        }

        private void GetViewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.client.TryGetRepresentedView(this, true, out var view);
        }

        private async void GetCaptureImageButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.client.TryGetRepresentedView(this.GetCaptureImageButton, false, out var view);
            if (view is InspectView inspectView)
            {
                await inspectView.Capture();
                if (inspectView.CapturedImage.Any())
                {
                    await File.WriteAllBytesAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.png"), inspectView.CapturedImage);
                }
            }
        }
    }
}
