// <copyright file="WinUIInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client.WinUI.Representations;
using Drastic.Diagnostics.Remote;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;

namespace Drastic.Diagnostics.Client.WinUI.Remote
{
    public class WinUIInspectView : InspectView
    {
        private readonly UIElement control;

        public WinUIInspectView(FrameworkElement control, bool withSubviews = true)
        {
            this.control = control;
            var bounds = control.GetPlatformViewBounds();

            this.Transform = control.GetViewTransform().ToViewTransform();

            this.Width = (long)Math.Max(bounds.Width, control.RenderSize.Width);
            this.Height = (long)Math.Max(bounds.Height, control.RenderSize.Height);
            this.X = (long)(this.Transform?.OffsetX ?? 0);
            this.Y = (long)(this.Transform?.OffsetY ?? 0);

            this.PopulateTypeInformationFromObject(control);
            this.DisplayName = control.GetType().Name;
            if (!string.IsNullOrEmpty(control.Name))
            {
                this.DisplayName += " - " + control.Name;
            }

            if (!withSubviews)
            {
                return;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                var child = VisualTreeHelper.GetChild(control, i) as FrameworkElement;
                if (child == null)
                    continue;

                this.AddSubview(new WinUIInspectView(child));
            }
        }

        protected override async Task<bool> UpdateCapturedImage()
        {
            this.CapturedImage = await this.control.ToImage();

            return this.CapturedImage is not null && this.CapturedImage.Any();
        }
    }
}
