using EventSourcing.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Worker.Balance.Services;

namespace Worker.Balance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountBalancesController : ControllerBase
    {
        private readonly IAccountBalanceService accountBalanceService;

        public AccountBalancesController(IAccountBalanceService accountBalanceService)
        {
            this.accountBalanceService = accountBalanceService;
        }

        [HttpGet]
        public List<AccountDto> GetAllAccounts()
        {
            return accountBalanceService.GetBalances();
        }
    }
}
