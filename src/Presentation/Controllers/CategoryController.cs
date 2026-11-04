using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.Customer},{Roles.Administrator}")]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;
        private readonly IAdministratorService administratorService;

        public CategoryController(
            ICategoryService categoryService,
            IAdministratorService administratorService)
        {
            this.categoryService = categoryService;
            this.administratorService = administratorService;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<CategoryResponse>> GetAll()
        {
            if (User.IsInRole(Roles.Administrator))
                return Ok(administratorService.ListSystemCategories());

            return Ok(categoryService.GetVisibleTo(User.GetUserId()));
        }

        [HttpPost]
        public ActionResult<CategoryResponse> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = User.IsInRole(Roles.Administrator)
                    ? administratorService.CreateSystemCategory(request)
                    : categoryService.CreateForCustomer(User.GetUserId(), request);

                return Created($"api/Category/{category.Id}", category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
