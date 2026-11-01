using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGoalService
    {
        GoalResponse Create(Guid customerId, CreateGoalRequest request);
        GoalResponse? GetById(Guid id);
        IReadOnlyList<GoalResponse> GetByCustomerId(Guid customerId);
    }
}