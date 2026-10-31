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
    }
}
