using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        AccountResponse Create(CreateAccountRequest request);

        AccountResponse? GetById(Guid id);

       IReadOnlyList<AccountResponse> GetAll();

        AccountResponse DeleteById(Guid id);

        AccountResponse Update(Guid id, UpdateAccountRequest request);

    }
}