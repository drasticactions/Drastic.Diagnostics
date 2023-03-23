// <copyright file="DiagnosticsServer.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Server
{
    public class DiagnosticsServer : TempestServer
    {
        private readonly ILogger? logger;

        public DiagnosticsServer(IConnectionProvider provider, ILogger? logger = default)
            : base(provider, MessageTypes.Reliable)
        {
            this.logger = logger;
        }

        private readonly List<IConnection> connections = new List<IConnection>();

        /// <inheritdoc/>
        protected override void OnConnectionMade(object sender, ConnectionMadeEventArgs e)
        {
            lock (this.connections)
            {
                this.connections.Add(e.Connection);
            }

            this.logger?.LogInformation(e.ToString());
            base.OnConnectionMade(sender, e);
        }

        /// <inheritdoc/>
        protected override void OnConnectionDisconnected(object sender, DisconnectedEventArgs e)
        {
            lock (this.connections)
            {
                this.connections.Remove(e.Connection);
            }

            this.logger?.LogInformation($"Disconnect");
            base.OnConnectionDisconnected(sender, e);
        }
    }
}
