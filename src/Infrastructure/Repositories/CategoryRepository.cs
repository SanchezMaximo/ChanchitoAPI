using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void Add(Category category) => context.Categories.Add(category);

        public Category? GetById(Guid id) =>
            context.Categories.FirstOrDefault(c => c.Id == id);

        public IReadOnlyList<Category> GetSystemCategories() =>
            context.Categories
                .Where(c => c.CustomerId == null)
                .OrderBy(c => c.Name)
                .ToList();

        public bool SystemCategoryNameExists(string name, Guid? excludeId = null) =>
            context.Categories.Any(c =>
                c.CustomerId == null
                && c.Name == name
                && (excludeId == null || c.Id != excludeId));

        public void Remove(Category category) => context.Categories.Remove(category);

        public void SaveChanges() => context.SaveChanges();
    }
}