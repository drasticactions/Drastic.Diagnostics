// <copyright file="iOSInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;
using Drastic.Diagnostics.Core;
using Drastic.Diagnostics.Inspection;
using Drastic.Diagnostics.Remote;

namespace Drastic.Diagnostics.Client.iOS.Remote
{
    public class iOSInspectView : InspectView
    {
        readonly UIView? view;
        readonly CALayer? layer;

        public NSObject? Material { get; set; }

        public new iOSInspectView? Parent {
            get { return base.Parent as iOSInspectView; }
            set { base.Parent = value; }
        }

        public new iOSInspectView? Root {
            get { return base.Root as iOSInspectView; }
        }

        public iOSInspectView()
        {
        }

        public iOSInspectView(UIView parent, CALayer layer, HashSet<IntPtr> visitedLayers, bool withSublayers = true)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            view = parent;
            this.layer = layer;

            SetHandle(ObjectCache.Shared.GetHandle(layer));
            PopulateTypeInformationFromObject(layer);

            Description = layer.Description;

            Transform = ViewRenderer.GetViewTransform(layer);
            if (Transform != null)
            {
                X = (long)layer.Bounds.X;
                Y = (long)layer.Bounds.Y;
                Width = (long)layer.Bounds.Width;
                Height = (long)layer.Bounds.Height;
            }
            else
            {
                X = (long)layer.Frame.X;
                Y = (long)layer.Frame.Y;
                Width = (long)layer.Frame.Width;
                Height = (long)layer.Frame.Height;
            }

            Kind = ViewKind.Secondary;
            // iOS doesn't have a concept of hidden but laid out, so it's either collapsed or visible.
            Visibility = layer.Hidden ? ViewVisibility.Collapsed : ViewVisibility.Visible;

            if (!withSublayers)
            {
                var point = view.ConvertPointToView(
                    new CoreGraphics.CGPoint(X, Y),
                    null);

                X = (long)point.X;
                Y = (long)point.Y;
                return;
            }

            var sublayers = layer.Sublayers;
            if (sublayers != null && sublayers.Length > 0)
            {
                for (int i = 0; i < sublayers.Length; i++)
                    if (!visitedLayers.Contains(sublayers[i].Handle))
                    {
                        AddSublayer(new iOSInspectView(parent, sublayers[i], visitedLayers));
                        visitedLayers.Add(sublayers[i].Handle);
                    }
            }
        }

        public iOSInspectView(UIView view, bool withSubviews = true)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            this.view = view;

            PopulateTypeInformationFromObject(view);

            // FIXME: special case certain view types and fill in the Description property

            if (view is UILabel label)
                Description = label.Text ?? string.Empty;
            else if (view is UIButton button)
                Description = button.TitleLabel.Text ?? string.Empty;
            else if (view is UITextField textField)
                Description = textField.Text ?? string.Empty;

            if (!view.Transform.IsIdentity)
            {
                var transform = CGAffineTransform.MakeIdentity();
                transform.Translate(-view.Bounds.Width * .5f, -view.Bounds.Height * .5f);
                transform = CGAffineTransform.Multiply(transform, view.Transform);
                transform.Translate(view.Center.X, view.Center.Y);
                Transform = new ViewTransform
                {
                    M11 = transform.xx,
                    M12 = transform.yx,
                    M21 = transform.xy,
                    M22 = transform.yy,
                    OffsetX = transform.x0,
                    OffsetY = transform.y0
                };
                X = (long)view.Bounds.X;
                Y = (long)view.Bounds.Y;
                Width = (long)view.Bounds.Width;
                Height = (long)view.Bounds.Height;
            }
            else
            {
                X = (long)view.Frame.X;
                Y = (long)view.Frame.Y;
                Width = (long)view.Frame.Width;
                Height = (long)view.Frame.Height;
            }
            Kind = ViewKind.Primary;
            Visibility = view.Hidden ? ViewVisibility.Collapsed : ViewVisibility.Visible;

            if (!withSubviews)
            {
                var point = view.ConvertPointToView(
                    new CoreGraphics.CGPoint(0, 0),
                    null);

                X = (long)point.X;
                Y = (long)point.Y;
                return;
            }

            // MKMapView has a subview that is so large (5901507x5901507 in the case encountered)
            // that it causes the SceneKit camera to zoom out so much that every other node is
            // effectively hidden. This should ideally be fixed in the client.
            if (view is MapKit.MKMapView)
                return;

            var visitedLayers = new HashSet<IntPtr>();

            var subviews = view.Subviews;
            if (subviews != null && subviews.Length > 0)
            {
                for (int i = 0; i < subviews.Length; i++)
                {
                    var subview = new iOSInspectView(subviews[i]);
                    AddSubview(subview);

                    if (subview.Layer == null)
                        continue;

                    // After calling AddSubview, add any visited layers to the list. We track
                    // visited layers here so that when we actually recurse into the layer that
                    // belongs to this view, we don't duplicate things. This is needed because of
                    // the pointer-into-a-tree nature of layers, as explained above in the constructor
                    // remarks.
                    var subviewLayer = (iOSInspectView)subview.Layer;
                    if (subviewLayer.layer != null)
                        visitedLayers.Add(subviewLayer.layer.Handle);

                    subviewLayer.layer?.Sublayers?.ForEach(
                        layer => visitedLayers.Add(layer.Handle));
                }
            }

            if (view.Layer != null && !visitedLayers.Contains(view.Layer.Handle))
                Layer = new iOSInspectView(view, view.Layer, visitedLayers) { Parent = this };
        }

        public UIImage? Capture(float? scale = null)
        {
            if (this.view is not null)
                return ViewRenderer.Render(view.Window, view, scale == null ? UIScreen.MainScreen.Scale : scale.Value);
            return null;
        }

        protected override Task<bool> UpdateCapturedImage()
        {
            if (view != null && layer == null)
                CapturedImage = ViewRenderer.RenderAsPng(view.Window, view, UIScreen.MainScreen.Scale) ?? new byte[0];
            else if (view != null && layer != null)
                CapturedImage = ViewRenderer.RenderAsPng(view.Window, layer, UIScreen.MainScreen.Scale) ?? new byte[0];

            return Task.FromResult(this.CapturedImage.Length > 0);
        }
    }
}
