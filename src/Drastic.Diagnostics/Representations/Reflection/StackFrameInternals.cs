// <copyright file="StackFrameInternals.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;
using SDStackFrame = global::System.Diagnostics.StackFrame;
using SDStackTrace = global::System.Diagnostics.StackTrace;

namespace Drastic.Diagnostics.Representations.Reflection
{
    static class StackTraceInternals
    {
        static readonly FieldInfo? capturedTraces;
        static readonly MethodInfo? getInternalMethodName;
        static readonly MethodInfo?  getMethodAddress;
        static readonly MethodInfo? getMethodIndex;

        static StackTraceInternals ()
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            capturedTraces = typeof(SDStackTrace).GetField ("captured_traces", bindingFlags);

            var type = typeof(SDStackFrame);
            getInternalMethodName = type.GetMethod ("GetInternalMethodName", bindingFlags);
            getMethodAddress = type.GetMethod ("GetMethodAddress", bindingFlags);
            getMethodIndex = type.GetMethod ("GetMethodIndex", bindingFlags);
        }

        public static SDStackTrace[]? GetCapturedTraces (this SDStackTrace trace)
        {
            return capturedTraces?.GetValue (trace) as SDStackTrace[];
        }

        public static string? GetInternalMethodName (this SDStackFrame frame)
        {
            return getInternalMethodName?.Invoke (frame, null) as string;
        }

        public static long GetMethodAddress (this SDStackFrame frame)
        {
            if (getMethodAddress == null)
            {
                return 0;
            }

            return getMethodAddress.Invoke (frame, null) as long? ?? 0;
        }

        public static uint GetMethodIndex (this SDStackFrame frame)
        {
            if (getMethodIndex == null)
            {
                return 0xffffff;
            }

            return getMethodIndex.Invoke (frame, null) as uint? ??  0xffffff;
        }
    }
}