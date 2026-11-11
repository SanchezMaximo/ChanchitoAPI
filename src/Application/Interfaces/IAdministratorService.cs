using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAdministratorService
    {
        IReadOnlyList<CustomerResponse> ListCustomers();

        CustomerCountResponse CountCustomers();

        AdministratorResponse CreateAdministrator(CreateAdministratorRequest request);

        IReadOnlyList<AdministratorResponse> ListAdministrators();

        CustomerResponse SuspendCustomer(Guid customerId);

        CustomerResponse ReactivateCustomer(Guid customerId);

        IReadOnlyList<CategoryResponse> ListSystemCategories();

        CategoryResponse CreateSystemCategory(CreateCategoryRequest request);

        CategoryResponse UpdateSystemCategory(Guid categoryId, UpdateCategoryRequest request);

        AdministratorResponse SuspendAdministrator(Guid currentAdminId, Guid administratorId);

        AdministratorResponse ReactivateAdministrator(Guid administratorId);

        void DeleteSystemCategory(Guid categoryId);
    }
}