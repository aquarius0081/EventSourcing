using EventSourcing.API.Repositories;
using EventSourcing.Shared;
using EventSourcing.Shared.Commands;
using Microsoft.Extensions.Logging;
using System;

namespace EventSourcing.API.Services
{
    public class EventSourcingService : IEventSourcingService
    {
        private readonly ILogger<EventSourcingService> logger;
        private readonly IEventSourcingProducerRepository producerRepository;
        private readonly IEventSourcingConsumerRepository consumerRepository;

        public EventSourcingService(ILogger<EventSourcingService> logger,
            IEventSourcingProducerRepository producerRepository,
            IEventSourcingConsumerRepository consumerRepository)
        {
            this.logger = logger;
            this.producerRepository = producerRepository;
            this.consumerRepository = consumerRepository;
        }

        public void CreateAccount(AccountDto account)
        {
            logger.LogInformation($"{nameof(EventSourcingService)}.{nameof(CreateAccount)} call: Start");
            var createAccountCommand = new CreateAccountCommand
            {
                Id = Guid.NewGuid(),
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                Balance = account.Balance,
                CreatedDateTimeUtc = DateTime.UtcNow
            };
            producerRepository.AddCommand(createAccountCommand);
            logger.LogInformation($"{nameof(EventSourcingService)}.{nameof(CreateAccount)} call: End");
        }

        public string GetAllEvents()
        {
            logger.LogInformation($"{nameof(EventSourcingService)}.{nameof(GetAllEvents)} called");

            return consumerRepository.ConsumeAllEvents();
        }

        public void MoveFunds(MoveFundsDto moveFundsDto)
        {
            logger.LogInformation($"{nameof(EventSourcingService)}.{nameof(MoveFunds)} call: Start");
            var moveFundsCommand = new MoveFundsCommand
            {
                Id = Guid.NewGuid(),
                SourceAccountId = moveFundsDto.SourceAccountId,
                TargetAccountId = moveFundsDto.TargetAccountId,
                CurrencyType = moveFundsDto.CurrencyType,
                Amount = moveFundsDto.Amount,
                CreatedDateTimeUtc = DateTime.UtcNow
            };
            producerRepository.AddCommand(moveFundsCommand);
            logger.LogInformation($"{nameof(EventSourcingService)}.{nameof(MoveFunds)} call: End");
        }
    }
}
