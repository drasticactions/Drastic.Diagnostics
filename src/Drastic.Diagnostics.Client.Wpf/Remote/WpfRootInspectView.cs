// <copyright file="WpfRootInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Drastic.Diagnostics.Client.Wpf.Remote
{
    public class WpfRootInspectView : InspectView
    {
        public WpfRootInspectView()
        {
            SetHandle(IntPtr.Zero);
            DisplayName = "Root";

            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsVisible)
                    AddSubview(new WpfInspectView(window));
            }
        }

        protected override Task<bool> UpdateCapturedImage()
        {
            return Task.FromResult(false);
        }
    }
}
