// <copyright file="BaseClientExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Messages;

namespace Drastic.Diagnostics.Client
{
    public static class BaseClientExtensions
    {
        public static Task SendMessageAsync(this BaseClient client, DiagnosticMessage message)
        {
            ArgumentNullException.ThrowIfNull(nameof(message));

            message.Id = client.Id;
            return client.Connection.SendAsync(message);
        }
    }
}
