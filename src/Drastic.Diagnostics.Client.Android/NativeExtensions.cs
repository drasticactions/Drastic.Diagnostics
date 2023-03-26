using System;
using System.IO;
using SD = System.Drawing;

using AG = Android.Graphics;
using AL = Android.Locations;
using Android.Views;
using Drastic.Diagnostics.Inspection;

namespace Drastic.Diagnostics.Client.Android
{
    static class NativeExtensions
    {
        public static View[]? Subviews(this View parent)
        {
            var viewGroup = parent as ViewGroup;

            if (viewGroup == null)
                return null;

            var subViews = new View[viewGroup.ChildCount];
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                subViews[i] = viewGroup.GetChildAt(i) ?? throw new NullReferenceException();
            }

            return subViews;
        }

        public static ViewVisibility ToViewVisibility(this ViewStates state)
        {
            switch (state)
            {
                case ViewStates.Gone:
                    // Android's Gone means "Not shown, not considered for layout."
                    return ViewVisibility.Collapsed;
                case ViewStates.Invisible:
                    // Invisibile means "Not shown, considered for layout."
                    return ViewVisibility.Hidden;
                case ViewStates.Visible:
                    return ViewVisibility.Visible;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(state),
                        state,
                        "Don't know how to convert given ViewState to ViewVisibility.");
            }
        }

        public static string TrimStart(this string source, string trimString)
        {
            string trimmed = source;
            if (source.StartsWith(trimString))
                trimmed = trimmed.Substring(trimString.Length);

            return trimmed;
        }

        public static string TrimId(this string source)
        {
            var idOffset = source.IndexOf(":id/");
            if (idOffset > 0)
                return source.Substring(idOffset + 1);

            return source;
        }

    }
}
