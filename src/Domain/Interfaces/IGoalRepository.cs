using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGoalRepository
    {
        void Add(Goal goal);
        Goal? GetById(Guid id);
        IReadOnlyList<Goal> GetByCustomerId(Guid customerId);
        void SaveChanges();
    }

}