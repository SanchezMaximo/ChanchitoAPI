using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<CustomerResponse> Create([FromBody] CreateCustomerRequest request) 
        {
            try
            {
                var customer = customerService.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer); 
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

        [HttpGet]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult<IReadOnlyList<CustomerResponse>> GetAll()
        {
            var customers = customerService.GetAll();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<CustomerResponse> GetById([FromRoute] Guid id)
        {
            if (!User.IsInRole(Roles.Administrator) && User.GetUserId() != id)
                return Forbid();

            var customer = customerService.GetById(id);

            if (customer == null)
            {
                return NotFound($"There is no customer with id {id}.");
            }

            return Ok(customer);
        }
    }
}
