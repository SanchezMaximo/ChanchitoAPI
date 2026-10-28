using Application.DTOs;

namespace Application.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse> RegisterAsync(Guid accountId, CreateTransactionRequest request, CancellationToken cancellationToken = default);
        TransactionResponse ChangeAmount(Guid accountId, Guid transactionId, UpdateTransactionRequest request);
        TransactionResponse Cancel(Guid accountId, Guid transactionId);
        IReadOnlyList<TransactionResponse>? GetByAccount(Guid accountId);
    }
}
