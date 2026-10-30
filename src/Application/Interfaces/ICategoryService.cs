using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        IReadOnlyList<CategoryResponse> GetVisibleTo(Guid customerId);

        CategoryResponse CreateForCustomer(Guid customerId, CreateCategoryRequest request);
    }
}
