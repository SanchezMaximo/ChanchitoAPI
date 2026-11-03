using Domain.Entities;

namespace Application.DTOs
{
    public record AdministratorResponse(Guid Id, string Email, bool IsActive)
    {
        public static AdministratorResponse From(Administrator administrator) =>
            new(administrator.Id, administrator.Email, administrator.IsActive);
    }
}
