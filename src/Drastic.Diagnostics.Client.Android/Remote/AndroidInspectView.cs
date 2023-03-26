// <copyright file="AndroidInspectView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Android.Views;
using Drastic.Diagnostics.Remote;

namespace Drastic.Diagnostics.Client.Android.Remote
{
    public class AndroidInspectView : InspectView
    {
        const string androidIdPrefix = "android:";

        View? view;

        public new AndroidInspectView? Parent {
            get { return base.Parent as AndroidInspectView; }
        }

        public new AndroidInspectView Root {
            get { return (AndroidInspectView)base.Root; }
        }

        public AndroidInspectView()
        {
        }

        public AndroidInspectView(View view, bool withSubviews = true)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            this.view = view;

            // FIXME: special case certain view types and fill in the Description property
            if (withSubviews)
            {
                Transform = ViewRenderer.GetViewTransform(view);
                if (Transform == null)
                {
                    X = view.Left;
                    Y = view.Top;
                }
            }
            else
            {
                var locArray = new int[2];
                view.GetLocationOnScreen(locArray);
                X = locArray[0];
                Y = locArray[1];
            }

            Width = view.Width;
            Height = view.Height;
            Kind = ViewKind.Primary;
            Visibility = view.Visibility.ToViewVisibility();

            PopulateTypeInformationFromObject(view);

            DisplayName = view.GetType().Name;
            try
            {
                DisplayName += " :" + (view.Resources?.GetResourceName(view.Id) ?? string.Empty).TrimId();
            }
            catch
            { }

            if (view is Button button)
            {
                Description = button.Text ?? string.Empty;
            }
            else if (view is TextView textView)
            {
                Description = textView.Text ?? string.Empty;
            }

            if (!withSubviews)
                return;

            var subviews = view.Subviews();
            if (subviews != null && subviews.Length > 0)
            {
                for (int i = 0; i < subviews.Length; i++)
                    AddSubview(new AndroidInspectView(subviews[i]));
            }
        }

        protected override Task<bool> UpdateCapturedImage()
        {
            CapturedImage = ViewRenderer.RenderAsPng(view, true);

            return Task.FromResult(CapturedImage.Length > 0);
        }
    }
}
