// <copyright file="ItemsRepeaterLogBroker.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Serilog.Events;
using Serilog.Sinks.WinUi3;
using Serilog.Sinks.WinUi3.LogViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Server.WinUI
{
    public class LogBroker : IWinUi3LogBroker
    {
        private readonly ILogViewModelBuilder _logViewModelBuilder;

        public LogBroker(
            DispatcherQueue dispatcher,
            ILogViewModelBuilder logViewModelBuilder)
        {
            _logViewModelBuilder = logViewModelBuilder;

            DispatcherQueue = dispatcher;
            AddLogEvent = logEvent => Logs.Add(_logViewModelBuilder.Build(logEvent));
            Logs.CollectionChanged += ((sender, e) =>
            {
            });
        }

        public Action<LogEvent> AddLogEvent { get; }
        public DispatcherQueue DispatcherQueue { get; }
        public bool IsAutoScrollOn { get; set; }

        public ObservableCollection<ILogViewModel> Logs { get; init; } = new();
    }
}
