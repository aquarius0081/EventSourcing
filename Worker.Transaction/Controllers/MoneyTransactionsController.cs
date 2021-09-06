using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Worker.Transaction.Models;
using Worker.Transaction.Services;

namespace Worker.Transaction.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoneyTransactionsController : ControllerBase
    {
        private readonly IMoneyTransactionService moneyTransactionService;

        public MoneyTransactionsController(IMoneyTransactionService moneyTransactionService)
        {
            this.moneyTransactionService = moneyTransactionService;
        }

        [HttpGet]
        public IEnumerable<MoneyTransactionDto> GetAllTransactions()
        {
            return moneyTransactionService.GetAllTransactions();
        }
    }
}
