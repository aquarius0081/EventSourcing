using EventSourcing.Shared;
using EventSourcing.Shared.Queries;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Worker.Balance.Services;

namespace Worker.Balance.Handlers
{
    public class QueryHandler : IQueryHandler
    {
        private readonly ILogger<EventHandler> logger;
        private readonly IAccountBalanceService accountBalanceService;

        public QueryHandler(ILogger<EventHandler> logger, IAccountBalanceService accountBalanceService)
        {
            this.logger = logger;
            this.accountBalanceService = accountBalanceService;
        }

        public List<AccountDto> Handle(GetAllAccountsQuery query)
        {
            return accountBalanceService.GetBalances();
        }
    }
}
