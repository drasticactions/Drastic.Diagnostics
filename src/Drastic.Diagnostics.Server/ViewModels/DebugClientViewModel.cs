// <copyright file="DebugClientViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client;
using Drastic.Tempest.Providers.Network;
using Drastic.Tools;
using Drastic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Server.ViewModels
{
    public class DebugClientViewModel : BaseViewModel
    {
        private string? ipAddress;
        private int? port = 8888;
        private DiagnosticsClient? client;
        private ILogger? logger;

        private bool isValidPort => this.port is not null && (this.port > 0 && this.port <= 65535);

        private bool isValidIp => !string.IsNullOrEmpty(this.IPAddress);

        public DebugClientViewModel(IServiceProvider services, ILogger? logger = default)
            : base(services)
        {
            var loggerFactory = services.GetService<ILoggerProvider>();
            if (loggerFactory is not null)
            {
                this.logger = loggerFactory.CreateLogger("DebugClient");
            }

            this.DisconnectFromServerCommand = new AsyncCommand(this.DisconnectFromServerAsync, () => this.IsConnected, this.Dispatcher, this.ErrorHandler);
            this.ConnectToServerCommand = new AsyncCommand(this.ConnectToServerAsync, () => !this.IsConnected && this.isValidPort && this.isValidIp, this.Dispatcher, this.ErrorHandler);
        }

        public bool IsConnected => this.client != null && this.client.IsConnected;

        public AsyncCommand ConnectToServerCommand { get; }

        public AsyncCommand DisconnectFromServerCommand { get; }

        /// <summary>
        /// Gets or sets the selected port.
        /// </summary>
        public string? IPAddress
        {
            get
            {
                return this.ipAddress;
            }

            set
            {
                this.SetProperty(ref this.ipAddress, value);
                this.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected port.
        /// </summary>
        public int? Port
        {
            get
            {
                return this.port;
            }

            set
            {
                this.SetProperty(ref this.port, value);
                this.RaiseCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public override void RaiseCanExecuteChanged()
        {
            this.ConnectToServerCommand.RaiseCanExecuteChanged();
            this.DisconnectFromServerCommand.RaiseCanExecuteChanged();
            this.OnPropertyChanged(nameof(this.IsConnected));
            base.RaiseCanExecuteChanged();
        }

        private DiagnosticsClient SetupClient()
        {
            var connection = new NetworkClientConnection(DiagnosticsProtocol.Instance);
            var client = new DiagnosticsClient(connection, this.logger);

            client.Connected += this.Client_Connected;
            client.Disconnected += this.Client_Disconnected;
            return client;
        }

        private async Task ConnectToServerAsync()
        {
            this.client = this.SetupClient();
            var result = await this.client.ConnectAsync(new Drastic.Tempest.Target(this.ipAddress ?? "127.0.0.1", this.port ?? 8888));
            if (result.Result != Tempest.ConnectionResult.Success)
            {
                throw new Exception(result.Result.ToString());
            }
        }

        private async Task DisconnectFromServerAsync()
        {
            if (this.client is not null)
            {
                await this.client.DisconnectAsync();
            }
        }

        private void Client_Disconnected(object? sender, Tempest.ClientDisconnectedEventArgs e)
        {
            if (this.client is not null)
            {
                this.client.Connected -= this.Client_Connected;
                this.client.Disconnected -= this.Client_Disconnected;
                this.client = null;
            }

            this.RaiseCanExecuteChanged();
        }

        private void Client_Connected(object? sender, Tempest.ClientConnectionEventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }
    }
}
