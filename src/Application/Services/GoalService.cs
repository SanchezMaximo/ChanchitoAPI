using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class GoalService : IGoalService
    {
        
        private readonly IGoalRepository goalRepository;
        public GoalService(IGoalRepository goalRepository)
        {
            this.goalRepository = goalRepository;
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

        
        
    }
}