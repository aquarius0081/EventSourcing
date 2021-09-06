using Confluent.Kafka;
using EventSourcing.Shared.Events;
using EventSourcing.Shared.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Worker.Transaction.Handlers;

namespace Worker.Transaction.Services
{
    public class TransactionQueueService : BackgroundService
    {
        private readonly ILogger<TransactionQueueService> logger;
        private readonly IConsumer<Ignore, string> consumer;
        private readonly KafkaConsumerSettings consumerSettings;
        private readonly IEventHandler eventHandler;

        public TransactionQueueService(ILogger<TransactionQueueService> logger,
            IOptions<KafkaConsumerSettings> consumerSettingsAccessor,
            IEventHandler eventHandler)
        {
            this.logger = logger;
            this.consumerSettings = consumerSettingsAccessor.Value;
            this.eventHandler = eventHandler;

            var config = new ConsumerConfig
            {
                GroupId = consumerSettings.EventGroupId,
                BootstrapServers = consumerSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(TransactionQueueService)}.{nameof(ExecuteAsync)} call: Start");
            cancellationToken.ThrowIfCancellationRequested();

            Task.Run(() =>
            {
                consumer.Subscribe(consumerSettings.EventTopic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cancellationToken);
                            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value);

                            if (baseEvent.Type == nameof(AccountWithdrawnEvent))
                            {
                                var concreteEvent = JsonSerializer.Deserialize<AccountWithdrawnEvent>(consumeResult.Message.Value);
                                eventHandler.HandleAccountWithdrawnEvent(concreteEvent);
                            }
                            else if (baseEvent.Type == nameof(AccountDepositedEvent))
                            {
                                var concreteEvent = JsonSerializer.Deserialize<AccountDepositedEvent>(consumeResult.Message.Value);
                                eventHandler.HandleAccountDepositedEvent(concreteEvent);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            logger.LogError($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    logger.LogWarning($"Operation cancelled: {e.StackTrace}");
                }
            });
            logger.LogInformation($"{nameof(TransactionQueueService)}.{nameof(ExecuteAsync)} call: End");

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            consumer.Close();
            consumer.Dispose();
            base.Dispose();
        }
    }
}
