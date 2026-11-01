using Domain.Enums;

namespace Application.DTOs
{
    public record CreateGoalRequest(string Name, decimal TargetAmount, Currency Currency);
}