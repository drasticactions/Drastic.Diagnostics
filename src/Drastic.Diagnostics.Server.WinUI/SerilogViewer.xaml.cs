using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Drastic.Diagnostics.Server.WinUI
{
    public sealed partial class SerilogViewer : UserControl
    {
        private LogBroker logBroker;

        public SerilogViewer()
        {
            this.InitializeComponent();

            this.logBroker = Ioc.Default.GetRequiredService<LogBroker>();
            this.logBroker.Logs.CollectionChanged += this.Logs_CollectionChanged;
            this.DataContext = this.LogBroker;
        }

        private void Logs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.LogScrollViewer.ChangeView(
                    horizontalOffset: 0,
                    verticalOffset: this.LogScrollViewer.ScrollableHeight,
                    zoomFactor: 1,
                    disableAnimation: true);
        }

        public LogBroker LogBroker => this.logBroker;
    }
}
