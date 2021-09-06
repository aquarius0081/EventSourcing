using Confluent.Kafka;
using EventSourcing.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Worker.Balance.Repositories
{
    public class BalanceQueueRepository : IBalanceQueueRepository, IDisposable
    {
        private readonly KafkaProducerSettings producerSettings;
        private readonly IProducer<Null, string> producer;
        private readonly ILogger<BalanceQueueRepository> logger;
        private readonly Action<DeliveryReport<Null, string>> deliveryHandler;

        public BalanceQueueRepository(IOptions<KafkaProducerSettings> producerSettingsAccessor,
            ILogger<BalanceQueueRepository> logger)
        {
            this.logger = logger;
            this.producerSettings = producerSettingsAccessor.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = producerSettings.BootstrapServers
            };
            this.producer = new ProducerBuilder<Null, string>(config).Build();
            deliveryHandler = r => logger.LogInformation(!r.Error.IsError
                ? $"Delivered Message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");
        }

        public void Dispose()
        {
            producer.Dispose();
        }

        public void AddEvent<T>(T @event) where T : class
        {
            logger.LogInformation($"{nameof(BalanceQueueRepository)}.{nameof(AddEvent)} call: Start");
            string message = JsonSerializer.Serialize(@event);
            var wrapper = new Message<Null, string> { Value = message };
            try
            {
                producer.Produce(producerSettings.EventTopic, wrapper, deliveryHandler);
                producer.Flush(timeout: TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                logger.LogError($"Error occured during adding event to Kafka: {e.Message}, {e.StackTrace}");
            }
            logger.LogInformation($"{nameof(BalanceQueueRepository)}.{nameof(AddEvent)} call: End");
        }
    }
}
