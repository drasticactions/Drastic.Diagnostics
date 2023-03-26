// <copyright file="WpfAppClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;
using System.Security.Principal;
using System.Windows;
using Drastic.Diagnostics.Client.Wpf.Remote;
using Drastic.Diagnostics.Inspection;
using Drastic.Diagnostics.Remote;
using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client.Wpf
{
    public class WpfAppClient : AppClient, IViewHierarchyHandler
    {
        public WpfAppClient(Protocol protocol, string name = "", ILogger? logger = null)
           : base(protocol, name, logger)
        {
            this.ViewHierarchyHandlerManager.AddViewHierarchyHandler("WPF", this);
        }

        public override InspectView? GetVisualTree(string hierarchyKind)
               => this.ViewHierarchyHandlerManager.GetView(Application.Current, hierarchyKind, true);

        public bool TryGetRepresentedView(object view, bool withSubviews, out IInspectView? representedView)
        {
            if (view is Application)
            {
                representedView = new WpfRootInspectView { DisplayName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? string.Empty };
                return true;
            }

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                representedView = new WpfInspectView(frameworkElement, withSubviews);
                return true;
            }

            representedView = null;
            return false;
        }
    }
}
