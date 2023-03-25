using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Inspection
{
    public interface IInspectView
    {
        long X { get; set; }
        long Y { get; set; }
        long Width { get; set; }
        long Height { get; set; }
        void AddSubview(IInspectView subView);
    }
}
