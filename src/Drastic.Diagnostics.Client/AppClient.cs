// <copyright file="AppClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Inspection;
using Drastic.Diagnostics.Messages;
using Drastic.Diagnostics.Remote;
using Drastic.Tempest;
using Drastic.Tempest.Providers.Network;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client
{
    public abstract class AppClient
        : BaseClient
    {
        public AppClient(Protocol protocol, string name = "", ILogger? logger = default)
            : base(ClientType.AppClient, new NetworkClientConnection(protocol), name, logger)
        {
            this.Connected += this.AppClient_Connected;
            this.RegisterMessageHandler<TestInspectViewRequest>(this.OnTestInspectViewRequest);
        }

        private void OnTestInspectViewRequest(MessageEventArgs<TestInspectViewRequest> args)
        {
            var result = this.GetVisualTree("WinUI");
        }

        public ViewHierarchyHandlerManager ViewHierarchyHandlerManager { get; }
            = new ViewHierarchyHandlerManager();

        public abstract InspectView? GetVisualTree(string hierarchyKind);

        public abstract bool TryGetRepresentedView(object view, bool withSubviews, out IInspectView? representedView);

        private void AppClient_Connected(object? sender, ClientConnectionEventArgs e)
        {
            // Tell the server our ID.
            this.SendMessageAsync(new ClientRegistrationMessage());
        }
    }
}
