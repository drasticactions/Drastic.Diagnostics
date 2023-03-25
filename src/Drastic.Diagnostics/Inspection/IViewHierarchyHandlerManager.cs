using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Inspection
{
    internal interface IViewHierarchyHandlerManager
    {
        IReadOnlyList<string> AvailableHierarchyKinds { get; }
        void AddViewHierarchyHandler(string hierarchyKind, IViewHierarchyHandler handler);
    }
}
