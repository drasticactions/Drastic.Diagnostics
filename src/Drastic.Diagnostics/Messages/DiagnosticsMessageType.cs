// <copyright file="DiagnosticsMessageType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.Diagnostics.Messages
{
    public enum DiagnosticsMessageType
        : ushort
    {
        TestRequest = 1,
        TestResponse = 2,
        ClientRegistration = 3,
        DiagnosticsRegistration = 4,
        AppClientDiscoveryRequest = 5,
        AppClientDiscoveryResponse = 6,
        AppClientDisconnect = 7,
        AppClientConnect = 8,
        TestInspectViewRequest = 9,
        TestInspectViewResponse = 10,
    }
}
