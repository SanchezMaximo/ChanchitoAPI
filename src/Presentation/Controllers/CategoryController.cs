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

        [HttpGet("{id}")]
        public ActionResult<CategoryResponse> GetById([FromRoute] Guid id)
        {
            var category = categoryService.GetById(id);
            if (category is null)
                return NotFound($"There is no category with id {id}.");

            if (category.CustomerId is not null && category.CustomerId != User.GetUserId())
                return Forbid();

            return Ok(category);
        }

        [HttpPost]
        public ActionResult<CategoryResponse> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = User.IsInRole(Roles.Administrator)
                    ? administratorService.CreateSystemCategory(request)
                    : categoryService.CreateForCustomer(User.GetUserId(), request);

                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
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

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Customer)]
        public ActionResult<CategoryResponse> Rename([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                return Ok(categoryService.Rename(User.GetUserId(), id, request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Customer)]
        public IActionResult Delete([FromRoute] Guid id)
        {
            try
            {
                categoryService.Delete(User.GetUserId(), id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}