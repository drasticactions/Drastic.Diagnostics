﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Server.Tools
{
    internal static class DictionaryExtensions
    {
        internal static bool TryGetKey<K, V>(this IDictionary<K, V> instance, V? value, out K? key)
        {
            foreach (var entry in instance)
            {
                if (entry.Value is not null && !entry.Value.Equals(value))
                {
                    continue;
                }

                key = entry.Key;
                return true;
            }

            key = default(K);
            return false;
        }
    }
}
