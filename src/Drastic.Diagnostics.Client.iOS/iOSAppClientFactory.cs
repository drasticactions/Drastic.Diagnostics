// <copyright file="iOSAppClientFactory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client.iOS
{
    public class iOSAppClientFactory : IAppClientFactory
    {
        public AppClient GenerateAppClient(Protocol protocol, string name = "", ILogger? logger = null)
            => new iOSAppClient(protocol, name, logger);
    }
}
