// <copyright file="AppClientDiscoveryResponseMessage.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Transport;
using Drastic.Tempest;

namespace Drastic.Diagnostics.Messages
{
    public class AppClientDiscoveryResponseMessage
         : DiagnosticMessage
    {
        public AppClientDiscoveryResponseMessage()
            : base(DiagnosticsMessageType.AppClientDiscoveryResponse)
        {
        }

        public IEnumerable<string> AppClientIds { get; internal set; } = new List<string>();

        /// <inheritdoc/>
        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            this.AppClientIds = reader.ReadEnumerable<string>(context, new StringArraySerializer());
            base.ReadPayload(context, reader);
        }

        /// <inheritdoc/>
        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteEnumerable<string>(context, new StringArraySerializer(), this.AppClientIds);
            base.WritePayload(context, writer);
        }
    }
}
