// <copyright file="AndroidAppClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Drastic.Diagnostics.Client.Android.Remote;
using Drastic.Diagnostics.Inspection;
using Drastic.Diagnostics.Remote;
using Drastic.Tempest;
using Microsoft.Extensions.Logging;

using AG = Android.Graphics;

namespace Drastic.Diagnostics.Client.Android
{
    public class AndroidAppClient
        : AppClient, IViewHierarchyHandler
    {
        public IActivityTracker ActivityTracker { get; internal set; }
        internal int ContentId { get; }

        public AndroidAppClient(IActivityTracker tracker, Protocol protocol, int contentId = -1, string name = "", ILogger? logger = null)
            : base(protocol, name, logger)
        {
            this.ActivityTracker = tracker;
            this.ContentId = contentId;

            var displaySize = GetRealSize(Application
               .Context
               .GetSystemService(global::Android.Content.Context.WindowService)
               .JavaCast<IWindowManager>()?
               .DefaultDisplay);

            ViewHierarchyHandlerManager.AddViewHierarchyHandler("Android", this);
        }

        public override InspectView? GetVisualTree(string hierarchyKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Equivalent of Display.GetRealSize (introduced in API 17), except this version works as far back as
        /// API 14.
        /// </summary>
        static AG.Point GetRealSize(Display? display)
        {
            var realSize = new AG.Point();

            if (display is null)
                return realSize;

            var klassDisplay = JNIEnv.FindClass("android/view/Display");
            var displayHandle = JNIEnv.ToJniHandle(display);
            try
            {
                // If the OS is running Jelly Bean (API 17), we can call Display.GetRealSize via JNI
                if ((int)Build.VERSION.SdkInt >= 17/*BuildVersionCodes.JellyBeanMr1*/)
                {
                    var getRealSizeMethodId = JNIEnv.GetMethodID(
                        klassDisplay,
                        "getRealSize",
                        "(Landroid/graphics/Point;)V");

                    JNIEnv.CallVoidMethod(
                        displayHandle, getRealSizeMethodId, new JValue(realSize));
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                {
                    // Otherwise, this OS is older. As long as it's API 14-16, these private
                    // methods can get the real size.
                    var rawHeightMethodId = JNIEnv.GetMethodID(
                        klassDisplay,
                        "getRawHeight",
                        "()I");
                    var rawWidthMethodId = JNIEnv.GetMethodID(
                        klassDisplay,
                        "getRawWidth",
                        "()I");

                    var height = JNIEnv.CallIntMethod(displayHandle, rawHeightMethodId);
                    var width = JNIEnv.CallIntMethod(displayHandle, rawWidthMethodId);

                    realSize = new AG.Point(width, height);
                }
                else
                {
                    // Just return something for API < 14
                    display.GetSize(realSize);
                }
            }
            finally
            {
                JNIEnv.DeleteGlobalRef(klassDisplay);
            }

            return realSize;
        }

        static View? GetFrontmostChildAt(ViewGroup viewGroup, int x, int y)
        {
            var locArray = new int[2];

            for (int i = viewGroup.ChildCount - 1; i >= 0; i--)
            {
                var child = viewGroup.GetChildAt(i);

                child?.GetLocationOnScreen(locArray);
                var frame = new AG.Rect(
                    locArray[0],
                    locArray[1],
                    locArray[0] + (child?.Width ?? 0),
                    locArray[1] + (child?.Height ?? 0));

                if (!frame.Contains(x, y))
                    continue;

                var childGroup = child as ViewGroup;
                if (childGroup != null)
                {
                    var grandChild = GetFrontmostChildAt(childGroup, x, y);
                    if (grandChild != null)
                        return grandChild;
                }

                return child;
            }

            return null;
        }

        static View? GetViewAt(Activity activity, double x, double y)
        {
            var rootLayout = activity?.Window?.DecorView?.RootView as ViewGroup;
            if (rootLayout == null)
                return null;

            return GetFrontmostChildAt(rootLayout, (int)x, (int)y);
        }

        string GetApplicationName()
        {
            var context = Application.Context;
            var packageManager = context.PackageManager;
            ApplicationInfo? applicationInfo = null;
            try
            {
                applicationInfo = packageManager?.GetApplicationInfo(context.ApplicationInfo?.PackageName ?? string.Empty, PackageInfoFlags.Configurations);
            }
            catch
            {
            }

            return (applicationInfo != null && packageManager != null ? packageManager.GetApplicationLabel(applicationInfo) : "Unknown");
        }

        internal Activity? GetTopActivity()
        {
            return ActivityTracker?.StartedActivities?.LastOrDefault();
        }

        public bool TryGetRepresentedView(object view, bool withSubviews, out IInspectView? representedView)
        {
            var androidView = view as View;
            if (androidView != null)
            {
                representedView = new AndroidInspectView(androidView, withSubviews);
                return true;
            }

            representedView = null;
            return false;
        }
    }
}
