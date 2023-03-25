// <copyright file="WinUIAppClientFactory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client.WinUI
{
    public class WinUIAppClientFactory : IAppClientFactory
    {
        public AppClient GenerateAppClient(Protocol protocol, string name = "", ILogger? logger = null)
            => new WinUIAppClient(protocol, name, logger);
    }
}
