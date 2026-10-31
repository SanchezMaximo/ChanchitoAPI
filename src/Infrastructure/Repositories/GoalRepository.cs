using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly AppDbContext context;

        public GoalRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void Add(Goal goal) => context.Goals.Add(goal);

        public Goal? GetById(Guid id) => 
            context.Goals
                .FirstOrDefault(a => a.Id == id);

        public IReadOnlyList<Goal> GetByCustomerId(Guid customerId) =>
          context.Goals.Where(a => a.CustomerId == customerId).ToList();
        
        public void SaveChanges() => context.SaveChanges();


    }
}