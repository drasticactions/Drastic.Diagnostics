// <copyright file="WinUIAppClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client.WinUI.Remote;
using Drastic.Diagnostics.Inspection;
using Drastic.Diagnostics.Messages;
using Drastic.Diagnostics.Remote;
using Drastic.Tempest;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Reflection;

namespace Drastic.Diagnostics.Client.WinUI
{
    public class WinUIAppClient : AppClient, IViewHierarchyHandler
    {
        public WinUIAppClient(Protocol protocol, string name = "", ILogger? logger = null)
            : base(protocol, name, logger)
        {
            this.ViewHierarchyHandlerManager.AddViewHierarchyHandler("WinUI", this);
        }

        public override InspectView? GetVisualTree(string hierarchyKind)
              => this.ViewHierarchyHandlerManager.GetView(Application.Current, hierarchyKind, true);

        public override bool TryGetRepresentedView(object view, bool withSubviews, out IInspectView? representedView)
        {
            if (view is Application)
            {
                // Windows.ApplicationModel.Package.Current.DisplayName
                representedView = new WinUIRootInspectView { DisplayName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? string.Empty };
                return true;
            }

            if (view is Window window)
            {
                representedView = new WinUIWindowInspectView(window, withSubviews);
                return true;
            }

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                representedView = new WinUIInspectView(frameworkElement, withSubviews);
                return true;
            }

            representedView = null;
            return false;
        }
    }
}
