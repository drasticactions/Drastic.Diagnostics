// <copyright file="AndroidAppClientFactory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client.Android
{
    public class AndroidAppClientFactory : IAppClientFactory
    {
        public AppClient GenerateAppClient(Protocol protocol, string name = "", ILogger? logger = null)
            => new AndroidAppClient(new ActivityTrackerWrapper(), protocol, global::Android.Resource.Id.Content, name, logger);
    }
}
