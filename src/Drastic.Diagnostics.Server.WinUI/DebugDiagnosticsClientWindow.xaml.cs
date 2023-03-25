// <copyright file="DebugDiagnosticsClientWindow.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CommunityToolkit.Mvvm.DependencyInjection;
using Drastic.Diagnostics.Debug.ViewModels;
using Drastic.Diagnostics.Server.ViewModels;
using WinUIEx;

namespace Drastic.Diagnostics.Server.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugDiagnosticsClientWindow : WindowEx
    {
        public DebugDiagnosticsClientWindow(string? ip = default, int? port = default)
        {
            this.InitializeComponent();
            this.DebugViewModel = Ioc.Default.GetService<DebugDiagnosticsClientViewModel>()!;

            this.DebugViewModel.Port = port;
            this.DebugViewModel.IPAddress = ip;

            var manager = WinUIEx.WindowManager.Get(this);
            manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

            this.Width = 500;
            this.Height = 500;
        }

        public bool Invert(bool value) => !value;

        private DebugDiagnosticsClientViewModel DebugViewModel { get; }
    }
}
