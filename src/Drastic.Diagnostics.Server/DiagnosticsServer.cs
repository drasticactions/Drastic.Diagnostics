// <copyright file="DiagnosticsServer.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Messages;
using Drastic.Diagnostics.Server.Tools;
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
            this.RegisterMessageHandler<TestRequestMessage>(this.OnTestRequestMessage);
            this.RegisterMessageHandler<TestResponseMessage>(this.OnTestResponseMessage);
            this.RegisterMessageHandler<ClientRegistrationMessage>(this.OnClientRegistration);
            this.RegisterMessageHandler<DiagnosticsRegistrationMessage>(this.OnDiagnosticsRegistration);
            this.RegisterMessageHandler<AppClientDiscoveryRequestMessage>(this.OnAppClientDiscoveryRequest);
            this.RegisterMessageHandler<TestInspectViewRequest>(this.OnTestInspectViewRequest);
        }

        private void OnTestInspectViewRequest(MessageEventArgs<TestInspectViewRequest> args)
        {
            // TODO: Maybe handle AppClient here?
            this.SendToAppClients(args.Message);
        }

        private void OnAppClientDiscoveryRequest(MessageEventArgs<AppClientDiscoveryRequestMessage> args)
        {
            args.Connection.SendAsync(new AppClientDiscoveryResponseMessage() { AppClientIds = this.clientConnections.Select(n => n.Key) });
        }

        private void OnDiagnosticsRegistration(MessageEventArgs<DiagnosticsRegistrationMessage> args)
        {
            var clientMessage = args.Message;
            if (this.diagnosticsConnections.ContainsKey(clientMessage.Id))
            {
                this.logger?.LogWarning($"Diagnostics ID {clientMessage.Id} already registered");
                return;
            }

            lock (this.diagnosticsConnections)
            {
                this.diagnosticsConnections.Add(clientMessage.Id, args.Connection);
            }

            this.logger?.LogInformation($"Diagnostics ID {clientMessage.Id} registered");
        }

        private void OnClientRegistration(MessageEventArgs<ClientRegistrationMessage> args)
        {
            var clientMessage = args.Message;
            if (this.clientConnections.ContainsKey(clientMessage.Id))
            {
                this.logger?.LogWarning($"Client ID {clientMessage.Id} already registered");
                return;
            }

            lock (this.clientConnections)
            {
                this.clientConnections.Add(clientMessage.Id, args.Connection);
                this.SendToDiagnosticsClients(new AppClientConnectMessage() { AppClientId = clientMessage.Id });
            }

            this.logger?.LogInformation($"Client ID {clientMessage.Id} registered");
        }

        private void OnTestResponseMessage(MessageEventArgs<TestResponseMessage> args)
        {
            this.logger?.LogInformation(args.Message.ToString());

            this.SendToAll(args.Message, args.Connection);
        }

        private void OnTestRequestMessage(MessageEventArgs<TestRequestMessage> args)
        {
            this.logger?.LogInformation(args.Message.ToString());

            this.SendToAll(args.Message, args.Connection);
        }

        private void SendToDiagnosticsClients(DiagnosticMessage message, IConnection? ogSender = default)
        {
            lock (this.diagnosticsConnections)
            {
                var list = this.diagnosticsConnections.Where(n => n.Value != ogSender);
                foreach (var connection in list)
                {
                    connection.Value.SendAsync(message);
                }
            }
        }

        private void SendToAppClients(DiagnosticMessage message, IConnection? ogSender = default)
        {
            lock (this.clientConnections)
            {
                var list = this.clientConnections.Where(n => n.Value != ogSender);
                foreach (var connection in list)
                {
                    connection.Value.SendAsync(message);
                }
            }
        }

        private void SendToAll(DiagnosticMessage message, IConnection? ogSender = default)
        {
            lock (this.connections)
            {
                var list = this.connections.Where(n => n != ogSender);
                foreach (var connection in list) {
                    connection.SendAsync(message);
                }
            }
        }

        private readonly Dictionary<string, IConnection> clientConnections = new Dictionary<string, IConnection>();

        private readonly Dictionary<string, IConnection> diagnosticsConnections = new Dictionary<string, IConnection>();

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

            lock (this.clientConnections)
            {
                if (this.clientConnections.TryGetKey(e.Connection, out var connection))
                {
                    System.Diagnostics.Debug.Assert(connection != null, "Connection key should not be null");
                    this.clientConnections.Remove(connection!);
                    this.SendToDiagnosticsClients(new AppClientDisconnectMessage() { AppClientId = connection });
                }
            }

            lock (this.diagnosticsConnections)
            {
                if (this.diagnosticsConnections.TryGetKey(e.Connection, out var connection))
                {
                    System.Diagnostics.Debug.Assert(connection != null, "Connection key should not be null");
                    this.diagnosticsConnections.Remove(connection!);
                }
            }

            this.logger?.LogInformation($"Disconnect");
            base.OnConnectionDisconnected(sender, e);
        }
    }
}
