using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        CustomerResponse Create(CreateCustomerRequest request);

        IReadOnlyList<CustomerResponse> GetAll();

        CustomerResponse? GetById(Guid id);
    }
}