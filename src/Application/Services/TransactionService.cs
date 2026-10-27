using Application.Interfaces;
using Application.DTOs;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {  
        private readonly IAccountRepository accountRepository;
        private readonly IExchangeRateService exchangeRateService;


        public TransactionService(IAccountRepository accountRepository, IExchangeRateService exchangeRateService)
        {
            this.accountRepository = accountRepository;
            this.exchangeRateService = exchangeRateService;
        }

        public async Task<TransactionResponse> RegisterAsync(Guid accountId, CreateTransactionRequest request, CancellationToken cancellationToken = default)
        {   
            var account = GetAccountOrThrow(accountId);

            // Dollar accounts store the official rate of the moment; pesos accounts do not need it.
            Guid? exchangeRateId = account.Currency == Currency.USD
                ? await exchangeRateService.GetCurrentRateIdAsync(ExchangeRateSource.Official, cancellationToken)
                : null;

            var transaction = request.Type == TransactionType.Income
                ? account.RegisterIncome(request.Description, request.Amount, exchangeRateId)
                : account.RegisterExpense(request.Description, request.Amount, exchangeRateId);
            
            accountRepository.AddTransaction(transaction);
            accountRepository.SaveChanges();
            return TransactionResponse.From(transaction);

        }
        public TransactionResponse ChangeAmount(Guid accountId, Guid transactionId, UpdateTransactionRequest request)
        {   
            var account = GetAccountOrThrow(accountId);

            var transaction = account.ChangeTransactionAmount(transactionId, request.Amount);

            accountRepository.SaveChanges();
            return TransactionResponse.From(transaction);

        }

        public TransactionResponse Cancel(Guid accountId, Guid transactionId)
        {   
            var account = GetAccountOrThrow(accountId);
            var transaction = account.CancelTransaction(transactionId);
            accountRepository.SaveChanges();
            return TransactionResponse.From(transaction);
        }

        public IReadOnlyList<TransactionResponse>? GetByAccount(Guid accountId)
        {
            var account = accountRepository.GetById(accountId);
            if (account is null)
            {
                return null;
            }
            return account.Transactions
                .OrderByDescending(t => t.Date)
                .Select(TransactionResponse.From)
                .ToList();
        }

        private Account GetAccountOrThrow(Guid accountId) =>
            accountRepository.GetById(accountId)
            ?? throw new KeyNotFoundException($"There is no account with id {accountId}.");
    }
    
}
