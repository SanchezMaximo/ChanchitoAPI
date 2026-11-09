using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        IReadOnlyList<CategoryResponse> GetVisibleTo(Guid customerId);

        CategoryResponse CreateForCustomer(Guid customerId, CreateCategoryRequest request);

        CategoryResponse? GetById(Guid id);
        CategoryResponse Rename(Guid customerId, Guid id, UpdateCategoryRequest request);
        void Delete(Guid customerId, Guid id);
    }
}
