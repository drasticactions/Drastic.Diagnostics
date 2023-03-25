// <copyright file="NativeHelper.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Representations.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics
{
    class NativeHelper
    {
        struct Disposable : IDisposable
        {
            void IDisposable.Dispose() { }
        }

        readonly static IDisposable disposable = new Disposable();

        static NativeHelper? sharedInstance;
        public static NativeHelper SharedInstance {
            get {
                if (sharedInstance == null)
                    sharedInstance = new NativeHelper();
                return sharedInstance;
            }
        }

        /// <summary>
        /// Replace default SharedInstance with this instance.
        /// </summary>
        public void Initialize()
        {
            sharedInstance = this;
        }

        public virtual IDisposable TrapNativeExceptions()
            => disposable;

        /// <summary>
        /// Check that a CLR property can be safely invoked on a target object that
        /// may wrap a native object.
        /// </summary>
        /// <returns>Null if invocation is safe, an error string suitable for an
        /// exception message otherwise.</returns>
        public virtual string? CheckProperty(
            PropertyInfo property,
            object target,
            RepresentedType? declaringType)
            => null;
    }
}
