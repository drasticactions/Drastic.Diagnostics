// <copyright file="ViewHierarchyHandlerManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Remote;

namespace Drastic.Diagnostics.Inspection
{
    public class ViewHierarchyHandlerManager : IViewHierarchyHandlerManager
    {
        const string TAG = nameof(ViewHierarchyHandlerManager);

        OrderedMapOfList<string, IViewHierarchyHandler>? handlers;

        public void AddViewHierarchyHandler(string hierarchyKind, IViewHierarchyHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            MainThread.Ensure();

            if (handlers == null)
                handlers = new OrderedMapOfList<string, IViewHierarchyHandler>();

            handlers.Add(hierarchyKind, handler);
        }

        public IReadOnlyList<string> AvailableHierarchyKinds
            => handlers?.Keys ?? Array.Empty<string>();

        public InspectView? GetView(object? view, string hierarchyKind, bool withSubviews = true)
        {
            if (view == null || hierarchyKind == null || handlers == null)
                return null;

            IReadOnlyList<IViewHierarchyHandler> handlersForKind;
            if (!handlers.TryGetValue(hierarchyKind, out handlersForKind))
                return null;

            foreach (var handler in handlersForKind)
            {
                IInspectView? representedView;
                if (handler.TryGetRepresentedView(view, withSubviews, out representedView))
                    return representedView as InspectView;
            }

            return null;
        }
    }
}
