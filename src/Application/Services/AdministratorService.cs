using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IUserRepository userRepository;
        private readonly ICategoryRepository categoryRepository;

        public AdministratorService(IUserRepository userRepository, ICategoryRepository categoryRepository)
        {
            this.userRepository = userRepository;
            this.categoryRepository = categoryRepository;
        }

        public IReadOnlyList<CustomerResponse> ListCustomers() =>
            userRepository.GetAllCustomers().Select(CustomerResponse.From).ToList();

        public CustomerCountResponse CountCustomers()
        {
            var total = userRepository.CountCustomers();
            var active = userRepository.CountActiveCustomers();

            return new CustomerCountResponse(total, active, total - active);
        }

        public CustomerResponse SuspendCustomer(Guid customerId)
        {
            var customer = GetCustomerOrThrow(customerId);

            customer.Suspend();
            userRepository.SaveChanges();

            return CustomerResponse.From(customer);
        }

        public CustomerResponse ReactivateCustomer(Guid customerId)
        {
            var customer = GetCustomerOrThrow(customerId);

            customer.Reactivate();
            userRepository.SaveChanges();

            return CustomerResponse.From(customer);
        }

        public IReadOnlyList<CategoryResponse> ListSystemCategories() =>
            categoryRepository.GetSystemCategories().Select(CategoryResponse.From).ToList();

        public CategoryResponse CreateSystemCategory(CreateCategoryRequest request)
        {
            var category = Category.CreateSystem(request.Name);

            if (categoryRepository.SystemCategoryNameExists(category.Name))
                throw new InvalidOperationException($"A system category named '{category.Name}' already exists.");
            categoryRepository.Add(category);
            categoryRepository.SaveChanges();

            return CategoryResponse.From(category);
        }

        public CategoryResponse UpdateSystemCategory(Guid categoryId, UpdateCategoryRequest request)
        {
            var category = GetSystemCategoryOrThrow(categoryId);

            category.Rename(request.Name);

            if (categoryRepository.SystemCategoryNameExists(category.Name, category.Id))
                throw new InvalidOperationException($"A system category named '{category.Name}' already exists.");

            categoryRepository.SaveChanges();

            return CategoryResponse.From(category);
        }

        public void DeleteSystemCategory(Guid categoryId)
        {
            var category = GetSystemCategoryOrThrow(categoryId);

            categoryRepository.Remove(category);
            categoryRepository.SaveChanges();
        }

        private Customer GetCustomerOrThrow(Guid customerId) =>
            userRepository.GetCustomerById(customerId)
                ?? throw new KeyNotFoundException($"There is no customer with id {customerId}.");

        private Category GetSystemCategoryOrThrow(Guid categoryId)
        {
            var category = categoryRepository.GetById(categoryId);

            if (category is null || !category.IsSystem)
                throw new KeyNotFoundException($"There is no system category with id {categoryId}.");

            return category;
        }
    }
}