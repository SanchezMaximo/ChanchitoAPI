using Application.DTOs;

namespace Application.Interfaces
{
    public interface ITransactionService
    {
        TransactionResponse Register(Guid accountId, CreateTransactionRequest request);
        TransactionResponse ChangeAmount(Guid accountId, Guid transactionId, UpdateTransactionRequest request);
        TransactionResponse Cancel(Guid accountId, Guid transactionId);
        IReadOnlyList<TransactionResponse>? GetByAccount(Guid accountId);
    }
}