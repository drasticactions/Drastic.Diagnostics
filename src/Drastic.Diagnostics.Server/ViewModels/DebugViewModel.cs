using Drastic.Diagnostics.Client;
using Drastic.Diagnostics.Messages;
using Drastic.Tempest.Providers.Network;
using Drastic.Tools;
using Drastic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Server.ViewModels
{
    public class DebugViewModel : BaseViewModel
    {
        private string? ipAddress;
        private int? port = 8888;
        internal BaseClient? client;

        private bool isValidPort => this.port is not null && (this.port > 0 && this.port <= 65535);

        private bool isValidIp => !string.IsNullOrEmpty(this.IPAddress);

        public DebugViewModel(IServiceProvider services)
            : base(services)
        {
            this.SendTestRequestCommand = new AsyncCommand(this.SendTestRequestAsync, () => this.IsConnected, this.Dispatcher, this.ErrorHandler);
            this.DisconnectFromServerCommand = new AsyncCommand(this.DisconnectFromServerAsync, () => this.IsConnected, this.Dispatcher, this.ErrorHandler);
            this.ConnectToServerCommand = new AsyncCommand(this.ConnectToServerAsync, () => !this.IsConnected && this.isValidPort && this.isValidIp, this.Dispatcher, this.ErrorHandler);
        }

        public ILogger? Logger { get; internal set; }

        public bool IsConnected => this.client != null && this.client.IsConnected;

        public AsyncCommand SendTestRequestCommand { get; }

        public AsyncCommand ConnectToServerCommand { get; }

        public AsyncCommand DisconnectFromServerCommand { get; }

        /// <summary>
        /// Gets or sets the selected port.
        /// </summary>
        public string? IPAddress {
            get {
                return this.ipAddress;
            }

            set {
                this.SetProperty(ref this.ipAddress, value);
                this.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected port.
        /// </summary>
        public int? Port {
            get {
                return this.port;
            }

            set {
                this.SetProperty(ref this.port, value);
                this.RaiseCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public override void RaiseCanExecuteChanged()
        {
            this.SendTestRequestCommand.RaiseCanExecuteChanged();
            this.ConnectToServerCommand.RaiseCanExecuteChanged();
            this.DisconnectFromServerCommand.RaiseCanExecuteChanged();
            this.OnPropertyChanged(nameof(this.IsConnected));
            base.RaiseCanExecuteChanged();
        }

        public virtual BaseClient SetupClient()
        {
            throw new NotImplementedException();
        }

        private async Task ConnectToServerAsync()
        {
            this.client = this.SetupClient();
            var result = await this.client!.ConnectAsync(new Drastic.Tempest.Target(this.ipAddress ?? "127.0.0.1", this.port ?? 8888));
            if (result.Result != Tempest.ConnectionResult.Success)
            {
                throw new Exception(result.Result.ToString());
            }
        }

        private Task SendTestRequestAsync()
        {
            return this.client?.SendMessageAsync(new TestRequestMessage()) ?? Task.CompletedTask;
        }

        private async Task DisconnectFromServerAsync()
        {
            if (this.client is not null)
            {
                await this.client.DisconnectAsync();
            }
        }

        internal virtual void Client_Disconnected(object? sender, Tempest.ClientDisconnectedEventArgs e)
        {
            if (this.client is not null)
            {
                this.client.Connected -= this.Client_Connected;
                this.client.Disconnected -= this.Client_Disconnected;
                this.client = null;
            }

            this.RaiseCanExecuteChanged();
        }

        internal virtual void Client_Connected(object? sender, Tempest.ClientConnectionEventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }
    }
}
