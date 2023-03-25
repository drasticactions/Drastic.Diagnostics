// <copyright file="WinUIWindowInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Remote;
using Microsoft.UI.Xaml;

namespace Drastic.Diagnostics.Client.WinUI.Remote
{
    public class WinUIWindowInspectView : InspectView
    {
        private UIElement control;

        public WinUIWindowInspectView(Window window, bool withSubviews)
        {
            this.control = window.Content;
            this.SetHandle(WinRT.Interop.WindowNative.GetWindowHandle(window));
            this.DisplayName = window.Title;

            if (window.Content is FrameworkElement element)
            {
                this.AddSubview(new WinUIInspectView(element, withSubviews));
            }
        }

        protected override async Task<bool> UpdateCapturedImage()
        {
            this.CapturedImage = await this.control.ToImage();

            return this.CapturedImage is not null && this.CapturedImage.Any();
        }
    }
}
