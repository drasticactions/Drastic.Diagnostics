// <copyright file="AppClientConnectMessage.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;

namespace Drastic.Diagnostics.Messages
{
    public class AppClientConnectMessage
        : DiagnosticMessage
    {
        public AppClientConnectMessage()
            : base(DiagnosticsMessageType.AppClientConnect)
        {
            this.AppClientId = "Unknown";
        }

        public string AppClientId { get; internal set; }

        /// <inheritdoc/>
        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            this.AppClientId = reader.ReadString();
            base.ReadPayload(context, reader);
        }

        /// <inheritdoc/>
        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteString(this.AppClientId);
            base.WritePayload(context, writer);
        }
    }
}
