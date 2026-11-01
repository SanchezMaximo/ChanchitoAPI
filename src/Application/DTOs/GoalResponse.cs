using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs
{
    public record GoalResponse(Guid Id, string Name, decimal TargetAmount, decimal SavedAmount, Currency Currency, bool IsCompleted, decimal Remaining, Guid CustomerId)
    {
        public static GoalResponse From(Goal goal) =>
            new(goal.Id, goal.Name, goal.TargetAmount, goal.SavedAmount, goal.Currency, goal.IsCompleted, goal.Remaining, goal.CustomerId);
    }
}