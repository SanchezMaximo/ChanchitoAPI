using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICategoryRepository
    {
        void Add(Category category);

        Category? GetById(Guid id);

        IReadOnlyList<Category> GetSystemCategories();

        IReadOnlyList<Category> GetVisibleToCustomer(Guid customerId);

        bool SystemCategoryNameExists(string name, Guid? excludeId = null);

        bool CustomerCategoryNameExists(Guid customerId, string name);

        void Remove(Category category);

        void SaveChanges();
    }
}
