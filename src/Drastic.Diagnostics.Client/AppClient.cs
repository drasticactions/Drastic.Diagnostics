// <copyright file="AppClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Messages;
using Drastic.Tempest;
using Drastic.Tempest.Providers.Network;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client
{
    public class AppClient
        : BaseClient
    {
        public AppClient(Protocol protocol, string name = "", ILogger? logger = default)
            : base(ClientType.AppClient, new NetworkClientConnection(protocol), name, logger)
        {
            this.Connected += this.AppClient_Connected;
        }

        private void AppClient_Connected(object? sender, ClientConnectionEventArgs e)
        {
            // Tell the server our ID.
            this.SendMessageAsync(new ClientRegistrationMessage());
        }
    }
}
