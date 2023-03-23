// <copyright file="MainWindow.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CommunityToolkit.Mvvm.DependencyInjection;
using Drastic.Diagnostics.Server.ViewModels;
using Drastic.Tools;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using WinUIEx;

namespace Drastic.Diagnostics.Server.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.MainViewModel = Ioc.Default.GetService<MainViewModel>()!;
            this.MainGrid.DataContext = this.MainViewModel;

            this.MainViewModel.OnLoad().FireAndForgetSafeAsync();

            this.Closed += this.MainWindow_Closed;

            var manager = WinUIEx.WindowManager.Get(this);
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            this.Width = 1000;
            this.Height = 600;
        }

        public bool Invert(bool value) => !value;

        private MainViewModel MainViewModel { get; }

        private void TextBlock_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.MainViewModel.IPAddress))
            {
                return;
            }

            var dataPackage = new DataPackage();
            dataPackage.SetText(this.MainViewModel.IPAddress);
            Clipboard.SetContent(dataPackage);
        }

        private void DebugClientMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var clientWindow = new DebugClientWindow(this.MainViewModel.IPAddress, this.MainViewModel.Port);
            clientWindow.Activate();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            Application.Current.Exit();
        }
    }
}
