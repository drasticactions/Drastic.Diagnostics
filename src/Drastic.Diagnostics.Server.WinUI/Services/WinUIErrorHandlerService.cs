// <copyright file="WinUIErrorHandlerService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Services;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Server.WinUI.Services
{
    /// <summary>
    /// WinUI Error handler Service.
    /// </summary>
    public class WinUIErrorHandlerService : IErrorHandlerService
    {
        private readonly Serilog.ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinUIErrorHandlerService"/> class.
        /// </summary>
        /// <param name="logLocation">Location of logs.</param>
        public WinUIErrorHandlerService(Serilog.ILogger logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc/>
        public void HandleError(Exception ex)
        {
            this._logger.Error(ex, nameof(IErrorHandlerService));
        }
    }
}
