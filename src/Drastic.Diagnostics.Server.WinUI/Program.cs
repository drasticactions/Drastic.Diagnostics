// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace Drastic.Diagnostics.Server.WinUI
{
    /// <summary>
    /// Main Program.
    /// </summary>
    internal class Program
    {
        private const string AppKey = "914fd47d-9c41-4e54-98c7-712e5f115ed4";

        [STAThread]
        private static int Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            bool isRedirect = DecideRedirection();
            if (!isRedirect)
            {
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }

            return 0;
        }

        private static bool DecideRedirection()
        {
            bool isRedirect = false;
            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
            ExtendedActivationKind kind = args.Kind;
            AppInstance keyInstance = AppInstance.FindOrRegisterForKey(AppKey);

            if (keyInstance.IsCurrent)
            {
                keyInstance.Activated += OnActivated;
            }
            else
            {
                isRedirect = true;
                keyInstance.RedirectActivationToAsync(args).GetAwaiter().GetResult();
            }

            return isRedirect;
        }

        private static void OnActivated(object? sender, AppActivationArguments args)
        {
            ExtendedActivationKind kind = args.Kind;
        }
    }
}
