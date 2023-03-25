// <copyright file="DebugDiagnosticsClientViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using Drastic.Diagnostics.Client;
using Drastic.Diagnostics.Messages;
using Drastic.Tempest;
using Drastic.Tempest.Providers.Network;
using Drastic.Tools;
using Drastic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Debug.ViewModels
{
    public class DebugDiagnosticsClientViewModel : DebugViewModel
    {
        private string? selectedAppClient;

        public DebugDiagnosticsClientViewModel(IServiceProvider services)
           : base(services)
        {
            var loggerFactory = services.GetService<ILoggerProvider>();
            if (loggerFactory is not null)
            {
                this.Logger = loggerFactory.CreateLogger("DebugDiagnosticsClient");
            }

            this.TestInspectViewRequestCommand = new AsyncCommand(this.SendTestInspectViewRequestCommand, () => this.IsConnected, this.Dispatcher, this.ErrorHandler);
            this.AppClientDiscoveryRequestCommand = new AsyncCommand(this.SendAppClientDiscoveryRequest, () => this.IsConnected, this.Dispatcher, this.ErrorHandler);

            this.SelectedAppClient = this.AppClients.First();
        }

        public ObservableCollection<string> AppClients { get; } = new ObservableCollection<string>() { "All" };

        public string? SelectedAppClient {
            get {
                return this.selectedAppClient;
            }

            set {
                this.SetProperty(ref this.selectedAppClient, value);
                this.RaiseCanExecuteChanged();
            }
        }

        public AsyncCommand TestInspectViewRequestCommand { get; }

        public AsyncCommand AppClientDiscoveryRequestCommand { get; }

        public override BaseClient SetupClient()
        {
            var client = new DiagnosticsClient(DiagnosticsProtocol.Instance, "Diagnostics", this.Logger);

            client.Connected += this.Client_Connected;
            client.Disconnected += this.Client_Disconnected;
            client.RegisterMessageHandler<AppClientConnectMessage>(this.OnAppClientConnect);
            client.RegisterMessageHandler<AppClientDisconnectMessage>(this.OnAppClientDisconnect);
            client.RegisterMessageHandler<AppClientDiscoveryResponseMessage>(this.OnAppClientDiscoveryResponse);
            return client;
        }

        private void OnAppClientDiscoveryResponse(MessageEventArgs<AppClientDiscoveryResponseMessage> args)
        {
            this.Dispatcher.Dispatch(() =>
            {
                this.AppClients.Clear();
                this.AppClients.Add("All");

                foreach (var appClient in args.Message.AppClientIds)
                {
                    this.AppClients.Add(appClient);
                }

                this.SelectedAppClient = this.AppClients.First();
            });
        }

        private Task SendTestInspectViewRequestCommand()
        {
            return this.client?.SendMessageAsync(new TestInspectViewRequest()) ?? Task.CompletedTask;
        }

        private Task SendAppClientDiscoveryRequest()
        {
            return this.client?.SendMessageAsync(new AppClientDiscoveryRequestMessage()) ?? Task.CompletedTask;
        }

        public override void RaiseCanExecuteChanged()
        {
            this.AppClientDiscoveryRequestCommand.RaiseCanExecuteChanged();
            this.TestInspectViewRequestCommand.RaiseCanExecuteChanged();
            base.RaiseCanExecuteChanged();
        }

        private void OnAppClientConnect(MessageEventArgs<AppClientConnectMessage> args)
        {
            this.Dispatcher.Dispatch(() =>
            {
                if (!this.AppClients.Contains(args.Message.AppClientId))
                {
                    this.AppClients.Add(args.Message.AppClientId);
                }
            });
        }

        private void OnAppClientDisconnect(MessageEventArgs<AppClientDisconnectMessage> args)
        {
            this.Dispatcher.Dispatch(() =>
            {
                if (this.AppClients.Contains(args.Message.AppClientId))
                {
                    this.AppClients.Remove(args.Message.AppClientId);
                }

                if (this.SelectedAppClient == args.Message.AppClientId)
                {
                    this.SelectedAppClient = this.AppClients.FirstOrDefault();
                }
            });
        }

        internal override void Client_Disconnected(object? sender, ClientDisconnectedEventArgs e)
        {
            base.Client_Disconnected(sender, e);

            this.Dispatcher.Dispatch(() =>
            {
                this.AppClients.Clear();
                this.AppClients.Add("All");
            });
        }

        internal override void Client_Connected(object? sender, ClientConnectionEventArgs e)
        {
            base.Client_Connected(sender, e);

            this.SendAppClientDiscoveryRequest().FireAndForgetSafeAsync();
        }
    }
}
