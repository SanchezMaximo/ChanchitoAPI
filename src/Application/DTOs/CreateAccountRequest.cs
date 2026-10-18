using Domain.Enums;

namespace Application.DTOs
{
     public record CreateAccountRequest(string Name, decimal Balance, AccountType Type, Currency Currency, Guid CustomerId);
}