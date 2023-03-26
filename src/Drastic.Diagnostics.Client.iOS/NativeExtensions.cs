// <copyright file="NativeExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UIKit;
using ObjCRuntime;
using CoreGraphics;
using CoreAnimation;
using Drastic.Diagnostics.Remote;

namespace Drastic.Diagnostics.Client.iOS
{
    static class NativeExtensions
    {
        [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
        static extern bool bool_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

        [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
        static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

        static class Selectors
        {
            public static readonly Selector statusBarWindow = new Selector("statusBarWindow");
            public static readonly Selector respondsToSelector = new Selector("respondsToSelector:");
        }

        public static UIWindow? GetStatusBarWindow(this UIApplication app)
        {
            if (!bool_objc_msgSend_IntPtr(app.Handle,
                Selectors.respondsToSelector.Handle,
                Selectors.statusBarWindow.Handle))
                return null;

            var ptr = IntPtr_objc_msgSend(app.Handle, Selectors.statusBarWindow.Handle);
            return ptr != IntPtr.Zero ? ObjCRuntime.Runtime.GetNSObject(ptr) as UIWindow : null;
        }

        public static void TryHideStatusClockView(this UIApplication app)
        {
            var statusBarWindow = app.GetStatusBarWindow();
            if (statusBarWindow == null)
                return;

            var clockView = statusBarWindow.FindSubview(
                "UIStatusBar",
                "UIStatusBarForegroundView",
                "UIStatusBarTimeItemView"
            );

            if (clockView != null)
                clockView.Hidden = true;
        }

        public static UIView? FindSubview(this UIView view, params string[] classNames)
        {
            return FindSubview(view, ((IEnumerable<string>)classNames).GetEnumerator());
        }

        static UIView? FindSubview(UIView view, IEnumerator<string> classNames)
        {
            if (!classNames.MoveNext())
                return view;

            foreach (var subview in view.Subviews)
            {
                if (subview.ToString().StartsWith("<" + classNames.Current + ":", StringComparison.Ordinal))
                    return FindSubview(subview, classNames);
            }

            return null;
        }

        public static UIView? FindLayerView(this UIView root, CALayer layer)
        {
            var memo = new Dictionary<IntPtr, UIView>();
            foreach (var view in root.TraverseTree(v => v.Subviews))
            {
                if (view.Layer == layer)
                    return view;

                if (view.Layer != null)
                    memo[view.Layer.Handle] = view;
            }

            var superLayer = layer.SuperLayer;
            while (superLayer != null)
            {
                UIView? viewParent;

                if (memo.TryGetValue(superLayer.Handle, out viewParent))
                    return viewParent;

                superLayer = superLayer.SuperLayer;
            }
            return null;
        }
    }
}
