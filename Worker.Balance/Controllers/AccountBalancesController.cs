using EventSourcing.Shared;
using EventSourcing.Shared.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Worker.Balance.Handlers;

namespace Worker.Balance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountBalancesController : ControllerBase
    {
        private readonly IQueryHandler queryHandler;

        public AccountBalancesController(IQueryHandler queryHandler)
        {
            this.queryHandler = queryHandler;
        }

        [HttpGet]
        public List<AccountDto> GetAllAccounts()
        {
            return queryHandler.Handle(new GetAllAccountsQuery());
        }
    }
}
