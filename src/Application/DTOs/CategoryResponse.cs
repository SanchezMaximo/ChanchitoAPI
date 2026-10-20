using Domain.Entities;

namespace Application.DTOs
{
    public record CategoryResponse(Guid Id, string Name, bool IsSystem)
    {
        public static CategoryResponse From(Category category) =>
            new(category.Id, category.Name, category.IsSystem);
    }
}