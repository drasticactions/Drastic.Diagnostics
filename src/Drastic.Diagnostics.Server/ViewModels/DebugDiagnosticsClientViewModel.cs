// <copyright file="DebugClientViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client;
using Drastic.Diagnostics.Messages;
using Drastic.Tempest.Providers.Network;
using Drastic.Tools;
using Drastic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Server.ViewModels
{
    public class DebugDiagnosticsClientViewModel : DebugViewModel
    {
        public DebugDiagnosticsClientViewModel(IServiceProvider services)
           : base(services)
        {
            var loggerFactory = services.GetService<ILoggerProvider>();
            if (loggerFactory is not null)
            {
                this.Logger = loggerFactory.CreateLogger("DebugDiagnosticsClient");
            }

            this.AppClientDiscoveryRequestCommand = new AsyncCommand(this.SendAppClientDiscoveryRequest, () => this.IsConnected, this.Dispatcher, this.ErrorHandler);
        }

        public AsyncCommand AppClientDiscoveryRequestCommand { get; }

        public override BaseClient SetupClient()
        {
            var client = new DiagnosticsClient(DiagnosticsProtocol.Instance, "Diagnostics", this.Logger);

            client.Connected += this.Client_Connected;
            client.Disconnected += this.Client_Disconnected;
            return client;
        }

        private Task SendAppClientDiscoveryRequest()
        {
            return this.client?.SendMessageAsync(new AppClientDiscoveryRequestMessage()) ?? Task.CompletedTask;
        }

        public override void RaiseCanExecuteChanged()
        {
            this.AppClientDiscoveryRequestCommand.RaiseCanExecuteChanged();
            base.RaiseCanExecuteChanged();
        }
    }
}
