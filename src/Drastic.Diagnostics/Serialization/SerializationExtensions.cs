﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Serialization
{
    static class SerializationExtensions
    {
        public static T? GetValue<T>(this SerializationInfo info, string name)
        {
            return (T?)info.GetValue(name, typeof(T));
        }
    }
}
