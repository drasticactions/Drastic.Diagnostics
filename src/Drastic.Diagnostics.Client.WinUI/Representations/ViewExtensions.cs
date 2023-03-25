// <copyright file="ViewExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Numerics;
using Drastic.Diagnostics.Representations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Input.Inking;
using WinPoint = Windows.Foundation.Point;

namespace Drastic.Diagnostics.Client.WinUI.Representations
{
    internal static class ViewExtensions
    {
        internal static Matrix4x4 GetViewTransform(this FrameworkElement element)
        {
            var root = element?.XamlRoot;
            var offset = element?.TransformToVisual(root?.Content ?? element) as MatrixTransform;
            if (offset == null)
                return new Matrix4x4();
            Matrix matrix = offset.Matrix;
            return new Matrix4x4()
            {
                M11 = (float)matrix.M11,
                M12 = (float)matrix.M12,
                M21 = (float)matrix.M21,
                M22 = (float)matrix.M22,
                Translation = new Vector3((float)matrix.OffsetX, (float)matrix.OffsetY, 0)
            };
        }

        internal static Rectangle GetPlatformViewBounds(this FrameworkElement platformView)
        {
            if (platformView == null)
                return new Rectangle();

            var root = platformView.XamlRoot;
            var offset = platformView.TransformToVisual(root.Content) as Microsoft.UI.Xaml.Media.MatrixTransform;
            if (offset != null)
                return new Rectangle(offset.Matrix.OffsetX, offset.Matrix.OffsetY, platformView.ActualWidth, platformView.ActualHeight);

            return new Rectangle();
        }

        internal static Rectangle GetBoundingBox(this FrameworkElement? platformView)
        {
            if (platformView == null)
                return new Rectangle();

            var rootView = platformView.XamlRoot.Content;
            if (platformView == rootView)
            {
                if (rootView is not FrameworkElement el)
                    return new Rectangle();

                return new Rectangle(0, 0, el.ActualWidth, el.ActualHeight);
            }

            var topLeft = platformView.TransformToVisual(rootView).TransformPoint(new WinPoint());
            var topRight = platformView.TransformToVisual(rootView).TransformPoint(new WinPoint(platformView.ActualWidth, 0));
            var bottomLeft = platformView.TransformToVisual(rootView).TransformPoint(new WinPoint(0, platformView.ActualHeight));
            var bottomRight = platformView.TransformToVisual(rootView).TransformPoint(new WinPoint(platformView.ActualWidth, platformView.ActualHeight));

            var x1 = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Min();
            var x2 = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Max();
            var y1 = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Min();
            var y2 = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Max();
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
