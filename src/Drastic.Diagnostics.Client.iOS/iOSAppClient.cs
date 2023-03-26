// <copyright file="iOSAppClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client.iOS.Remote;
using Drastic.Diagnostics.Inspection;
using Drastic.Diagnostics.Remote;
using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client.iOS
{
    public class iOSAppClient : AppClient, IViewHierarchyHandler
    {
        public iOSAppClient(Protocol protocol, string name = "", ILogger? logger = null)
            : base(protocol, name, logger)
        {
            ViewHierarchyHandlerManager.AddViewHierarchyHandler("UIKit", this);
        }

        public override InspectView? GetVisualTree(string hierarchyKind)
        => ViewHierarchyHandlerManager.GetView(
                UIApplication.SharedApplication,
                hierarchyKind);

        public override bool TryGetRepresentedView(object view, bool withSubviews, out IInspectView? representedView)
        {
            if (view is UIApplication app)
            {
                representedView = new iOSRootInspectView(app, withSubviews);
            }

            var uiview = view as UIView;
            if (uiview != null)
            {
                representedView = new iOSInspectView(uiview, withSubviews);
                return true;
            }

            representedView = null;
            return false;
        }
    }
}
