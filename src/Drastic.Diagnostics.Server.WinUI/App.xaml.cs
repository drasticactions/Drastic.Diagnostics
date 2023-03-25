// <copyright file="App.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Drastic.Diagnostics.Client;
using Drastic.Diagnostics.Client.WinUI;
using Drastic.Diagnostics.Debug.ViewModels;
using Drastic.Diagnostics.Server.ViewModels;
using Drastic.Diagnostics.Server.WinUI.Services;
using Drastic.Diagnostics.Server.WinUI.Tools;
using Drastic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.WinUi3;
using Serilog.Sinks.WinUi3.LogViewModels;
using Serilog.Templates;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Drastic.Diagnostics.Server.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            var dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            var model = new EmojiLogViewModelBuilder(Color.FromArgb(255, 225, 225, 225))

                .SetTimestampFormat(new ExpressionTemplate("[{@t:yyyy-MM-dd HH:mm:ss.fff}]"))

                .SetLevelsFormat(new ExpressionTemplate("{@l:u3}"))
                .SetLevelForeground(LogEventLevel.Verbose, Colors.Gray)
                .SetLevelForeground(LogEventLevel.Debug, Colors.Gray)
                .SetLevelForeground(LogEventLevel.Warning, Colors.Yellow)
                .SetLevelForeground(LogEventLevel.Error, Colors.Red)
                .SetLevelForeground(LogEventLevel.Fatal, Colors.HotPink)

                .SetSourceContextFormat(new ExpressionTemplate("{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}"))

                .SetMessageFormat(new ExpressionTemplate("{@m}"))
                .SetMessageForeground(LogEventLevel.Verbose, Colors.Gray)
                .SetMessageForeground(LogEventLevel.Debug, Colors.Gray)
                .SetMessageForeground(LogEventLevel.Warning, Colors.Yellow)
                .SetMessageForeground(LogEventLevel.Error, Colors.Red)
                .SetMessageForeground(LogEventLevel.Fatal, Colors.HotPink)

                .SetExceptionFormat(new ExpressionTemplate("{@x}"))
                .SetExceptionForeground(Colors.HotPink);

            var logBroker = new LogBroker(dispatcherQueue, model);

            var serverLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.WinUi3Control(logBroker)
                .CreateLogger();

            this.InitializeComponent();
            Ioc.Default.ConfigureServices(
             new ServiceCollection()
             .AddSingleton(logBroker)
             .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(serverLogger, dispose: true))
             .AddSingleton<IErrorHandlerService>(new WinUIErrorHandlerService(serverLogger))
             .AddSingleton<IAppDispatcher>(new AppDispatcher(dispatcherQueue))
             .AddSingleton<IAppClientFactory, WinUIAppClientFactory>()
             .AddSingleton<MainViewModel>()
             .AddTransient<DebugAppClientViewModel>()
             .AddTransient<DebugDiagnosticsClientViewModel>()
             .BuildServiceProvider());
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window? m_window;
    }
}
