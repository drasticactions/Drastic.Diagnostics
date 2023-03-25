// <copyright file="IViewHierarchyHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Inspection
{
    public interface IViewHierarchyHandler
    {
        bool TryGetRepresentedView(object view, bool withSubviews, out IInspectView? representedView);
    }
}
