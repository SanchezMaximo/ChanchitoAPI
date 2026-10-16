using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAdministratorService
    {
        IReadOnlyList<CustomerResponse> ListCustomers();

        CustomerCountResponse CountCustomers();

        CustomerResponse SuspendCustomer(Guid customerId);

        CustomerResponse ReactivateCustomer(Guid customerId);

        IReadOnlyList<CategoryResponse> ListSystemCategories();

        CategoryResponse CreateSystemCategory(CreateCategoryRequest request);

        CategoryResponse UpdateSystemCategory(Guid categoryId, UpdateCategoryRequest request);

        void DeleteSystemCategory(Guid categoryId);
    }
}