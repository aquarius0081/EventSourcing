using Confluent.Kafka;
using EventSourcing.Shared.Commands;
using EventSourcing.Shared.Events;
using EventSourcing.Shared.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Worker.Balance.Handlers;
using Worker.Balance.Repositories;

namespace Worker.Balance.Services
{
    public class BalanceQueueService : BackgroundService
    {
        private readonly IAccountBalanceService accountBalanceService;
        private readonly ILogger<BalanceQueueService> logger;
        private readonly IBalanceQueueRepository queueRepository;
        private readonly IConsumer<Ignore, string> commandConsumer;
        private readonly IConsumer<Ignore, string> eventConsumer;
        private readonly KafkaConsumerSettings consumerSettings;
        private readonly ICommandHandler commandHandler;
        private readonly IEventHandler eventHandler;

        public BalanceQueueService(IAccountBalanceService accountBalanceService,
            IBalanceQueueRepository queueRepository,
            ILogger<BalanceQueueService> logger,
            IOptions<KafkaConsumerSettings> consumerSettingsAccessor,
            ICommandHandler commandHandler,
            IEventHandler eventHandler)
        {
            this.accountBalanceService = accountBalanceService;
            this.logger = logger;
            this.queueRepository = queueRepository;
            this.consumerSettings = consumerSettingsAccessor.Value;
            this.commandHandler = commandHandler;
            this.eventHandler = eventHandler;

            var eventConfig = new ConsumerConfig
            {
                GroupId = consumerSettings.EventGroupId,
                BootstrapServers = consumerSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            eventConsumer = new ConsumerBuilder<Ignore, string>(eventConfig).Build();

            var commandConfig = new ConsumerConfig
            {
                GroupId = consumerSettings.CommandGroupId,
                BootstrapServers = consumerSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            commandConsumer = new ConsumerBuilder<Ignore, string>(commandConfig).Build();
        }

        private void ProcessEvents(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(BalanceQueueService)}.{nameof(ProcessEvents)} call: Start");
            cancellationToken.ThrowIfCancellationRequested();

            eventConsumer.Subscribe(consumerSettings.EventTopic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = eventConsumer.Consume(cancellationToken);
                        var isHistoricalEvent = accountBalanceService.GetLastProcessedMessageId() >= consumeResult.Offset.Value;
                        var baseEvent = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value);

                        if (baseEvent.Type == nameof(AccountWithdrawnEvent))
                        {
                            var concreteEvent = JsonSerializer.Deserialize<AccountWithdrawnEvent>(consumeResult.Message.Value);
                            eventHandler.HandleAccountWithdrawnEvent(concreteEvent);
                        }
                        else if (baseEvent.Type == nameof(AccountCreatedEvent))
                        {
                            var concreteEvent = JsonSerializer.Deserialize<AccountCreatedEvent>(consumeResult.Message.Value);
                            eventHandler.HandleAccountCreatedEvent(concreteEvent);
                        }
                        else if (baseEvent.Type == nameof(AccountDepositedEvent))
                        {
                            var concreteEvent = JsonSerializer.Deserialize<AccountDepositedEvent>(consumeResult.Message.Value);
                            eventHandler.HandleAccountDepositedEvent(concreteEvent);
                        }

                        if (!isHistoricalEvent)
                        {
                            accountBalanceService.UpdateLastProcessedMessageId(consumeResult.Offset.Value);
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

            logger.LogInformation($"{nameof(BalanceQueueService)}.{nameof(ExecuteAsync)} call: End");
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(BalanceQueueService)}.{nameof(ExecuteAsync)} call: Start");
            cancellationToken.ThrowIfCancellationRequested();

            Task.Run(() =>
            {
                ProcessEvents(cancellationToken);
            });

            Task.Run(() =>
            {
                ProcessCommands(cancellationToken);
            });

            logger.LogInformation($"{nameof(BalanceQueueService)}.{nameof(ExecuteAsync)} call: End");

            return Task.CompletedTask;
        }

        private void ProcessCommands(CancellationToken cancellationToken)
        {
            commandConsumer.Subscribe(consumerSettings.CommandTopic);
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = commandConsumer.Consume(cancellationToken);
                        var baseCommand = JsonSerializer.Deserialize<BaseCommand>(consumeResult.Message.Value);

                        if (baseCommand.Type == nameof(MoveFundsCommand))
                        {
                            var concreteCommand = JsonSerializer.Deserialize<MoveFundsCommand>(consumeResult.Message.Value);
                            commandHandler.HandleMoveFundsCommand(concreteCommand, ConsumeAllEvents);
                        }
                        else if (baseCommand.Type == nameof(CreateAccountCommand))
                        {
                            var concreteCommand = JsonSerializer.Deserialize<CreateAccountCommand>(consumeResult.Message.Value);
                            commandHandler.HandleCreateAccountCommand(concreteCommand, ConsumeAllEvents);
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
        }

        public List<BaseEvent> ConsumeAllEvents()
        {
            logger.LogInformation($"{nameof(BalanceQueueService)}.{nameof(ConsumeAllEvents)} call: Start");
            var eventConfig = new ConsumerConfig
            {
                GroupId = "Worker.Balance.AllEvents",
                BootstrapServers = consumerSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            var consumer = new ConsumerBuilder<Ignore, string>(eventConfig).Build();
            var result = new List<BaseEvent>();
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
                        var baseEvent = JsonSerializer.Deserialize<BaseEvent>(cr.Message.Value);

                        if (baseEvent.Type == nameof(AccountCreatedEvent))
                        {
                            var concreteEvent = JsonSerializer.Deserialize<AccountCreatedEvent>(cr.Message.Value);
                            result.Add(concreteEvent);
                        }
                        else if (baseEvent.Type == nameof(AccountWithdrawnEvent))
                        {
                            var concreteEvent = JsonSerializer.Deserialize<AccountWithdrawnEvent>(cr.Message.Value);
                            result.Add(concreteEvent);
                        }
                        else if (baseEvent.Type == nameof(AccountDepositedEvent))
                        {
                            var concreteEvent = JsonSerializer.Deserialize<AccountDepositedEvent>(cr.Message.Value);
                            result.Add(concreteEvent);
                        }
                        Console.WriteLine($"Partition offset: {cr.Offset.Value}");
                    }
                    catch (ConsumeException e)
                    {
                        logger.LogError($"Error occured: {e.Error.Reason}");
                    }
                }
            }

            result.Sort((x, y) =>
            {
                if (x.CreatedDateTimeUtc < y.CreatedDateTimeUtc)
                {
                    return -1;
                }
                if (x.CreatedDateTimeUtc == y.CreatedDateTimeUtc)
                {
                    return 0;
                }
                if (x.CreatedDateTimeUtc > y.CreatedDateTimeUtc)
                {
                    return 1;
                }
                return 0;
            });
            logger.LogInformation($"{nameof(BalanceQueueService)}.{nameof(ConsumeAllEvents)} call: End");

            return result;
        }

        public override void Dispose()
        {
            commandConsumer.Close();
            commandConsumer.Dispose();
            eventConsumer.Close();
            eventConsumer.Dispose();
            base.Dispose();
        }
    }
}
