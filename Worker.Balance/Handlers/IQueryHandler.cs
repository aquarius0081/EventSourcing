using EventSourcing.Shared;
using EventSourcing.Shared.Queries;
using System.Collections.Generic;

namespace Worker.Balance.Handlers
{
    public interface IQueryHandler
    {
        List<AccountDto> Handle(GetAllAccountsQuery query);
    }
}