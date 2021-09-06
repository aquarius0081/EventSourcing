using Confluent.Kafka;
using EventSourcing.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace EventSourcing.API.Repositories
{
    public class EventSourcingProducerRepository : IEventSourcingProducerRepository, IDisposable
    {
        private readonly KafkaProducerSettings producerSettings;
        private readonly IProducer<Null, string> producer;
        private readonly ILogger<EventSourcingProducerRepository> logger;
        private readonly Action<DeliveryReport<Null, string>> deliveryHandler;

        public EventSourcingProducerRepository(IOptions<KafkaProducerSettings> producerSettingsAccessor,
            ILogger<EventSourcingProducerRepository> logger)
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

        public void AddCommand<T>(T command) where T : class
        {
            logger.LogInformation($"{nameof(EventSourcingProducerRepository)}.{nameof(AddCommand)} call: Start");
            string message = JsonSerializer.Serialize(command);
            var wrapper = new Message<Null, string> { Value = message };
            try
            {
                producer.Produce(producerSettings.CommandTopic, wrapper, deliveryHandler);
                producer.Flush(timeout: TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                logger.LogError($"Error occured during adding event to Kafka: {e.Message}, {e.StackTrace}");
            }
            logger.LogInformation($"{nameof(EventSourcingProducerRepository)}.{nameof(AddCommand)} call: End");
        }
    }
}
