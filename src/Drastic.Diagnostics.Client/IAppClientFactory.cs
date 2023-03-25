// <copyright file="IAppClientFactory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client
{
    public interface IAppClientFactory
    {
        AppClient GenerateAppClient(Protocol protocol, string name = "", ILogger? logger = default);
    }
}
