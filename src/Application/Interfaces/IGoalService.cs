using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGoalService
    {
        GoalResponse Create(Guid customerId, CreateGoalRequest request);
        GoalResponse? GetById(Guid id);
        IReadOnlyList<GoalResponse> GetByCustomerId(Guid customerId);
        Task<GoalResponse> ContributeAsync(Guid customerId, Guid goalId, ContributeRequest request, CancellationToken cancellationToken = default);    }
}
