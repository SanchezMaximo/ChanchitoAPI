namespace Application.DTOs
{
    public record ContributeRequest(Guid AccountId, decimal Amount);
}