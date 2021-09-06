using EventSourcing.Shared.Commands;
using EventSourcing.Shared.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Worker.Balance.Aggregates;
using Worker.Balance.Services;

namespace Worker.Balance.Handlers
{
    public class CommandHandler : ICommandHandler
    {
        private readonly ILogger<CommandHandler> logger;
        private readonly IAccountBalanceService accountBalanceService;

        public CommandHandler(ILogger<CommandHandler> logger, IAccountBalanceService accountBalanceService)
        {
            this.logger = logger;
            this.accountBalanceService = accountBalanceService;
        }

        public void HandleCreateAccountCommand(CreateAccountCommand command, Func<List<BaseEvent>> consumeAllEvents)
        {
            logger.LogInformation($"{nameof(CommandHandler)}.{nameof(HandleCreateAccountCommand)} call: Start");

            var accountAggregate = RehydrateAccountAggregate(command.AccountId, consumeAllEvents());
            if (accountAggregate.AggregateId == command.AccountId)
            {
                var reason = $"Account with ID={command.AccountId} already exists";
                var createAccountFailedEvent = new CreateAccountFailedEvent
                {
                    Id = Guid.NewGuid(),
                    AccountId = command.AccountId,
                    AccountName = command.AccountName,
                    Balance = command.Balance,
                    RelatedCommandId = command.Id,
                    CreatedDateTimeUtc = DateTime.UtcNow,
                    Reason = reason
                };
                logger.LogError(reason);
                accountBalanceService.FailCreateAccount(createAccountFailedEvent);

                return;
            }

            var accountCreatedEvent = new AccountCreatedEvent
            {
                Id = Guid.NewGuid(),
                AccountId = command.AccountId,
                AccountName = command.AccountName,
                Balance = command.Balance,
                RelatedCommandId = command.Id,
                CreatedDateTimeUtc = DateTime.UtcNow,
                Version = 1
            };

            accountBalanceService.AddAccountCreatedEvent(accountCreatedEvent);

            logger.LogInformation($"{nameof(CommandHandler)}.{nameof(HandleCreateAccountCommand)} call: End");
        }

        public void HandleMoveFundsCommand(MoveFundsCommand command, Func<List<BaseEvent>> consumeAllEvents)
        {
            logger.LogInformation($"{nameof(CommandHandler)}.{nameof(HandleMoveFundsCommand)} call: Start");
            var events = consumeAllEvents();
            var sourceAccountAggregate = RehydrateAccountAggregate(command.SourceAccountId, events);
            var targetAccountAggregate = RehydrateAccountAggregate(command.TargetAccountId, events);
            var reason = String.Empty;

            if (sourceAccountAggregate.AggregateId != command.SourceAccountId
                || targetAccountAggregate.AggregateId != command.TargetAccountId)
            {
                reason = "Source or target account does not exist";
            }
            else if (sourceAccountAggregate.Balance < command.Amount)
            {
                reason = "Source account does not have enough funds";
            }

            var accountWithdrawnEvent = new AccountWithdrawnEvent
            {
                Id = Guid.NewGuid(),
                AccountId = command.SourceAccountId,
                Amount = command.Amount,
                CurrencyType = command.CurrencyType,
                RelatedCommandId = command.Id,
                Version = sourceAccountAggregate.Version + 1
            };

            var accountDepositedEvent = new AccountDepositedEvent
            {
                Id = Guid.NewGuid(),
                AccountId = command.TargetAccountId,
                Amount = command.Amount,
                CurrencyType = command.CurrencyType,
                RelatedCommandId = command.Id,
                Version = targetAccountAggregate.Version + 1
            };

            if (String.IsNullOrEmpty(reason))
            {
                events = consumeAllEvents();
                foreach (var @event in events)
                {
                    if (@event is AccountWithdrawnEvent)
                    {
                        var accountWithdrawnEvent2 = @event as AccountWithdrawnEvent;
                        if (accountWithdrawnEvent2.AccountId == accountWithdrawnEvent.AccountId &&
                            accountWithdrawnEvent2.Version == accountWithdrawnEvent.Version)
                        {
                            reason = $"Concurrency exception: AccountWithdrawnEvent already exists with the same version: {accountWithdrawnEvent2.Version} for account with ID: {accountWithdrawnEvent2.AccountId}";
                            break;
                        }
                    }
                    else if (@event is AccountDepositedEvent)
                    {
                        var accountDepositedEvent2 = @event as AccountDepositedEvent;
                        if (accountDepositedEvent2.AccountId == accountDepositedEvent.AccountId &&
                            accountDepositedEvent2.Version == accountDepositedEvent.Version)
                        {
                            reason = $"Concurrency exception: AccountDepositedEvent already exists with the same version: {accountDepositedEvent2.Version} for account with ID: {accountDepositedEvent2.AccountId}";
                            break;
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(reason))
            {
                var moveFundsFailedEvent = new MoveFundsFailedEvent
                {
                    Id = Guid.NewGuid(),
                    RelatedCommandId = command.Id,
                    SourceAccountId = command.SourceAccountId,
                    TargetAccountId = command.TargetAccountId,
                    CurrencyType = command.CurrencyType,
                    Amount = command.Amount,
                    Reason = reason
                };


                accountBalanceService.FailMoveFunds(moveFundsFailedEvent);
                logger.LogError(reason);

                return;
            }

            accountBalanceService.UpdateTransactions(accountWithdrawnEvent, accountDepositedEvent);
            logger.LogInformation($"{nameof(CommandHandler)}.{nameof(HandleMoveFundsCommand)} call: End");
        }

        private static AccountAggregate RehydrateAccountAggregate(int accountId, List<BaseEvent> events)
        {
            var accountAggregate = new AccountAggregate();
            foreach (var @event in events)
            {
                if (@event is AccountCreatedEvent)
                {
                    var accountCreationEvent = @event as AccountCreatedEvent;
                    if (accountCreationEvent.AccountId == accountId)
                    {
                        accountAggregate.Apply(accountCreationEvent);
                    }
                }
                else if (@event is AccountWithdrawnEvent)
                {
                    var accountWithdrawnEvent = @event as AccountWithdrawnEvent;
                    if (accountWithdrawnEvent.AccountId == accountId)
                    {
                        accountAggregate.Apply(accountWithdrawnEvent);
                    }
                }
                else if (@event is AccountDepositedEvent)
                {
                    var accountDepositedEvent = @event as AccountDepositedEvent;
                    if (accountDepositedEvent.AccountId == accountId)
                    {
                        accountAggregate.Apply(accountDepositedEvent);
                    }
                }
            }

            return accountAggregate;
        }
    }
}
