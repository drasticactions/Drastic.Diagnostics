using Drastic.Diagnostics.Messages;
using Drastic.Tempest;
using Microsoft.Extensions.Logging;

namespace Drastic.Diagnostics.Client
{
    public abstract class BaseClient
        : TempestClient
    {
        private readonly ILogger? logger;
        private ClientType type;
        private Guid id;
        private string name;

        public BaseClient(ClientType type, IClientConnection connection, string name = "", ILogger? logger = default)
            : base(connection, MessageTypes.Reliable)
        {
            if (type == ClientType.Unknown)
            {
                throw new ArgumentException(nameof(type));
            }

            this.name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
            this.id = Guid.NewGuid();
            this.type = type;
            this.logger = logger;
            this.Connected += this.DiagnosticsClient_Connected;
            this.Disconnected += this.DiagnosticsClient_Disconnected;
            this.RegisterMessageHandler<TestRequestMessage>(this.OnTestRequest);
            this.RegisterMessageHandler<TestResponseMessage>(this.OnTestResponse);
        }

        public string Id => $"{this.name}-{this.id}";

        internal ILogger? Logger => this.logger;

        private void OnTestResponse(MessageEventArgs<TestResponseMessage> args)
        {
            this.logger?.LogInformation(args.Message.ToString());
        }

        private void OnTestRequest(MessageEventArgs<TestRequestMessage> args)
        {
            this.logger?.LogInformation(args.Message.ToString());

            this.SendMessageAsync(new TestResponseMessage());
        }

        private void DiagnosticsClient_Disconnected(object? sender, ClientDisconnectedEventArgs e)
        {
            this.logger?.LogInformation($"Disconnect: {e.Reason}");
        }

        private void DiagnosticsClient_Connected(object? sender, ClientConnectionEventArgs e)
        {
            this.logger?.LogInformation($"Connect");
        }
    }
}
