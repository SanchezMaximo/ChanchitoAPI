using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Customer)]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<CategoryResponse>> GetAll() =>
            Ok(categoryService.GetVisibleTo(User.GetUserId()));

        [HttpPost]
        public ActionResult<CategoryResponse> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = categoryService.CreateForCustomer(User.GetUserId(), request);
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
