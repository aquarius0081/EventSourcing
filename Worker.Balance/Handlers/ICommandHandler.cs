using EventSourcing.Shared.Commands;
using EventSourcing.Shared.Events;
using System;
using System.Collections.Generic;

namespace Worker.Balance.Handlers
{
    public interface ICommandHandler
    {
        void HandleCreateAccountCommand(CreateAccountCommand command, Func<List<BaseEvent>> consumeAllEvents);
        void HandleMoveFundsCommand(MoveFundsCommand command, Func<List<BaseEvent>> consumeAllEvents);
    }
}