using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Worker.Transaction.Models;
using Worker.Transaction.Repositories;

namespace Worker.Transaction.Services
{
    public class MoneyTransactionService : IMoneyTransactionService
    {
        private readonly ITransactionDbRepository dbRepository;
        private readonly ILogger<MoneyTransactionService> logger;

        public MoneyTransactionService(ITransactionDbRepository dbRepository,
            ILogger<MoneyTransactionService> logger)
        {
            this.dbRepository = dbRepository;
            this.logger = logger;
        }

        public void AddTransaction(MoneyTransactionDto transactionDto)
        {
            logger.LogInformation($"{nameof(MoneyTransactionService)}.{nameof(AddTransaction)} call: Start");
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MoneyTransactionDto, MoneyTransaction>());
            var mapper = new Mapper(config);
            var transaction = mapper.Map<MoneyTransaction>(transactionDto);
            dbRepository.SaveTransaction(transaction);
            logger.LogInformation($"{nameof(MoneyTransactionService)}.{nameof(AddTransaction)} call: End");
        }

        public List<MoneyTransactionDto> GetAllTransactions()
        {
            logger.LogInformation($"{nameof(MoneyTransactionService)}.{nameof(GetAllTransactions)} call: Start");
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MoneyTransaction, MoneyTransactionDto>());
            var mapper = new Mapper(config);
            var transactions = mapper.Map<List<MoneyTransactionDto>>(dbRepository.GetAllTransactions());
            logger.LogInformation($"{nameof(MoneyTransactionService)}.{nameof(GetAllTransactions)} call: End");

            return transactions;
        }
    }
}
