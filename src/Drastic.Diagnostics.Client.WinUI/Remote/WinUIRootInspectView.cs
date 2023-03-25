// <copyright file="WinUIRootInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Remote;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Drastic.Diagnostics.Client.WinUI.Remote
{
    public class WinUIRootInspectView : InspectView
    {
        public WinUIRootInspectView()
        {
            this.SetHandle(IntPtr.Zero);
            this.DisplayName = "Root";

            // TODO: Figure out how to get windows for subviews.
        }
    }
}
