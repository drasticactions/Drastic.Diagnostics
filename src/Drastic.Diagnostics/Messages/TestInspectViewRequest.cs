// <copyright file="TestInspectViewRequest.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;

namespace Drastic.Diagnostics.Messages
{
    public class TestInspectViewRequest :
        DiagnosticMessage
    {
        public TestInspectViewRequest()
            : base(DiagnosticsMessageType.TestInspectViewRequest)
        {
        }

        public string ToAppClientId { get; internal set; } = string.Empty;

        public bool WithSubviews { get; internal set; } = false;

        /// <inheritdoc/>
        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            this.ToAppClientId = reader.ReadString();
            this.WithSubviews = reader.ReadBool();
            base.ReadPayload(context, reader);
        }

        /// <inheritdoc/>
        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteString(this.ToAppClientId);
            writer.WriteBool(this.WithSubviews);
            base.WritePayload(context, writer);
        }
    }
}
