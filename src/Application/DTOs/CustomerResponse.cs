using Domain.Entities;

namespace Application.DTOs
{
    public record CustomerResponse(Guid Id, string Email, bool IsActive)
    {
        public static CustomerResponse From(Customer customer) =>
            new(customer.Id, customer.Email, customer.IsActive);
    }
}