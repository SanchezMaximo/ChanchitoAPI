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
        private readonly IPasswordHasher passwordHasher;

        public AdministratorService(
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.categoryRepository = categoryRepository;
            this.passwordHasher = passwordHasher;
        }

        public AdministratorResponse CreateAdministrator(CreateAdministratorRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("The email is required.", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                throw new ArgumentException("The password must be at least 8 characters long.", nameof(request));

            var email = request.Email.Trim();

            if (userRepository.GetByEmail(email) is not null)
                throw new InvalidOperationException($"A user with the email {email} already exists.");

            var administrator = new Administrator(email, passwordHasher.Hash(request.Password));

            userRepository.Add(administrator);
            userRepository.SaveChanges();

            return AdministratorResponse.From(administrator);
        }

        public IReadOnlyList<AdministratorResponse> ListAdministrators() =>
            userRepository.GetAllAdministrators().Select(AdministratorResponse.From).ToList();

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