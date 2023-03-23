// <copyright file="DiagnosticsClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Messages;
using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client
{
    public class DiagnosticsClient
        : TempestClient
    {
        private readonly ILogger? logger;

        public DiagnosticsClient(IClientConnection connection, ILogger? logger = default)
            : base(connection, MessageTypes.Reliable)
        {
            this.logger = logger;
            this.Connected += this.DiagnosticsClient_Connected;
            this.Disconnected += this.DiagnosticsClient_Disconnected;
        }

        private void DiagnosticsClient_Disconnected(object? sender, ClientDisconnectedEventArgs e)
        {
            this.logger?.LogInformation($"Disconnect: {e.Reason}");
        }

        private void DiagnosticsClient_Connected(object? sender, ClientConnectionEventArgs e)
        {
            this.logger?.LogInformation($"Connect");
        }
    }
}
