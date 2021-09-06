using AutoMapper;
using EventSourcing.Shared;
using EventSourcing.Shared.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Worker.Balance.Models;
using Worker.Balance.Repositories;

namespace Worker.Balance.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBalanceDbRepository dbRepository;
        private readonly ILogger<AccountBalanceService> logger;
        private readonly IBalanceQueueRepository queueRepository;

        public AccountBalanceService(IBalanceDbRepository dbRepository,
            ILogger<AccountBalanceService> logger, IBalanceQueueRepository queueRepository)
        {
            this.dbRepository = dbRepository;
            this.logger = logger;
            this.queueRepository = queueRepository;
        }

        public void CreateAccount(AccountDto accountDto)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(CreateAccount)} call: Start");
            var config = new MapperConfiguration(cfg => cfg.CreateMap<AccountDto, Account>());
            var mapper = new Mapper(config);
            var account = mapper.Map<Account>(accountDto);
            dbRepository.CreateAccount(account);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(CreateAccount)} call: End");
        }

        public void FailMoveFunds(MoveFundsFailedEvent moveFundsFailedEvent)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(FailMoveFunds)} call: Start");
            moveFundsFailedEvent.CreatedDateTimeUtc = DateTime.UtcNow;
            queueRepository.AddEvent(moveFundsFailedEvent);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(FailMoveFunds)} call: End");
        }

        public List<AccountDto> GetBalances()
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(GetBalances)} call: Start");
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Account, AccountDto>());
            var mapper = new Mapper(config);
            var accounts = mapper.Map<List<AccountDto>>(dbRepository.GetAllBalances());
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(GetBalances)} call: End");

            return accounts;
        }

        public void UpdateAccountBalance(int accountId, decimal amount)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(UpdateAccountBalance)} call: Start");
            dbRepository.UpdateAccountBalance(accountId, amount);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(UpdateAccountBalance)} call: End");
        }

        public void UpdateTransactions(AccountWithdrawnEvent accountWithdrawnEvent, AccountDepositedEvent accountDepositedEvent)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(UpdateTransactions)} call: Start");
            accountWithdrawnEvent.CreatedDateTimeUtc = DateTime.UtcNow;
            queueRepository.AddEvent(accountWithdrawnEvent);
            accountDepositedEvent.CreatedDateTimeUtc = DateTime.UtcNow;
            queueRepository.AddEvent(accountDepositedEvent);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(UpdateTransactions)} call: End");
        }

        public long GetLastProcessedMessageId()
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(GetLastProcessedMessageId)} called");
            return dbRepository.GetLastProcessedMessageId();
        }

        public void UpdateLastProcessedMessageId(long newValue)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(UpdateLastProcessedMessageId)} call: Start");
            dbRepository.UpdateLastProcessedMessageId(newValue);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(UpdateLastProcessedMessageId)} call: End");
        }

        public void AddAccountCreatedEvent(AccountCreatedEvent accountCreatedEvent)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(AddAccountCreatedEvent)} call: Start");
            queueRepository.AddEvent(accountCreatedEvent);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(AddAccountCreatedEvent)} call: End");
        }

        public void FailCreateAccount(CreateAccountFailedEvent createAccountFailedEvent)
        {
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(AddAccountCreatedEvent)} call: Start");
            queueRepository.AddEvent(createAccountFailedEvent);
            logger.LogInformation($"{nameof(AccountBalanceService)}.{nameof(AddAccountCreatedEvent)} call: End");
        }
    }
}
