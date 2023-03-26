// <copyright file="iOSRootInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Remote;
using ObjCRuntime;

namespace Drastic.Diagnostics.Client.iOS.Remote
{
    public class iOSRootInspectView : InspectView
    {
        private UIApplication control;

        public iOSRootInspectView(UIApplication app, bool withSubviews = true)
        {
            this.control = app;
            this.SetHandle(app.GetHandle());
            this.DisplayName = "App";

            foreach (var window in app.Windows)
            {
                this.AddSubview(new iOSInspectView(window, withSubviews));
            }
        }
    }
}
