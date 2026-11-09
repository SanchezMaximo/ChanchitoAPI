using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public IReadOnlyList<CategoryResponse> GetVisibleTo(Guid customerId) =>
            categoryRepository.GetVisibleToCustomer(customerId).Select(CategoryResponse.From).ToList();

        public CategoryResponse CreateForCustomer(Guid customerId, CreateCategoryRequest request)
        {
            var category = Category.CreateForCustomer(request.Name, customerId);

            if (categoryRepository.CustomerCategoryNameExists(customerId, category.Name))
                throw new InvalidOperationException($"You already have a category named '{category.Name}'.");

            categoryRepository.Add(category);
            categoryRepository.SaveChanges();

            return CategoryResponse.From(category);
        }

        public CategoryResponse? GetById(Guid id)
        {
            var category = categoryRepository.GetById(id);
            return category is null ? null : CategoryResponse.From(category);
        }

        public CategoryResponse Rename(Guid customerId, Guid id, UpdateCategoryRequest request)
        {
            var category = GetOwnedOrThrow(customerId, id);

            category.Rename(request.Name);

            if (categoryRepository.CustomerCategoryNameExists(customerId, category.Name, category.Id))
                throw new InvalidOperationException($"You already have a category named '{category.Name}'.");

            categoryRepository.SaveChanges();
            return CategoryResponse.From(category);
        }

        public void Delete(Guid customerId, Guid id)
        {
            var category = GetOwnedOrThrow(customerId, id);

            categoryRepository.Remove(category);
            categoryRepository.SaveChanges();
        }

        private Category GetOwnedOrThrow(Guid customerId, Guid id)
        {
            var category = categoryRepository.GetById(id)
                ?? throw new KeyNotFoundException($"There is no category with id {id}.");

            if (category.CustomerId != customerId)
                throw new UnauthorizedAccessException();

            return category;
        }

    }
}
