using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public CustomerService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public CustomerResponse Create(CreateCustomerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.", nameof(request));

            if (userRepository.GetByEmail(request.Email) is not null)
                throw new InvalidOperationException($"Ya existe un usuario con el email {request.Email}.");

            var customer = new Customer(request.Email, passwordHasher.Hash(request.Password));    

            userRepository.Add(customer);
            userRepository.SaveChanges();

            return CustomerResponse.From(customer);
        }

        public IReadOnlyList<CustomerResponse> GetAll() =>
            userRepository.GetAllCustomers().Select(CustomerResponse.From).ToList();

        public CustomerResponse? GetById(Guid id)
        {
            var customer = userRepository.GetCustomerById(id);

            return customer is null ? null : CustomerResponse.From(customer);
        }
    }
}