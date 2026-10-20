using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IAdministratorService administratorService;

        public AdministratorController(IAdministratorService administratorService)
        {
            this.administratorService = administratorService;
        }

        [HttpGet("customers")]
        public ActionResult<IReadOnlyList<CustomerResponse>> ListCustomers()
        {
            return Ok(administratorService.ListCustomers());
        }

        [HttpGet("customers/count")]
        public ActionResult<CustomerCountResponse> CountCustomers()
        {
            return Ok(administratorService.CountCustomers());
        }

        [HttpPatch("customers/{id}/suspend")]
        public ActionResult<CustomerResponse> SuspendCustomer([FromRoute] Guid id)
        {
            try
            {
                return Ok(administratorService.SuspendCustomer(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("customers/{id}/reactivate")]
        public ActionResult<CustomerResponse> ReactivateCustomer([FromRoute] Guid id)
        {
            try
            {
                return Ok(administratorService.ReactivateCustomer(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("categories")]
        public ActionResult<IReadOnlyList<CategoryResponse>> ListSystemCategories()
        {
            return Ok(administratorService.ListSystemCategories());
        }

        [HttpPost("categories")]
        public ActionResult<CategoryResponse> CreateSystemCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = administratorService.CreateSystemCategory(request);

                return CreatedAtAction(nameof(ListSystemCategories), new { id = category.Id }, category);
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

        [HttpPut("categories/{id}")]
        public ActionResult<CategoryResponse> UpdateSystemCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                return Ok(administratorService.UpdateSystemCategory(id, request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("categories/{id}")]
        public IActionResult DeleteSystemCategory([FromRoute] Guid id)
        {
            try
            {
                administratorService.DeleteSystemCategory(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}