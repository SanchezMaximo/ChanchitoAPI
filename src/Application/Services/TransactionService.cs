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
        private readonly ICategoryRepository categoryRepository;


        public TransactionService(IAccountRepository accountRepository, IExchangeRateService exchangeRateService, ICategoryRepository categoryRepository)
        {
            this.accountRepository = accountRepository;
            this.exchangeRateService = exchangeRateService;
            this.categoryRepository = categoryRepository;
        }

        public async Task<TransactionResponse> RegisterAsync(Guid accountId, CreateTransactionRequest request, CancellationToken cancellationToken = default)
        {   
            var account = GetAccountOrThrow(accountId);

            ValidateCategory(request.CategoryId, account.CustomerId);

            Guid? exchangeRateId = account.Currency == Currency.USD
                ? await exchangeRateService.GetCurrentRateIdAsync(ExchangeRateSource.Official, cancellationToken)
                : null;

            var transaction = request.Type == TransactionType.Income
                ? account.RegisterIncome(request.Description, request.Amount, exchangeRateId, request.CategoryId)
                : account.RegisterExpense(request.Description, request.Amount, exchangeRateId, request.CategoryId);
            
            accountRepository.AddTransaction(transaction);
            accountRepository.SaveChanges();
            return TransactionResponse.From(transaction);

        }
        public TransactionResponse ChangeAmount(Guid accountId, Guid transactionId, UpdateTransactionRequest request)
        {   
            var account = GetAccountOrThrow(accountId);
            EnsureNotGoalContribution(account, transactionId);

            var transaction = account.ChangeTransactionAmount(transactionId, request.Amount);

            accountRepository.SaveChanges();
            return TransactionResponse.From(transaction);

        }

        public TransactionResponse Cancel(Guid accountId, Guid transactionId)
        {   
            var account = GetAccountOrThrow(accountId);
            EnsureNotGoalContribution(account, transactionId);
            
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

        private void ValidateCategory(Guid? categoryId, Guid customerId)
        {
            if (categoryId is null)
                return;

            var category = categoryRepository.GetById(categoryId.Value);

            if (category is null || (!category.IsSystem && category.CustomerId != customerId))
                throw new ArgumentException("The category does not exist.", nameof(categoryId));
        }

        private Account GetAccountOrThrow(Guid accountId) =>
            accountRepository.GetById(accountId)
            ?? throw new KeyNotFoundException($"There is no account with id {accountId}.");


        private static void EnsureNotGoalContribution(Account account, Guid transactionId)
        {
            var transaction = account.Transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transaction?.GoalId is not null)
                throw new InvalidOperationException("This transaction is a goal contribution. Modify or cancel it from the goal.");
        }
    }
    
    
}
