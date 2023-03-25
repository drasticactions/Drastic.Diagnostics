// <copyright file="TaskCompletionSource.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics
{
    sealed class TaskCompletionSource : TaskCompletionSource<TaskCompletionSource.Void>
    {
        internal struct Void
        {
        }

        public void SetResult() => SetResult(new Void());
    }
}
