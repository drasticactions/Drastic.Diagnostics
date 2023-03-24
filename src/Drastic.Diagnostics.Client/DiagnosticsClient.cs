// <copyright file="DiagnosticsClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Messages;
using Drastic.Tempest;
using Drastic.Tempest.Providers.Network;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client
{
    public class DiagnosticsClient
        : BaseClient
    {
        public DiagnosticsClient(Protocol protocol, string name = "", ILogger? logger = default)
            : base(ClientType.Diagnostics, new NetworkClientConnection(protocol), name, logger)
        {
            this.Connected += this.DiagnosticsClient_Connected;
            this.RegisterMessageHandler<AppClientDiscoveryResponseMessage>(this.OnAppClientDiscoveryResponse);
        }

        private void OnAppClientDiscoveryResponse(MessageEventArgs<AppClientDiscoveryResponseMessage> args)
        {
            this.Logger?.LogInformation(args.Message.ToString());
        }

        private void DiagnosticsClient_Connected(object? sender, ClientConnectionEventArgs e)
        {
            // Tell the server our ID.
            this.SendMessageAsync(new DiagnosticsRegistrationMessage());
        }
    }
}
