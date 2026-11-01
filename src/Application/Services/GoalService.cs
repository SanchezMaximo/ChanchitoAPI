using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.Services
{
    public class GoalService : IGoalService
    {
        
        private readonly IGoalRepository goalRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IExchangeRateService exchangeRateService;

        public GoalService(IGoalRepository goalRepository, IAccountRepository accountRepository, IExchangeRateService exchangeRateService)
        {
            this.goalRepository = goalRepository;
            this.accountRepository = accountRepository;
            this.exchangeRateService = exchangeRateService;
        }

        public GoalResponse Create(Guid customerId, CreateGoalRequest request)
        {
            var goal = new Goal(request.Name, request.TargetAmount, request.Currency, customerId);
            goalRepository.Add(goal);
            goalRepository.SaveChanges();

            return GoalResponse.From(goal);
        }
        public GoalResponse? GetById(Guid id)
        {
            var goal = goalRepository.GetById(id);
            return goal is null ? null : GoalResponse.From(goal);
        }

        public IReadOnlyList<GoalResponse> GetByCustomerId(Guid customerId) =>
            goalRepository.GetByCustomerId(customerId).Select(GoalResponse.From).ToList();

        public async Task<GoalResponse> ContributeAsync(Guid customerId, Guid goalId, ContributeRequest request, CancellationToken cancellationToken = default)
        {
            var goal = goalRepository.GetById(goalId)
                ?? throw new KeyNotFoundException($"There is no goal with the id {goalId}.");

            if (goal.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("The goal does not belong to the customer.");
            }

            var account = accountRepository.GetById(request.AccountId)
                ?? throw new KeyNotFoundException($"There is no account with the id {request.AccountId}.");
            
            if (account.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("The account does not belong to the customer.");
            }

            if (account.Currency != goal.Currency)
            {
                throw new InvalidOperationException("The account currency does not match the goal currency.");
            }

            Guid? exchangeRateId = account.Currency == Currency.USD
                ? await exchangeRateService.GetCurrentRateIdAsync(ExchangeRateSource.Official, cancellationToken)
                : null;

            goal.Contribute(request.Amount);
            var transaction = account.RegisterExpense(
                $"Contribution to goal {goal.Name}",
                request.Amount,
                exchangeRateId: exchangeRateId,
                goalId: goal.Id);

            accountRepository.AddTransaction(transaction);
            accountRepository.SaveChanges();

            return GoalResponse.From(goal);
        }

        
    }
}