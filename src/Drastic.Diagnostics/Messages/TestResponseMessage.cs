// <copyright file="TestResponseMessage.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;

namespace Drastic.Diagnostics.Messages
{
    public class TestResponseMessage
           : DiagnosticMessage
    {
        public TestResponseMessage()
            : base(DiagnosticsMessageType.TestResponse)
        {
        }

        /// <inheritdoc/>
        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            base.ReadPayload(context, reader);
        }

        /// <inheritdoc/>
        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            base.WritePayload(context, writer);
        }
    }
}
