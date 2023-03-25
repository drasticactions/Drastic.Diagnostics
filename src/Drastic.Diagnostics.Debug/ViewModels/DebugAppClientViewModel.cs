// <copyright file="DebugAppClientViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Client;
using Drastic.Tempest.Providers.Network;
using Drastic.Tools;
using Drastic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Debug.ViewModels
{
    public class DebugAppClientViewModel : DebugViewModel
    {
        public DebugAppClientViewModel(IServiceProvider services)
            : base(services)
        {
            var loggerFactory = services.GetService<ILoggerProvider>();
            if (loggerFactory is not null)
            {
                this.Logger = loggerFactory.CreateLogger("DebugAppClient");
            }
        }

        public override BaseClient SetupClient()
        {
            var client = this.appClientFactory.GenerateAppClient(DiagnosticsProtocol.Instance, "DebugApp", this.Logger);
            client.Connected += this.Client_Connected;
            client.Disconnected += this.Client_Disconnected;
            return client;
        }
    }
}
