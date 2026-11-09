using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        AccountResponse Create(Guid customerId,CreateAccountRequest request);

        AccountResponse? GetById(Guid id);

        IReadOnlyList<AccountResponse> GetByCustomerId(Guid customerId);

        AccountResponse Update(Guid id, UpdateAccountRequest request);

    }
}