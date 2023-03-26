// <copyright file="WpfInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drastic.Diagnostics.Remote;

namespace Drastic.Diagnostics.Client.Wpf.Remote
{
    public class WpfInspectView : InspectView
    {
        readonly UIElement control;
        readonly Transform? xform;

        public WpfInspectView(FrameworkElement control, bool withSubviews = true)
        {
            this.control = control;
            var parent = VisualTreeHelper.GetParent(control) as UIElement;
            var bounds = VisualTreeHelper.GetDescendantBounds(control);
            var window = Window.GetWindow(control);

            if (parent == null)
            {
                var transformToDevice = PresentationSource.FromVisual(window).CompositionTarget.TransformToDevice;
                var controlLocation = window.PointToScreen(bounds.TopLeft);

                // return X and Y in application space not screen space
                // so that they are in the same coordinate space as Width and Height
                X = (long)(controlLocation.X / transformToDevice.M11);
                Y = (long)(controlLocation.Y / transformToDevice.M22);
            }
            else
            {
                xform = (withSubviews && parent != null ?
                    control.TransformToAncestor(parent) :
                    control.TransformToVisual(Window.GetWindow(control))) as Transform;

                if (xform != null)
                {
                    Transform = new ViewTransform
                    {
                        M11 = xform.Value.M11,
                        M12 = xform.Value.M12,
                        M21 = xform.Value.M21,
                        M22 = xform.Value.M22,
                        OffsetX = xform.Value.OffsetX,
                        OffsetY = xform.Value.OffsetY
                    };
                }

                var controlLocation = VisualTreeHelper.GetOffset(control);
                X = (long)controlLocation.X;
                Y = (long)controlLocation.Y;
            }

            Width = (long)Math.Max(bounds.Width, control.RenderSize.Width);
            Height = (long)Math.Max(bounds.Height, control.RenderSize.Height);
            Visibility = control.Visibility.ToViewVisibility();

            PopulateTypeInformationFromObject(control);
            DisplayName = control.GetType().Name;
            if (!String.IsNullOrEmpty(control.Name))
                DisplayName += " - " + control.Name;

            if (!withSubviews)
            {
                var coords = control.TranslatePoint(new Point(X, Y), window);
                var controlLocation = window.PointToScreen(coords);
                var transformToDevice = PresentationSource.FromVisual(window).CompositionTarget.TransformToDevice;
                X = (long)(controlLocation.X / transformToDevice.M11);
                Y = (long)(controlLocation.Y / transformToDevice.M22);
                return;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                var child = VisualTreeHelper.GetChild(control, i) as FrameworkElement;
                if (child == null)
                {
                    continue;
                }

                this.AddSubview(new WpfInspectView(child));
            }
        }

        protected override Task<bool> UpdateCapturedImage()
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(control);
            if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
                return Task.FromResult(false);

            var target = new RenderTargetBitmap(
                (int)Math.Ceiling((decimal)Width),
                (int)Math.Ceiling((decimal)Height),
                96,
                96,
                PixelFormats.Pbgra32);

            if (control != null)
            {
                var dv = new DrawingVisual();

                using (var ctx = dv.RenderOpen())
                {
                    var vb = new VisualBrush(control);
                    var inverse = xform?.Inverse as Transform;
                    if (inverse != null)
                    {
                        bounds = xform?.TransformBounds(bounds) ?? Rect.Empty;
                        ctx.PushTransform(inverse);
                    }
                    ctx.DrawRectangle(vb, null, bounds);
                }
                target.Render(dv);
            }
            else
            {
                target.Render(control);
            }

            var encoder = new PngBitmapEncoder();
            var outputFrame = BitmapFrame.Create(target);
            encoder.Frames.Add(outputFrame);

            using (var memory = new MemoryStream())
            {
                encoder.Save(memory);
                CapturedImage = memory.GetBuffer();
            }

            return Task.FromResult(true);
        }
    }
}
