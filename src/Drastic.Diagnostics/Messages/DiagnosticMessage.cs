// <copyright file="DiagnosticMessage.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;

namespace Drastic.Diagnostics.Messages
{
    public abstract class DiagnosticMessage
           : Message
    {
        protected DiagnosticMessage(DiagnosticsMessageType type)
            : base(DiagnosticsProtocol.Instance, (ushort)type)
        {
        }
    }
}
