// <copyright file="NativeExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Inspection;
using System.Windows;

namespace Drastic.Diagnostics.Client.Wpf
{
    internal static class NativeExtensions
    {
        public static ViewVisibility ToViewVisibility(this Visibility state)
        {
            switch (state)
            {
                case Visibility.Visible:
                    return ViewVisibility.Visible;
                case Visibility.Hidden:
                    return ViewVisibility.Hidden;
                case Visibility.Collapsed:
                    return ViewVisibility.Collapsed;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(state),
                        state,
                        "Don't know how to convert given ViewState to ViewVisibility.");
            }
        }
    }
}
