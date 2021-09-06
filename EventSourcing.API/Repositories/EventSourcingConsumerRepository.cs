using Confluent.Kafka;
using EventSourcing.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourcing.API.Repositories
{
    public class EventSourcingConsumerRepository : IEventSourcingConsumerRepository, IDisposable
    {
        private readonly IConsumer<Ignore, string> consumer;
        private readonly KafkaConsumerSettings consumerSettings;
        private readonly ILogger<EventSourcingConsumerRepository> logger;

        public EventSourcingConsumerRepository(IOptions<KafkaConsumerSettings> consumerSettingsAccessor,
            ILogger<EventSourcingConsumerRepository> logger)
        {
            this.consumerSettings = consumerSettingsAccessor.Value;
            this.logger = logger;

            var config = new ConsumerConfig
            {
                GroupId = consumerSettings.EventGroupId,
                BootstrapServers = consumerSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public string ConsumeAllEvents()
        {
            logger.LogInformation($"{nameof(EventSourcingConsumerRepository)}.{nameof(ConsumeAllEvents)} call: Start");
            var eventsStringBuilder = new StringBuilder();
            eventsStringBuilder.Append('[');
            for (var i = 0; i < consumerSettings.PartitionsCount; i++)
            {
                consumer.Assign(new TopicPartitionOffset(consumerSettings.EventTopic, new Partition(i), Offset.Beginning));

                while (true)
                {
                    try
                    {
                        var cr = consumer.Consume(TimeSpan.FromSeconds(2));
                        if (cr.IsPartitionEOF)
                        {
                            break;
                        }
                        Console.WriteLine(cr.Message.Value);
                        eventsStringBuilder.Append($"{cr.Message.Value},");
                        Console.WriteLine($"Partition offset: {cr.Offset.Value}");
                    }
                    catch (ConsumeException e)
                    {
                        logger.LogError($"Error occured: {e.Error.Reason}");
                    }
                }
            }

            eventsStringBuilder.Append(']');
            logger.LogInformation($"{nameof(EventSourcingConsumerRepository)}.{nameof(ConsumeAllEvents)} call: End");

            return eventsStringBuilder.ToString();
        }

        public void Dispose()
        {
            consumer.Close();
            consumer.Dispose();
        }
    }
}
