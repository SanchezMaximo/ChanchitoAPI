using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICategoryRepository
    {
        void Add(Category category);

        Category? GetById(Guid id);

        IReadOnlyList<Category> GetSystemCategories();

        bool SystemCategoryNameExists(string name, Guid? excludeId = null);

        void Remove(Category category);

        void SaveChanges();
    }
}