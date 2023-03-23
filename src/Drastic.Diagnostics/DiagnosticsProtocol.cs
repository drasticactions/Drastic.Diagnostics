// <copyright file="DiagnosticsProtocol.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;

namespace Drastic.Diagnostics
{
    public static class DiagnosticsProtocol
    {
        public static readonly Protocol Instance = new Protocol(42, 1);

        static DiagnosticsProtocol()
        {
            Instance.Discover();
        }
    }
}
