// <copyright file="WinUIInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client.WinUI.Representations;
using Drastic.Diagnostics.Remote;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Drastic.Diagnostics.Client.WinUI.Remote
{
    public class WinUIInspectView : InspectView
    {
        private readonly UIElement control;

        public WinUIInspectView(FrameworkElement control, bool withSubviews = true)
        {
            this.control = control;
            var bounds = control.GetPlatformViewBounds();

            this.Width = (long)Math.Max(bounds.Width, control.RenderSize.Width);
            this.Height = (long)Math.Max(bounds.Height, control.RenderSize.Height);

            this.PopulateTypeInformationFromObject(control);
            this.DisplayName = control.GetType().Name;
            if (!string.IsNullOrEmpty(control.Name))
            {
                this.DisplayName += " - " + control.Name;
            }
        }
    }
}
