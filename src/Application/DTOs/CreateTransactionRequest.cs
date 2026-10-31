using Domain.Enums;

namespace Application.DTOs
{
        public record CreateTransactionRequest(TransactionType Type, string Description, decimal Amount, Guid? CategoryId = null);
}
